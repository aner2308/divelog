using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using divelog.Models;

namespace divelog.Controllers;

public class HomeController : Controller
{
    
    //Hämtar Indexsidan
    public IActionResult Index()
    {
        return View();
    }

    //Hämtar About-sidan
    public IActionResult About()
    {
        return View();
    }

    //Sida vid error
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
