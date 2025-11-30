using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

public class GroupController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public GroupController()
    {
        _context = new AppDbContext();
    }

    // GET: Group
    public ActionResult Index()
    {
        return View(_context.Groups.ToList());
    }

    // GET: Groups/Details/5
    public ActionResult Details(string id)
    {
        var groups = _context.Groups.Find(id);
        if (groups is null)
            return NotFound();

        return View(groups);
    }

    // GET: Group/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Group/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name")] GroupCreate group)
    {
        if (ModelState.IsValid)
        {
            string id;
            do
            {
                id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
            } while (_context.Groups.FirstOrDefault(p => p.Id.Equals(id)) is not null);

            _context.Groups.Add(new Group
            {
                Id = id,
                Name = group.Name
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["error"] = "There has beed an error while creating new group";
        return View(group);
    }

    // GET: Group/Edit/5
    public ActionResult Edit(string id)
    {
        var group = _context.Groups.Find(id);
        if (group is null)
            return NotFound();

        return View(group);
    }

    // POST: Group/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name")] Group group)
    {
        if (ModelState.IsValid)
        {
            var dbGroup = _context.Groups.Find(id);
            if (dbGroup is null)
                return NotFound();
            dbGroup.Name = group.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(group);
    }

    // GET: Group/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var group = _context.Groups.Find(id);
        if (group == null)
            return NotFound();

        return View(group);
    }

    // POST: Group/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var group = _context.Groups.Find(id);
        if (group is null)
            return NotFound();
        _context.Groups.Remove(group);
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