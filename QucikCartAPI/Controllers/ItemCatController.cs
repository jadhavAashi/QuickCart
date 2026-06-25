using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCatController : Controller
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
        public List<ItemCat> Get()
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

        [HttpGet("{id}")]
        public ItemCat Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from ItemCat where catId=" + id;
            dr = cmd.ExecuteReader();
            ItemCat i = new ItemCat();
            if (dr.Read())
            {
                i = new ItemCat();
                i.catId = Convert.ToInt32(dr[0]);
                i.catNm = Convert.ToString(dr[1]);
            }
            return i;
        }

        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(catId) from ItemCat";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        [HttpPost]
        public int Post([FromBody] ItemCat i)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            i.catId = GetNewId();
            cmd.CommandText = "Insert into ItemCat values(@catId,@catNm)";
            cmd.Parameters.AddWithValue("@catId", i.catId);
            cmd.Parameters.AddWithValue("@catNm", i.catNm);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] ItemCat i)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update ItemCat set catNm=@catNm where catId=@catId";
            cmd.Parameters.AddWithValue("@catId", i.catId);
            cmd.Parameters.AddWithValue("@catNm", i.catNm);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from ItemCat where catId=@catId";
            cmd.Parameters.AddWithValue("@catId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }
    }
}
