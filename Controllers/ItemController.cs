using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;
using StacklyBackend.Utils.FormFactory; // Upewnij się, że masz ten namespace

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

    // --- GET: INDEX ---
    public ActionResult Index([FromQuery] ItemQuery query)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId");
        var userId = _userManager.GetUserId(User);
        
        if (string.IsNullOrEmpty(selectedGroupId))
        {
            ViewBag.IsGroupSelected = false;
            ViewData["categories"] = new List<Category>();
            ViewData["search"] = query.Search;
            return View(new List<Item>());
        }
        
        ViewBag.IsGroupSelected = true;
        
        var items = Item.GetItemsByGroupId(_context, selectedGroupId, userId!).AsQueryable();
        
        string? search = string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search}%";

        if (!string.IsNullOrEmpty(search))
        {
            items = items.Where(i =>
                    EF.Functions.Like(i.Name, search)
                    || (i.Description != null && EF.Functions.Like(i.Description, search))
            );
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            items = items.Where(i => i.Category != null && i.Category.Name == query.Category);
        }

        if (query.MinQuantity.HasValue)
            items = items.Where(i => i.Quantity >= query.MinQuantity.Value);

        if (query.MaxQuantity.HasValue)
            items = items.Where(i => i.Quantity <= query.MaxQuantity.Value);
        
        ViewData["search"] = query.Search;
        ViewData["category"] = query.Category;
        ViewData["categories"] = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        
        return View(items.ToList());
    }

    // --- GET: CREATE ---
    public ActionResult Create()
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId");
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

    // --- POST: CREATE ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(include: "Name,Description,Quantity,CategoryId,Files")] ItemCreate item)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId");
        var userId = _userManager.GetUserId(User);
        
        if (string.IsNullOrEmpty(selectedGroupId))
        {
            return RedirectToAction("Index", "Item"); // Lub do wyboru grupy
        }

        if (ModelState.IsValid)
        {
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
                } while (_context.Items.Any(p => p.Id == id));
                
                var newItem = new Item
                {
                    Id = id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    CategoryId = item.CategoryId
                    // GroupId = selectedGroupId
                };

                _context.Items.Add(newItem);
                
                if (item.Files is not null && item.Files.Count > 0)
                {
                    const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
                    var uploadsPath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", id);
                    Directory.CreateDirectory(uploadsPath);

                    foreach (var file in item.Files)
                    {
                        if (file.Length > MaxFileSize)
                        {
                            ModelState.AddModelError("Files", $"File {file.FileName} too large. Max 10 MB.");
                            goto ErrorHandler;
                        }

                        string newId;
                        do
                        {
                            newId = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
                        } while (_context.ItemFiles.Any(p => p.Id == newId));

                        var storedFileName = $"{newId}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadsPath, storedFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var dbFile = new ItemFile
                        {
                            Id = newId,
                            ItemId = id,
                            OriginalFileName = file.FileName,
                            StoredFileName = storedFileName,
                            ContentType = file.ContentType
                        };

                        _context.ItemFiles.Add(dbFile);
                    }
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        ErrorHandler:
        
        ViewBag.IsGroupSelected = true;
        
        var categoriesList = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        
        ViewData["categories"] = new SelectList(categoriesList, "Id", "Name", item.CategoryId);
        
        // debug
        // foreach (var state in ModelState)
        // {
        //     if (state.Value.Errors.Any())
        //     {
        //         Console.WriteLine($"Field: {state.Key}, Error: {state.Value.Errors.First().ErrorMessage}");
        //     }
        // }

        return View(item);
    }

    // --- GET: DETAILS ---
    public ActionResult Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!); 
        
        if (item is null)
            return NotFound();

        return View(item);
    }

    // --- GET: EDIT ---
    public ActionResult Edit(string id)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId");
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(selectedGroupId)) return RedirectToAction("Index");

        var item = Item.GetItemById(_context, id, userId!);
        if (item is null) return NotFound();
        
        var categories = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        ViewData["categories"] = new SelectList(categories, "Id", "Name", item.CategoryId);

        return View(item);
    }

    // --- POST: EDIT ---
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind(include: "Name,Description,Quantity,CategoryId,Files")] ItemEdit item)
    {
        var selectedGroupId = HttpContext.Session.GetString("SelectedGroupId");
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(selectedGroupId)) return RedirectToAction("Index");

        if (ModelState.IsValid)
        {
            var dbitem = Item.GetItemById(_context, id, userId!);
            if (dbitem is null) return NotFound();
            
            if (!string.IsNullOrEmpty(item.CategoryId))
            {
                var cat = _context.Categories.FirstOrDefault(c => c.Id == item.CategoryId);
                if (cat == null || cat.GroupId != selectedGroupId)
                {
                    ModelState.AddModelError("CategoryId", "Invalid category.");
                    goto EditErrorHandler;
                }
                dbitem.CategoryId = item.CategoryId;
            }

            if (!string.IsNullOrEmpty(item.Name)) dbitem.Name = item.Name;
            if (!string.IsNullOrEmpty(item.Description)) dbitem.Description = item.Description;
            if (item.Quantity.HasValue) dbitem.Quantity = (int)item.Quantity;
            
            if (item.Files is not null && item.Files.Count > 0)
            {
                const long MaxFileSize = 10 * 1024 * 1024;
                var uploadsPath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", id);
                Directory.CreateDirectory(uploadsPath);

                foreach (var file in item.Files)
                {
                    if (file.Length > MaxFileSize)
                    {
                         ModelState.AddModelError("Files", $"File {file.FileName} too large.");
                         goto EditErrorHandler;
                    }
                    
                    string newId;
                    do
                    {
                        newId = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
                    } while (_context.ItemFiles.Any(p => p.Id == newId));

                    var storedFileName = $"{newId}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsPath, storedFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var dbFile = new ItemFile
                    {
                        Id = newId,
                        ItemId = id,
                        OriginalFileName = file.FileName,
                        StoredFileName = storedFileName,
                        ContentType = file.ContentType
                    };
                    _context.ItemFiles.Add(dbFile);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new {id = id});
        }

        EditErrorHandler:
        var categories = Category.GetCategoriesByGroupId(_context, selectedGroupId, userId!);
        ViewData["categories"] = new SelectList(categories, "Id", "Name", item.CategoryId);
        
        return View(item);
    }

    // --- DELETE (Mamy modala, więc niepotrzebne) ---
    // public ActionResult Delete(string id)
    // {
    //     if (id is null) return BadRequest();
    //     var userId = _userManager.GetUserId(User);
    //     var item = Item.GetItemById(_context, id, userId!);
    //     if (item == null) return NotFound();
    //     return View(item);
    // }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var userId = _userManager.GetUserId(User);
        var item = Item.GetItemById(_context, id, userId!);
        
        if (item is null) return NotFound();
        
        foreach (var file in item.Files)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", file.ItemId, file.StoredFileName);
            if (System.IO.File.Exists(filePath))
            {
                try { System.IO.File.Delete(filePath); } catch { /* ignore */ }
            }
            _context.ItemFiles.Remove(file);
        }
        
        _context.Items.Remove(item);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // --- METODY DO PLIKÓW (PREVIEW / DOWNLOAD / REMOVE) ---
    
    [Authorize]
    public async Task<IActionResult> Preview(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);
        if (file == null) return NotFound();
        if (!file.ContentType.StartsWith("image/")) return BadRequest();

        var filePath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", file.ItemId.ToString(), file.StoredFileName);
        return PhysicalFile(filePath, file.ContentType);
    }

    [Authorize]
    public async Task<IActionResult> Download(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);
        if (file == null) return NotFound();

        var filePath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", file.ItemId.ToString(), file.StoredFileName);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        return PhysicalFile(filePath, file.ContentType, $"{file.Item.Id}_{file.Item.Name}");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveUpload(string fileId)
    {
        var userId = _userManager.GetUserId(User);
        var file = ItemFile.GetFileById(_context, fileId, userId!);
        if (file == null) return NotFound();

        var filePath = Path.Combine(AppContext.BaseDirectory, "Uploads", "items", file.ItemId, file.StoredFileName);
        if (System.IO.File.Exists(filePath))
        {
            try { System.IO.File.Delete(filePath); } catch { }
        }
        _context.ItemFiles.Remove(file);
        _context.SaveChanges();
        return Redirect(Request.Headers.Referer.ToString());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _context.Dispose();
        base.Dispose(disposing);
    }
}