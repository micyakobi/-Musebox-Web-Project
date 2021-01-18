using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Musebox_Web_Project.Data;
using Musebox_Web_Project.Models;
using Newtonsoft.Json;

using System.Web;
using Microsoft.AspNetCore.Authorization;
//using System.Web.Mvc;

namespace Musebox_Web_Project.Controllers
{
    public class BranchesController : Controller
    {
        private readonly Musebox_Web_ProjectContext _context;

        public BranchesController(Musebox_Web_ProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult GetAllLocation()
        {

            var data = _context.Branch.ToList();
            return Json(data);
        }

        // GET: Branches
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Branch.ToListAsync());
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> FilterSearch(string name, string address, double lat, double lng)
        {
            var result = from b in _context.Branch
                         select b;

            if (name != null)
            {
                result = from b in result
                         where b.BranchName.Contains(name)
                         select b;
            }
            if (address != null)
            {
                result = from b in result
                         where b.Address.Contains(address)
                         select b;
            }
            if (lat != 0)
            {
                result = from b in result
                         where b.Latitude == lat
                         select b;
            }
            if (lng != 0)
            {
                result = from b in result
                         where b.Longitude == lng
                         select b;
            }

            return View("Index", await result.ToListAsync());
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SearchBra(string name, string address, double lat, double lng)
        {
            var result = from b in _context.Branch
                         select b;

            if (name != null)
            {
                result = from b in result
                         where b.BranchName.Contains(name)
                         select b;
            }
            if (address != null)
            {
                result = from b in result
                         where b.Address.Contains(address)
                         select b;
            }
            if (lat != 0)
            {
                result = from b in result
                         where b.Latitude == lat
                         select b;
            }
            if (lng != 0)
            {
                result = from b in result
                         where b.Longitude == lng
                         select b;
            }

            return PartialView(await result.ToListAsync());
        }

        [Authorize(Policy = "Admin")]
        public JsonResult GetBranches()
        {
            var result = _context.Branch.Select(loc => new
            {
                lat = loc.Latitude,
                lng = loc.Longitude
            }).ToList();

            return Json(result);
        }

        // GET: Branches/Details/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branch
                .FirstOrDefaultAsync(m => m.BranchId == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        [Authorize(Policy = "Admin")]
        // GET: Branches/Create
        public IActionResult Create()
        {
            facebookCreatePostB();
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create([Bind("BranchId,BranchName,Address,Latitude,Longitude")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        // GET: Branches/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branch.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("BranchId,BranchName,Address,Latitude,Longitude")] Branch branch)
        {
            if (id != branch.BranchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.BranchId))
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
            return View(branch);
        }

        // GET: Branches/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branch
                .FirstOrDefaultAsync(m => m.BranchId == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branch.FindAsync(id);
            _context.Branch.Remove(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region Private Methods

        private bool BranchExists(int id)
        {
            return _context.Branch.Any(e => e.BranchId == id);
        }

        private void facebookCreatePostB()
        {
            dynamic messagePost = new ExpandoObject();
            messagePost.message =
                "A new store opening today :) check and found where in our website!!";


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
}
