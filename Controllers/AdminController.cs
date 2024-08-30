using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using mvcmvc.Data;
using Microsoft.EntityFrameworkCore;
using mvcmvc.Models;
using System.Reflection.Metadata;

namespace mvcmvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BlogContext _blogcontext;
        private readonly FilmContext _filmcontext;



        public AdminController(BlogContext context, FilmContext context1)
        {
            _blogcontext = context;
            _filmcontext = context1;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if(User.Identity.IsAuthenticated)
        {
                return RedirectToAction("Index", "Admin");
            }

            // Login sayfasını döndür.
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            const string adminUsername = "admin";
            const string adminPassword = "123456";

            if (username == adminUsername && password == adminPassword)
            {
                // Kullanıcı kimlik bilgilerini oluştur
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, adminUsername),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Oturumun süresi
                };

                // Kullanıcıyı oturum açmış olarak işaretle
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Başarılı giriş sonrası yönlendirme
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }


        public async Task<IActionResult> Index()
        {
            var blogs = await _blogcontext.Blogs.ToListAsync();
            var films = await _filmcontext.Films.ToListAsync();

            var viewModel = new AdminIndexViewModel
            {
                Blogs = (IEnumerable<Data.Blog>)blogs,
                Films = films
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Author,PublishDate")] Data.Blog blog, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    Console.WriteLine("ImageFile yüklendi: " + ImageFile.FileName);

                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    blog.ImagePath = $"/images/{fileName}";
                }
                else
                {
                    Console.WriteLine("ImageFile null veya boş.");

                    blog.ImagePath = null;
                    return View(blog);
                }
                _blogcontext.Add(blog);
                await _blogcontext.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            return View(blog);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _blogcontext.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Author,PublishDate,ImagePath")] Data.Blog blog, IFormFile ImageFile)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        blog.ImagePath = $"/images/{fileName}";
                    }
                    else
                    {
                        // Mevcut resim yolunu koru
                        var existingBlog = await _blogcontext.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                        blog.ImagePath = existingBlog?.ImagePath;
                    }

                    _blogcontext.Update(blog);
                    await _blogcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Hataları loglayın
                }
                return View(blog);
            }

            return View(blog);
        }


        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _blogcontext.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _blogcontext.Blogs.FindAsync(id);
            _blogcontext.Blogs.Remove(blog);
            await _blogcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _blogcontext.Blogs.Any(e => e.Id == id);
        }

        public IActionResult CreateFilm()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFilm([Bind("Id,Title,Content,Director, Genre, PublishDate")] Data.Film film, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    film.PosterPath = $"/images/{fileName}";
                }
                else
                {
                    film.PosterPath = null;
                    return View(film);
                }
                _filmcontext.Add(film);
                await _filmcontext.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            return View(film);
        }

        // GET: Admin/EditFilm/5
        public async Task<IActionResult> EditFilm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _filmcontext.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        // POST: Admin/EditFilm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFilm(int id, [Bind("Id,Title,Content,Director,Genre,ReleaseDate,PosterPath")] Film film, IFormFile ImageFile)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        film.PosterPath = $"/images/{fileName}";
                    }
                    else
                    {
                        // Mevcut resim yolunu koru
                        var existingFilm = await _filmcontext.Films.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
                        if (existingFilm == null)
                        {
                            Console.WriteLine("Mevcut blog bulunamadı.");
                        }
                        else
                        {
                            Console.WriteLine("Mevcut resim yolu: " + existingFilm.PosterPath);
                            film.PosterPath = existingFilm.PosterPath;
                        }
                    }

                    _filmcontext.Update(film);
                    await _filmcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Hataları loglayın
                }
                return View(film);
            }
            return View(film);
        }

        // GET: Admin/DeleteFilm/5
        public async Task<IActionResult> DeleteFilm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _filmcontext.Films
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Admin/DeleteFilm/5
        [HttpPost, ActionName("DeleteFilm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFilmConfirmed(int id)
        {
            var film = await _filmcontext.Films.FindAsync(id);
            _filmcontext.Films.Remove(film);
            await _filmcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _filmcontext.Films.Any(e => e.Id == id);
        }
    }
}
