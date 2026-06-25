using System.ComponentModel.DataAnnotations;

namespace QucikCartAPI.Models
{
    public class Payment
    {
        [Key]
        public int payId { get; set; }
        public DateTime payDate { get; set; }
        public int orderId { get; set; }
        public int compId { get; set; }
        public Decimal orderGrandTot { get; set; }
        public string payMethod {  get; set; }  

    }
}
