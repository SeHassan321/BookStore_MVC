using BookStore.Models;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using UISCOM.Models;
using UISCOM.ViewModels;

namespace UISCOM.Controllers
{
    public class BooksController : Controller
    {
        private  List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public BooksController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.OrderByDescending(b => b.Rate).ToListAsync();
            return View(books);
        }


        public async Task<IActionResult> FindByAny(string? searchItem)
        {
            bool result = double.TryParse(searchItem, out double priceOrRate);
            List<Book> books = null;
            if (result)
                books = await _context.Books.Where(b => b.Price == (int)priceOrRate || b.Rate == priceOrRate).ToListAsync();
            else
                books = await _context.Books.Where(b => b.Title.Contains(searchItem) || b.Category.Name.Contains(searchItem)).ToListAsync();
            return View("Index", books);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new BookFormViewModel
            {
                Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync(),
                Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync()
            };

            return View("BookForm", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                return View("BookForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select book poster!");
                return View("BookForm", model);
            }

            var poster = files.FirstOrDefault();

            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("BookForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("BookForm", model);
            }

            using var dataStream = new MemoryStream();

            await poster.CopyToAsync(dataStream);


            Book book = new Book
            {
                Title = model.Title,
                CategoryId = model.CategoryId,
                Year = model.Year,
                Rate = model.Rate,
                Price = model.Price,
                Storeline = model.Storeline,
                Poster = dataStream.ToArray(),
            };

            foreach (var item in model.AutherIds)
            {
                var auhtor = await _context.Authers.FindAsync(item);
                AuthorBook authorBooks = new AuthorBook()
                {
                    Author = auhtor,
                    Book = book
                };
                _context.AuthorBooks.Add(authorBooks);

            }
            _context.Books.Add(book);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Book created successfully");
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            var viewModel = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                CategoryId = book.CategoryId,
                Rate = book.Rate,
                Price = book.Price,
                Year = book.Year,
                Storeline = book.Storeline,
                Poster = book.Poster,
                Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync(),
                Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync()
            };

            return View("BookForm", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                return View("BookForm", model);
            }

            var book = await _context.Books.FindAsync(model.Id);

            if (book == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                    model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("BookForm", model);
                }

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Categories = await _context.Categories.OrderBy(m => m.Name).ToListAsync();
                    model.Authors = await _context.Authers.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("BookForm", model);
                }

                book.Poster = model.Poster;
            }

            book.Title = model.Title;
            book.CategoryId = model.CategoryId;
            book.Year = model.Year;
            book.Rate = model.Rate;
            book.Price = model.Price;
            book.Storeline = model.Storeline;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Book updated successfully");
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var book = await _context.Books.Include(b => b.Category).Include(b=>b.AuthorBooks).ThenInclude(ab=>ab.Author).SingleOrDefaultAsync(m => m.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();

            return Ok();
        }
    }
}
