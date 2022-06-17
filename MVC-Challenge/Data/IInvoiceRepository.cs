using System.Data;

namespace MVC_Challenge.Models
{
    public interface IInvoiceRepository
    {
        List<Invoice> GetAll();
        Invoice Get(int id);
        bool Add(Invoice invoice);
        Invoice Details(int id);
    }
}
