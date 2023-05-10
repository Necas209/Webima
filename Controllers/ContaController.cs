using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Webima.Data;
using Webima.Filters;
using Webima.Models;

namespace Webima.Controllers
{
    [Authorize]
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Conta
        public IActionResult Index()
        {
            if (User.IsInRole("Cliente")) return RedirectToAction("Index", "Clientes");
            return User.IsInRole("Admin")
                ? RedirectToAction("Index", "Admins")
                : RedirectToAction("Index", User.IsInRole("Funcionario") ? "Funcionarios" : "Home");
        }

        // GET: Conta/AlterarNome
        [AjaxFilter]
        public async Task<IActionResult> AlterarNome(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            return PartialView(nameof(AlterarNome), utilizador);
        }

        // POST: Conta/AlterarNome
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> AlterarNome(Utilizador utilizador)
        {
            _context.Update(utilizador);
            await _context.SaveChangesAsync();
            return utilizador.Nome;
        }
    }
}