using Microsoft.EntityFrameworkCore;

namespace TAIProject.Models
{
    [Keyless]
    public class Item
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}

