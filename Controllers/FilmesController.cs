using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webima.Data;
using Webima.Models;

namespace Webima.Controllers;

public class FilmesController : Controller
{
    private readonly ApplicationDbContext _context;

    public FilmesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Cartaz()
    {
        var filmes = await _context.Filmes
            .Where(x => x.Estreia.Date <= DateTime.Now.Date)
            .Include(x => x.Bilhetes)
            .Where(x => x.Bilhetes.Any(bil => bil.Data.Date >= DateTime.Now.Date))
            .Include(x => x.Categoria)
            .ToListAsync();

        return View(filmes);
    }

    public async Task<IActionResult> Estreias()
    {
        var filmes = await _context.Filmes
            .Where(x => x.Estreia.Date > DateTime.Now.Date)
            .Include(x => x.Categoria)
            .ToListAsync();

        return View(filmes);
    }

    // GET: Filmes/Details/id?
    public async Task<IActionResult> Detalhes(int id)
    {
        var filme = await _context.Filmes
            .Where(x => x.Id == id)
            .Include(x => x.Categoria)
            .SingleOrDefaultAsync();
        if (filme == null) return NotFound();
        return View(filme);
    }

    // GET: Filmes/EscolherSessao/id?
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> EscolherSessao(int id)
    {
        var bilhetes = await _context.Bilhetes
            .Where(x => x.FilmeId == id && x.Data.Date >= DateTime.Now.Date)
            .Include(x => x.Sessao)
            .OrderBy(x => x.Data).ThenBy(x => x.Sessao.Horas)
            .GroupBy(x => x.Data)
            .ToListAsync();
        ViewBag.Filme = await _context.Filmes.FindAsync(id);
        return View(bilhetes);
    }

    // GET: Filmes/Comprar
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Comprar(int bilheteId)
    {
        var bilhete = await _context.Bilhetes
            .Where(x => x.Id == bilheteId)
            .Include(x => x.Filme)
            .Include(x => x.Sala)
            .Include(x => x.Sessao)
            .FirstOrDefaultAsync();
        if (bilhete == null) return NotFound("Ticket does not exist.");
        // Calcular lotação disponível
        var lotacao = bilhete.Sala.Lotacao;
        lotacao -= await _context.Compras
            .Where(x => x.BilheteId == bilheteId)
            .SumAsync(x => x.NumeroBilhetes);
        ViewBag.Lotacao = lotacao;
        return View(bilhete);
    }

    // POST: Filmes/Comprar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comprar(IFormCollection formValues)
    {
        var bilheteId = Convert.ToInt32(formValues["BilheteId"]);
        var numeroBilhetes = Convert.ToInt32(formValues["NumeroBilhetes"]);
        var userId = HttpContext.Session.GetString("UserId");
        Compra compra = new()
        {
            BilheteId = bilheteId,
            ClienteId = userId,
            NumeroBilhetes = numeroBilhetes,
            DataCompra = DateTime.Now
        };

        _context.Compras.Add(compra);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Conta");
    }
}