using AppProducts.Data;
using AppProducts.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppProducts.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public OrderDetailsCart orderDetailsCart { get; set; }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            orderDetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            orderDetailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var cart = _context.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                orderDetailsCart.ShoppingCarts = cart.ToList();
            }

            foreach (var items in orderDetailsCart.ShoppingCarts)
            {
                items.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == items.ProductId);
                orderDetailsCart.OrderHeader.OrderTotal = orderDetailsCart.OrderHeader.OrderTotal + (items.Product.Price * items.Count);
            }

            return View(orderDetailsCart);
        }
    }
}
