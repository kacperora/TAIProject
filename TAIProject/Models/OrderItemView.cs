namespace TAIProject.Models
{
    public class OrderItemView
    {
            public Guid Id { get; set; }
            public string? Adress { get; set; }
            public DateTime CreatedDate { get; set; }
            public decimal Total { get; set; }
            public string PaymentState { get; set; }
            public string UserID { get; set; }
            public List<OrderProduct>? items { get; set; }
    }
}
