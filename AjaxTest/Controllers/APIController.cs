using AjaxTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AjaxTest.Controllers
{
    public class APIController : Controller
    {
        private readonly ILogger<APIController> _logger;
        private readonly MyDbContext _context;
       private readonly IWebHostEnvironment _webHostEnvironment;
        public APIController(ILogger<APIController> logger, MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult Avatar(int id = 1)
        {
            var member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                if (img != null) { return File(img, "image/jpeg"); }
            }
            return NotFound();
        }

        public IActionResult Register(userDTO _user)
        {
            if (string.IsNullOrEmpty(_user.userName))
            {
                _user.userName = "Guest";
            }

            return Content($"{_user.userName} - {_user.userEmail} - {_user.userAge}", "text/plain");
        }

        public IActionResult RegisterFile(userDTO _user)
        {
            if (string.IsNullOrEmpty(_user.userName))
            {
                _user.userName = "Guest";
            }

            string info = $"{_user.userPhoto.FileName}-{_user.userPhoto.Length}-{_user.userPhoto.ContentType}";


            //實際路徑 C:\Shared\012_Ajax\workspace\AjaxDemo\wwwroot\uploads\abc.jpg
            //string strPath = @"C:\Shared\012_Ajax\workspace\AjaxDemo\wwwroot\uploads\abc.jpg";
            //string strPath = _webHostEnvironment.WebRootPath;
            //C:\Shared\012_Ajax\workspace\AjaxDemo\wwwroot
            //string strPath = _webHostEnvironment.ContentRootPath;
            //C:\Shared\012_Ajax\workspace\AjaxDemo


            //檔案上傳
            string strPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", _user.userPhoto.FileName);

            using (var fileStream = new FileStream(strPath, FileMode.Create))
            {
                _user.userPhoto.CopyTo(fileStream);
            }

            //return Content($"{_user.userName} - {_user.userEmail} - {_user.userAge}", "text/plain");
            //return Content(info,"text/plain",System.Text.Encoding.UTF8);
            return Content(strPath);

        }

    }
}
