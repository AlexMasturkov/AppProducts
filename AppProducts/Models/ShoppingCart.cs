using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppProducts.Models
{
    public class ShoppingCart
    {

        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }
        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Range(1, 30,ErrorMessage="Please enter max 30 and more than 1 value ")]
        public int Count { get; set; }
    }
}
