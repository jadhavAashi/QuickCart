namespace QucikCartAPI.Models
{
    public class OrderDetails
    {
        public int orderDetId { get; set; }
        public int orderId { get; set; }
        public int itemId { get; set; }
        public int compId { get; set; }
        public int itemQty { get; set; }
        public Decimal itemAmt { get; set; }
        public Decimal itemRate { get; set; }
    }
}
