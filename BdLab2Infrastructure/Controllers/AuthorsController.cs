using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bd2Domain.Model;
using BdLab2Infrastructure;

namespace BdLab2Infrastructure.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ScienceArchiveContext _context;

        public AuthorsController(ScienceArchiveContext context)
        {
            _context = context;
        }
        public IActionResult SearchAuthorsBySubjectsOfAuthor(string authorLastName)
        {
            if (string.IsNullOrEmpty(authorLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище автора.";
                return View("NoResults");
            }

            string sqlQuery = @"
    SELECT DISTINCT A2.Id
    FROM Authors A1
    JOIN AuthorsPublications AP1 ON A1.Id = AP1.AuthorId
    JOIN PublicationSubject PS1 ON AP1.PublicationsId = PS1.PublicationId
    JOIN PublicationSubject PS2 ON PS1.SubjectId = PS2.SubjectId
    JOIN AuthorsPublications AP2 ON PS2.PublicationId = AP2.PublicationsId
    JOIN Authors A2 ON AP2.AuthorId = A2.Id
    WHERE A1.LastName = {0}
    AND A1.Id != A2.Id
    ";

            var authorIds = _context.Authors
                .FromSqlRaw(sqlQuery, authorLastName)
                .Select(a => a.Id)
                .ToList();

            if (!authorIds.Any())
            {
                ViewBag.Message = "Немає авторів, що відповідають вказаним критеріям.";
                return View("NoResults");
            }

            var authors = _context.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subjects)
                .Include(a => a.Department)
                .ToList();

            return View(authors);
        }
        public IActionResult SearchAuthorsByExactSubjectsOfAuthor(string authorLastName)
        {
            if (string.IsNullOrEmpty(authorLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище автора.";
                return View("NoResults");
            }

            string sqlQuery = @"
    SELECT A2.Id
    FROM Authors A1
    JOIN AuthorsPublications AP1 ON A1.Id = AP1.AuthorId
    JOIN PublicationSubject PS1 ON AP1.PublicationsId = PS1.PublicationId
    JOIN PublicationSubject PS2 ON PS1.SubjectId = PS2.SubjectId
    JOIN AuthorsPublications AP2 ON PS2.PublicationId = AP2.PublicationsId
    JOIN Authors A2 ON AP2.AuthorId = A2.Id
    WHERE A1.LastName = {0}
    AND NOT EXISTS (
        SELECT 1
        FROM PublicationSubject PS3
        WHERE PS3.PublicationId = AP2.PublicationsId
        AND PS3.SubjectId NOT IN (
            SELECT PS4.SubjectId
            FROM AuthorsPublications AP3
            JOIN PublicationSubject PS4 ON AP3.PublicationsId = PS4.PublicationId
            WHERE AP3.AuthorId = A1.Id
        )
    )
    ";

            var authorIds = _context.Authors
                .FromSqlRaw(sqlQuery, authorLastName)
                .Select(a => a.Id)
                .ToList();

            if (!authorIds.Any())
            {
                ViewBag.Message = "Немає авторів, що відповідають вказаним критеріям.";
                return View("NoResults");
            }

            var authors = _context.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subjects)
                .Include(a => a.Department)
                .ToList();

            return View(authors);
        }

        public IActionResult SearchAuthorsByExactSubjectsOnly(string authorLastName)
        {
            if (string.IsNullOrEmpty(authorLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище автора.";
                return View("NoResults");
            }

            string sqlQuery = @"
    SELECT A2.Id
    FROM Authors A1
    JOIN AuthorsPublications AP1 ON A1.Id = AP1.AuthorId
    JOIN PublicationSubject PS1 ON AP1.PublicationsId = PS1.PublicationId
    JOIN PublicationSubject PS2 ON PS1.SubjectId = PS2.SubjectId
    JOIN AuthorsPublications AP2 ON PS2.PublicationId = AP2.PublicationsId
    JOIN Authors A2 ON AP2.AuthorId = A2.Id
    WHERE A1.LastName = {0}
    AND NOT EXISTS (
        SELECT 1
        FROM PublicationSubject PS3
        WHERE PS3.PublicationId = AP2.PublicationsId
        AND PS3.SubjectId NOT IN (
            SELECT PS4.SubjectId
            FROM AuthorsPublications AP3
            JOIN PublicationSubject PS4 ON AP3.PublicationsId = PS4.PublicationId
            WHERE AP3.AuthorId = A1.Id
        )
    )
    AND NOT EXISTS (
        SELECT 1
        FROM PublicationSubject PS5
        JOIN AuthorsPublications AP4 ON PS5.PublicationId = AP4.PublicationsId
        WHERE AP4.AuthorId = A1.Id
        AND PS5.SubjectId NOT IN (
            SELECT PS6.SubjectId
            FROM PublicationSubject PS6
            WHERE PS6.PublicationId = AP2.PublicationsId
        )
    )
    ";

            var authorIds = _context.Authors
                .FromSqlRaw(sqlQuery, authorLastName)
                .Select(a => a.Id)
                .ToList();

            if (!authorIds.Any())
            {
                ViewBag.Message = "Немає авторів, що відповідають вказаним критеріям.";
                return View("NoResults");
            }

            var authors = _context.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subjects)
                .Include(a => a.Department)
                .ToList();

            return View(authors);
        }
        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var scienceArchiveContext = _context.Authors.Include(a => a.Department).ThenInclude(b => b.Organization);
            return View(await scienceArchiveContext.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Biography,LastName,FirstName,Country,Email,DepartmentId")] Author author)
        {
            ModelState.Remove("Department");
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", author.DepartmentId);
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", author.DepartmentId);
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Biography,LastName,FirstName,Country,Email,DepartmentId")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Department");
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", author.DepartmentId);
            return View(author);
        }

        // GET: Authors/Delete/5
     

        // POST: Authors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
