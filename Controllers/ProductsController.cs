using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Data;
using Musebox_Web_Project.Models;

namespace Musebox_Web_Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Musebox_Web_ProjectContext _context;
        public static string ClientName = "";

        public ProductsController(Musebox_Web_ProjectContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var my_MuseboxContext = _context.Products.Include(p => p.Brand);
            return View(await my_MuseboxContext.ToListAsync());
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public ActionResult Group()
        {

            var group = _context.Products.ToList()
                   .GroupBy(q => q.ProductType)
                   .Select(g => new Group<string, Product>
                   {
                       Key = g.Key,
                       Values = g.ToList()
                   });

            return View(group.ToList());
        }

        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetMyCart()
        {
            User user = await _context.Users.FirstAsync(u => u.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));

            var data = _context.Products.Include(b => b.Brand).Include(p => p.UserProducts).ThenInclude(u => u.User);
            var products = from p in data
                           where p.UserProducts.Any(up => up.UserId == user.UserId)
                           select p;

            return View(await products.ToListAsync());
        }

        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteFromCart(int productId)
        {
            User user = await _context.Users
                .Include(u => u.UserProducts)
                .ThenInclude(p => p.Product)
                .FirstAsync(u => u.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));

            foreach (UserProduct up in user.UserProducts)
            {
                if (up.ProductId == productId)
                {
                    user.UserProducts.Remove(up);
                    break;
                }
            }

            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetMyCart));
        }

        public async Task<IActionResult> FilterSearch(string name, int price, string type, string brand)
        {
            var my_MuseboxContext = _context.Products.Include(p => p.Brand);
            var result = from p in my_MuseboxContext
                         select p;

            if (name != null)
            {
                result = from p in result
                         where p.ProductName.Contains(name)
                         select p;
            }
            if (price > 0)
            {
                result = from p in result
                         where p.ProductPrice <= price
                         select p;
            }
            if (type != null)
            {
                result = from p in result
                         where p.ProductType.Contains(type)
                         select p;
            }
            if (brand != null)
            {
                result = from p in result
                         join b in _context.Brand on p.BrandId equals b.BrandId
                         where b.BrandName.Contains(brand)
                         select p;
            }

            return View("Index", await result.ToListAsync());
        }

        public async Task<IActionResult> SearchP(string name, int price, string type, string brand)
        {
            var my_MuseboxContext = _context.Products.Include(p => p.Brand);
            var result = from p in my_MuseboxContext
                         select p;

            if (name != null)
            {
                result = from p in result
                         where p.ProductName.Contains(name)
                         select p;
            }
            if (price > 0)
            {
                result = from p in result
                         where p.ProductPrice <= price
                         select p;
            }
            if (type != null)
            {
                result = from p in result
                         where p.ProductType.Contains(type)
                         select p;
            }
            if (brand != null)
            {
                result = from p in result
                         join b in _context.Brand on p.BrandId equals b.BrandId
                         where b.BrandName.Contains(brand)
                         select p;
            }

            return PartialView(await result.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Policy = "Admin")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brand, "BrandId", "BrandName");
            facebookCreatePost();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductType,BrandId,ImageFile")] Product product)
        {
            product.Brand = _context.Brand.SingleOrDefault(b => b.BrandId == product.BrandId);
            if (ModelState.IsValid)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    product.ImageFile.CopyTo(ms);
                    product.Image = ms.ToArray();
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, "BrandId", "BrandName", product.BrandId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, "BrandId", "BrandName", product.BrandId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductType,BrandId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brand, "BrandId", "BrandName", product.BrandId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region Private Methods


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        private void facebookCreatePost()
        {

            dynamic messagePost = new ExpandoObject();
            messagePost.message = " A new Product was added to our store! " +
                " Check it out in our website";


            string acccessToken = "EAAF0QzzmkkUBABj0glej1ZAhlf9GdW793rZC1B1D9ZBkem9Deqh5WjD7DyipTdJsyzsAQ9H42cf9XRjSiELZChaZBXNpEcKJ2anNMaLteZC83pT1YGFE5IZAI8DYRKeekhc4KLUmREwhY0mqcJz97RQwckJhH5uZA9DF9MYZCTVM4e99YKvOqyrZAp";
            FacebookClient appp = new FacebookClient(acccessToken);
            try
            {
                var postId = appp.Post("100473905172532" + "/feed", messagePost);
            }
            catch (FacebookOAuthException ex) { }

        }

        #endregion

    }

    public class Group<K, T>
    {
        public K Key { get; set; }
        public IEnumerable<T> Values { get; set; }
    }


}