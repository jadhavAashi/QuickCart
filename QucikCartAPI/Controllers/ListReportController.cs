using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    public class ListReportController : Controller
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
        [HttpGet("BrandList")]
        public List<Brand> BrandList()
        {

            OpenConn();
            List<Brand> List = new List<Brand>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Brand";
            dr = cmd.ExecuteReader();
            Brand b = new Brand();
            while (dr.Read())
            {
                b = new Brand();
                b.brandId = Convert.ToInt32(dr[0]);
                b.brandNm = Convert.ToString(dr[1]);
                List.Add(b);
            }
            return List;

        }
        [HttpGet("CompanyList")]
        public List<Company> CompanyList()
        {
            OpenConn();
            List<Company> list = new List<Company>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Company";
            dr = cmd.ExecuteReader();
            Company c = new Company();
            while (dr.Read())
            {
                c = new Company();
                c.compId = Convert.ToInt32(dr[0]);
                c.compNm = Convert.ToString(dr[1]);
                c.compAddr = Convert.ToString(dr[2]);
                c.compPhno = Convert.ToString(dr[3]);
                c.compEmail = Convert.ToString(dr[4]);
                c.compCity = Convert.ToString(dr[5]);
                c.compDescr = Convert.ToString(dr[6]);
                c.compPassword = Convert.ToString(dr[7]);
                list.Add(c);
            }
            return list;
        }
        [HttpGet("CustomerList")]
        public List<Customer> CustomerList()
        {
            OpenConn();
            List<Customer> list = new List<Customer>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Customer";
            dr = cmd.ExecuteReader();
            Customer c = new Customer();
            while (dr.Read())
            {
                c = new Customer();
                c.custId = Convert.ToInt32(dr[0]);
                c.custNm = Convert.ToString(dr[1]);
                c.custAddr = Convert.ToString(dr[2]);
                c.custPhno = Convert.ToString(dr[3]);
                c.custEmail = Convert.ToString(dr[4]);
                c.custPincode = Convert.ToInt32(dr[5]);
                c.custPassword = Convert.ToString(dr[6]);
                list.Add(c);
            }
            return list;
        }
        [HttpGet("ItemCatList")]
        public List<ItemCat> ItemCatList()
        {
            OpenConn();
            List<ItemCat> list = new List<ItemCat>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from ItemCat";
            dr = cmd.ExecuteReader();
            ItemCat i = new ItemCat();
            while (dr.Read())
            {
                i = new ItemCat();
                i.catId = Convert.ToInt32(dr[0]);
                i.catNm = Convert.ToString(dr[1]);
                list.Add(i);
            }
            return list;
        }
        [HttpGet("ItemMasterList")]
        public List<dynamic> ItemMasterList()
        {
            OpenConn();
            List<dynamic> list = new List<dynamic>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select itemId,itemNm,Company.compNm,ItemCat.catNm,Brand.brandNm,itemRate,itemStock,itemDescr,itemPhoto from Company,ItemCat,Brand,ItemMaster where ItemMaster.compId=Company.compId and ItemMaster.catId=ItemCat.catId and ItemMaster.brandId=Brand.brandId";
            dr = cmd.ExecuteReader();
            ItemMaster m = new ItemMaster();
            while (dr.Read())
            {

                var i = new
                {
                    itemId = Convert.ToInt32(dr[0]),
                    itemNm = Convert.ToString(dr[1]),
                    compNm = Convert.ToString(dr[2]),
                    catNm = Convert.ToString(dr[3]),
                    brandNm = Convert.ToString(dr[4]),
                    itemRate = Convert.ToDecimal(dr[5]),
                    itemStock = Convert.ToInt32(dr[6]),
                    itemDescr = Convert.ToString(dr[7]),
                    itemPhoto = Convert.ToString(dr[8]),
                };
                list.Add(i);
            }
            return list;
        }

        [HttpGet("OrderMasterList")]
        public List<dynamic> OrderMasterList()
        {
            OpenConn();
            List<dynamic> list = new List<dynamic>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select orderId,orderDate,Customer.custNm,orderAmt,orderGSTAmt,orderGrandTot from Customer,OrderMaster where OrderMaster.custId=Customer.custId";
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
        [HttpGet("OrderDetailsList")]
        public List<dynamic> OrderDetailsList()
        {
            OpenConn();
            List<dynamic> list = new List<dynamic>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select orderDetId,orderId,ItemMaster.itemNm,Company.compNm,itemQty,itemAmt,itemRate from ItemMaster,Company,OrderDetails where OrderDetails.itemId=ItemMaster.itemId and OrderDetails.compId=Company.compId";
            dr = cmd.ExecuteReader();
            OrderDetails d = new OrderDetails();
            while (dr.Read())
            {
                var o = new
                {
                    orderDetId = Convert.ToInt32(dr[0]),
                    orderId = Convert.ToInt32(dr[1]),
                    itemNm = Convert.ToString(dr[2]),
                    compNm = Convert.ToString(dr[3]),
                    itemQty = Convert.ToInt32(dr[4]),
                    itemAmt = Convert.ToDecimal(dr[5]),
                    itemRate = Convert.ToDecimal(dr[6]),

                };
                list.Add(o);
            }
            return list;
        }

        [HttpGet("PaymentList")]
        public List<dynamic> PaymentList()
        {
            OpenConn();
            List<dynamic> list = new List<dynamic>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select payId,payDate,orderId,Company.compNm,orderGrandTot from Company,Payment where Payment.compId=Company.compId";
            dr = cmd.ExecuteReader();
            Payment p = new Payment();
            while (dr.Read())
            {
                var o = new
                {
                    payId = Convert.ToInt32(dr[0]),
                    payDate = Convert.ToDateTime(dr[1]),
                    orderId = Convert.ToInt32(dr[2]),
                    compNm = Convert.ToString(dr[3]),
                    orderGrandTot = Convert.ToDecimal(dr[4]),

                };
                list.Add(o);
            }
            return list;
        }
    }
}
