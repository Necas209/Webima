using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
            if (User.IsInRole("Cliente"))
            {
                return RedirectToAction("Index", "Clientes");
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admins");
            }
            if (User.IsInRole("Funcionario"))
            {
                return RedirectToAction("Index", "Funcionarios");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Conta/AlterarNome
        [AjaxFilter]
        public async Task<IActionResult> AlterarNome(int Id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(Id);
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
