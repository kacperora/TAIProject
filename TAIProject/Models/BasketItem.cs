using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAIProject.Models
{
    [Keyless]
    public class BasketItem
    {
        public Product Product { get; set; }
        public User User { get; set; }
        public int ItemQuantity
            { get; set; }
        public decimal CostTotal { get; set; }

    }
}
