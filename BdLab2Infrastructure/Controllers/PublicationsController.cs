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
    public class PublicationsController : Controller
    {
        private readonly ScienceArchiveContext _context;

        public PublicationsController(ScienceArchiveContext context)
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
        public IActionResult SearchSubjectsByAuthor(string authorLastName)
        {
            if (string.IsNullOrEmpty(authorLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище автора.";
                return View("NoResults");
            }

            // SQL запит для отримання унікальних ID тематик, на які писав автор
            string sqlQuery = @"
    SELECT DISTINCT S.Id, S.Name
    FROM Authors A
    JOIN AuthorsPublications AP ON A.Id = AP.AuthorId
    JOIN Publications P ON AP.PublicationsId = P.Id
    JOIN PublicationSubject PS ON P.Id = PS.PublicationId
    JOIN Subjects S ON PS.SubjectId = S.Id
    WHERE A.LastName = {0}
    ";

            // Виконання SQL запиту та конвертація результату в список
            var subjects = _context.Subjects
                .FromSqlRaw(sqlQuery, authorLastName)
                .ToList();

            if (!subjects.Any())
            {
                ViewBag.Message = "Немає тематик, що відповідають вказаному автору.";
                return View("NoResults");
            }

            // Передаємо знайдені тематики у представлення
            return View(subjects);
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



        public IActionResult SearchPublicationsByPublicationSubjects(string publicationName)
        {
            if (string.IsNullOrEmpty(publicationName))
            {
                ViewBag.Message = "Please enter a publication name.";
                return View("NoResults");
            }

            // SQL-запит для отримання списку Id публікацій
            string sqlQuery = @"
        SELECT P1.Id
        FROM Publications P1
        WHERE NOT EXISTS (
            SELECT 1
            FROM PublicationSubject PS1
            WHERE PS1.PublicationId = P1.Id
            AND PS1.SubjectId NOT IN (
                SELECT PS2.SubjectId
                FROM Publications P2
                JOIN PublicationSubject PS2 ON P2.Id = PS2.PublicationId
                WHERE P2.Name = {0}
            )
        )
        AND NOT EXISTS (
            SELECT 1
            FROM PublicationSubject PS3
            JOIN Publications P3 ON P3.Id = PS3.PublicationId
            WHERE P3.Name = {0}
            AND PS3.SubjectId NOT IN (
                SELECT PS4.SubjectId
                FROM PublicationSubject PS4
                WHERE PS4.PublicationId = P1.Id
            )
        )
    ";

            // Виконання запиту через контекст бази даних для отримання списку Id
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, publicationName)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "No publications found with the specified subjects.";
                return View("NoResults");
            }

            // Отримання повних об'єктів публікацій за списком Id
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Subjects) 
                .Include(p=> p.Authors)
                .Include(p=> p.Journal)// Додаємо зв'язок з тематиками, якщо потрібно
                .ToList();

            // Передача результатів до представлення
            return View(publications);
        }


        public IActionResult SearchPublicationsByPublicationNameWithAdditionalAuthors(string publicationName)
        {
            if (string.IsNullOrEmpty(publicationName))
            {
                ViewBag.Message = "Please enter a publication name.";
                return View("NoResults");
            }

            // SQL-запит для отримання списку Id публікацій
            string sqlQuery = @"
    SELECT DISTINCT P1.Id
    FROM Publications P1
    JOIN AuthorsPublications AP1 ON P1.Id = AP1.PublicationsId
    WHERE P1.Id IN (
        SELECT AP2.PublicationsId
        FROM AuthorsPublications AP2
        WHERE AP2.AuthorId IN (
            SELECT AP3.AuthorId
            FROM Publications P2
            JOIN AuthorsPublications AP3 ON P2.Id = AP3.PublicationsId
            WHERE P2.Name = {0}
        )
        GROUP BY AP2.PublicationsId
        HAVING COUNT(DISTINCT AP2.AuthorId) = (
            SELECT COUNT(DISTINCT AP4.AuthorId)
            FROM Publications P3
            JOIN AuthorsPublications AP4 ON P3.Id = AP4.PublicationsId
            WHERE P3.Name = {0}
        )
    )
    AND P1.Id IN (
        SELECT AP5.PublicationsId
        FROM AuthorsPublications AP5
        GROUP BY AP5.PublicationsId
        HAVING COUNT(DISTINCT AP5.AuthorId) > (
            SELECT COUNT(DISTINCT AP6.AuthorId)
            FROM Publications P4
            JOIN AuthorsPublications AP6 ON P4.Id = AP6.PublicationsId
            WHERE P4.Name = {0}
        )
    )
";

            // Виконання запиту через контекст бази даних для отримання списку Id
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, publicationName)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "No publications found with the specified authors and criteria.";
                return View("NoResults");
            }

            // Отримання повних об'єктів публікацій за списком Id
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                 .Include(p => p.Journal)
                 .Include(p => p.Type)
                // Додаємо зв'язок з авторами, якщо потрібно
                .ToList();

            // Передача результатів до представлення
            return View(publications);
        }

        public IActionResult SearchPublicationsByPublicationName(string publicationName)
        {
            if (string.IsNullOrEmpty(publicationName))
            {
                ViewBag.Message = "Please enter a publication name.";
                return View("NoResults");
            }

            // SQL-запит для отримання списку Id публікацій
            string sqlQuery = @"
        SELECT DISTINCT P1.Id
        FROM Publications P1
        JOIN AuthorsPublications AP1 ON P1.Id = AP1.PublicationsId
        WHERE AP1.AuthorId IN (
            SELECT AP2.AuthorId
            FROM Publications P2
            JOIN AuthorsPublications AP2 ON P2.Id = AP2.PublicationsId
            WHERE P2.Name = {0}
        )";

            // Виконання запиту через контекст бази даних для отримання списку Id
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, publicationName)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "No publications found with the specified authors.";
                return View("NoResults");
            }

            // Отримання повних об'єктів публікацій за списком Id
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Authors)
                 .Include(p => p.Subjects)
             .Include(p => p.Journal)
             .Include(p => p.Type)// Додаємо зв'язок з авторами, якщо потрібно
                .ToList();

            // Передача результатів до представлення
            return View(publications);
        }




        [HttpGet]
        public IActionResult SearchByAuthor(string authorLastName)
        {
            if (string.IsNullOrEmpty(authorLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище автора.";
                return View("NoResults");
            }

            // SQL-запит для отримання списку Id публікацій
            string sqlQuery = @"
        SELECT P.Id
        FROM Publications P
        JOIN AuthorsPublications AP ON P.Id = AP.PublicationsId
        JOIN Authors A ON AP.AuthorId = A.Id
        WHERE A.LastName = {0}
    ";

            // Виконання запиту через контекст бази даних для отримання списку Id
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, authorLastName)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "Немає публікацій для вказаного автора.";
                return View("NoResults");
            }

            // Отримання повних об'єктів публікацій за списком Id
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                .Include(p => p.Journal)
                .Include(p => p.Type)
                .ToList();

            // Передача результатів до представлення
            return View(publications);
        }
        [HttpGet]
        public IActionResult SearchByLanguageAndCitations(string language, int minCitations)
        {
            if (string.IsNullOrEmpty(language))
            {
                ViewBag.Message = "Будь ласка, введіть мову.";
                return View("NoResults");
            }

            // SQL-запит для отримання списку Id публікацій
            string sqlQuery = @"
        SELECT P.Id
        FROM Publications P
        WHERE P.Language = {0} AND P.CitationsNumber >= {1}
    ";

            // Виконання запиту через контекст бази даних для отримання списку Id
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, language, minCitations)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "Немає публікацій для вказаної мови з мінімальною кількістю цитувань.";
                return View("NoResults");
            }

            // Отримання повних об'єктів публікацій за списком Id
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                .Include(p => p.Journal)
                .Include(p => p.Type)
                .ToList();

            // Передача результатів до представлення
            return View(publications);
        }


        [HttpGet]
        public IActionResult SearchAuthorsByPublicationCount(string subjectName, int publicationCount)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                ViewBag.Message = "Будь ласка, введіть назву тематики.";
                return View("NoResults");
            }

            
            string sqlQuery = @"
        SELECT A.Id
        FROM Authors A
        WHERE (
            SELECT COUNT(*)
            FROM AuthorsPublications AP
            JOIN Publications P ON AP.PublicationsId = P.Id
            JOIN PublicationSubject PS ON P.Id = PS.PublicationId
            JOIN Subjects S ON PS.SubjectId = S.Id
            WHERE AP.AuthorId = A.Id AND S.Name = {0}
        ) > {1}
    ";

           
            var authorIds = _context.Authors
                .FromSqlRaw(sqlQuery, subjectName, publicationCount)
                .Select(a => a.Id)
                .ToList();

            if (!authorIds.Any())
            {
                ViewBag.Message = "Немає авторів, які відповідають вказаним критеріям.";
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

        [HttpGet]
        public IActionResult SearchAuthorsByDepartment(string departmentName)
        {
            if (string.IsNullOrEmpty(departmentName))
            {
                ViewBag.Message = "Будь ласка, введіть назву департаменту.";
                return View("NoResults");
            }

            string sqlQuery = @"
        SELECT A.Id
        FROM Authors A
        JOIN Departments D ON A.DepartmentId = D.Id
        WHERE D.Name = {0}
    ";

    
            var authorIds = _context.Authors
                .FromSqlRaw(sqlQuery, departmentName)
                .Select(a => a.Id)
                .ToList();

            if (!authorIds.Any())
            {
                ViewBag.Message = "Немає авторів у вказаному департаменті.";
                return View("NoResults");
            }

          
            var authors = _context.Authors
                .Where(a => authorIds.Contains(a.Id))
                .Include(a => a.Department)
                    .ThenInclude(d => d.Organization)
                .Include(a => a.Publications)
                    .ThenInclude(p => p.Subjects)
                .ToList();

     
            return View(authors);
        }

        public IActionResult SearchPublicationsByReviewerLastName(string reviewerLastName)
        {
            if (string.IsNullOrEmpty(reviewerLastName))
            {
                ViewBag.Message = "Будь ласка, введіть прізвище рецензента.";
                return View("NoResults");
            }

           
            string sqlQuery = @"
        SELECT DISTINCT P.Id
        FROM Publications P
        JOIN Review R ON P.Id = R.PublicationId
        JOIN Reviewers Rev ON R.ReviewerId = Rev.Id
        WHERE Rev.LastName = {0}
    ";

          
            var publicationIds = _context.Publications
                .FromSqlRaw(sqlQuery, reviewerLastName)
                .Select(p => p.Id)
                .ToList();

            if (!publicationIds.Any())
            {
                ViewBag.Message = "Немає публікацій для вказаного рецензента.";
                return View("NoResults");
            }

            
            var publications = _context.Publications
                .Where(p => publicationIds.Contains(p.Id))
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                .Include(p => p.Journal)
                .Include(p => p.Type)
                .ToList();

          
            return View(publications);
        }


        // GET: Publications
        public async Task<IActionResult> Index()
        {
            var scienceArchiveContext = _context.Publications.Include(p => p.Journal).Include(p => p.Subjects).Include(p => p.Type).Include(p => p.Authors);
            return View(await scienceArchiveContext.ToListAsync());
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Journal)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // GET: Publications/Create
        public IActionResult Create()
        {
           
            ViewData["AuthorId"] = new SelectList(_context.Authors.Select(r => new {
                Id = r.Id,
                FullName = r.FirstName + " " + r.LastName
            }), "Id", "FullName");

            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "Name");
            ViewData["TypeId"] = new SelectList(_context.PublicationTypes, "Id", "TypeName");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            
            return View();
        }

        // POST: Publications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,PublicationDate,AuthorId,SubjectId,Keywords,Language,PdfUrl,CitationsNumber,Status,JournalId,TypeId,Name")] Publication publication, List<int> selectedAuthorIds, List<int> selectedSubjectsIds)
        {
            ModelState.Remove("Type");
            ModelState.Remove("Journal");
            
            if (ModelState.IsValid)
            {
                foreach (var authorId in selectedAuthorIds)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        publication.Authors.Add(author);
                    }
                }
                foreach (var selectedSubject in selectedSubjectsIds)
                {
                    var subject = await _context.Subjects.FindAsync(selectedSubject);
                    if (subject != null)
                    {
                        publication.Subjects.Add(subject);
                    }
                }
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "Issn", publication.JournalId);
            ViewData["TypeId"] = new SelectList(_context.PublicationTypes, "Id", "TypeName", publication.TypeId);
            return View(publication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publication == null)
            {
                return NotFound();
            }

            var currentAuthorIds = publication.Authors.Select(a => a.Id).ToList();
            var authors = await _context.Authors
                                .Where(a => !currentAuthorIds.Contains(a.Id))
                                .ToListAsync();

            ViewBag.Authors = new SelectList(authors.Select(a => new {
                Id = a.Id,
                FullName = a.FirstName + " " + a.LastName
            }), "Id", "FullName");

            var currentSubjectIds = publication.Subjects.Select(s => s.Id).ToList();
            var subjects = await _context.Subjects
                                .Where(s => !currentSubjectIds.Contains(s.Id))
                                .ToListAsync();

            ViewBag.Subjects = new SelectList(subjects.Select(s => new {
                Id = s.Id,
                Name = s.Name
            }), "Id", "Name");

            var selectedAuthorIds = publication.Authors.Select(a => a.Id).ToList();
            var selectedSubjectIds = publication.Subjects.Select(s => s.Id).ToList();

            ViewBag.CurrentAuthorIds = selectedAuthorIds;
            ViewBag.CurrentSubjectIds = selectedSubjectIds;
            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "Name", publication.JournalId);
            ViewData["TypeId"] = new SelectList(_context.PublicationTypes, "Id", "TypeName", publication.TypeId);

            return View(publication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,PublicationDate,AuthorId,SubjectId,Keywords,Language,PdfUrl,CitationsNumber,Status,JournalId,TypeId,Name")] Publication publication, List<int> authorsToRemove, List<int> selectedAuthorIds, List<int> subjectsToRemove, List<int> selectedSubjectIds)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            var existingPublication = await _context.Publications
                .Include(p => p.Authors)
                .Include(p => p.Subjects)
                .FirstOrDefaultAsync(p => p.Id == id);

            existingPublication.Description = publication.Description;
            existingPublication.PublicationDate = publication.PublicationDate;
            existingPublication.AuthorId = publication.AuthorId;
            existingPublication.SubjectId = publication.SubjectId;
            existingPublication.Keywords = publication.Keywords;
            existingPublication.Language = publication.Language;
            existingPublication.PdfUrl = publication.PdfUrl;
            existingPublication.CitationsNumber = publication.CitationsNumber;
            existingPublication.Status = publication.Status;
            existingPublication.JournalId = publication.JournalId;
            existingPublication.TypeId = publication.TypeId;
            existingPublication.Name = publication.Name;

            ModelState.Remove("Type");
            ModelState.Remove("Journal");

            if (ModelState.IsValid)
            {
                foreach (var authorId in authorsToRemove)
                {
                    var author = existingPublication.Authors.FirstOrDefault(a => a.Id == authorId);
                    if (author != null)
                    {
                        existingPublication.Authors.Remove(author);
                    }
                }

                foreach (var authorId in selectedAuthorIds)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        existingPublication.Authors.Add(author);
                    }
                }

                foreach (var subjectId in subjectsToRemove)
                {
                    var subject = existingPublication.Subjects.FirstOrDefault(s => s.Id == subjectId);
                    if (subject != null)
                    {
                        existingPublication.Subjects.Remove(subject);
                    }
                }

                foreach (var subjectId in selectedSubjectIds)
                {
                    var subject = await _context.Subjects.FindAsync(subjectId);
                    if (subject != null)
                    {
                        existingPublication.Subjects.Add(subject);
                    }
                }

                try
                {
                    _context.Update(existingPublication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
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

            ViewData["JournalId"] = new SelectList(_context.Journals, "Id", "Issn", publication.JournalId);
            ViewData["TypeId"] = new SelectList(_context.PublicationTypes, "Id", "TypeName", publication.TypeId);
            return View(publication);
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Journal)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicationExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}
