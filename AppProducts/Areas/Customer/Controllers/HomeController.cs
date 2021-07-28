using AppProducts.Data;
using AppProducts.Models;
using AppProducts.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppProducts.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Products = await _context.Products.Where(p=>p.IsDeleted == false).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cartCount = _context.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32("ssCartCount", cartCount);
            }
            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            try
            {                
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    Product = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync()
                };
                return View(shoppingCart);
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            try
            {
                shoppingCart.Id = 0;
                if (ModelState.IsValid)
                {
                    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    shoppingCart.ApplicationUserId = claim.Value;

                    ShoppingCart shoppingCartDb = await _context.ShoppingCarts.
                        Where(c => c.ApplicationUserId == shoppingCart.ApplicationUserId && c.ProductId == shoppingCart.Product.Id)
                        .FirstOrDefaultAsync();

                    if (shoppingCartDb == null)//if cart is not created
                    {
                        shoppingCart.ProductId = shoppingCart.Product.Id;
                        await _context.ShoppingCarts.AddAsync(shoppingCart);
                    }
                    else
                    {
                        shoppingCartDb.Count = shoppingCartDb.Count + shoppingCart.Count;//need to update cart count
                    }
                    await _context.SaveChangesAsync();

                    var count = _context.ShoppingCarts.Where(c => c.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                    HttpContext.Session.SetInt32("ssCartCount", count);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View(shoppingCart);
            }
            ShoppingCart cart = new ShoppingCart()
            {
                Product = await _context.Products.Where(p => p.Id == shoppingCart.ProductId).FirstOrDefaultAsync()
            };
            return View(cart);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
