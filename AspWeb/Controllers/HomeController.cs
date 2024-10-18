using AspWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TelecomApp.Models;

namespace AspWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Db8328Context _context;


        public HomeController(ILogger<HomeController> logger, Db8328Context context)
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

        [Route("table/tariff")]
        public IActionResult Table()
        {
            var model = _context.TariffPlans.ToList();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
