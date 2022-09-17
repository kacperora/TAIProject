using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAIProject.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? Adress { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Total { get; set; }
        public string PaymentState { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
    }
}
