using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {

        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader dr;

        [NonAction]
        public void OpenConn()
        {
            cn = new SqlConnection();
            cn.ConnectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            cn.Open();
        }
        [HttpGet]
        public IActionResult GetItems()
        {
            var items = new List<dynamic>();

            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
SELECT i.itemId, i.itemNm, i.itemRate, i.itemPhoto,i.itemstock,cm.compId,
       b.brandNm, c.catNm
FROM ItemMaster i
INNER JOIN Brand b ON i.brandId = b.brandId
INNER JOIN Company cm ON i.compId = cm.compId
INNER JOIN ItemCat c ON i.catId = c.catId";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string imageFile = reader["itemPhoto"].ToString();
                    string fullUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{imageFile}";

                    items.Add(new
                    {
                        itemId = reader["itemId"],
                        name = reader["itemNm"],
                        price = reader["itemRate"],
                        itemstock=reader["itemstock"],
                        compId = reader["compId"],
                        brand = reader["brandNm"],
                        category = reader["catNm"],                   
                        imageUrl = fullUrl
                    });
                }
            }

            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult SearchDetails(int id)
        {
            OpenConn();
          
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select itemId, itemNm, Company.compNm,Company.compId,ItemCat.catNm,Brand.brandNm,itemRate,itemstock," +
                "itemDescr,itemPhoto from Company,ItemCat,Brand,ItemMaster " +
                "where ItemMaster.compId = Company.compId and ItemMaster.catId = ItemCat.catId and " +
                "ItemMaster.brandId = Brand.brandId and itemId= " + id;
            dr = cmd.ExecuteReader();
            ItemMaster m = new ItemMaster();
            if(dr.Read())
            {
                string imageFile = dr["itemPhoto"].ToString();
                string fullUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{imageFile}";
                var b = new
                {
                    itemId = dr["itemId"],
                    name = dr["itemNm"],
                    compId = dr["compId"],
                    price = dr["itemRate"],
                    brand = dr["brandNm"],
                    itemstock = dr["itemstock"],
                    category = dr["catNm"],
                    company = dr["compNm"],   
                    imageUrl = fullUrl,
                    description = dr["itemDescr"]
                };
                return Ok(b);
            }
            return NotFound();
        }

        private readonly string connectionString ="Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private int GetNewOrderId(SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(orderId),0)+1 FROM OrderMaster", cn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        private int GetNeworderDetId(SqlConnection cn)
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(orderDetId),0)+1 FROM OrderDetails", cn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        [HttpPost]
        public IActionResult PlaceOrder([FromBody] OrderRequest request)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                int orderId = GetNewOrderId(cn);
                SqlCommand cmd = new SqlCommand(
                @"INSERT INTO OrderMaster (orderId,  orderDate,custId, orderAmt, orderGSTAmt, orderGrandTot)
          VALUES (@orderId, @orderDate,@custId, @orderAmt, @orderGSTAmt, @orderGrandTot)", cn);

                cmd.Parameters.AddWithValue("@orderId", orderId);           
                cmd.Parameters.AddWithValue("@orderDate", request.orderDate);
                cmd.Parameters.AddWithValue("@custId", request.custId);
                cmd.Parameters.AddWithValue("@orderAmt", request.orderAmt);
                cmd.Parameters.AddWithValue("@orderGSTAmt", request.orderGSTAmt);
                cmd.Parameters.AddWithValue("@orderGrandTot", request.orderGrandTot);

                cmd.ExecuteNonQuery();
                foreach (var item in request.Items)
                {
                    int orderDetId = GetNeworderDetId(cn);

                    SqlCommand cmd2 = new SqlCommand(
                    @"INSERT INTO OrderDetails 
              (orderDetId, orderId, itemId,compId, itemQty, itemAmt, itemRate)
              VALUES (@orderDetId, @orderId, @itemId,@compId, @itemQty, @itemAmt, @itemRate)", cn);

                    cmd2.Parameters.AddWithValue("@orderDetId", orderDetId);
                    cmd2.Parameters.AddWithValue("@orderId", orderId);
                    cmd2.Parameters.AddWithValue("@itemId", item.itemId);
                    cmd2.Parameters.AddWithValue("@compId", item.compId);
                    cmd2.Parameters.AddWithValue("@itemQty", item.itemQty);
                    cmd2.Parameters.AddWithValue("@itemAmt", item.itemAmt);
                    cmd2.Parameters.AddWithValue("@itemRate", item.itemAmt * item.itemQty);
                    cmd2.ExecuteNonQuery();


                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "Update ItemMaster set itemstock=itemstock-@itemQty where itemId=@itemId";

                    cmd.Parameters.AddWithValue("@itemQty", item.itemQty);
                    cmd.Parameters.AddWithValue("@itemId", item.itemId);

                    cmd.ExecuteNonQuery();

                }
                return Ok(new { message = "Order placed successfully", orderId = orderId });
            }
        }
       
        [HttpGet("GetInvoice/{orderId}")]
        public IActionResult GetInvoice(int orderId)
        {
            OpenConn();

            SqlCommand cmd = new SqlCommand(@"SELECT 
    b.brandNm,
    c.compNm, 
    cu.custNm,
    cu.custAddr,
    cu.custPhno,
    im.itemNm,
    om.orderId,
    om.orderDate,
    om.orderAmt,
    om.orderGSTAmt,
    om.orderGrandTot,
    od.itemQty,
    od.itemAmt,
    od.itemRate 
FROM OrderMaster om
JOIN Customer cu ON cu.custId = om.custId
JOIN OrderDetails od ON om.orderId = od.orderId
JOIN ItemMaster im ON im.itemId = od.itemId 
JOIN Brand b ON b.brandId = im.brandId
JOIN Company c ON c.compId = im.compId
WHERE om.orderId = @orderId ", cn);

            cmd.Parameters.AddWithValue("@orderId", orderId);

            SqlDataReader dr = cmd.ExecuteReader();

            List<object> invoice = new List<object>();

            while (dr.Read())
            {
                invoice.Add(new
                {
                    brandNm = dr["brandNm"].ToString(),
                    compNm= dr["compNm"].ToString(),
                    custNm = dr["custNm"].ToString(),
                    custAddr = dr["custAddr"].ToString(),
                    custPhno = dr["custPhno"].ToString(),
                    itemNm = dr["itemNm"].ToString(),
                    orderId = Convert.ToInt32(dr["orderId"]),
                    orderDate = Convert.ToDateTime(dr["orderDate"]).ToString("dd-MM-yyyy"),
                    orderAmt=Convert.ToDecimal(dr["orderAmt"]),
                    orderGSTAmt = Convert.ToDecimal(dr["orderGSTAmt"]),
                    orderGrandTot = Convert.ToDecimal(dr["orderGrandTot"]),
                    itemQty = Convert.ToInt32(dr["itemQty"]),
                    itemAmt = Convert.ToDecimal(dr["itemAmt"]),
                    itemRate = Convert.ToDecimal(dr["itemRate"]),   
 
                    
                });
            }

            return Ok(invoice);
        }

        [HttpGet("OrderHistory/{orderId}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            OpenConn();
            List<object> items = new List<object>();

            cmd = new SqlCommand(@"
        SELECT 
            OM.orderId, OM.orderDate, OM.orderGrandTot, OM.orderGSTAmt,
            IM.itemNm, IM.itemPhoto, OD.itemQty, OD.itemRate, OD.itemAmt
        FROM OrderMaster OM
        INNER JOIN OrderDetails OD ON OM.orderId = OD.orderId
        INNER JOIN ItemMaster IM ON OD.itemId = IM.itemId
        WHERE OM.orderId = @orderId", cn);

            cmd.Parameters.AddWithValue("@orderId", orderId);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string imageFile = dr["itemPhoto"].ToString();
                string fullUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{imageFile}";
                items.Add(new
                {
                    orderId = dr["orderId"],
                    orderDate = Convert.ToDateTime(dr["orderDate"]).ToString("yyyy-MM-dd"),
                    itemNm = dr["itemNm"],
                    imageFile = fullUrl,
                    itemQty = dr["itemQty"],
                    itemRate = dr["itemRate"],
                    itemAmt = dr["itemAmt"],
                    orderGSTAmt = dr["orderGSTAmt"],
                    orderGrandTot = dr["orderGrandTot"]
                });
            }
            return Ok(items);
        }
        // 1. Endpoint for the "My Orders" LIST (OrderMaster + Customer Name)
        [HttpGet("AllOrders/{custId}")]
        public IActionResult GetCustomerOrders(int custId)
        {
            OpenConn();
            List<object> orders = new List<object>();
            cmd = new SqlCommand(@"
        SELECT OM.orderId, OM.orderDate, OM.orderAmt, OM.orderGSTAmt, OM.orderGrandTot, C.custNm
        FROM OrderMaster OM
        INNER JOIN Customer C ON OM.custId = C.custId
        WHERE OM.custId = @custId", cn);

            cmd.Parameters.AddWithValue("@custId", custId);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                orders.Add(new
                {
                    orderId = dr["orderId"],
                    orderDate = Convert.ToDateTime(dr["orderDate"]).ToString("yyyy-MM-dd"),
                    custNm = dr["custNm"], // Changed from custId to custNm
                    orderAmt = dr["orderAmt"],
                    orderGSTAmt = dr["orderGSTAmt"],
                    orderGrandTot = dr["orderGrandTot"]
                });
            }
            return Ok(orders);
        }

    }
}

