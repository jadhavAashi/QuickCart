using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
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
        public List<Payment> Get()
        {
            OpenConn();
            List<Payment> list = new List<Payment>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Payment";
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
                p.payMethod = Convert.ToString(dr[5]);
                list.Add(p);
            }
            return list;
        }

        [HttpGet("{id}")]
        public Payment Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Payment where payId=" + id;
            dr = cmd.ExecuteReader();
            Payment p = new Payment();
            if (dr.Read())
            {
                p = new Payment();
                p.payId = Convert.ToInt32(dr[0]);
                p.payDate = Convert.ToDateTime(dr[1]);
                p.orderId = Convert.ToInt32(dr[2]);
                p.compId = Convert.ToInt32(dr[3]);
                p.orderGrandTot = Convert.ToDecimal(dr[4]);
                p.payMethod = Convert.ToString(dr[5]);
            }
            return p;
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
        public int Post([FromBody] Payment p)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            p.payId = GetNewId();
            cmd.CommandText = "Insert into Payment values(@payId,@payDate,@orderId,@compId,@orderGrandTot,@payMethod)";
            cmd.Parameters.AddWithValue("@payId", p.payId);
            cmd.Parameters.AddWithValue("@payDate", p.payDate);
            cmd.Parameters.AddWithValue("@orderId", p.orderId);
            cmd.Parameters.AddWithValue("@compId", p.compId);
            cmd.Parameters.AddWithValue("@orderGrandTot", p.orderGrandTot);
            cmd.Parameters.AddWithValue("@payMethod", p.payMethod);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] Payment p)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update Payment set payDate=@payDate,orderId=@orderId,compId=@compId,orderGrandTot=@orderGrandTot,payMethod=@payMethod where payId=@payId";
            cmd.Parameters.AddWithValue("@payId", p.payId);
            cmd.Parameters.AddWithValue("@payDate", p.payDate);
            cmd.Parameters.AddWithValue("@orderId", p.orderId);
            cmd.Parameters.AddWithValue("@compId", p.compId);
            cmd.Parameters.AddWithValue("@orderGrandTot", p.orderGrandTot);
            cmd.Parameters.AddWithValue("@payMethod", p.payMethod);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from Payment where payId=@payId";
            cmd.Parameters.AddWithValue("@payId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

    }
}
