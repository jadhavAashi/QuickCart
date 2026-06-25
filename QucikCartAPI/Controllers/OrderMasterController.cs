using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderMasterController : Controller
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
        public List<OrderMaster> Get()
        {
            OpenConn();
            List<OrderMaster> list = new List<OrderMaster>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from OrderMaster";
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

        [HttpGet("{id}")]
        public OrderMaster Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from OrderMaster where orderId=" + id;
            dr = cmd.ExecuteReader();
            OrderMaster o = new OrderMaster();
            if (dr.Read())
            {
                o = new OrderMaster();
                o.orderId = Convert.ToInt32(dr[0]);
                o.orderDate = Convert.ToDateTime(dr[1]);
                o.custId = Convert.ToInt32(dr[2]);
                o.orderAmt = Convert.ToInt32(dr[3]);
                o.orderGSTAmt = Convert.ToDecimal(dr[4]);
                o.orderGrandTot = Convert.ToDecimal(dr[5]);
            }
            return o;
        }
        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(orderId) from OrderMaster";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }
        [HttpPost]
        public int Post([FromBody] OrderMaster o)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            o.orderId = GetNewId();
            cmd.CommandText = "Insert into OrderMaster values(@orderId,@orderDate,@custId,@orderAmt,@orderGSTAmt,@orderGrandTot)";
            cmd.Parameters.AddWithValue("@orderId", o.orderId);
            cmd.Parameters.AddWithValue("@orderDate", o.orderDate);
            cmd.Parameters.AddWithValue("@custId", o.custId);
            cmd.Parameters.AddWithValue("@orderAmt", o.orderAmt);
            cmd.Parameters.AddWithValue("@orderGSTAmt", o.orderGSTAmt);
            cmd.Parameters.AddWithValue("@orderGrandTot", o.orderGrandTot);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] OrderMaster o)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update OrderMaster set orderDate=@orderDate,custId=@custId,orderAmt=@orderAmt,orderGSTAmt=@orderGSTAmt,orderGrandTot=@orderGrandTot where orderId=@orderId";
            cmd.Parameters.AddWithValue("@orderId", o.orderId);
            cmd.Parameters.AddWithValue("@orderDate", o.orderDate);
            cmd.Parameters.AddWithValue("@custId", o.custId);
            cmd.Parameters.AddWithValue("@orderAmt", o.orderAmt);
            cmd.Parameters.AddWithValue("@orderGSTAmt", o.orderGSTAmt);
            cmd.Parameters.AddWithValue("@orderGrandTot", o.orderGrandTot);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from OrderMaster where orderId=@orderId";
            cmd.Parameters.AddWithValue("@orderId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

    }
}
