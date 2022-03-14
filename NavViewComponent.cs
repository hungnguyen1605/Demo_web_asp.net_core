using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webDemo.Enums;
using webDemo.Models;

namespace webDemo.Controllers.Component
{
    public class NavViewComponent:ViewComponent
    {

        private readonly demoContext _context;
        private IMemoryCache _memoryCache;
        public NavViewComponent(demoContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var _IsDanhmuc = GetIscategories();
            return View(_IsDanhmuc);
        }
        public List<Category> GetIscategories()
        {
            List<Category> Istis = new List<Category>();
            Istis = _context.Categories.Where(x => x.Published == true).OrderBy(x => x.Ordering).ToList();
            return Istis;
        }
    }
}
