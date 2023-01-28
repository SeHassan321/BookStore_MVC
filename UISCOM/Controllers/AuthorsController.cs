using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using UISCOM.Models;
using UISCOM.ViewModels;
using System.Runtime.Intrinsics.Arm;
using static System.Reflection.Metadata.BlobBuilder;

namespace UISCOM.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authers.ToListAsync());
        }



        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var auhtor = await _context.Authers.SingleOrDefaultAsync(m => m.Id == id);

            if (auhtor == null)
                return NotFound("no author found");

            var result = from a in _context.Authers
                         join ab in _context.AuthorBooks
                         on a.Id equals ab.AuthorId
                         join b in _context.Books
                         on ab.BookId equals b.Id
                         where a.Id == id
                         group new { a, b } by a.Name into g
                         select new { Name = g.Key, CountOfBooks = g.Count(), TotalPrice = g.Sum(g => g.b.Price) };

            var autherDetails = new AutherDetails();

            if (result != null)
                foreach (var item in result)
                {
                    {
                        autherDetails.AutherName = item.Name;
                        autherDetails.TotalBooksCount = item.CountOfBooks;
                        autherDetails.TotalBooksPrice = item.TotalPrice;
                    };
                    return View(autherDetails);
                }

            return View(autherDetails);

        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View("AuthorForm");
        }

        // POST: Authors/Createy
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var author = new Author
                {
                    Name = viewModel.Name
                };
                _context.Authers.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("AuthorForm", viewModel);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authers == null)
            {
                return NotFound();
            }

            var author = await _context.Authers.FindAsync(id);
            if (author == null)
            {
                return NotFound("Author Does not exist");
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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
            return View(author);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authers == null)
            {
                return NotFound();
            }

            var author = await _context.Authers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            _context.Remove(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        private bool AuthorExists(int id)
        {
            return _context.Authers.Any(e => e.Id == id);
        }
    }
}
