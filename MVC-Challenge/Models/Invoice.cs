using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Challenge.Models
{
    public class Invoice
    {
        public long Id { get; set; }
        public string? Recipient { get; set; }
        public double Vat { get; set; }
        public double Total{ get; set; }
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime InvoiceDate { get; set; }
        public List<Item>? Items { get; set; }
    }
}
