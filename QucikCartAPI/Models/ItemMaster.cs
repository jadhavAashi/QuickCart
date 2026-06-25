namespace QucikCartAPI.Models
{
    public class ItemMaster
    {
        public int itemId { get; set; }
        public string itemNm { get; set; }
        public int compId { get; set; }
        public int catId { get; set; }
        public int brandId { get; set; }
        public Decimal itemRate { get; set; }
        public Decimal itemStock { get; set; }
        public string itemDescr { get; set; }
        public string itemPhoto { get; set; }
    }
}
