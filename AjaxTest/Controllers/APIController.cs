using AjaxTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AjaxTest.Controllers
{
    public class APIController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public APIController(MyDBContext context, IWebHostEnvironment webHostEnvironment)
        {
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

        public async Task<IActionResult> RegisterFile(userDTO _user)
        {
            //string info = $"{_user.userPhoto.FileName}-{_user.userPhoto.Length}-{_user.userPhoto.ContentType}";
            //return Content(info,"text/plain",System.Text.Encoding.UTF8);

            //檔案上傳
            //WebRootPath: 傳到wwwroot
            //ContentRootPath: 傳到專案根目錄
            //string strPath = _webHostEnvironment.WebRootPath;
            //string strPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", _user.userPhoto.FileName);

            //using (var fileStream = new FileStream(strPath, FileMode.Create))
            //{
            //    _user.userPhoto.CopyTo(fileStream);
            //}

            // 確保信箱、密碼和確認密碼和年齡不為 null
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(_user.userName))
                errorMessages.Add("姓名");
            if (string.IsNullOrEmpty(_user.userEmail))
                errorMessages.Add("信箱");
            if (string.IsNullOrEmpty(_user.userPassword))
                errorMessages.Add("密碼");
            if (string.IsNullOrEmpty(_user.userPasswordConfirm))
                errorMessages.Add("密碼確認");
            if (string.IsNullOrEmpty(_user.userAge.ToString()))
                errorMessages.Add("年齡");

            if (errorMessages.Any())
            {
                return Content(string.Join(", ", errorMessages) + "尚未輸入，請確認", "text/html");
            }
            else
            {
                try
                {
                    //寫進資料庫
                    Member member = new Member();
                    member.Name = _user.userName;
                    member.Email = _user.userEmail;
                    member.Age = _user.userAge;
                    member.FileName = _user.userPhoto != null ? _user.userPhoto.FileName : "default";
                    member.Password = new PasswordHasher<Member>().HashPassword(null, _user.userPassword);
                    member.Salt = new PasswordHasher<Member>().HashPassword(null, _user.userPasswordConfirm);

                    //將上船的檔案轉成二進位
                    byte[] imgByte = null;
                    //using (var memorySteam = new MemoryStream())
                    //{
                    //    _user.userPhoto.CopyTo(memorySteam);
                    //    imgByte = memorySteam.ToArray();
                    //}
                    if (_user.userPhoto != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await _user.userPhoto.CopyToAsync(memoryStream);
                            imgByte = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        string defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "s512_halloween201901_30_0.png");
                        imgByte = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                    }


                    member.FileData = imgByte;

                    _context.Members.Add(member);
                    _context.SaveChanges();


                    //return Content(strPath);
                    return Content("Saved");
                }
                catch (Exception ex)
                {
                    return Content($"Error: {ex.Message}");
                }
            }




        }

        public async Task<IActionResult> CheckAccount(string inputName)
        {
            var userExists = await _context.Members.AnyAsync(m => m.Name == inputName);
            
            var result = new { exists = userExists };
            return Json(result);

        }

        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _searchDTO)
        {
            //按照分類編號讀取景點
            var spots = _searchDTO.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == _searchDTO.categoryId);
            
            if (!string.IsNullOrEmpty(_searchDTO.keyword)){ 
                spots= spots.Where(s=>s.SpotTitle.Contains(_searchDTO.keyword) || s.SpotDescription.Contains(_searchDTO.keyword));
            };

            //關鍵字搜尋
            if (!string.IsNullOrEmpty(_searchDTO.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_searchDTO.keyword) || s.SpotDescription.Contains(_searchDTO.keyword));
            }

            //排序
            switch (_searchDTO.sortBy)
            {
                case "spotTitle":
                    spots = _searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _searchDTO.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = _searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }



            //總共有多少筆資料
            int totalCount = spots.Count();
            int pageSize = _searchDTO.pageSize;
            int page = _searchDTO.page;
            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            //設定回傳資料
            SpotsPagingDTO pagingDTO = new SpotsPagingDTO();
            pagingDTO.TotalPages = totalPages;
            pagingDTO.SpotsResult = spots.ToList();

            return Json(pagingDTO);

        }

        public IActionResult SpotCategory()
        {
            var spots = _context.Categories
                         .Select(x => new { x.CategoryId, x.CategoryName })
                         .ToList();
            return Json(spots);
        }
    }
}
