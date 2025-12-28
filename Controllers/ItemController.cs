using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;
using StacklyBackend.Utils.FormFactory;

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

        if (string.IsNullOrEmpty(selectedGroupId))
        {
            ViewBag.IsGroupSelected = false;
        }
        else
        {
            ViewBag.IsGroupSelected = true;
            var categories = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
            ViewData["categories"] = new SelectList(categories, "Id", "Name");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name,Description,Quantity,CategoryId")] ItemCreate item)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        var userId = _userManager.GetUserId(User);

        // Zabezpieczenie jeśli sesja wygasła
        if (string.IsNullOrEmpty(selectedGroupId))
        {
            return RedirectToAction("Index", "Item");
        }

        if (ModelState.IsValid)
        {
            // weryfikacja czy id grupy się zgadza, bo mozna podmienic w f12
            var category = _context.Categories.FirstOrDefault(c => c.Id == item.CategoryId);

            if (category == null || category.GroupId != selectedGroupId)
            {
                ModelState.AddModelError("CategoryId", "Invalid category for the selected group.");
            }
            else
            {
                string id;
                do
                {
                    id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
                } while (_context.Items.FirstOrDefault(p => p.Id.Equals(id)) is not null);

                var newItem = new Item
                {
                    Id = id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    CategoryId = item.CategoryId
                };

                _context.Items.Add(newItem);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // Jeśli błąd
        // wyświetl formularz a nie komunikat o wybraniu grupy
        ViewBag.IsGroupSelected = true;

        var categoriesList = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);

        ViewData["categories"] = new SelectList(categoriesList, "Id", "Name", item.CategoryId);
        // ViewData["error"] = "There has been an error while creating new item";
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

        ViewData["categories"] = _context.Categories
            .OrderBy(c => c.Name)
            .ToList()
            .ToSelectList(c => c.Id, c => c.Name);
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

        ViewData["categories"] = _context.Categories
            .OrderBy(c => c.Name)
            .ToList()
            .ToSelectList(c => c.Id, c => c.Name);
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
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload(string itemId, IFormFile file)
    {
        var userId = _userManager.GetUserId(User);
        var dbitem = Item.GetItemById(_context, itemId, userId!);
        if (dbitem is null)
            return NotFound();
        if (file == null || file.Length == 0)
            return BadRequest("File missing");

        const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
        if (file.Length > MaxFileSize)
        {
            return BadRequest("File too large. Maximum allowed size is 10 MB.");
        }

        var uploadsPath = Path.Combine(
            AppContext.BaseDirectory,
            "Uploads",
            "items",
            itemId.ToString()
        );

        Directory.CreateDirectory(uploadsPath);

        string newId;
        do
        {
            newId = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
        } while (_context.Items.FirstOrDefault(p => p.Id.Equals(newId)) is not null);

        var storedFileName = $"{newId}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, storedFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var dbFile = new ItemFile
        {
            Id = newId,
            ItemId = itemId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            ContentType = file.ContentType
        };

        _context.ItemFiles.Add(dbFile);
        await _context.SaveChangesAsync();

        return Redirect(Request.Headers.Referer.ToString());
    }

    [Authorize]
    public async Task<IActionResult> Preview(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);

        if (file == null)
            return NotFound();

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest();

        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Uploads",
            "items",
            file.ItemId.ToString(),
            file.StoredFileName
        );

        return PhysicalFile(filePath, file.ContentType);
    }

    [Authorize]
    public async Task<IActionResult> Download(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);

        if (file == null)
            return NotFound();

        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Uploads",
            "items",
            file.ItemId.ToString(),
            file.StoredFileName
        );

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        return PhysicalFile(
            filePath,
            file.ContentType,
            $"{file.Item.Id}_{file.Item.Name}"
        );
    }

    [Authorize]
    public async Task<IActionResult> RemoveUpload(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);

        if (file == null)
            return NotFound();

        _context.ItemFiles.Remove(file);
        _context.SaveChanges();
        return Redirect(Request.Headers.Referer.ToString());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _context.Dispose();
        base.Dispose(disposing);
    }
}


