using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAIProject.Models
{
    public class OrderProduct
    {
        [Key]
        [Column(Order = 1)]
        public Guid OrderId { get; set; }
        [Key]
        [Column(Order = 2)]
        public Guid ProductID { get; set; }
        public Product Product;
        public int Price { get; set; }
        public int ProductAmount { get; set; }
    }
}
