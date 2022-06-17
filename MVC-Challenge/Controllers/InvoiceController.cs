using Microsoft.AspNetCore.Mvc;
using MVC_Challenge.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MVC_Challenge.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IInvoiceRepository _dbLayer;
        

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceRepository dBLayer)
        {
            _logger = logger;
            _dbLayer = dBLayer;
        }
        
        public IActionResult Index()
        {
            return View(_dbLayer.GetAll());
        }

        public IActionResult Error()
        {
            return View("Error");
        }

        public IActionResult Details(int id)
        {
            if (id == -1)
            {
                return NotFound();
            }
            var model = _dbLayer.GetAll().FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (id == -1)
            {
                return NotFound();
            }
            var model = _dbLayer.GetAll().FirstOrDefault(x => x.Id == id);
            return View(model);
        }
        public ActionResult AddItem()
        {
            return PartialView("Item");
        }
        public IActionResult Create()
        {
            return View(new Invoice());
        }

        [HttpPost]
        public ActionResult Create(Invoice invoice, List<Item> items)
        {
            
            try
            {
                invoice.Items = items;
                _dbLayer.Add(invoice);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(new ErrorViewModel(ex.Message));
            }
        }
    }
}