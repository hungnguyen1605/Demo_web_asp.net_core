using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using webDemo.HomeViewModels;
using webDemo.Models;

namespace webDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly demoContext _context;
        public HomeController(ILogger<HomeController> logger,demoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            var Is = _context.Posts.Include(x => x.Cat).AsNoTracking().ToList();
            model.LatestPosts=Is;
            model.Populars=Is;
            model.Recents=Is;
            model.Trendings=Is;
            model.Inspiration=Is;
            model.Featured = Is.FirstOrDefault();

            return View(model);
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
}
