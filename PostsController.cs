using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using webDemo.Helpers;
using webDemo.Models;

namespace webDemo.Controllers
{
    public class PostsController : Controller
    {
        private readonly demoContext _context;

        public PostsController(demoContext context)
        {
            _context = context;
        }

        // GET: Posts
        [Route("{Alias}",Name ="ListTin")]
        public async Task<IActionResult> List(string Alias, int? page)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Home", "Index");
            var danhmuc = _context.Categories.FirstOrDefault(x => x.Alias == Alias);
            if (danhmuc == null) return RedirectToAction("Home", "Index");
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.Page_Size;
            var IsPosts = _context.Posts.Include(x => x.Cat).AsNoTracking().OrderByDescending(x => x.CreatedAt);
            PagedList<Post> model = new PagedList<Post>(IsPosts, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.danhmuc = danhmuc;


            return View(model);
        }

        // GET: Posts/Details/5
        [Route("/{Alias}.html", Name = "PostsDetails")]
        public async Task<IActionResult> Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.Alias == Alias);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
     
    }
}
