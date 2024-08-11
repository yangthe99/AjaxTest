using AjaxTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AjaxTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;
        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var add = _context.Addresses.Where(p => p.Id == 34204);
            return View(add);
        }

        public IActionResult First()
        {
            var cName = _context.Categories;
            return View(cName);
        }
        public IActionResult CallAPI()
        {
            return View();
        }
        public IActionResult JsonTest()
        {
            return View();
        }
        public IActionResult Address()
        {
            return View();
        }
        public IActionResult ShowImg()
        {
            return View();
        }
        public IActionResult Spot()
        {
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

        public IActionResult Register()
        {
            return View();

        }
        public IActionResult RegisterForm()
        {
            return View();

        }
        public IActionResult RegisterFile()
        {
            return View();
        }
              public IActionResult RegistertoServer()
        {
            return View();
        }  public IActionResult MemberRegister()
        {
            return View();
        }
    }
}
