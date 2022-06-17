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

            //try
            //{
            //    con.Open();
            //    //sqlData.Fill(dt);
            //    using (SqlCommand cmd = new SqlCommand(invQuery, con))
            //    {
            //        using (SqlDataReader sdr = cmd.ExecuteReader())
            //        {
            //            while (sdr.Read())
            //            {
            //                invoices.Add(new Invoice
            //                {
            //                    Id = Convert.ToInt32(sdr["Id"]),
            //                    Recipient = sdr["Recipient"].ToString(),
            //                    InvoiceDate = Convert.ToDateTime(sdr["Date"]),
            //                    Total = Convert.ToDouble(sdr["Total"]),
            //                    Vat = Convert.ToDouble(sdr["Vat"]),
            //                });
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    con.Close();
            //}
            return invoices;
        }

        public Invoice Get(int id)
        {
            connection();return null;
        }

        public bool Add(Invoice model)
        {
            connection();
            Random rnd = new Random();
            int number = rnd.Next(1, 500);
            model.Id = number;
            model.Items.ForEach(it =>
            {
                model.Total += it.Price * it.Qty;
            });

            string query = "insert into Invoice values('" + model.Id + "'," +
                "'" + model.Recipient +
                "', '"+ model.Vat +
                "', '"+ model.Total +"', " +
                "'"+ model.InvoiceDate + "')";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Name", model.Recipient);
            cmd.Parameters.AddWithValue("@Total", model.Total);
            cmd.Parameters.Add("@Date", SqlDbType.DateTime2).Value =  model.InvoiceDate;
            cmd.Parameters.AddWithValue("@Vat", model.Vat);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                bool it = addItem(model.Items, model.Id);
                if (it)
                {
                    return it;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool addItem(List<Item> items, long id)
        {
            Random random = new Random();

            bool flag = false;

            for(int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.Id = random.Next(12, 100);
                item.InvoiceId = id;
                int a = (int) item.Remark;
                string query = "insert into Item values('" + item.Id + "'," +
                "'" + item.Name +
                "', '" + item.Qty +
                "', '" + item.Price + 
                "', '" + item.InvoiceId + 
                "', '" + (int)item.Remark + "')";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", item.Name);
                cmd.Parameters.AddWithValue("@Qty", item.Qty);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@Remark", (int)item.Remark);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                con.Open();
                int x = cmd.ExecuteNonQuery();
                con.Close();

                if (x >= 1)
                    flag = true;
                else
                    flag =false;
            }

            return flag;
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
