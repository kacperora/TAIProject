namespace TAIProject.Models
{
    public class ReturnRequest
    {
        public Status status = new();
        public string redirectUrl;
        public string orderId;
        public string extOrderId;
    }
    public class Status
    {
        public string statusCode;
    }
}
