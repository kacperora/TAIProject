using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TAIProject.Models
{
    public class ProductEditModel
    {
        public Product product { get; set; }
        public FormFile Picture { get; set; }
    }
}
