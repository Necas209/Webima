using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Webima.Data;
using Webima.Filters;
using Webima.Models;
using Webima.ViewModels;

namespace Webima.Controllers;

[Authorize(Roles = "Funcionario")]
public class FuncionariosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHostEnvironment _he;
    private readonly IEmailSender _sender;

    public FuncionariosController(ApplicationDbContext context, IHostEnvironment he, IEmailSender sender)
    {
        _context = context;
        _he = he;
        _sender = sender;
    }

    // GET: Funcionarios
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var funcionario = await _context.Funcionarios
            .Where(x => x.Id == userId)
            .Include(x => x.Utilizador)
            .FirstOrDefaultAsync();
        ViewData["Email"] = funcionario.Utilizador.Email;
        return View(funcionario);
    }

    // GET: AdicionarFilme
    public async Task<IActionResult> AdicionarFilme()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var sessoes = await _context.Sessoes
            .Where(x => x.Estado == true)
            .OrderBy(x => x.Horas)
            .Select(x => new SessaoViewModel
            {
                Id = x.Id,
                Horas = x.Horas,
                Estado = x.Estado
            })
            .ToListAsync();

        var filme = new FilmeViewModel
        {
            FuncionarioId = userId,
            Poster = "",
            Sessoes = sessoes
        };

        ViewData["IdCat"] = new SelectList(_context.Categoria
            .Where(x => x.Estado == true), "Id", "Nome");
        ViewData["IdSala"] = new SelectList(_context.Salas
            .Where(x => x.Estado == true), "Id", "Nome");

        return View(filme);
    }

    // POST: AdicionarFilme
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarFilme(FilmeViewModel input, IFormFile poster)
    {
        ModelState.Remove("Poster");
        if (poster == null)
            ModelState.AddModelError("Poster", "Poster obrigatório.");
        else if (poster.ContentType is not ("image/png" or "image/jpeg" or "image/webp"))
            ModelState.AddModelError("Poster", "Tipo de ficheiro não permitido.");

        if (input.Ano <= 1900 || input.Ano > DateTime.Now.AddYears(1).Year)
            ModelState.AddModelError("Ano", "Ano inválido.");

        if (input.Estreia < DateTime.Now.Date)
            ModelState.AddModelError("Estreia", "Estreia tem de ser uma data futura.");

        if (input.DataFim < input.Estreia)
            ModelState.AddModelError("DataFim", "A data final tem de ser depois da data de estreia.");

        if (!ModelState.IsValid || poster == null)
        {
            ViewData["IdCat"] = new SelectList(_context.Categoria
                .Where(x => x.Estado == true), "Id", "Nome");
            ViewData["IdSala"] = new SelectList(_context.Salas
                .Where(x => x.Estado == true), "Id", "Nome");
            return View(input);
        }

        var destination = Path.Combine(_he.ContentRootPath, "wwwroot/posters/", Path.GetFileName(poster.FileName));
        await using var fs = new FileStream(destination, FileMode.Create);
        await poster.CopyToAsync(fs);

        input.Poster = Path.GetFileName(poster.FileName);

        _context.Add((Filme)input);
        await _context.SaveChangesAsync();

        var filmeId = await _context.Filmes
            .Where(x => x.Titulo == input.Titulo)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        foreach (var date in Enumerable.Range(0, (int)input.DataFim.Subtract(input.Estreia).TotalDays + 1)
                     .Select(i => input.Estreia.AddDays(i)))
        foreach (var sessao in input.Sessoes.Where(s => s.Selected))
            _context.Add(new Bilhete
            {
                FilmeId = filmeId,
                SalaId = input.IdSala,
                Data = date,
                SessaoId = sessao.Id,
                Preco = input.Preco
            });

        await _context.SaveChangesAsync();
        await NotificarClientes(filmeId);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> EditarFilme(int id)
    {
        var filme = await _context.Filmes.FindAsync(id);
        if (filme == null) return NotFound();
        ViewData["IdCat"] = new SelectList(_context.Categoria
            .Where(x => x.Estado == true), "Id", "Nome");
        return View(filme);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarFilme(Filme filme, IFormFile poster)
    {
        ModelState.Remove("Poster");

        if (poster is { ContentType: not ("image/png" or "image/jpeg" or "image/webp") })
            ModelState.AddModelError("Poster", "Tipo de ficheiro não permitido.");
        if (filme.Ano <= 1900 || filme.Ano > DateTime.Now.AddYears(1).Year)
            ModelState.AddModelError("Ano", "Ano inválido.");

        if (!ModelState.IsValid)
        {
            ViewData["IdCat"] = new SelectList(_context.Categoria
                .Where(x => x.Estado == true), "Id", "Nome");
            return View(filme);
        }

        var source = Path.Combine(_he.ContentRootPath, "wwwroot/posters/", filme.Poster);
        System.IO.File.Delete(source);

        var destination = Path.Combine(_he.ContentRootPath, "wwwroot/posters/", Path.GetFileName(poster.FileName));
        FileStream fs = new(destination, FileMode.Create);
        await poster.CopyToAsync(fs);
        fs.Close();

        filme.Poster = Path.GetFileName(poster.FileName);

        _context.Update(filme);
        await _context.SaveChangesAsync();
        return RedirectToAction("Detalhes", "Filmes", new { id = filme.Id });
    }

    private async Task NotificarClientes(int id)
    {
        var filme = await _context.Filmes.FindAsync(id);
        if (filme == null) return;
        var clientes = await _context.Clientes
            .Include(x => x.CategoriasPreferidas)
            .Include(x => x.Utilizador)
            .Where(x => x.CategoriasPreferidas.Any(y => y.CategoriaId == filme.CategoriaId))
            .ToListAsync();
        var filmeUrl = Url.Action(
            "Detalhes",
            "Filmes",
            new { id },
            Request.Scheme);
        await Task.WhenAll(clientes.Select(x => _sender.SendEmailAsync(x.Utilizador.Email!,
                "Próximas Estreias - Webima",
                $"Olá, {x.Utilizador.UserName}, temos um novo filme que achamos que podes gostar! <a href='{filmeUrl}'>Clique aqui</a>.")
            )
        );
    }

    // GET: AlterarTelefone
    [AjaxFilter]
    public async Task<IActionResult> AlterarTelefone(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        return PartialView(nameof(AlterarTelefone), funcionario);
    }

    // POST: AlterarTelefone
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<string> AlterarTelefone(Funcionario funcionario)
    {
        _context.Update(funcionario);
        await _context.SaveChangesAsync();
        return funcionario.Utilizador.PhoneNumber;
    }
}