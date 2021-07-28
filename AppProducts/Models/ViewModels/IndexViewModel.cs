using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppProducts.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}
