using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;

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

    public ActionResult Create()
    {
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
                CategoryId = item.CategoryId,
                
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["error"] = "There has beed an error while creating new example";
        return View(item);
    }
}


