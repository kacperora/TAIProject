namespace TAIProject.Models
{
    public class JSONOrderCreateRequest
    {
        public string notifyUrl;
        public string customerIp;
        public string merchantPosId = "457973";
        public string description = "sent from TAIProject";
        public string currencyCode = "PLN";
        public string totalAmount;
        public string extOrderId;
        public Buyer buyer;
        public List<OrderItem> products = new();
        
    }
    public class Buyer
    {
        public string email;
        public string? phone;
        public string? firstName;
        public string? lastName;
        public string language;

    }
    public class OrderItem
    {
        public string name;
        public string unitPrice;
        public string quantity;
    }
}
