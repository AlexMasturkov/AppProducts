using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppProducts.Data;
using AppProducts.Models;
using Microsoft.AspNetCore.Authorization;
using AppProducts.Utility;

namespace AppProducts.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticData.Manager)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public IActionResult Index( string id = "")
        {
            var products = _context.Products.ToList();
            switch (id)
            {
                case "Name":
                    products = products.Where(p => p.IsDeleted == false).OrderBy(p => p.Name).ToList();
                    return View(products);
                case "Price":
                    products = products.Where(p => p.IsDeleted == false).OrderBy(p => p.Price).ToList();
                    return View(products);
                case "Date":
                    products = products.Where(p => p.IsDeleted == false).OrderBy(p => p.CreatedDate).ToList();
                    return View(products);
                case "Created5":
                    var today = DateTime.Now.AddDays(-5);
                    products = products.Where(p => p.CreatedDate.CompareTo(today) >= 0).ToList();
                    return View(products);
                case "Deleted10":
                    var dateToday = DateTime.Now.AddDays(-10);
                    products = products.Where(p => p.IsDeleted == true).Where(p => p.DeletedDate.CompareTo(dateToday) >= 0).ToList();
                    return View(products);
                default:
                    return View(products.Where(p => p.IsDeleted == false).ToList());                  
            }            
        }

        // GET: Admin/Products/Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;                
                product.IsDeleted = false;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Products/Edit/id
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
            return View(product);
        }

        // POST: Admin/Products/Edit/id  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Product product)
        {
            if (id != product.Id)
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
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Admin/Products/Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            product.DeletedDate = DateTime.Now;
            product.IsDeleted = true;//Changed to not available product according request
           /* _context.Products.Remove(product);*/
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
