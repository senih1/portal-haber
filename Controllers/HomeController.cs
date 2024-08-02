using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using portal_haber.Models;
using System.Diagnostics;
using System.Reflection;

namespace portal_haber.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = "Server=X;Initial Catalog=X;User Id =X; Password =X;TrustServerCertificate=X";
        public bool CheckLogin()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return false;
            }
            return true;
        }
        public IActionResult Index()
        {
            ViewData["username"] = HttpContext.Session.GetString("username");

            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM haberportal ORDER BY id DESC";
            var news = connection.Query<New>(sql).ToList();

            return View(news);
        }

        [Route("login")]
        public IActionResult Login()
        {
            ViewData["username"] = HttpContext.Session.GetString("username");

            if (CheckLogin())
            {
                TempData["AuthErrorCss"] = "alert-warning";
                TempData["AuthError"] = "Zaten bir hesaba giriş yaptın.";
                return RedirectToAction("Login");
            }

            ViewBag.AuthError = TempData["AuthError"] as string;
            return View();
        }

        [Route("userlogin")]
        [HttpPost]
        public IActionResult UserLogin(User model)
        {
            if (CheckLogin())
            {
                TempData["AuthErrorCss"] = "alert-warning";
                TempData["AuthError"] = "Zaten bir hesaba giriş yaptın.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid) 
            {
                TempData["AuthErrorCss"] = "alert-danger" ;
                TempData["AuthError"] = "Eksik form bilgisi.";
                return RedirectToAction("Login");
            }

            using var connection = new SqlConnection(connectionString);

            var sql = "SELECT * FROM haberportalusers WHERE Username = @Username AND Password = @Password";
            var user = connection.QueryFirstOrDefault<User>(sql, new { Username = model.Username, Password = model.Password});

            if (user != null)
            {
                HttpContext.Session.SetString("isUserAuth", "true");
                HttpContext.Session.SetString("username", model.Username);

                TempData["Login"] = $"Giriş başarılı. Hoşgeldin {model.Username}";
                TempData["LoginCss"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["AuthError"] = "Kullanıcı adı veya şifre hatalı." ;
                TempData["AuthErrorCss"] = "alert-danger" ;
                return RedirectToAction("Login");
            }
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register(User model)
        {
            if (CheckLogin())
            {
                TempData["LoginCss"] = "alert-warning";
                TempData["Login"] = "Zaten bir hesaba giriş yaptın.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["AuthErrorCss"] = "alert-danger";
                TempData["AuthError"] = "Eksik form bilgisi.";
                return RedirectToAction("Login");
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO haberportalusers (username, password, createdDate) VALUES (@Username, @Password, @CreatedDate)";

            var data = new
            {
                Username = model.Username,
                Password = model.Password,
                CreatedDate = DateTime.Now,
            };

            var rowsAffected = connection.Execute(sql, data);

            TempData["AuthErrorCss"] = "alert-success";
            TempData["AuthError"] = "Kullanıcı kayıdı başarı ile oluşturuldu!";
            return RedirectToAction("Login");
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["AuthErrorCss"] = "alert-warning";
            TempData["AuthError"] = "Hesaptan çıkış yapıldı.";
            return RedirectToAction("Login");
        }

        [Route("detail/{slug?}")]
        public IActionResult Detail(string? slug)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login");
            }

            ViewData["username"] = HttpContext.Session.GetString("username");

            using var connection = new SqlConnection(connectionString);
            var content = connection.QuerySingleOrDefault<New>("SELECT * FROM haberportal WHERE slug = @Slug", new { Slug = slug});

            return View(content);
        }
    }
}
