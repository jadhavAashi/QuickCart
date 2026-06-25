using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : Controller
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

        // GET: api/<BrandController>
        [HttpGet]
        public List<Brand> Get()
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

        // GET api/<BrandController>/5
        [HttpGet("{id}")]
        public Brand Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from Brand where brandId=" + id;
            dr = cmd.ExecuteReader();
            Brand b = new Brand();
            if (dr.Read())
            {
                b = new Brand();
                b.brandId = Convert.ToInt32(dr[0]);
                b.brandNm = Convert.ToString(dr[1]);
            }
            return b;
        }
        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(brandId) from Brand";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        // POST api/<BrandController>
        [HttpPost]
        public int Post([FromBody] Brand b)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            b.brandId = GetNewId();
            cmd.CommandText = "Insert into Brand values(@brandId,@brandNm)";
            cmd.Parameters.AddWithValue("@brandId", b.brandId);
            cmd.Parameters.AddWithValue("@brandNm", b.brandNm);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        // PUT api/<BrandController>/5
        [HttpPut]
        public int Put(int id, [FromBody] Brand b)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update Brand set brandNm=@brandNm where brandId=@brandId";
            cmd.Parameters.AddWithValue("@brandId", b.brandId);
            cmd.Parameters.AddWithValue("@brandNm", b.brandNm);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        // DELETE api/<BrandController>/5
        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from Brand where brandId=@brandId";
            cmd.Parameters.AddWithValue("@brandId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }
    }
}
