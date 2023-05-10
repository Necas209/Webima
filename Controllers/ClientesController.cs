using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webima.Data;
using Webima.Filters;
using Webima.Models;

namespace Webima.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientesController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                UserId = (await _context.Utilizadores
                    .SingleOrDefaultAsync(x => x.Username == User.Identity.Name)).Id;
                HttpContext.Session.SetInt32("UserId", (int)UserId);
            }

            var user = await _userManager.GetUserAsync(User);

            var cliente = await _context.Clientes
                .Where(x => x.Id == UserId)
                .Include(x => x.IdNavigation)
                .Include(x => x.Compras).ThenInclude(x => x.IdBilNavigation).ThenInclude(x => x.IdSessaoNavigation)
                .Include(x => x.Compras).ThenInclude(x => x.IdBilNavigation).ThenInclude(x => x.IdSalaNavigation)
                .Include(x => x.Compras).ThenInclude(x => x.IdBilNavigation).ThenInclude(x => x.IdFilmeNavigation)
                .Include(x => x.Categorias).ThenInclude(x => x.IdCatNavigation)
                .FirstOrDefaultAsync();

            cliente.Compras = cliente.Compras
                .OrderByDescending(x => x.DataCompra)
                .ToList();

            var categorias = (await _context.Categoria.ToListAsync()).GroupJoin(cliente.Categorias,
                x => x.Id, y => y.IdCat, (x, y) => new { Categoria = x, Preferida = y.Any() });

            var lista = new List<PreferidasViewModel>();

            foreach (var c in categorias)
            {
                lista.Add(new PreferidasViewModel
                {
                    Categoria = c.Categoria,
                    IsChecked = c.Preferida,
                });
            }

            ViewData["Email"] = user.Email;
            ViewData["Categorias"] = lista;

            return View(cliente);
        }

        [HttpPost]
        [AjaxFilter]
        public async Task MudarPreferida(int id, bool check)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");

            var cliente = await _context.Clientes.FindAsync((int)UserId);

            CliCat cliCat = new()
            {
                IdCliente = cliente.Id,
                IdCat = id
            };

            if (check)
            {
                _context.CliCats.Add(cliCat);
            }
            else
            {
                _context.CliCats.Remove(cliCat);
            }
            await _context.SaveChangesAsync();
        }

        // GET: AlterarDataNasc
        [AjaxFilter]
        public async Task<IActionResult> AlterarDataNasc(int Id)
        {
            var cliente = await _context.Clientes.FindAsync(Id);
            return PartialView(nameof(AlterarDataNasc), cliente);
        }

        // POST: AlterarDataNasc
        [HttpPost]
        public async Task<string> AlterarDataNasc(Cliente cliente)
        {
            _context.Update(cliente);
            await _context.SaveChangesAsync();
            return cliente.DataNasc.ToShortDateString();
        }
    }
}
