using System.Data;
using System.Data.SqlClient;

namespace MVC_Challenge.Models
{
    public class DBLayer : IInvoiceRepository
    {
        public IConfiguration Configuration { get; set; }
        private SqlConnection con;
        public List<Invoice> invoices;
        public List<Item> items;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DBLayer(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Configuration = configuration;
        }

        public List<Invoice> GetAll()
        {
            invoices = new List<Invoice>();
            items = new List<Item>();
            connection();
            string invQuery = "Select Id, Recipient, Date, Total, Vat from Invoice";
            string itQuery = "Select Id, Name, InvoiceId, Price, Qty, Remark from Item";

            SqlDataAdapter inDa = new SqlDataAdapter(invQuery, con);
            SqlDataAdapter sql = new SqlDataAdapter(itQuery, con);

            DataSet ds = new DataSet();


            inDa.Fill(ds, "Invoices");
            sql.Fill(ds, "Items");

            //DataColumn paCol = ds.Tables["Invoices"].Columns["Id"];
            //DataColumn chCol = ds.Tables["Items"].Columns["InvoiceId"];

            DataTable dtInvoice = new DataTable();
            DataTable dtItem = new DataTable();

            dtInvoice.Columns.Add(new DataColumn("Id", typeof(int)));
            dtInvoice.Columns.Add(new DataColumn("Recipient", typeof(string)));
            dtInvoice.Columns.Add(new DataColumn("Vat", typeof(double)));
            dtInvoice.Columns.Add(new DataColumn("Date", typeof(DateTime)));

            dtItem.Columns.Add(new DataColumn("Id", typeof(int)));
            dtItem.Columns.Add(new DataColumn("InvoiceId", typeof(int)));
            dtItem.Columns.Add(new DataColumn("Name", typeof(string)));
            dtItem.Columns.Add(new DataColumn("Price", typeof(double)));
            dtItem.Columns.Add(new DataColumn("Qty", typeof(int)));
            dtItem.Columns.Add(new DataColumn("Remark", typeof(string)));

            dtInvoice.PrimaryKey = new DataColumn[] { dtInvoice.Columns[0] };
            ds.Tables.Add(dtInvoice);
            ds.Tables.Add(dtItem);

#pragma warning disable CS8604 // Possible null reference argument.
            DataRelation relation = new DataRelation("Inv_Items", parentColumn: ds.Tables[0].Columns["Id"], ds.Tables[1].Columns["InvoiceId"]);
#pragma warning restore CS8604 // Possible null reference argument.

            ds.Relations.Add(relation);

            foreach (DataRow invRow in ds.Tables["Invoices"].Rows)
            {
                DataRow[] childRows = invRow.GetChildRows("Inv_Items");

                foreach (DataRow dataRow in childRows)
                {
                    items.Add(new Item
                    {
                        Id = Convert.ToInt32(dataRow["Id"]),
                        InvoiceId = Convert.ToInt32(dataRow["InvoiceId"]),
                        Name = dataRow["Name"].ToString(),
                        Qty = Convert.ToInt32(dataRow["Qty"]),
                        Price = Convert.ToDouble(dataRow["Price"]),
                        Remark = (Remark)Convert.ToInt32(dataRow["Remark"])
                    });
                }
                invoices.Add(new Invoice
                {
                    Id = Convert.ToInt32(invRow["Id"]),
                    Recipient = invRow["Recipient"].ToString(),
                    InvoiceDate = Convert.ToDateTime(invRow["Date"]),
                    Total = Convert.ToDouble(invRow["Total"]),
                    Vat = Convert.ToDouble(invRow["Vat"]),
                    Items = items
                });
                items = new List<Item>(0);
            }
            return invoices;
        }

        public Invoice Get(int id)
        {
            connection();
            
            return null;
        }

public void Add(Invoice invoice)
{
    using var conn = new SqlConnection(_connectionString);
    conn.Open();
    using var tran = conn.BeginTransaction();
    try
    {
        using var cmd = new SqlCommand(
            "INSERT INTO Invoices (CustomerName, InvoiceDate, Total) OUTPUT INSERTED.Id VALUES (@name, @date, @total)",
            conn, tran);
        cmd.Parameters.AddWithValue("@name", invoice.CustomerName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@date", invoice.Date);
        cmd.Parameters.AddWithValue("@total", invoice.Total);
        var newId = (int)cmd.ExecuteScalar();

        foreach (var item in invoice.Items ?? Enumerable.Empty<Item>())
        {
            using var cmdItem = new SqlCommand(
                "INSERT INTO Items (InvoiceId, Description, Quantity, UnitPrice) VALUES (@invId, @desc, @qty, @price)",
                conn, tran);
            cmdItem.Parameters.AddWithValue("@invId", newId);
            cmdItem.Parameters.AddWithValue("@desc", item.Description ?? (object)DBNull.Value);
            cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
            cmdItem.Parameters.AddWithValue("@price", item.UnitPrice);
            cmdItem.ExecuteNonQuery();
        }

        tran.Commit();
    }
    catch
    {
        tran.Rollback();
        throw;
    }
}

        public Invoice Details(int id)
        {
            connection();
            throw new NotImplementedException();
        }
        private void connection()
        {
            string constring = Configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(constring);
        }

        
    }
}
