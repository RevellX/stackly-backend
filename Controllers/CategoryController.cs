using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

public class CategoryController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public CategoryController()
    {
        _context = new AppDbContext();
    }


    // GET: Category
    public ActionResult Index()
    {
        return View(_context.Categories.ToList());
    }

    // GET: Category/Details/5
    public ActionResult Details(string id)
    {
        var category = _context.Categories.Find(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    // GET: Category/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name,DisplayName")] Category category)
    {
        if (ModelState.IsValid)
        {
            var dbCategory = _context.Categories.Find(category.Name);
            if (dbCategory is not null)
            {
                _context.Categories.Add(new Category
                {
                    Name = category.Name,
                    DisplayName = category.DisplayName
                });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        ViewData["error"] = "There has been an error while creating new category";
        return View(category);
    }

    // GET: Category/Edit/5
    public ActionResult Edit(string id)
    {
        var category = _context.Categories.Find(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name, DisplayName")] Category category)
    {
        if (ModelState.IsValid)
        {
            var dbCategory = _context.Categories.Find(id);
            if (dbCategory is null)
                return NotFound();
            dbCategory.Name = category.Name;
            dbCategory.DisplayName = category.DisplayName;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["error"] = "There has been an error while editing category";
        return View(category);
    }

    // GET: Category/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var category = _context.Categories.Find(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var category = _context.Categories.Find(id);
        if (category is null)
            return NotFound();
        _context.Categories.Remove(category);
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