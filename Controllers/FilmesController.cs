using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webima.Data;
using Webima.Models;

namespace Webima.Controllers
{
    public class FilmesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FilmesController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Cartaz()
        {
            var filmes = await _context.Filmes
                .Where(x => x.Estreia.Date <= DateTime.Now.Date)
                .Include(x => x.Bilhetes)
                .Where(x => x.Bilhetes.Any(x => x.Data.Date >= DateTime.Now.Date))
                .Include(x => x.IdCatNavigation)
                .ToListAsync();

            return View(filmes);
        }

        public async Task<IActionResult> Estreias()
        {
            var filmes = await _context.Filmes
                .Where(x => x.Estreia.Date > DateTime.Now.Date)
                .Include(x => x.IdCatNavigation)
                .ToListAsync();

            return View(filmes);
        }

        // GET: Filmes/Details/id?
        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .Where(x => x.Id == id)
                .Include(x => x.IdCatNavigation)
                .SingleOrDefaultAsync();
                
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/EscolherSessao/id?
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> EscolherSessao(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var bilhetes = (await _context.Bilhetes
                .Where(x => x.IdFilme == id && x.Data.Date >= DateTime.Now.Date)
                .Include(x => x.IdSessaoNavigation)
                .ToListAsync())
                .OrderBy(x => x.Data).ThenBy(x => x.IdSessaoNavigation.Horas)
                .GroupBy(x => x.Data);

            ViewData["Filme"] = await _context.Filmes.FindAsync(id);

            return View(bilhetes);
        }

        // GET: Filmes/Comprar
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Comprar(int? id_bil)
        {
            if (id_bil == null)
            {
                return NotFound();
            }

            var bilhete = await _context.Bilhetes
                .Where(x => x.Id == id_bil)
                .Include(x => x.IdFilmeNavigation)
                .Include(x => x.IdSalaNavigation)
                .Include(x => x.IdSessaoNavigation)
                .FirstOrDefaultAsync();

            if (bilhete == null)
            {
                return NotFound("Ticket does not exist.");
            }

            // Calcular lotação disponível
            int lotacao = bilhete.IdSalaNavigation.Lotacao;

            lotacao -= await _context.Compras
                .Where(x => x.IdBil == id_bil)
                .SumAsync(x => x.NumBil);

            ViewBag.Lotacao = lotacao;

            return View(bilhete);
        }

        // POST: Filmes/Comprar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar(IFormCollection formValues)
        {
            int id_bil = Convert.ToInt32(formValues["Id_Bil"]);
            int num_bil = Convert.ToInt32(formValues["Num_Bil"]);

            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                UserId = (await _context.Utilizadores
                    .SingleOrDefaultAsync(x => x.Username == User.Identity.Name)).Id;
                HttpContext.Session.SetInt32("UserId", (int)UserId);
            }

            Compra compra = new()
            {
                IdBil = id_bil,
                IdCliente = (int)UserId,
                NumBil = num_bil,
                DataCompra = DateTime.Now
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Conta");
        }
    }
}
