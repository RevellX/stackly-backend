using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;
using StacklyBackend.Utils.FormFactory;

namespace StacklyBackend.Controllers;

public class ItemController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public ItemController()
    {
        _context = new AppDbContext();
    }

    // GET: Item
    // Accept optional route/query parameters (bound from route or query string)
    public ActionResult Index([FromQuery] ItemQuery query)
    {
        var items = _context.Items.AsQueryable();

        string? search = string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search}%";

        if (!string.IsNullOrEmpty(search))
            items = items.Where(i =>
                EF.Functions.Like(i.Id, search)
                || EF.Functions.Like(i.Name, search)
                || (i.Description != null && EF.Functions.Like(i.Description, search))
                || (i.CategoryId != null && EF.Functions.Like(i.Category!.Name, search))
            );

        if (query.MinQuantity.HasValue)
            items = items.Where(i =>
                i.Quantity >= query.MinQuantity.Value
            );

        if (query.MaxQuantity.HasValue)
            items = items.Where(i =>
                i.Quantity <= query.MaxQuantity.Value
            );

        return View(items.ToList());
    }

    // public ActionResult Create()
    // {
    //     ViewData["categories"] = _context.Categories.ToList();
    //     return View();
    // }
    public ActionResult Create()
    {
        ViewData["categories"] = _context.Categories
            .OrderBy(c => c.Name)
            .ToList()
            .ToSelectList(c => c.Id, c => c.Name);

        return View();
    }

    [HttpPost]
    public ActionResult Create([Bind(include: "Name,Description,Quantity,CategoryId")] ItemCreate item)
    {
        if (ModelState.IsValid)
        {
            string id;
            do
            {
                id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
            } while (_context.Items.FirstOrDefault(p => p.Id.Equals(id)) is not null);

            _context.Items.Add(new Item
            {
                Id = id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                CategoryId = item.CategoryId

            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["categories"] = _context.Categories.ToList();
        ViewData["error"] = "There has beed an error while creating new item";
        return View(item);
    }

    // GET: Item/Details/5
    public ActionResult Details(string id)
    {
        var item = _context.Items.Find(id);
        if (item is null)
            return NotFound();

        return View(item);
    }

    // GET: Item/Edit/5
    public ActionResult Edit(string id)
    {
        var item = _context.Items.Find(id);
        if (item is null)
            return NotFound();

        return View(item);
    }

    // POST: Item/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name,Description,Quantity")] Item item)
    {
        if (ModelState.IsValid)
        {
            var dbitem = _context.Items.Find(id);
            if (dbitem is null)
                return NotFound();
            dbitem.Name = item.Name;
            dbitem.Description = item.Description;
            dbitem.Quantity = item.Quantity;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(item);
    }
    // GET: Item/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var item = _context.Items.Find(id);
        if (item == null)
            return NotFound();

        return View(item);
    }

    // POST: Item/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var item = _context.Items.Find(id);
        if (item is null)
            return NotFound();
        _context.Items.Remove(item);
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


