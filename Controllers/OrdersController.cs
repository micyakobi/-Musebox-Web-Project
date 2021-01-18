using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Data;
using Musebox_Web_Project.Models;

namespace Musebox_Web_Project.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Musebox_Web_ProjectContext _context;

        public OrdersController(Musebox_Web_ProjectContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var my_MuseboxContext = _context.Order.Include(o => o.User);
            return View(await my_MuseboxContext.ToListAsync());
        }

        public async Task<IActionResult> GetMyOrders()
        {
            var result = from o in _context.Order.Include(o => o.User)
                         join u in _context.Users on o.UserId equals u.UserId
                         where u.Email.Equals(User.FindFirstValue(ClaimTypes.Email))
                         select o;

            return View(await result.ToListAsync());
        }

        public async Task<IActionResult> FilterSearch(string name, DateTime date)
        {
            var result = from o in _context.Order.Include(u => u.User)
                         select o;

            if (name != null)
            {
                result = from o in result
                         where o.User.UserName.Contains(name)
                         select o;
            }
            if (!date.Year.Equals(0001))
            {
                result = from o in result
                         where o.OrderDate.Date.Equals(date)
                         select o;
            }

            return View("Index", await result.ToListAsync());
        }

        public async Task<IActionResult> SearchO(string name, DateTime date)
        {
            var result = from o in _context.Order.Include(u => u.User)
                         select o;

            if (name != null)
            {
                result = from o in result
                         where o.User.UserName.Contains(name)
                         select o;
            }
            if (!date.Year.Equals(0001))
            {
                result = from o in result
                         where o.OrderDate.Date.Equals(date)
                         select o;
            }

            return PartialView(await result.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(u => u.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            var productsInOrderData = from p in _context.Products
                                      .Include(b => b.Brand)
                                      .Include(op => op.OrderProducts)
                                      .ThenInclude(o => o.Order)
                                      where p.OrderProducts
                                      .Any(op => op.OrderId == order.OrderId)
                                      select p;

            if (order == null || productsInOrderData == null)
            {
                return NotFound();
            }

            ViewData["productsInOrderData"] = productsInOrderData;
            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,UserId")] Order order)
        {
            order.User = _context.Users.SingleOrDefault(u => u.UserId == order.UserId);

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,UserId,UserName")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }


    }
}
