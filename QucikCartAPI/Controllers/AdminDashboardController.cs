using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AdminDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("summary")]
        public IActionResult GetDashboardSummary([FromQuery] DateTime? from,
[FromQuery] DateTime? to)
        {
            string connStr = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                var summary = new
                {
                    totalBrands = GetScalar(con, "SELECT COUNT(*) FROM Brand"),
                    totalCompanies = GetScalar(con, "SELECT COUNT(*) FROM Company"),
                    totalCustomers = GetScalar(con, "SELECT COUNT(*) FROM Customer"),
                    totalCategories = GetScalar(con, "SELECT COUNT(*) FROM ItemCat"),
                    totalItems = GetScalar(con, "SELECT COUNT(*) FROM ItemMaster"),
                    totalOrders = GetScalar(con, "SELECT COUNT(*) FROM OrderDetails"),
                    totalOrderMasters = GetScalar(con, "SELECT COUNT(*) FROM OrderMaster"),
                    totalPayments = GetScalar(con, "SELECT COUNT(*) FROM Payment"),
                    totalSales = GetScalar(con, "SELECT SUM(orderGrandTot) FROM Payment"),
                    salesByMonth = GetSalesByMonth(con, from, to),
                    recentInvoices = GetRecentInvoices(con),
                    stockAlerts = GetStockAlerts(con)
                };

                return Ok(summary);
            }
        }

        private int GetScalar(SqlConnection con, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            }
        }

        private List<object> GetSalesByMonth(SqlConnection con,
DateTime? from,
DateTime? to)
        {
            var list = new List<object>();
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MONTH(o.orderDate) as Month, SUM(p.orderGrandTot) as Sales 
        FROM Payment p
JOIN OrderMaster o ON p.orderId = o.orderId

WHERE
(@From IS NULL OR o.orderDate >= @From)
AND
(@To IS NULL OR o.orderDate <= @To)

GROUP BY MONTH(o.orderDate)
          ORDER BY Month", con))
            {

                cmd.Parameters.AddWithValue("@From", (object?)from ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@To", (object?)to ?? DBNull.Value);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new
                        {
                            month = rdr.GetInt32(0),
                            sales = Convert.ToDecimal(rdr[1]) // Safe casting
                        });
                    }
                }
            }
            return list;
        }

        private List<object> GetRecentInvoices(SqlConnection con)
        {
            var list = new List<object>();
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 8 o.orderId, c.custNm, o.orderDate, p.orderGrandTot, p.payMethod
          FROM OrderMaster o
          JOIN Customer c ON o.custId = c.custId
          JOIN Payment p ON o.orderId = p.orderId
          ORDER BY o.orderDate DESC", con))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new
                        {
                            orderId = rdr.GetInt32(0),
                            custNm = rdr.GetString(1),
                            orderDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd"),
                            paidAmount = Convert.ToDecimal(rdr[3]), // <-- Safe conversion
                            payMethod = rdr.GetString(4)
                        });
                    }
                }
            }
            return list;
        }

        private List<object> GetStockAlerts(SqlConnection con)
        {
            var list = new List<object>();
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT itemId, itemNm, itemstock 
              FROM ItemMaster WHERE itemstock < 50", con))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new
                        {
                            itemId = rdr.GetInt32(0),
                            itemNm = rdr.GetString(1),
                            itemstock = rdr.GetInt32(2)
                        });
                    }
                }
            }
            return list;
        }

        private List<ReportItem> GetReportData(string query, string nameCol, string valueCol)
        {
            var list = new List<ReportItem>();
            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ReportItem
                    {
                        Name = dr[nameCol].ToString(),
                        Value = Convert.ToDecimal(dr[valueCol])
                    });
                }
            }
            return list;
        }

        [HttpGet("Brand")]
        public IActionResult Brand() => Ok(
            GetReportData(
                "SELECT b.brandNm, COUNT(i.itemId) AS TotalItems FROM Brand b LEFT JOIN ItemMaster i ON b.brandId = i.brandId GROUP BY b.brandNm",
                "brandNm", "TotalItems"
            )
        );

        [HttpGet("CustomerOrders")]
        public IActionResult CustomerOrders() => Ok(
            GetReportData(
                "SELECT c.custNm, COUNT(o.orderId) AS TotalOrders FROM Customer c LEFT JOIN OrderMaster o ON c.custId = o.custId GROUP BY c.custNm",
                "custNm", "TotalOrders"
            )
        );

        [HttpGet("ItemOrders")]
        public IActionResult ItemOrders() => Ok(
            GetReportData(
                "SELECT i.itemNm, SUM(od.itemQty) AS TotalQty FROM OrderDetails od JOIN ItemMaster i ON od.itemId = i.itemId GROUP BY i.itemNm",
                "itemNm", "TotalQty"
            )
        );

        [HttpGet("OrderDetails")]
        public IActionResult OrderDetails() => Ok(
            GetReportData(
                "SELECT o.orderId, SUM(od.itemQty*od.itemRate) AS TotalAmount FROM OrderMaster o JOIN OrderDetails od ON o.orderId = od.orderId GROUP BY o.orderId",
                "orderId", "TotalAmount"
            )
        );

        [HttpGet("OrderPayments")]
        public IActionResult OrderPayments() => Ok(
            GetReportData(
                "SELECT o.orderId, SUM(p.orderGrandTot) AS PaidAmount FROM OrderMaster o JOIN Payment p ON o.orderId = p.orderId GROUP BY o.orderId",
                "orderId", "PaidAmount"
            )
        );

        [HttpGet("OrdersByDate")]
        public IActionResult GetOrdersByDate(
[FromQuery] DateTime? from,
[FromQuery] DateTime? to)
        {
            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            CAST(orderDate AS DATE) AS orderDate,
            CAST(SUM(orderGrandTot) AS DECIMAL(18,2)) AS TotalAmount
        FROM OrderMaster
        WHERE (@From IS NULL OR orderDate >= @From)
        AND (@To IS NULL OR orderDate <= @To)
        GROUP BY CAST(orderDate AS DATE)
        ORDER BY CAST(orderDate AS DATE)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@From", (object?)from ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@To", (object?)to ?? DBNull.Value);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                var result = new List<object>();

                while (reader.Read())
                {
                    result.Add(new
                    {
                        Date = Convert.ToDateTime(reader["orderDate"]).ToString("yyyy-MM-dd"),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                    });
                }

                return Ok(result);
            }
        }

        [HttpGet("PaymentsByDate")]
        public IActionResult GetPaymentsByDate(
     [FromQuery] DateTime? from,
     [FromQuery] DateTime? to)
        {
            string connectionString =
                "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT
            CAST(payDate AS DATE) AS payDate,
            SUM(orderGrandTot) AS TotalPayment
        FROM Payment
        WHERE
        (@From IS NULL OR payDate >= @From)
        AND
        (@To IS NULL OR payDate <= @To)
        GROUP BY CAST(payDate AS DATE)
        ORDER BY CAST(payDate AS DATE)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@From",
                    (object?)from ?? DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "@To",
                    (object?)to ?? DBNull.Value
                );

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                var result = new List<object>();

                while (reader.Read())
                {
                    result.Add(new
                    {
                        Date = ((DateTime)reader["payDate"])
                            .ToString("yyyy-MM-dd"),

                        TotalPayment =
                            Convert.ToDecimal(reader["TotalPayment"])
                    });
                }

                return Ok(result);
            }
        }
        [HttpGet("SalesByDate")]
        public IActionResult GetSalesByDate([FromQuery] DateTime? from,
[FromQuery] DateTime? to)
        {
            var list = new List<object>();
            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT CONVERT(date, orderDate) AS Date, SUM(orderGrandTot) AS Sales
                FROM OrderMaster
            WHERE
(@From IS NULL OR orderDate >= @From)
AND
(@To IS NULL OR orderDate <= @To)
                GROUP BY CONVERT(date, orderDate)
                ORDER BY Date";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@From", (object?)from ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@To", (object?)to ?? DBNull.Value);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                Date = reader["Date"].ToString(),
                                Sales = Convert.ToDecimal(reader["Sales"])
                            });
                        }
                    }
                }
            }

            return Ok(list);
        }


        [HttpGet]
        public IActionResult GetItems(string? brand = null)
        {
            var items = new List<dynamic>();

            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // JOIN with Brand table to get BrandName
                string query = @"
            SELECT i.itemId, i.itemNm, i.itemRate, i.itemPhoto, b.brandNm
            FROM ItemMaster i
            INNER JOIN Brand b ON i.brandId = b.brandId";

                if (!string.IsNullOrEmpty(brand))
                {
                    query += " WHERE b.brandNm LIKE @brandNm";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(brand))
                {
                    cmd.Parameters.AddWithValue("@brandNm", "%" + brand + "%"); // partial search
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string imageFile = reader["itemPhoto"].ToString();
                    string fullUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{imageFile}";

                    items.Add(new
                    {
                        itemId = reader["itemId"],
                        itemNm = reader["itemNm"],
                        itemRate = reader["itemRate"],
                        Brand = reader["brandNm"],
                        ImageUrl = fullUrl
                    });
                }
            }

            return Ok(items);
        }



        //Search By ID

        [HttpGet("{id}")]
        public IActionResult GetItemsById(int id)
        {
            string connectionString = "Data Source=DESKTOP-U7J4E7G;Initial Catalog=QuickCartDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT 
    i.itemId,
    i.itemNm,
    b.brandNm,
    i.compNm,
    i.itemRate,
    i.itemStock,
    i.itemDescr,
    i.itemPhoto
FROM ItemMaster i
INNER JOIN Brand b ON i.brandId = b.brandId
WHERE i.itemId = @id"
, con);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string imageFile = reader["itemPhoto"].ToString();
                    string fullUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{imageFile}";

                    var item = new
                    {
                        itemId = reader["itemId"],
                        itemNm = reader["itemNm"],
                        brandNm = reader["brandNm"],
                        compNm = reader["compNm"],
                        itemRate = reader["itemRate"],
                        itemStock = reader["itemStock"],
                        itemDescr = reader["itemDescr"],
                        ImageUrl = fullUrl
                    };

                    return Ok(item);
                }
            }
            return NotFound();
        }


    }
}








