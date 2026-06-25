namespace QucikCartAPI.Models
{
    public class OrderMaster
    {
        public int orderId { get; set; }
        public DateTime orderDate { get; set; }
        public int custId { get; set; }
        public Decimal orderAmt { get; set; }
        public Decimal orderGSTAmt { get; set; }
        public Decimal orderGrandTot { get; set; }
    }
}
