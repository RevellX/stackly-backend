using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;

namespace StacklyBackend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["items_stored_total"] = 0;
        ViewData["categories_stored_total"] = 0;
        ViewData["users_stored_total"] = 0;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
