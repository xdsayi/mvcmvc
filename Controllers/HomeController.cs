using Microsoft.AspNetCore.Mvc;
using mvcmvc.Data;
using mvcmvc.Models;
using System.Diagnostics;

namespace mvcmvc.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly BlogContext _context;
        private readonly FilmContext _context1;

        public HomeController(BlogContext context, FilmContext context1)
        {
            _context = context;
            _context1 = context1;
        }

        public IActionResult Index()
        {
            // Veritaban�ndan bloglar� �ekiyoruz
            var blogs = _context.Blogs.ToList();

            // Bloglar� view'e g�nderiyoruz
            return View(blogs);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Blog()
        {

            // Veritaban�ndan bloglar� �ekiyoruz
            var blogs = _context.Blogs.ToList();

            // Bloglar� view'e g�nderiyoruz
            return View(blogs);

        }

        public IActionResult BlogDetail(int id)
        {
            var blogs = _context.Blogs.ToList();
            var blog = blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        public IActionResult Film()
        {
            // Veritaban�ndan bloglar� �ekiyoruz
            var films = _context1.Films.ToList();

            // Bloglar� view'e g�nderiyoruz
            return View(films);
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
    }
}
