using Microsoft.AspNetCore.Mvc;
using Mcq.Models;
using System.Linq;

namespace Mcq.Controllers
{
    public class AccountController : Controller
    {
        private readonly McqsDbContext _context;

        public AccountController(McqsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email và mật khẩu
            var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Lưu thông tin user vào session
            HttpContext.Session.SetString("UserId", user.Iduser.ToString());
            HttpContext.Session.SetString("RoleId", user.Idrole.ToString());
            if (user.Idrole == 2) // Teacher
                {
                    return RedirectToAction("Dashboard", "Teacher");
                }
                else if (user.Idrole == 3) // Student
                {
                    return RedirectToAction("Dashboard", "Student");
                }

            return RedirectToAction("Index", "Home"); // Chuyển hướng sau khi đăng nhập thành công
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa session khi logout
            if( HttpContext.Session.GetString("StartTime") != null){
                TempData["AlertMessage"] = "You have an ongoing exam. Please finish it before navigating away!";
                return RedirectToAction("Dashboard","Student");
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
