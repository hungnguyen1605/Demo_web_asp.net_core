using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webDemo.Areas.Admin.Models;
using webDemo.Extension;
using webDemo.Models;

namespace webDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles="Admin")]

    public class AccountsController : Controller
    {
        private readonly demoContext _context;

        public AccountsController(demoContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            var demoContext = _context.Accounts.Include(a => a.Role);
            return View(await demoContext.ToListAsync());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html",Name ="Login")]
        public IActionResult Login(string returnUrl)
        
        {
            var TaikhoanID = HttpContext.Session.GetString("AccountId");
            if (TaikhoanID != null) return RedirectToAction("Index", "Home", new { Areas = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("dang-nhap.html", Name = "Login")]
        public async Task<IActionResult> Login(LoignViewModel model, string returnUrl=null)
        {
            if (!ModelState.IsValid)
                return View(model);
            Account kh = _context.Accounts.Include(p => p.Role)
                .SingleOrDefault(p => p.Email.ToLower() == model.Email.ToLower().Trim());
            if (kh == null)
            {
                ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                return View(model);
            }
            //string pass = (model.Password.Trim() + kh.Salt.Trim()).toMD5();
            if (kh.Password.Trim() != model.Password)
            {
                ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                return View(model);
            }
            kh.LastLogin = DateTime.Now;
            _context.Update(kh);
            await _context.SaveChangesAsync();
            var TaikhoanID = HttpContext.Session.GetString("AccountId");
            HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());
            var UserClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,kh?.FullName),
                        new Claim(ClaimTypes.Email,kh?.Email),
                        new Claim("AccountId",kh?.AccountId.ToString()),
                       

                    };
            var roles = _context.Roles.Where(n => n.RoleId == kh.RoleId).ToList();
            foreach (var item in roles)
            {
                UserClaims.Add(new Claim(ClaimTypes.Role,item.RoleName));
                
            }
            var grandmaIdentity = new ClaimsIdentity(UserClaims, "User Identity");
            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
            await HttpContext.SignInAsync(userPrincipal);

            return Redirect("/Admin/Home/index");
        }
        // GET: Admin/Accounts/Details/5
    
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleDescription");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (ModelState.IsValid)
            {
            
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleDescription", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleDescription", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleDescription", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
