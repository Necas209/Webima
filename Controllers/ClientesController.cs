using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webima.Data;
using Webima.Filters;
using Webima.Models;
using Webima.ViewModels;

namespace Webima.Controllers;

[Authorize(Roles = "Cliente")]
public class ClientesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Clientes
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var cliente = await _context.Clientes
            .Where(x => x.Id == userId)
            .Include(x => x.Utilizador)
            .Include(x => x.Compras).ThenInclude(x => x.Bilhete).ThenInclude(x => x.Sessao)
            .Include(x => x.Compras).ThenInclude(x => x.Bilhete).ThenInclude(x => x.Sala)
            .Include(x => x.Compras).ThenInclude(x => x.Bilhete).ThenInclude(x => x.Filme)
            .Include(x => x.CategoriasPreferidas)
            .FirstOrDefaultAsync();

        cliente.Compras = cliente.Compras
            .OrderByDescending(x => x.DataCompra)
            .ToList();

        var categoriasIds = cliente.CategoriasPreferidas.Select(x => x.CategoriaId);
        var lista = await _context.Categoria
            .Select(categoria => new PreferidasViewModel
            {
                Categoria = categoria,
                IsChecked = categoriasIds.Contains(categoria.Id)
            })
            .ToListAsync();

        ViewData["Email"] = cliente.Utilizador.Email;
        ViewData["Categorias"] = lista;

        return View(cliente);
    }

    [HttpPost]
    [AjaxFilter]
    public async Task MudarPreferida(int id, bool check)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var categoriaPreferida = new CategoriaPreferida
        {
            ClienteId = userId,
            CategoriaId = id
        };

        if (check)
            _context.CategoriasPreferidas.Add(categoriaPreferida);
        else
            _context.CategoriasPreferidas.Remove(categoriaPreferida);
        await _context.SaveChangesAsync();
    }

    // GET: AlterarDataNasc
    [AjaxFilter]
    public async Task<IActionResult> AlterarDataNasc(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        return PartialView(nameof(AlterarDataNasc), cliente);
    }

    // POST: AlterarDataNasc
    [HttpPost]
    public async Task<string> AlterarDataNasc(Cliente cliente)
    {
        _context.Update(cliente);
        await _context.SaveChangesAsync();
        return cliente.DataNascimento.ToShortDateString();
    }
}