using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using portal_haber.Models;
using System.Reflection;

namespace portal_haber.Controllers
{
    public class AdminController : Controller
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
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM haberportal ORDER BY id DESC";
            var news = connection.Query<New>(sql).ToList();

            ViewData["username"] = HttpContext.Session.GetString("username");

            return View(news);
        }

        public IActionResult DeleteNew(int id)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM haberportal WHERE id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            TempData["Message"] = "Haber başarılı bir şekilde silindi.";
            TempData["MessageCss"] = "alert-success";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditNew(int id)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            ViewData["username"] = HttpContext.Session.GetString("username");

            using var connection = new SqlConnection(connectionString);
            var news = connection.QuerySingleOrDefault<New>("SELECT * FROM haberportal WHERE id = @Id", new { Id = id });

            return View(news);
        }

        [HttpPost]
        public IActionResult EditNew(New model)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["MessageCss"] = "alert-danger";
                TempData["Message"] = "Eksik veya hatalı işlem yaptın";
                return RedirectToAction("Index");

            }

            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var path = Path.Combine(/*Directory.GetCurrentDirectory(), */"wwwroot", "uploads", imageName);

            using var stream = new FileStream(path, FileMode.Create);
            model.Image.CopyTo(stream);

            ViewBag.Image = $"/uploads/{imageName}";
            string imageUrl = ViewBag.Image;

            using var connection = new SqlConnection(connectionString);
            var sql = "UPDATE haberportal SET title = @Title, summary = @Summary, contents = @Contents, imageUrl = @ImageUrl, slug = @Slug WHERE id = @Id";

            var param = new
            {
                Title = model.Title,
                Summary = model.Summary,
                Contents = model.Contents,
                ImageUrl = imageUrl,
                Slug = model.Slug,
                Id = model.Id,
            };

            var affectedRows = connection.Execute(sql, param);

            TempData["Message"] = "Güncellendi.";
            TempData["MessageCss"] = "alert-success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Add(New model)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            ViewData["username"] = HttpContext.Session.GetString("username");

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["MessageCss"] = "alert-danger";
                return RedirectToAction("Index");
            }

            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var path = Path.Combine(/*Directory.GetCurrentDirectory(), */"wwwroot", "uploads", imageName);

            using var stream = new FileStream(path, FileMode.Create);
            model.Image.CopyTo(stream);

            ViewBag.Image = $"/uploads/{imageName}";
            string imageUrl = ViewBag.Image;

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO haberportal (title, summary, contents, slug, imageUrl, createdDate) VALUES (@Title, @Summary, @Contents, @Slug, @ImageUrl, @CreatedDate)";

            var data = new
            {
                Title = model.Title,
                Summary = model.Summary,
                Contents = model.Contents,
                Slug = model.Slug,
                imageUrl = imageUrl,
                CreatedDate = DateTime.Now,
            };

            var rowsAffected = connection.Execute(sql, data);

            TempData["Message"] = "Haber başarı ile eklendi!";
            TempData["MessageCss"] = "alert-success";
            return RedirectToAction("Index");
        }

        public IActionResult Users()
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            ViewData["username"] = HttpContext.Session.GetString("username");

            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM haberportalusers ORDER BY id ASC";
            var users = connection.Query<User>(sql).ToList();

            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM haberportalusers WHERE id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            TempData["Message"] = "Kullanıcı başarılı bir şekilde silindi.";
            TempData["MessageCss"] = "alert-success";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult EditUserPassword(int id,string password)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["MessageCss"] = "alert-danger";
                TempData["Message"] = "Eksik veya hatalı işlem yaptın";
                return RedirectToAction("Index");

            }

            using var connection = new SqlConnection(connectionString);
            var sql = "UPDATE haberportalusers SET password=@Password WHERE id = @Id";

            var param = new
            {
                Password = password,
                id = id,
            };

            var affectedRows = connection.Execute(sql, param);

            TempData["Message"] = "Güncellendi.";
            TempData["MessageCss"] = "alert-success";
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditUserPassword(int id)
        {
            if (!CheckLogin())
            {
                TempData["AuthError"] = "Bu işlemi gerçekleştirmek için bir hesaba giriş yapmalısın.";
                TempData["AuthErrorCss"] = "alert-warning";
                return RedirectToAction("Login", "Home");
            }

            ViewData["username"] = HttpContext.Session.GetString("username");

            using var connection = new SqlConnection(connectionString);
            var user = connection.QuerySingleOrDefault<User>("SELECT * FROM haberportalusers WHERE id = @Id", new { Id = id });

            return View(user);
        }
    }
}