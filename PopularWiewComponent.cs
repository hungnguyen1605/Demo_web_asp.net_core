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
    public class PopularWiewComponent: ViewComponent
    {
        private readonly demoContext _context;
        private IMemoryCache _memoryCache;
        public PopularWiewComponent(demoContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var tinseo = _memoryCache.GetOrCreate(CacheKeys.Categories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetIsPost();
            });
            return View(tinseo);
        }
        public List<Post> GetIsPost()
        {
            List<Post> Istis = new List<Post>();
            Istis = _context.Posts.Where(x => x.Published == true).OrderBy(x => x.Views).Take(6).ToList();
            return Istis;
        }
    }
}
