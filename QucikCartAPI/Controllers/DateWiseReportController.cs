using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    public class DateWiseReportController : Controller
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

        [HttpGet("DateWiseOrderMasterReport")]
        public List<OrderMaster> DateWiseOrderMasterReport([FromQuery] string fdate, [FromQuery] string tdate)
        {
            OpenConn();
            List<OrderMaster> list = new List<OrderMaster>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from OrderMaster where orderDate between @fdate and @tdate";
            cmd.Parameters.AddWithValue("@fdate", fdate);
            cmd.Parameters.AddWithValue("@tdate", tdate);
            dr = cmd.ExecuteReader();
            OrderMaster o = new OrderMaster();
            while (dr.Read())
            {
                o = new OrderMaster();
                o.orderId = Convert.ToInt32(dr[0]);
                o.orderDate = Convert.ToDateTime(dr[1]);
                o.custId = Convert.ToInt32(dr[2]);
                o.orderAmt = Convert.ToInt32(dr[3]);
                o.orderGSTAmt = Convert.ToDecimal(dr[4]);
                o.orderGrandTot = Convert.ToDecimal(dr[5]);
                list.Add(o);
            }
            return list;
        }
        [HttpGet("DateWisePaymentReport")]
        public List<Payment> DateWisePayment([FromQuery] string fdate, [FromQuery] string tdate)
        {
            OpenConn();
            List<Payment> list = new List<Payment>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Payment where payDate between @fdate and @tdate";
            cmd.Parameters.AddWithValue("@fdate", fdate);
            cmd.Parameters.AddWithValue("@tdate", tdate);
            dr = cmd.ExecuteReader();
            Payment p = new Payment();
            while (dr.Read())
            {
                p = new Payment();
                p.payId = Convert.ToInt32(dr[0]);
                p.payDate = Convert.ToDateTime(dr[1]);
                p.orderId = Convert.ToInt32(dr[2]);
                p.compId = Convert.ToInt32(dr[3]);
                p.orderGrandTot = Convert.ToDecimal(dr[4]);
                list.Add(p);
            }
            return list;
        }
    }
}