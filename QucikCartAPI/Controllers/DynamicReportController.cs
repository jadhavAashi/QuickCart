using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    public class DynamicReportController : Controller
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
            [HttpGet("CompanyWiseItemMasterList")]
            public List<dynamic> CompanyWiseItemMaster(int compId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select ItemMaster.itemId, ItemMaster.itemNm, Company.compNm, " +
                      "ItemMaster.catId,ItemMaster.brandId, ItemMaster.itemRate, " +
                      "ItemMaster.itemStock, ItemMaster.itemDescr, ItemMaster.itemPhoto " +
                      "from Company, ItemMaster where ItemMaster.compId = Company.compId " +
                      "and ItemMaster.compId = " + compId;
                dr = cmd.ExecuteReader();
                ItemMaster m = new ItemMaster();
                while (dr.Read())
                {

                    var b = new
                    {
                        itemId = Convert.ToInt32(dr[0]),
                        itemNm = Convert.ToString(dr[1]),
                        compNm = Convert.ToString(dr[2]),
                        catId = Convert.ToInt32(dr[3]),
                        brandId = Convert.ToInt32(dr[4]),
                        itemRate = Convert.ToDecimal(dr[5]),
                        itemStock = Convert.ToInt32(dr[6]),
                        itemDescr = Convert.ToString(dr[7]),
                        itemPhoto = Convert.ToString(dr[8]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("ItemCatWiseItemMasterList")]
            public List<dynamic> ItemCatWiseItemMaster(int catId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select ItemMaster.itemId, ItemMaster.itemNm, ItemMaster.compId, " +
                      "ItemCat.catNm,ItemMaster.brandId, ItemMaster.itemRate, " +
                      "ItemMaster.itemStock, ItemMaster.itemDescr, ItemMaster.itemPhoto " +
                      "from ItemCat, ItemMaster where ItemMaster.catId = ItemCat.catId " +
                      "and ItemMaster.catId = " + catId;
                dr = cmd.ExecuteReader();
                ItemMaster m = new ItemMaster();
                while (dr.Read())
                {

                    var b = new
                    {
                        itemId = Convert.ToInt32(dr[0]),
                        itemNm = Convert.ToString(dr[1]),
                        compId = Convert.ToInt32(dr[2]),
                        catNm = Convert.ToString(dr[3]),
                        brandId = Convert.ToInt32(dr[4]),
                        itemRate = Convert.ToDecimal(dr[5]),
                        itemStock = Convert.ToInt32(dr[6]),
                        itemDescr = Convert.ToString(dr[7]),
                        itemPhoto = Convert.ToString(dr[8]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("BrandWiseItemMasterList")]
            public List<dynamic> BrandWiseItemMaster(int brandId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select ItemMaster.itemId, ItemMaster.itemNm, ItemMaster.compId, " +
                      "ItemMaster.catId,Brand.brandNm, ItemMaster.itemRate, " +
                      "ItemMaster.itemStock, ItemMaster.itemDescr, ItemMaster.itemPhoto " +
                      "from Brand, ItemMaster where ItemMaster.brandId = Brand.brandId " +
                      "and ItemMaster.brandId = " + brandId;
                dr = cmd.ExecuteReader();
                ItemMaster m = new ItemMaster();
                while (dr.Read())
                {

                    var b = new
                    {
                        itemId = Convert.ToInt32(dr[0]),
                        itemNm = Convert.ToString(dr[1]),
                        compId = Convert.ToInt32(dr[2]),
                        catId = Convert.ToInt32(dr[3]),
                        brandNm = Convert.ToString(dr[4]),
                        itemRate = Convert.ToDecimal(dr[5]),
                        itemStock = Convert.ToInt32(dr[6]),
                        itemDescr = Convert.ToString(dr[7]),
                        itemPhoto = Convert.ToString(dr[8]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("CustomerWiseOrderMasterList")]
            public List<dynamic> CustomerWiseOrderMaster(int custId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select OrderMaster.orderId, OrderMaster.orderDate, Customer.custNm, " +
                      "OrderMaster.orderAmt,OrderMaster.orderGSTAmt, OrderMaster.orderGrandTot, " +
                      "from Customer, OrderMaster where OrderMaster.custId = OrderMaster.custId " +
                      "and OrderMaster.custId = " + custId;
                dr = cmd.ExecuteReader();
                OrderMaster o = new OrderMaster();
                while (dr.Read())
                {

                    var b = new
                    {
                        orderId = Convert.ToInt32(dr[0]),
                        orderDate = Convert.ToDateTime(dr[1]),
                        custNm = Convert.ToString(dr[2]),
                        orderAmt = Convert.ToInt32(dr[3]),
                        orderGSTAmt = Convert.ToDecimal(dr[4]),
                        orderGrandTot = Convert.ToDecimal(dr[5]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("OrderWiseOrderDetailsList")]
            public List<dynamic> OrderrWiseOrderDetails(int orderId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select OrderDetails.orderDetId, OrderMaster.orderId, OrderDetails.itemId, " +
                      "OrderDetails.compId,OrderDetails.itemQty, OrderDetails.itemAmt,OrderDetails.itemRate " +
                      "from OrderMaster,OrderDetails where OrderDetails.orderId = OrderDetails.orderId " +
                      "and OrderDetails.orderId = " + orderId;
                dr = cmd.ExecuteReader();
                OrderDetails o = new OrderDetails();
                while (dr.Read())
                {

                    var b = new
                    {
                        orderDetId = Convert.ToInt32(dr[0]),
                        orderId = Convert.ToInt32(dr[1]),
                        itemId = Convert.ToInt32(dr[2]),
                        compId = Convert.ToInt32(dr[3]),
                        itemQty = Convert.ToInt32(dr[4]),
                        itemAmt = Convert.ToDecimal(dr[5]),
                        itemRate = Convert.ToDecimal(dr[6]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("ItemMasterWiseOrderDetailsList")]
            public List<dynamic> ItemMasterWiseOrderDetails(int itemId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select OrderDetails.orderDetId, OrderDetails.orderId, ItemMaster.itemNm, " +
                      "OrderDetails.compId,OrderDetails.itemQty, OrderDetails.itemAmt,OrderDetails.itemRate " +
                      "from ItemMaster,OrderDetails where OrderDetails.itemId = ItemMaster.itemId " +
                      "and OrderDetails.itemId = " + itemId;
                dr = cmd.ExecuteReader();
                OrderDetails o = new OrderDetails();
                while (dr.Read())
                {

                    var b = new
                    {
                        orderDetId = Convert.ToInt32(dr[0]),
                        orderId = Convert.ToInt32(dr[1]),
                        itemNm = Convert.ToString(dr[2]),
                        compId = Convert.ToInt32(dr[3]),
                        itemQty = Convert.ToInt32(dr[4]),
                        itemAmt = Convert.ToDecimal(dr[5]),
                        itemRate = Convert.ToDecimal(dr[6]),
                    };

                    list.Add(b);
                }
                return list;
            }
        //[HttpGet("CompanyWiseOrderDetailsList")]
        //public List<dynamic> CompanyWiseOrderDetails(int compId)
        //{
        //    OpenConn();
        //    List<dynamic> list = new List<dynamic>();
        //    cmd = new SqlCommand();
        //    cmd.Connection = cn;
        //    cmd.CommandText = "Select OrderDetails.orderDetId, OrderDetails.orderId, OrderDetails.itemId, " +
        //          "Company.compNm,OrderDetails.itemQty, OrderDetails.itemAmt,OrderDetails.itemRate " +
        //          "from Company,OrderDetails where OrderDetails.compId = Company.compId " +
        //          "and OrderDetails.compId = " + compId;
        //    dr = cmd.ExecuteReader();
        //    OrderDetails o = new OrderDetails();
        //    while (dr.Read())
        //    {

        //        var b = new
        //        {
        //            orderDetId = Convert.ToInt32(dr[0]),
        //            orderId = Convert.ToInt32(dr[1]),
        //            itemId = Convert.ToInt32(dr[2]),
        //            compNm = Convert.ToString(dr[3]),
        //            itemQty = Convert.ToInt32(dr[4]),
        //            itemAmt = Convert.ToDecimal(dr[5]),
        //            itemRate = Convert.ToDecimal(dr[6]),
        //        };

        //        list.Add(b);
        //    }
        //    return list;
        //}

        [HttpGet("CompanyWiseOrderDetailsList")]
        public List<dynamic> CompanyWiseOrderDetails(int compId)
        {
            OpenConn();

            List<dynamic> list = new List<dynamic>();

            cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.CommandText = @"
        SELECT 
            OrderDetails.orderDetId,
            OrderDetails.orderId,
            OrderDetails.itemId,
            Company.compNm,
            OrderDetails.itemQty,
            OrderDetails.itemAmt,
            OrderDetails.itemRate
        FROM OrderDetails
        INNER JOIN Company 
            ON OrderDetails.compId = Company.compId
        WHERE OrderDetails.compId = @compId";

            cmd.Parameters.AddWithValue("@compId", compId);

            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var b = new
                {
                    orderDetId = Convert.ToInt32(dr["orderDetId"]),
                    orderId = Convert.ToInt32(dr["orderId"]),
                    itemId = Convert.ToInt32(dr["itemId"]),
                    compNm = Convert.ToString(dr["compNm"]),
                    itemQty = Convert.ToInt32(dr["itemQty"]),
                    itemAmt = Convert.ToDecimal(dr["itemAmt"]),
                    itemRate = Convert.ToDecimal(dr["itemRate"])
                };

                list.Add(b);
            }

            cn.Close();

            return list;
        }
      
        [HttpGet("OrderWisePaymentList")]
            public List<dynamic> OrderWisePayment(int orderId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select Payment.payId,Payment.payDate,OrderMaster.orderId," +
                    "Payment.compId,Payment.orderGrandTot from OrderMaster,Payment " +
                    " where Payment.orderId = OrderMaster.orderId " +
                      "and Payment.orderId = " + orderId;
                dr = cmd.ExecuteReader();
                Payment p = new Payment();
                while (dr.Read())
                {

                    var b = new
                    {
                        payId = Convert.ToInt32(dr[0]),
                        payDate = Convert.ToDateTime(dr[1]),
                        orderId = Convert.ToInt32(dr[2]),
                        compId = Convert.ToInt32(dr[3]),
                        orderGrandTot = Convert.ToDecimal(dr[4]),
                    };

                    list.Add(b);
                }
                return list;
            }
            [HttpGet("CompanyWisePaymentList")]
            public List<dynamic> CompanyWisePaymentList(int compId)
            {
                OpenConn();
                List<dynamic> list = new List<dynamic>();
                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "Select Payment.payId,Payment.payDate,Payment.orderId," +
                    "Company.compNm,Payment.orderGrandTot from Company,Payment " +
                    " where Payment.compId = Company.compId " +
                      "and Payment.compId = " + compId;
                dr = cmd.ExecuteReader();
                Payment p = new Payment();
                while (dr.Read())
                {

                    var b = new
                    {
                        payId = Convert.ToInt32(dr[0]),
                        payDate = Convert.ToDateTime(dr[1]),
                        orderId = Convert.ToInt32(dr[2]),
                        compNm = Convert.ToString(dr[3]),
                        orderGrandTot = Convert.ToDecimal(dr[4]),
                    };

                    list.Add(b);
                }
                return list;
            }
        }
    }