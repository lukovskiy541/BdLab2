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
    public class ReviewsController : Controller
    {
        private readonly ScienceArchiveContext _context;

        public ReviewsController(ScienceArchiveContext context)
        {
            _context = context;
        }
        

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var scienceArchiveContext = _context.Reviews.Include(r => r.Publication).Include(r => r.Reviewer);
            return View(await scienceArchiveContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.Reviewer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Name");

            // Combine first name and last name for display in the SelectList
            ViewData["ReviewerId"] = new SelectList(_context.Reviewers.Select(r => new {
                Id = r.Id,
                FullName = r.FirstName + " " + r.LastName
            }), "Id", "FullName");

            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comment,SubmissionDate,ReviewerId,PublicationId")] Review review)
        {
            ModelState.Remove("Reviewer");
            ModelState.Remove("Publication");
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description", review.PublicationId);
            ViewData["ReviewerId"] = new SelectList(_context.Reviewers, "Id", "Email", review.ReviewerId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Name");

            // Combine first name and last name for display in the SelectList
            ViewData["ReviewerId"] = new SelectList(_context.Reviewers.Select(r => new {
                Id = r.Id,
                FullName = r.FirstName + " " + r.LastName
            }), "Id", "FullName");
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,SubmissionDate,ReviewerId,PublicationId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }
  
            ModelState.Remove("Publication");
            ModelState.Remove("Reviewer");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description", review.PublicationId);
            ViewData["ReviewerId"] = new SelectList(_context.Reviewers, "Id", "Email", review.ReviewerId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.Reviewer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
