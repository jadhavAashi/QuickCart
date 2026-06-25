using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : Controller
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
        public List<OrderDetails> Get()
        {
            OpenConn();
            List<OrderDetails> list = new List<OrderDetails>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from OrderDetails";
            dr = cmd.ExecuteReader();
            OrderDetails o = new OrderDetails();
            while (dr.Read())
            {
                o = new OrderDetails();
                o.orderDetId = Convert.ToInt32(dr[0]);
                o.orderId = Convert.ToInt32(dr[1]);
                o.itemId = Convert.ToInt32(dr[2]);
                o.compId = Convert.ToInt32(dr[3]);
                o.itemQty = Convert.ToInt32(dr[4]);
                o.itemAmt = Convert.ToDecimal(dr[5]);
                o.itemRate = Convert.ToDecimal(dr[6]);
                list.Add(o);
            }
            return list;
        }

        [HttpGet("{id}")]
        public OrderDetails Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from OrderDetails where orderDetId=" + id;
            dr = cmd.ExecuteReader();
            OrderDetails o = new OrderDetails();
            if (dr.Read())
            {
                o = new OrderDetails();
                o.orderDetId = Convert.ToInt32(dr[0]);
                o.orderId = Convert.ToInt32(dr[1]);
                o.itemId = Convert.ToInt32(dr[2]);
                o.compId = Convert.ToInt32(dr[3]);
                o.itemQty = Convert.ToInt32(dr[4]);
                o.itemAmt = Convert.ToDecimal(dr[5]);
                o.itemRate = Convert.ToDecimal(dr[6]);
            }
            return o;
        }
        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(orderDetId) from OrderDetails";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        [HttpPost]     
        public int Post([FromBody] OrderDetails o)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            o.orderDetId = GetNewId();
            cmd.CommandText = "Insert into OrderDetails values(@orderDetId,@orderId,@itemId,@compId,@itemQty,@itemAmt,@itemRate)";
            cmd.Parameters.AddWithValue("@orderDetId", o.orderDetId);
            cmd.Parameters.AddWithValue("@orderId", o.orderId);
            cmd.Parameters.AddWithValue("@itemId", o.itemId);
            cmd.Parameters.AddWithValue("@compId", o.compId);
            cmd.Parameters.AddWithValue("@itemQty", o.itemQty);
            cmd.Parameters.AddWithValue("@itemAmt", o.itemAmt);
            cmd.Parameters.AddWithValue("@itemRate", o.itemRate);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] OrderDetails o)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update OrderDetails set orderId=@orderId,itemId=@itemId,compId=@compId,itemQty=@itemQty,itemAmt=@itemAmt,itemRate=@itemRate where orderDetId=@orderDetId";
            cmd.Parameters.AddWithValue("@orderDetId", o.orderDetId);
            cmd.Parameters.AddWithValue("@orderId", o.orderId);
            cmd.Parameters.AddWithValue("@itemId", o.itemId);
            cmd.Parameters.AddWithValue("@compId", o.compId);
            cmd.Parameters.AddWithValue("@itemQty", o.itemQty);
            cmd.Parameters.AddWithValue("@itemAmt", o.itemAmt);
            cmd.Parameters.AddWithValue("@itemRate", o.itemRate);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from OrderDetails where orderDetId=@orderDetId";
            cmd.Parameters.AddWithValue("@orderDetId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

    }
}

