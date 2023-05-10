using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Webima.Data;
using Webima.Models;

namespace Webima.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var filmes = await _context.Filmes
                .Include(x => x.Bilhetes)
                .Where(x => x.Bilhetes.Any(x => x.Data.Date >= DateTime.Now.Date))
                .Include(x => x.IdCatNavigation)
                .ToListAsync();

            return View(filmes);
        }


        public IActionResult FAQs()
        {
            return View();
        }

        public IActionResult Acerca()
        {
            return View();
        }
        public IActionResult TermosCondicoes()
        {
            return View();
        }

        public IActionResult PoliticaPrivacidade()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
