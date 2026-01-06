using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private AppDbContext _context = null!;
    private UserManager<User> _userManager = null!;

    public CategoryController(UserManager<User> userManager, AppDbContext dbContext)
    {
        _context = dbContext;
        _userManager = userManager;
    }

    [Authorize]
    // GET: Category
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var selectedId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        var categories = Category.GetCategoriesByGroupId(_context, selectedId!, userId!);
        ViewBag.IsGroupSelected = categories.Count() == 0 ? false : true;
        return View(categories);

        // return View(Category.GetCategoriesByGroupId(_context, selectedId, userId!));
    }

    // GET: Category/Details/5
    public ActionResult Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var category = Category.GetCategoryById(_context, id, userId!);
        if (category is null)
            return NotFound();

        return View(category);
    }

    // GET: Category/Create
    public ActionResult Create()
    {
        var userId = _userManager.GetUserId(User);
        var selectedId = HttpContext.Session.GetString("SelectedGroupId") ?? "";
        if (string.IsNullOrEmpty(selectedId))
        {
            return RedirectToAction("Index");
        }
        ViewData["groups"] = Group.GetGroupsForUser(_context, userId!);
        return View(new CategoryCreate { GroupId = selectedId });
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(include: "Name,GroupId")] CategoryCreate category)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid)
        {
            if (!Group.IsUserGroupMember(_context, category.GroupId, userId!))
            {
                return Forbid();
            }
            if (Category.IsCategoryInGroup(_context, category.GroupId, category.Name))
            {
                ViewData["error"] = $"Category with name: \"{category.Name}\" already exists";
            }
            else
            {
                string id;
                do
                {
                    id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
                } while (Category.CategoryExistsById(_context, id));

                if (!Group.GroupExistsById(_context, category.GroupId))
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
        ViewData["groups"] = Group.GetGroupsForUser(_context, userId!);
        return View(category);
    }

    // GET: Category/Edit/5
    public ActionResult Edit(string id)
    {
        var userId = _userManager.GetUserId(User);

        var category = Category.GetCategoryById(_context, id, userId!);
        if (category is null)
            return NotFound();

        ViewData["groups"] = Group.GetGroupsForUser(_context, userId!);

        return View(category);
    }

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name,GroupId")] CategoryEdit categoryEdit)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid)
        {
            var dbCategory = Category.GetCategoryById(_context, id, userId!);
            if (dbCategory is null)
                return NotFound();

            var dbGroup = Group.GetGroupById(_context, categoryEdit.GroupId!, userId!);
            if (dbGroup is null)
                return NotFound();

            if (Category.IsCategoryInGroup(_context, categoryEdit.GroupId!, categoryEdit.Name!))
            {
                ViewData["error"] = $"Category with name: \"{categoryEdit.Name}\" already exists";
            }
            else
            {
                if (!string.IsNullOrEmpty(categoryEdit.Name))
                    dbCategory.Name = categoryEdit.Name;
                if (!string.IsNullOrEmpty(categoryEdit.GroupId))
                    dbCategory.GroupId = categoryEdit.GroupId!;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        ViewData["groups"] = Group.GetGroupsForUser(_context, userId!);
        return View(categoryEdit);
    }

    // Bo mamy modal do usuwania
    // GET: Category/Delete/5
    // public ActionResult Delete(string id)
    // {
    //     var userId = _userManager.GetUserId(User);
    //
    //     if (id is null)
    //         return BadRequest();
    //
    //     var category = Category.GetCategoryById(_context, id, userId!);
    //     if (category == null)
    //         return NotFound();
    //
    //     return View(category);
    // }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var userId = _userManager.GetUserId(User);
        var category = Category.GetCategoryById(_context, id, userId!);
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