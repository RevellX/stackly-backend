using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

public class ExampleController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public ExampleController()
    {
        _context = new AppDbContext();
    }


    // GET: Example
    public ActionResult Index()
    {
        return View(_context.Examples.ToList());
    }

    // GET: Example/Details/5
    public ActionResult Details(string id)
    {
        var example = _context.Examples.Find(id);
        if (example is null)
            return NotFound();

        return View(example);
    }

    // GET: Example/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Example/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name,Price")] ExampleCreate example)
    {
        if (ModelState.IsValid)
        {
            string id;
            do
            {
                id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
            } while (_context.Examples.FirstOrDefault(p => p.Id.Equals(id)) is not null);

            _context.Examples.Add(new Example
            {
                Id = id,
                Name = example.Name,
                Price = example.Price
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["error"] = "There has beed an error while creating new example";
        return View(example);
    }

    // GET: Example/Edit/5
    public ActionResult Edit(string id)
    {
        var example = _context.Examples.Find(id);
        if (example is null)
            return NotFound();

        return View(example);
    }

    // POST: Example/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Id,Name,Price")] Example example)
    {
        if (ModelState.IsValid)
        {
            // _context.Entry(example).State = EntityState.Modified;
            var dbExample = _context.Examples.Find(id);
            if (dbExample is null)
                return NotFound();
            dbExample.Name = example.Name;
            dbExample.Price = example.Price;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(example);
    }

    // GET: Example/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var example = _context.Examples.Find(id);
        if (example == null)
            return NotFound();

        return View(example);
    }

    // POST: Example/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var example = _context.Examples.Find(id);
        if (example is null)
            return NotFound();
        _context.Examples.Remove(example);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _context.Dispose();
        base.Dispose(disposing);
    }
}