using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TelecomWeb.Models;

namespace TelecomWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TelecomDbContext _context;

        public HomeController(ILogger<HomeController> logger, TelecomDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [Route("table/tariff")]
        public IActionResult TableTariff()
        {
            var model = _context.TariffPlans.ToList();
            return View(model);
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [Route("table/subscriber")]
        public IActionResult TableSubscribers()
        {
            var model = _context.Subscribers.ToList();
            return View(model);
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [Route("table/contract")]
        public IActionResult TableContracts()
        {
            var model = _context.Contracts
                .Include(c => c.Staff)         
                .Include(c => c.Subscriber)    
                .ToList();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
