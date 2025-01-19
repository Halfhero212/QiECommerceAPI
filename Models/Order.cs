namespace QiECommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Processing";
        public string TrackingNumber { get; set; } = string.Empty;

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
