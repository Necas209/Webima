using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webima.Data;
using Webima.Models;
using Webima.ViewModels;

namespace Webima.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var filmes = await _context.Filmes
                .Include(x => x.Bilhetes)
                .Where(x => x.Bilhetes.Any(bil => bil.Data.Date >= DateTime.Now.Date))
                .Include(x => x.Categoria)
                .ToListAsync();

            return View(filmes);
        }


        public IActionResult Faqs()
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