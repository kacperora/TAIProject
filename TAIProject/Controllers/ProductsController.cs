using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TAIProject.Data;
using TAIProject.Models;

namespace TAIProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, string Category)
        {
            var products = _context.Product.ToList();
            var categories = _context.Category.ToList();

            var view = from prod in products
                       join cat in categories on prod.CategoryID equals cat.Id
                       select new ProductIndexModel
                       {
                           Product = prod,
                           Category = cat,
                       };


            if (!String.IsNullOrEmpty(searchString))
            {
                view = view.Where(s => s.Product.Name!.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(Category))
            {
                view = view.Where(s => s.Category.Name!.Contains(Category));
            }
            return View(view);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }
            var products = _context.Product.Where(s => s.Id.Equals(id)).ToList();
            var categories = _context.Category.ToList();


            var view = from prod in products
                       join cat in categories on prod.CategoryID equals cat.Id
                       select new ProductIndexModel
                       {
                           Product = prod,
                           Category = cat
                       };
            var result =  view.FirstOrDefault(m => m.Product.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,AmountInStore,Description,UnitPrice,CategoryID")] Product product)
        {
            product.Id = Guid.NewGuid();
            var file = Request.Form.Files["Picture"];
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    product.Picture = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(product);
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

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Picture,AmountInStore,Description,UnitPrice,CategoryID")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var file = Request.Form.Files["Picture"];
            if (file!= null) {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    product.Picture = memoryStream.ToArray();
                }
            } else
            {
                product.Picture = _context.Product.FirstOrDefault(p => p.Id == product.Id).Picture;
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

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(Guid id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AddToBasket(Guid UserId, Guid ProductId)
        {

            return await Details(ProductId);
        }
    }
}
