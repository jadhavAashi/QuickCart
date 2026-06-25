using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
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
        public List<Company> Get()
        {

            OpenConn();
            List<Company> List = new List<Company>();
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
                List.Add(c);
            }
            return List;

        }

        [HttpGet("{id}")]
        public Company Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Company where compId=" + id;
            dr = cmd.ExecuteReader();
            Company c = new Company();
            if (dr.Read())
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
            }
            return c;
        }
        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(compId) from Company";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        [HttpPost]
        public int Post([FromBody] Company c)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            c.compId = GetNewId();
            cmd.CommandText = "Insert into Company values(@compId,@compNm,@compAddr,@compPhno,@compEmail,@compCity,@compDescr,@compPassword)";
            cmd.Parameters.AddWithValue("@compId", c.compId);
            cmd.Parameters.AddWithValue("@compNm", c.compNm);
            cmd.Parameters.AddWithValue("@compAddr", c.compAddr);
            cmd.Parameters.AddWithValue("@compPhno", c.compPhno);
            cmd.Parameters.AddWithValue("@compEmail", c.compEmail);
            cmd.Parameters.AddWithValue("@compCity", c.compCity);
            cmd.Parameters.AddWithValue("@compDescr", c.compDescr);
            cmd.Parameters.AddWithValue("@compPassword", c.compPassword);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put([FromBody] Company c)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;

            cmd.CommandText = @"UPDATE Company SET 
            compNm=@compNm,
            compAddr=@compAddr,
            compPhno=@compPhno,
            compEmail=@compEmail,
            compCity=@compCity,
            compDescr=@compDescr,
            compPassword=@compPassword
            WHERE compId=@compId";
          
            cmd.Parameters.AddWithValue("@compId", c.compId);
            cmd.Parameters.AddWithValue("@compNm", c.compNm);
            cmd.Parameters.AddWithValue("@compAddr", c.compAddr);
            cmd.Parameters.AddWithValue("@compPhno", c.compPhno);
            cmd.Parameters.AddWithValue("@compEmail", c.compEmail);
            cmd.Parameters.AddWithValue("@compCity", c.compCity);
            cmd.Parameters.AddWithValue("@compDescr", c.compDescr);
            cmd.Parameters.AddWithValue("@compPassword", c.compPassword);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from Company where compId=@compId";
            cmd.Parameters.AddWithValue("@compId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }
    }
}
 