namespace QucikCartAPI.Models
{
    public class OrderRequest
    {
        public int custId { get; set; }
        public DateTime orderDate { get; set; }
        public decimal orderAmt { get; set; }
        public decimal orderGSTAmt { get; set; }
        public decimal orderGrandTot { get; set; }

        public List<OrderDetailsItem> Items { get; set; }
    }

    public class OrderDetailsItem
    {
        public int itemId { get; set; }

        public int compId { get; set; }
        public int itemQty { get; set; }
        public decimal itemAmt { get; set; }
    }
}
