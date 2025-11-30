using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public CategoryController()
    {
        _context = new AppDbContext();
    }

    [Authorize]
    // GET: Category
    public ActionResult Index()
    {
        // if (User.Identity == null || !User.Identity.IsAuthenticated)
        // {
        //     return RedirectToPage("/Account/Login", new { area = "Identity" });
        // }
        return View(_context.Categories.Include(i => i.Group).ToList());
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
        ViewData["groups"] = _context.Groups.ToList();
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name,GroupId")] CategoryCreate category)
    {
        if (ModelState.IsValid)
        {
            var dbCategory = _context.Categories
                       .FirstOrDefault(c => c.Name == category.Name);

            if (dbCategory is not null)
            {
                ViewData["error"] = $"Category with name: \"{category.Name}\" already exists";
            }
            else
            {
                string id;
                do
                {
                    id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
                } while (_context.Examples.FirstOrDefault(p => p.Id.Equals(id)) is not null);

                if (!_context.Groups.Any(c => c.Id == category.GroupId))
                    return NotFound();

                _context.Categories.Add(new Category
                {
                    Id = id,
                    Name = category.Name,
                    GroupId = category.GroupId
                });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        ViewData["groups"] = _context.Groups.ToList();
        return View(category);
    }

    // GET: Category/Edit/5
    public ActionResult Edit(string id)
    {
        var category = _context.Categories.Find(id);
        if (category is null)
            return NotFound();

        ViewData["groups"] = _context.Groups.ToList();
        return View(category);
    }

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name,GroupId")] Category category)
    {
        if (ModelState.IsValid)
        {
            var dbCategory = _context.Categories.Include(i => i.Group).FirstOrDefault(i => i.Id == id);
            if (dbCategory is null)
                return NotFound();

            var dbGroup = _context.Groups.Find(category.GroupId);
            if (dbGroup is null)
                return NotFound();

            var otherDbCategory = _context.Categories.FirstOrDefault(c => c.Name == category.Name);
            if (otherDbCategory is not null)
            {
                ViewData["error"] = $"Category with name: \"{category.Name}\" already exists";
            }
            else
            {
                dbCategory.Name = category.Name;
                dbCategory.GroupId = category.GroupId;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        ViewData["groups"] = _context.Groups.ToList();
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