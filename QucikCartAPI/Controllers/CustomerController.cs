using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : Controller
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
        public List<Customer> Get()
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

        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Customer where custId=" + id;
            dr = cmd.ExecuteReader();
            Customer c = new Customer();
            if (dr.Read())
            {
                c = new Customer();
                c.custId = Convert.ToInt32(dr[0]);
                c.custNm = Convert.ToString(dr[1]);
                c.custAddr = Convert.ToString(dr[2]);
                c.custPhno = Convert.ToString(dr[3]);
                c.custEmail = Convert.ToString(dr[4]);
                c.custPincode = Convert.ToInt32(dr[5]);
                c.custPassword = Convert.ToString(dr[6]);
            }
            return c;
        }
        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(custId) from Customer";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        [HttpPost]
        public int Post([FromBody] Customer c)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            c.custId = GetNewId();
            cmd.CommandText = "Insert into Customer values(@custId,@custNm,@custAddr,@custPhno,@custEmail,@custPincode,@custPassword)";
            cmd.Parameters.AddWithValue("@custId", c.custId);
            cmd.Parameters.AddWithValue("@custNm", c.custNm);
            cmd.Parameters.AddWithValue("@custAddr", c.custAddr);
            cmd.Parameters.AddWithValue("@custPhno", c.custPhno);
            cmd.Parameters.AddWithValue("@custEmail", c.custEmail);
            cmd.Parameters.AddWithValue("@custPincode", c.custPincode);
            cmd.Parameters.AddWithValue("@custPassword", c.custPassword);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] Customer c)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update Customer set custNm=@custNm,custAddr=@custAddr,custPhno=@custPhno,custEmail=@custEmail,custPincode=@custPincode,custPassword=@custPassword where custId=@custId";
            cmd.Parameters.AddWithValue("@custId", c.custId);
            cmd.Parameters.AddWithValue("@custNm", c.custNm);
            cmd.Parameters.AddWithValue("@custAddr", c.custAddr);
            cmd.Parameters.AddWithValue("@custPhno", c.custPhno);
            cmd.Parameters.AddWithValue("@custEmail", c.custEmail);
            cmd.Parameters.AddWithValue("@custPincode", c.custPincode);
            cmd.Parameters.AddWithValue("@custPassword", c.custPassword);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from Customer where custId=@custId";
            cmd.Parameters.AddWithValue("@custId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }
    }
}