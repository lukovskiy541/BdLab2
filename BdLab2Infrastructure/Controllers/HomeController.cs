using BdLab2Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace BdLab2Infrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ScienceArchiveContext _context;

        public HomeController(ILogger<HomeController> logger, ScienceArchiveContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Authors = _context.Authors.Select(a => a.LastName).Distinct().ToList();

            ViewBag.Languages = _context.Publications.Select(p => p.Language).Distinct().ToList();
            ViewBag.Subjects = _context.Subjects.Select(s => s.Name).Distinct().ToList();
            ViewBag.Departments = _context.Departments.Select(d => d.Name).Distinct().ToList();
            ViewBag.Reviewers = _context.Reviewers.Select(r => r.LastName).Distinct().ToList();
            ViewBag.PublicationNames = _context.Publications.Select(p => p.Name).Distinct().ToList();
            return View();
        }

        public IActionResult Privacy()
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
