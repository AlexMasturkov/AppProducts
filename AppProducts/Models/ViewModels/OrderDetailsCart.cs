using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppProducts.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
