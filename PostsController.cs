using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webDemo.Helpers;
using webDemo.Models;

namespace webDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly demoContext _context;

        public PostsController(demoContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanId = HttpContext.Session.GetString("AccountId");
            if (taikhoanId == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            var Account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanId));
            if (Account == null) return NotFound();
            var data = _context.Posts.ToList();
            List<Post> Ispost = new List<Post>();
            if (Account.RoleId == 3)
            {
                Ispost = _context.Posts.Include(p => p.Account).Include(p => p.Cat).OrderByDescending(x => x.CatId).ToList();
            }
            else
            {
                Ispost = _context.Posts.Include(p => p.Account).Include(p => p.Cat)
                 .Where(x => x.AccountId == Account.AccountId)
                .OrderByDescending(x => x.CatId).ToList();
            }


            return View(Ispost);
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanId = HttpContext.Session.GetString("AccountId");
            if (taikhoanId == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            ViewData["Danhmuc"] = new SelectList(_context.Categories, "CatName", "CatId");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedAt,AccountId,Author,Tag,CatId,IsHot,IsNewfeed,Views,MetaKey,MetaDesc")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanId = HttpContext.Session.GetString("AccountId");
            if (taikhoanId == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            var accounts = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanId));
            if (accounts == null) return NotFound();
            post.Alias = Utilities.SEOURL(post.Title);
            if (ModelState.IsValid)
            {
                post.AccountId = accounts.AccountId;
                post.Author = accounts.FullName;
                if (post.CatId == null) post.CatId = 15;
                post.CreatedAt = DateTime.Now;
                post.Alias = Utilities.SEOURL(post.Title);
                post.Views = 0;
                if (fThumb != null)
                {
                    //string extension = Path.GetExtension(fThumb.FileName);
                    post.Thumb = Utilities.UploadFileToFolder(fThumb, "news");
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["Danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
        
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["Danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedAt,AccountId,Author,Tag,CatId,IsHot,IsNewfeed,Views,MetaKey,MetaDesc")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanId = HttpContext.Session.GetString("AccountId");
            if (taikhoanId == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            var accounts = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanId));
            if (accounts == null) return NotFound();
            if (accounts.RoleId != 3)
            {
                if (post.AccountId != accounts.AccountId) return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        //string extension = Path.GetExtension(fThumb.FileName);
                        post.Thumb = Utilities.UploadFileToFolder(fThumb, "news");
                    }
                    post.Alias = Utilities.SEOURL(post.Title);
                    post.AccountId = accounts.AccountId;
                    post.Author = accounts.FullName;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["Danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
