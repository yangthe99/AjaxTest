using AjaxTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace AjaxTest.Controllers
{
    public class APIController : Controller
    {
        private readonly ILogger<APIController> _logger;
        private readonly MyDbContext _context;
        public APIController(ILogger<APIController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            string content = "Hello API! 哈哈";
            return Content(content, "text/plain", System.Text.Encoding.UTF8);
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(x => x.City).Distinct();
            return Json(cities);
        }

        public IActionResult SiteID()
        {
            var siteID = _context.Addresses.Select(x => x.SiteId).Distinct();
            return Json(siteID);
        }
        public IActionResult Road()
        {
            var roads = _context.Addresses.Select(x => x.Road).Distinct();
            return Json(roads);
        }

        public IActionResult Avatar(int id=1)
        {
            var member = _context.Members.Find(id);
            if (member != null) {
                byte[] img = member.FileData;
                if (img != null) { return File(img, "image/jpeg"); }
            }
            return NotFound();
        }
    }
}
