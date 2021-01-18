using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Data;
using Musebox_Web_Project.Models;

namespace Musebox_Web_Project.Controllers
{
    public class UsersController : Controller
    {
        private readonly Musebox_Web_ProjectContext _context;

        public UsersController(Musebox_Web_ProjectContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl)
        {

            User user = await _context.Users.Include(up => up.UserProducts).FirstAsync(u => u.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
            Product productToAdd = await _context.Products.Include(b => b.Brand).FirstAsync(p => p.ProductId == productId);

            foreach (UserProduct item in user.UserProducts)
                if (item.ProductId == productId)
                    return RedirectToAction("Catalog", "Home");

            if (productToAdd != null)
            {
                if (user.UserProducts == null)
                    user.UserProducts = new List<UserProduct>();

                user.UserProducts.Add(new UserProduct()
                {
                    ProductId = productToAdd.ProductId,
                    Product = productToAdd,
                    UserId = user.UserId,
                    User = user
                });

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Catalog", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Purchase()
        {
            User user = await _context.Users.Include(u => u.Orders)
                .Include(up => up.UserProducts)
                .ThenInclude(p => p.Product)
                .FirstAsync(u => u.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));

            if (user.UserProducts.Count() != 0)
            {
                var data = _context.Products.Include(b => b.Brand).Include(p => p.UserProducts).ThenInclude(u => u.User);
                var products = from p in data
                               where p.UserProducts.Any(up => up.UserId == user.UserId)
                               select p;

                Order newOrder = new Order()
                {
                    UserId = user.UserId,
                    User = user,
                    OrderDate = DateTime.Now,
                    OrderProducts = new List<OrderProduct>()
                };

                var addedOrder = _context.Add(newOrder).Entity;
                await _context.SaveChangesAsync();

                foreach (var item in products)
                {

                    addedOrder.OrderProducts.Add(new OrderProduct()
                    {
                        OrderId = addedOrder.OrderId,
                        Order = addedOrder,
                        ProductId = item.ProductId,
                        Product = item
                    });

                }

                _context.Update(addedOrder);
                await _context.SaveChangesAsync();

                if (user.Orders == null)
                    user.Orders = new List<Order>();

                user.Orders.Add(addedOrder);
                user.UserProducts.Clear();

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("GetMyOrders", "Orders");

        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SearchU(string userName, string firstName, string lastName, string email)
        {
            var result = from u in _context.Users
                         select u;

            if (userName != null)
            {
                result = from u in result
                         where u.UserName.Contains(userName)
                         select u;
            }
            if (firstName != null)
            {
                result = from u in result
                         where u.FirstName.Contains(firstName)
                         select u;
            }
            if (lastName != null)
            {
                result = from u in result
                         where u.LastName.Contains(lastName)
                         select u;
            }
            if (email != null)
            {
                result = from u in result
                         where u.Email.Contains(email)
                         select u;
            }

            return PartialView(await result.ToListAsync());
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> FilterSearch(string userName, string firstName, string lastName, string email)
        {
            var result = from u in _context.Users
                         select u;

            if (userName != null)
            {
                result = from u in result
                         where u.UserName.Contains(userName)
                         select u;
            }
            if (firstName != null)
            {
                result = from u in result
                         where u.FirstName.Contains(firstName)
                         select u;
            }
            if (lastName != null)
            {
                result = from u in result
                         where u.LastName.Contains(lastName)
                         select u;
            }
            if (email != null)
            {
                result = from u in result
                         where u.Email.Contains(email)
                         select u;
            }

            return View("Index", await result.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [Authorize(Policy = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create([Bind("UserId,UserName,FirstName,LastName,Password,Email,IsManager")] User user)
        {
            if (user.IsManager)
                user.UserType = UserType.Admin;
            else user.UserType = UserType.Customer;

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FirstName,LastName,Password,Email,IsManager")] User user)
        {
            if (user.IsManager)
                user.UserType = UserType.Admin;
            else user.UserType = UserType.Customer;

            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }


        //Registration and login/logout

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password)
        {
            User userOrNull = _context.Users.SingleOrDefault(user => user.Email == email && user.Password == password);

            if (userOrNull == null)
            {
                // User doesn't exist - redirect to login page
                return RedirectToAction("Login", "Users");
            }
            else
            {
                // Found user with same email - login
                await SignInSession(userOrNull);
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        // ToDo: Move to another place
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string firstName, string lastName, string email, string password)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.RegisterError = "Error. Model is Invalid";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email, password and name cannot be empty");
            }

            // Check if user Already exists.

            User userOrNull = _context.Users.SingleOrDefault(user => user.Email == email);
            if (userOrNull != null)
            {
                throw new Exception("User already exists. Pick another username");
            }

            // If not, create a new user and add it to database
            // Only "customer" users can be created through the normal register, others should be added by admins

            User newUser = new User()
            {
                UserName = firstName + " " + lastName,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Email = email,

                IsManager = false,
                UserType = UserType.Customer,

            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            await SignInSession(newUser);

            return RedirectToAction("Index", "Home");
        }

        #region Private Methods

        private async Task SignInSession(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        #endregion

    }
}
