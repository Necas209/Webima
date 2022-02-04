using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Webima.Data;
using Webima.Filters;
using Webima.Models;

namespace Webima.Controllers
{
    [Authorize(Roles = "Funcionario")]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostEnvironment _he;
        private readonly IEmailSender _emailSender;

        public FuncionariosController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IHostEnvironment he,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _he = he;
            _emailSender = emailSender;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index()
        {
            int? idFunc = HttpContext.Session.GetInt32("UserId");
            if (idFunc == null)
            {
                idFunc = (await _context.Utilizadores
                    .SingleOrDefaultAsync(x => x.Username == User.Identity.Name)).Id;
                HttpContext.Session.SetInt32("UserId", (int)idFunc);
            }

            var user = await _userManager.GetUserAsync(User);

            var funcionario = await _context.Funcionarios
                .Where(x => x.Id == (int)idFunc)
                .Include(x => x.IdNavigation)
                .FirstOrDefaultAsync();

            ViewData["Email"] = user.Email;

            return View(funcionario);
        }

        // GET: AdicionarFilme
        public async Task<IActionResult> AdicionarFilme()
        {
            int? idFunc = HttpContext.Session.GetInt32("UserId");
            if (idFunc == null)
            {
                idFunc = (await _context.Utilizadores
                    .SingleOrDefaultAsync(x => x.Username == User.Identity.Name)).Id;
                HttpContext.Session.SetInt32("UserId", (int)idFunc);
            }

            FilmeViewModel filme = new()
            {
                IdFunc = (int)idFunc,
                Poster = "",
                Sessoes = new List<SessaoViewModel>(),
            };

            var sessoes = await _context.Sessoes
                .Where(x => x.Estado == true)
                .OrderBy(x => x.Horas)
                .ToListAsync();

            foreach (var sessao in sessoes)
            {
                filme.Sessoes.Add(new()
                {
                    Id = sessao.Id,
                    Horas = sessao.Horas,
                    Estado = sessao.Estado,
                });
            }

            ViewData["IdCat"] = new SelectList(_context.Categoria
                .Where(x => x.Estado == true), "Id", "Nome");
            ViewData["IdSala"] = new SelectList(_context.Salas
                .Where(x => x.Estado == true), "Id", "Nome");

            return View(filme);
        }

        // POST: AdicionarFilme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarFilme(FilmeViewModel input, IFormFile Poster)
        {
            ModelState.Remove("Poster");

            if (Poster == null)
                ModelState.AddModelError("Poster", "Poster obrigatório.");
            else
                if (Poster.ContentType is not ("image/png" or "image/jpeg" or "image/webp"))
                ModelState.AddModelError("Poster", "Tipo de ficheiro não permitido.");

            if (input.Ano <= 1900 || input.Ano > DateTime.Now.AddYears(1).Year)
                ModelState.AddModelError("Ano", "Ano inválido.");

            if (input.Estreia < DateTime.Now.Date)
                ModelState.AddModelError("Estreia", "Estreia tem de ser uma data futura.");

            if (input.DataFim < input.Estreia)
                ModelState.AddModelError("DataFim", "A data final tem de ser depois da data de estreia.");

            if (!ModelState.IsValid)
            {
                ViewData["IdCat"] = new SelectList(_context.Categoria
                    .Where(x => x.Estado == true), "Id", "Nome");
                ViewData["IdSala"] = new SelectList(_context.Salas
                    .Where(x => x.Estado == true), "Id", "Nome");

                return View(input);
            }

            string destination = Path.Combine(_he.ContentRootPath,
                "wwwroot/posters/",
                Path.GetFileName(Poster.FileName));
            FileStream fs = new(destination, FileMode.Create);
            Poster.CopyTo(fs);
            fs.Close();

            input.Poster = Path.GetFileName(Poster.FileName);

            _context.Add((Filme)input);
            await _context.SaveChangesAsync();

            int idFilme = (await _context.Filmes
                .SingleOrDefaultAsync(x => x.Titulo == input.Titulo)).Id;


            for (var date = input.Estreia; date <= input.DataFim; date = date.AddDays(1))
            {
                foreach (var sessao in input.Sessoes)
                {
                    if (sessao.Selected)
                    {
                        Bilhete bilhete = new()
                        {
                            IdFilme = idFilme,
                            IdSala = input.IdSala,
                            Data = date,
                            IdSessao = sessao.Id,
                            Preco = input.Preco,
                        };
                        _context.Add(bilhete);
                    }
                }
            }
            await _context.SaveChangesAsync();
            await NotificarClientes(idFilme);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditarFilme(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.FindAsync(id);

            if (filme == null)
            {
                return NotFound();
            }

            ViewData["IdCat"] = new SelectList(_context.Categoria
                .Where(x => x.Estado == true), "Id", "Nome");

            return View(filme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarFilme(Filme filme, IFormFile poster)
        {
            ModelState.Remove("Poster");

            if (poster != null && poster.ContentType is not ("image/png" or "image/jpeg" or "image/webp"))
            {
                ModelState.AddModelError("Poster", "Tipo de ficheiro não permitido.");
            }
            if (filme.Ano <= 1900 || filme.Ano > DateTime.Now.AddYears(1).Year)
            {
                ModelState.AddModelError("Ano", "Ano inválido.");
            }

            if (ModelState.IsValid)
            {
                if (poster != null)
                {
                    string source = Path.Combine(_he.ContentRootPath,
                        "wwwroot/posters/", filme.Poster);
                    System.IO.File.Delete(source);

                    string destination = Path.Combine(_he.ContentRootPath,
                        "wwwroot/posters/",
                        Path.GetFileName(poster.FileName));
                    FileStream fs = new(destination, FileMode.Create);
                    poster.CopyTo(fs);
                    fs.Close();

                    filme.Poster = Path.GetFileName(poster.FileName);
                }
                _context.Update(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction("Detalhes", "Filmes", new { id = filme.Id });
            }

            ViewData["IdCat"] = new SelectList(_context.Categoria
                .Where(x => x.Estado == true), "Id", "Nome");
            return View(filme);
        }

        private async Task NotificarClientes(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);

            var utilizadores = _context.CliCats
                .Where(x => x.IdCat == filme.IdCat)
                .Include(x => x.IdClienteNavigation).ThenInclude(x => x.IdNavigation)
                .Select(x => x.IdClienteNavigation.IdNavigation);

            var users = _context.Users
                .Where(x => utilizadores.Any(u => u.Username == x.UserName));

            var filmeUrl = Url.Action(
                action: "Detalhes",
                controller: "Filmes",
                values: new { id = id },
                protocol: Request.Scheme);

            foreach (var user in users)
            {
                await _emailSender.SendEmailAsync(user.Email, "Próximas Estreias - Webima",
                    $"Olá, {user.UserName}, temos um novo filme que achamos que podes gostar! <a href='{filmeUrl}'>Clique aqui</a>.");
            }
        }

        // GET: AlterarTelefone
        [AjaxFilter]
        public async Task<IActionResult> AlterarTelefone(int Id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(Id);
            return PartialView(nameof(AlterarTelefone), funcionario);
        }

        // POST: AlterarTelefone
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> AlterarTelefone(Funcionario funcionario)
        {
            _context.Update(funcionario);
            await _context.SaveChangesAsync();
            return funcionario.Telefone;
        }

    }
}
