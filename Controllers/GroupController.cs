using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils;

namespace StacklyBackend.Controllers;

[Authorize]
public class GroupController : Controller
{
    private AppDbContext _context = null!;
    private UserManager<User> _userManager = null!;

    public GroupController(UserManager<User> userManager, AppDbContext dbContext)
    {
        _context = dbContext;
        _userManager = userManager;
    }

    // GET: Group
    public ActionResult Index()
    {
        var userId = _userManager.GetUserId(User);
        return View(Group.GetGroupsForUser(_context, userId!));
    }

    // GET: Groups/Details/5
    public ActionResult Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var groups = Group.GetGroupById(_context, id, userId!);
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
            } while (Group.GroupExistsById(_context, id));

            _context.Groups.Add(new Group
            {
                Id = id,
                Name = group.Name,
                OwnerId = _userManager.GetUserId(User)!
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewData["error"] = "There has been an error while creating new group";
        return View(group);
    }

    // GET: Group/Edit/5
    public ActionResult Edit(string id)
    {
        var userId = _userManager.GetUserId(User);
        var group = Group.GetGroupById(_context, id, userId!);
        if (group is null)
            return NotFound();

        return View(group);
    }

    // POST: Group/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, [Bind(include: "Name")] GroupEdit groupEdit)
    {
        var userId = _userManager.GetUserId(User);
        var dbGroup = Group.GetGroupById(_context, id, userId!);
        if (ModelState.IsValid)
        {
            if (dbGroup is null)
                return NotFound();

            // // Add user by email
            // if (!string.IsNullOrEmpty(groupEdit.AddUserEmail))
            // {
            //     var dbUser = _context.Users.FirstOrDefault(u => u.Email == groupEdit.AddUserEmail);
            //     if (dbUser != null && dbGroup.Users.All(u => u.Id != dbUser.Id))
            //     {
            //         dbGroup.Users.Add(dbUser);
            //     }
            // }
            // else
            // // Remove user by email
            // if (!string.IsNullOrEmpty(groupEdit.RemoveUserEmail))
            // {
            //     var removeUser = dbGroup.Users.FirstOrDefault(u => u.Email == groupEdit.RemoveUserEmail);
            //     if (removeUser != null)
            //     {
            //         dbGroup.Users.Remove(removeUser);
            //     }
            // }

            if (!string.IsNullOrEmpty(groupEdit.Name))
                dbGroup.Name = groupEdit.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(dbGroup);
    }

    // GET: Group/Delete/5
    public ActionResult Delete(string id)
    {
        if (id is null)
            return BadRequest();

        var userId = _userManager.GetUserId(User);
        var group = Group.GetGroupById(_context, id, userId!);
        if (group == null)
            return NotFound();

        return View(group);
    }

    // POST: Group/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
        var userId = _userManager.GetUserId(User);
        var group = Group.GetGroupById(_context, id, userId!);
        if (group is null)
            return NotFound();
        _context.Groups.Remove(group);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // POST: Group/SetGroup
    [HttpPost]
    public IActionResult SetGroup(string selectedGroupId)
    {
        HttpContext.Session.SetString("SelectedGroupId", selectedGroupId);
        return Redirect(Request.Headers["Referer"].ToString());
    }

    // POST: Group/AddUser
    [HttpPost]
    public IActionResult AddUser([Bind(include: "GroupId,AddUserEmail")] GroupEdit groupEdit)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            if (!Group.IsUserGroupMember(_context, groupEdit.GroupId!, userId!))
            {
                return Forbid();
            }
            var dbGroup = Group.GetGroupById(_context, groupEdit.GroupId ?? "", userId!);
            if (dbGroup is not null)
            {
                var dbUser = _context.Users.FirstOrDefault(u => u.Email == groupEdit.AddUserEmail);
                if (dbUser != null && dbGroup.Users.All(u => u.Id != dbUser.Id))
                {
                    dbGroup.Users.Add(dbUser);
                    _context.SaveChanges();
                }
            }
        }
        return Redirect(Request.Headers["Referer"].ToString());
    }

    // POST: Group/RemoveGroup
    [HttpPost]
    public IActionResult RemoveUser([Bind(include: "GroupId,RemoveUserEmail")] GroupEdit groupEdit)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            if (!Group.IsUserGroupMember(_context, groupEdit.GroupId!, userId!))
            {
                return Forbid();
            }
            var dbGroup = Group.GetGroupById(_context, groupEdit.GroupId ?? "", userId!);
            if (dbGroup is not null)
            {
                var removeUser = dbGroup.Users.FirstOrDefault(u => u.Email == groupEdit.RemoveUserEmail);
                if (removeUser != null)
                {
                    dbGroup.Users.Remove(removeUser);
                    _context.SaveChanges();
                }
            }
        }
        return Redirect(Request.Headers["Referer"].ToString());
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _context.Dispose();
        base.Dispose(disposing);
    }
}