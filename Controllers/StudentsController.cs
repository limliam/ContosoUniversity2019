using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        // ASP.NET Core dependency injection takes care of passing an instance of SchoolContext
        // into the controller. You configured that in the Startup class.
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        //The controller contains an Index action method, which displays all students in the database.
        //The method gets a list of students from the Students entity set by reading the Students property of the database context instance:
        // GET: Students
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Students.ToListAsync());
        //}

        public async Task<IActionResult> Index(
             string sortOrder,
             string currentFilter,
             string searchString,
             int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            //This code receives a sortOrder parameter from the query string in the URL.
            //The query string value is provided by ASP.NET Core MVC as a parameter to the action method.
            //The parameter will be a string that's either "Name" or "Date", optionally followed by an underscore and the string "desc"
            //to specify descending order. The default sort order is ascending.
            //The first time the Index page is requested, there's no query string.
            //The students are displayed in ascending order by last name, which is the default as established by
            //the fall-through case in the switch statement. When the user clicks a column heading hyperlink,
            //the appropriate sortOrder value is provided in the query string.
            //The two ViewData elements(NameSortParm and DateSortParm) are used by the view to configure
            //the column heading hyperlinks with the appropriate query string values.

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            //The method uses LINQ to Entities to specify the column to sort by.
            //The code creates an IQueryable variable before the switch statement, modifies it in the switch statement,
            //and calls the ToListAsync method after the switch statement.
            //When you create and modify IQueryable variables, no query is sent to the database.
            //The query isn't executed until you convert the IQueryable object into a collection by calling a method such as ToListAsync.
            //Therefore, this code results in a single query that's not executed until the return View statement.

            int pageSize = 3;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
            //  return the value of pageNumber if it has a value, or return 1 if pageNumber is null.
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students
            //    .FirstOrDefaultAsync(m => m.ID == id);

            //In Controllers/ StudentsController.cs, the action method for the Details view
            //uses the FirstOrDefaultAsync method to retrieve a single Student entity.
            //Add code that calls Include.ThenInclude, and AsNoTracking methods.             
             var student = await _context.Students
                    .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.ID == id);
            // The Include and ThenInclude methods cause the context to load the Student.Enrollments navigation property,
            // and within each enrollment the Enrollment.Course navigation property.
            // The AsNoTracking method improves performance in scenarios where the entities returned won't be updated
            // in the current context's lifetime.

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //This code adds the Student entity created by the ASP.NET Core MVC model binder to the Students entity set
                //and then saves the changes to the database.
                //(Model binder refers to the ASP.NET Core MVC functionality that makes it easier for you to work with data submitted by a form;
                //a model binder converts posted form values to CLR types and passes them to the action method in parameters.
                //In this case, the model binder instantiates a Student entity for you using property values from the Form collection.)

                //You removed ID from the Bind attribute because ID is the primary key value which SQL Server will set automatically when the row is inserted.
                //Input from the user doesn't set the ID value.
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        //{
        //    if (id != student.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(student);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!StudentExists(student.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(student);
        //}

        // Updated code replacing above HttpPost Edit method
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var student = await _context.Students
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(student);
        //}

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        // POST: Students/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var student = await _context.Students.FindAsync(id);
        //    _context.Students.Remove(student);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}/*
  
** Asynchronous code **
Asynchronous programming is the default mode for ASP.NET Core and EF Core.

A web server has a limited number of threads available, and in high load situations all of the available threads might be in use. 
When that happens, the server can't process new requests until the threads are freed up. 
With synchronous code, many threads may be tied up while they aren't actually doing any work because they're waiting for I/O to complete. 
With asynchronous code, when a process is waiting for I/O to complete, its thread is freed up for the server to use for processing other requests. 
As a result, asynchronous code enables server resources to be used more efficiently, and the server is enabled to handle more traffic without delays.

Asynchronous code does introduce a small amount of overhead at run time, but for low traffic situations the performance hit is negligible, 
while for high traffic situations, the potential performance improvement is substantial.

In the following code, async, Task<T>, await, and ToListAsync make the code execute asynchronously.

- The async keyword tells the compiler to generate callbacks for parts of the method body and to automatically create the Task<IActionResult> object that's returned.
- The return type Task<IActionResult> represents ongoing work with a result of type IActionResult.
- The await keyword causes the compiler to split the method into two parts. The first part ends with the operation that's started asynchronously. 
The second part is put into a callback method that's called when the operation completes.
- ToListAsync is the asynchronous version of the ToList extension method.

Some things to be aware of when writing asynchronous code that uses EF:

- Only statements that cause queries or commands to be sent to the database are executed asynchronously. 
That includes, for example, ToListAsync, SingleOrDefaultAsync, and SaveChangesAsync. 
It doesn't include, for example, statements that just change an IQueryable, such as var students = context.Students.Where(s => s.LastName == "Davolio").
- An EF context isn't thread safe: don't try to do multiple operations in parallel. 
When you call any async EF method, always use the await keyword.
- To take advantage of the performance benefits of async code, make sure that any library packages used also use 
async if they call any EF methods that cause queries to be sent to the database.

  */
