using Microsoft.AspNetCore.Mvc;
using Mcq.Models;
using System;
using BCrypt.Net;

namespace Mcq.Controllers
{
    public class RegisterController : Controller
{
    private readonly McqsDbContext _context;

    public RegisterController(McqsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Tải danh sách quyền và tài nguyên
        Console.WriteLine("haa)");
        var rolePermissions = _context.RolePermissions.ToList();
        Console.WriteLine("haa)");
        var resources = _context.Resources.ToList();
        Console.WriteLine("haa)");
        ViewBag.Resources = resources;
        Console.WriteLine("haa)");
        ViewBag.RolePermissions = rolePermissions;

        return View();
    }

    [HttpPost]
    public IActionResult Index(User model, string roleType, Teacher teacher = null, Student student = null)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine("ModelState Error: " + error.ErrorMessage);
        }
            return View(model);
        }

        var existingUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
        if (existingUser != null)
        {
            Console.WriteLine("lỗi 40");
            ModelState.AddModelError("Email", "This email is already registered.");
            return View(model);
        }

        // Sinh GUID
        model.Iduser = Guid.NewGuid();

        // Gán RoleID và lưu User
        model.CreateAccountDate = DateTime.UtcNow;
        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // Thêm thông tin Teacher hoặc Student
        if (roleType == "Teacher")
        {
            model.Idrole = 2;
            teacher.Idteacher = model.Iduser;
            _context.Users.Add(model);
             _context.SaveChanges();
            _context.Teachers.Add(teacher);
        }
        else if (roleType == "Student")
        {
            model.Idrole = 3;
            student.Idstudent = model.Iduser;;
            _context.Users.Add(model);
            _context.SaveChanges();
            _context.Students.Add(student);
        }

        _context.SaveChanges();

        return RedirectToAction("Login","Account");
    }


    public IActionResult Success()
    {
        return View();
    }
}
}
