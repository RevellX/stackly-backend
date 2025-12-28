using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

[Authorize]
public class ItemController : Controller
{
    private AppDbContext _context = null!;
    private UserManager<User> _userManager = null!;

    public ItemController(UserManager<User> userManager, AppDbContext dbContext)
    {
        _context = dbContext;
        _userManager = userManager;
    }

    // GET: Item
    // Accept optional route/query parameters (bound from route or query string)
    public ActionResult Index([FromQuery] ItemQuery query)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        var userId = _userManager.GetUserId(User);
        var items = Item.GetItemsByGroupId(_context, selectedGroupId, userId!).AsQueryable();

        string? search = string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search}%";

        if (!string.IsNullOrEmpty(search))
            items = items.Where(i =>
                EF.Functions.Like(i.Id, search)
                || EF.Functions.Like(i.Name, search)
                || (i.Description != null && EF.Functions.Like(i.Description, search))
            // || EF.Functions.Like(i.Category!.Name, search)
            );

        if (!string.IsNullOrEmpty(query.Category))
            items = items.Where(i =>
            EF.Functions.Like(i.Category.Name, query.Category)
            );

        if (query.MinQuantity.HasValue)
            items = items.Where(i =>
                i.Quantity >= query.MinQuantity.Value
            );

        if (query.MaxQuantity.HasValue)
            items = items.Where(i =>
                i.Quantity <= query.MaxQuantity.Value
            );
        ViewData["search"] = query.Search;
        ViewData["category"] = query.Category;
        ViewData["categories"] = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        return View(items.ToList());
    }

    public ActionResult Create()
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        var userId = _userManager.GetUserId(User);
        ViewData["categories"] = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        return View();
    }

    [HttpPost]
    public ActionResult Create([Bind(include: "Name,Description,Quantity,CategoryId")] ItemCreate item)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        var userId = _userManager.GetUserId(User);
        var dbCategory = Category.GetCategoryById(_context, item.CategoryId, userId!);
        if (dbCategory is null)
            return NotFound();
        if (!Group.IsUserGroupMember(_context, dbCategory.GroupId, userId!))
        {
            return Forbid();
        }
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
        ViewData["error"] = "There has been an error while creating new item";
        return View(item);
    }

    // GET: Item/Details/5
    public ActionResult Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!);
        if (item is null)
            return NotFound();

        return View(item);
    }

    // GET: Item/Edit/5
    public ActionResult Edit(string id)
    {
        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!);
        if (item is null)
            return NotFound();
        ViewData["categories"] = _context.Categories.ToList();
        return View(item);
    }

    // POST: Item/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name,Description,Quantity,CategoryId")] ItemEdit item)
    {
        if (ModelState.IsValid)
        {

            var userId = _userManager.GetUserId(User);
            var dbitem = Item.GetItemById(_context, id, userId!);
            if (dbitem is null)
                return NotFound();


            if (!string.IsNullOrEmpty(item.Name))
                dbitem.Name = item.Name;
            if (!string.IsNullOrEmpty(item.Description))
                dbitem.Description = item.Description;
            if (item.Quantity.HasValue)
                dbitem.Quantity = (int)item.Quantity;
            if (!string.IsNullOrEmpty(item.CategoryId))
            {
                if (!Category.UserCanAccessCategory(_context, item.CategoryId, userId!))
                    return NotFound();
                dbitem.CategoryId = item.CategoryId;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["categories"] = _context.Categories.ToList();
        return View(item);
    }
    // GET: Item/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!);
        if (item == null)
            return NotFound();

        return View(item);
    }

    // POST: Item/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!);
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


