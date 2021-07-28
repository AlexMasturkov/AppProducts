using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppProducts.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name="Product Name")]
        public string Name { get; set; }
        
        [Display(Name = "Product Description")]
        public string Description { get; set; }

        [Display(Name = "Product Price")]
        public double Price { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime DeletedDate { get; set; }

        public bool IsDeleted { get; set; }

    }
}
