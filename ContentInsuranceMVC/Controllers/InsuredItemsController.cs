using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContentInsuranceMVC.Data;
using ContentInsuranceMVC.Models;

namespace ContentInsuranceMVC
{
    public class InsuredItemsController : Controller
    {
        private readonly ContentInsuranceMVCContext _context;

        public InsuredItemsController(ContentInsuranceMVCContext context)
        {
            _context = context;
        }

        // GET: InsuredItems
        public async Task<IActionResult> Index()
        {
            var contentInsuranceMVCContext = _context.InsuredItem.Include(i => i.Category)?.OrderBy(x => x.Category.Name).ThenBy(y => y.Name); // sort by category, name
            SetViewBag();
            return View(await contentInsuranceMVCContext.ToListAsync());
        }

       // GET: InsuredItems/Create
        public IActionResult Create()
        {
            SetViewBag();
            return View();
        }

        // POST: InsuredItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Value,CategoryId")] InsuredItem insuredItem)
        {
            if (ModelState.IsValid || insuredItem.Value >= 0)  // reject if user puts in zero or negative value
            {
                _context.Add(insuredItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            SetViewBag();
            return View(insuredItem);
        }

        // GET: InsuredItems/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredItem = await _context.InsuredItem.FindAsync(id);
            if (insuredItem == null)
            {
                return NotFound();
            }
            SetViewBag();
            return View(insuredItem);
        }

        // POST: InsuredItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Value,CategoryId")] InsuredItem insuredItem)
        {
            if (id != insuredItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuredItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuredItemExists(insuredItem.Id))
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
            SetViewBag();
            return View(insuredItem);
        }

        // GET: InsuredItems/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredItem = await _context.InsuredItem
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuredItem == null)
            {
                return NotFound();
            }

            return View(insuredItem);
        }

        // POST: InsuredItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuredItem = await _context.InsuredItem.FindAsync(id);
            _context.InsuredItem.Remove(insuredItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuredItemExists(int id)
        {
            return _context.InsuredItem.Any(e => e.Id == id);
        }

        private void SetViewBag()
        {
            ViewData["CategoryName"] = new SelectList(_context.Set<Category>(), "Id", "Name");
        }
    }
}
