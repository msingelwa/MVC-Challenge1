namespace MVC_Challenge.Models
{
    public class Item
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public Remark Remark { get; set; }
        public long InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
