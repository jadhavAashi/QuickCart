using Microsoft.AspNetCore.Mvc;
using QucikCartAPI.Models;
using System.Data.SqlClient;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemMasterController : Controller
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
        public List<ItemMaster> Get()
        {
            OpenConn();
            List<ItemMaster> list = new List<ItemMaster>();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from ItemMaster";
            dr = cmd.ExecuteReader();
            ItemMaster i = new ItemMaster();
            while (dr.Read())
            {
                i = new ItemMaster();
                i.itemId = Convert.ToInt32(dr[0]);
                i.itemNm = Convert.ToString(dr[1]);
                i.compId = Convert.ToInt32(dr[2]);
                i.catId = Convert.ToInt32(dr[3]);
                i.brandId = Convert.ToInt32(dr[4]);
                i.itemRate = Convert.ToDecimal(dr[5]);
                i.itemStock = Convert.ToInt32(dr[6]);
                i.itemDescr = Convert.ToString(dr[7]);
                i.itemPhoto = Convert.ToString(dr[8]);
                list.Add(i);
            }
            return list;
        }

        [HttpGet("{id}")]
        public ItemMaster Get(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select * from ItemMaster where itemId=" + id;
            dr = cmd.ExecuteReader();
            ItemMaster i = new ItemMaster();
            if (dr.Read())
            {
                i = new ItemMaster();
                i.itemId = Convert.ToInt32(dr[0]);
                i.itemNm = Convert.ToString(dr[1]);
                i.compId = Convert.ToInt32(dr[2]);
                i.catId = Convert.ToInt32(dr[3]);
                i.brandId = Convert.ToInt32(dr[4]);
                i.itemRate = Convert.ToDecimal(dr[5]);
                i.itemStock = Convert.ToInt32(dr[6]);
                i.itemDescr = Convert.ToString(dr[7]);
                i.itemPhoto = Convert.ToString(dr[8]);
            }
            return i;
        }
        [HttpPost("uploadImage")]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
            // Keep original filename
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            // Optional: overwrite if exists
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return Ok(new { FileName = fileName });
        }

        [NonAction]
        public int GetNewId()
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select max(itemId) from ItemMaster";
            object x = cmd.ExecuteScalar();
            if (Convert.ToString(x) == "")
                return 1;
            else
                return Convert.ToInt32(x) + 1;

        }

        [HttpPost]
        public int Post([FromBody] ItemMaster i)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            i.itemId = GetNewId();
            cmd.CommandText = "Insert into ItemMaster values(@itemId,@itemNm,@compId,@catId,@brandId,@itemRate,@itemStock,@itemDescr,@itemPhoto)";
            cmd.Parameters.AddWithValue("@itemId", i.itemId);
            cmd.Parameters.AddWithValue("@itemNm", i.itemNm);
            cmd.Parameters.AddWithValue("@compId", i.compId);
            cmd.Parameters.AddWithValue("@catId", i.catId);
            cmd.Parameters.AddWithValue("@brandId", i.brandId);
            cmd.Parameters.AddWithValue("@itemRate", i.itemRate);
            cmd.Parameters.AddWithValue("@itemStock", i.itemStock);
            cmd.Parameters.AddWithValue("@itemDescr", i.itemDescr);
            cmd.Parameters.AddWithValue("@itemPhoto", i.itemPhoto);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpPut]
        public int Put(int id, [FromBody] ItemMaster i)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Update ItemMaster set itemNm=@itemNm,compId=@compId,catId=@catId,brandId=@brandId,itemRate=@itemRate,itemStock=@itemStock,itemDescr=@itemDescr,itemPhoto=@itemPhoto where itemId=@itemId";
            cmd.Parameters.AddWithValue("@itemId", i.itemId);
            cmd.Parameters.AddWithValue("@itemNm", i.itemNm);
            cmd.Parameters.AddWithValue("@compId", i.compId);
            cmd.Parameters.AddWithValue("@catId", i.catId);
            cmd.Parameters.AddWithValue("@brandId", i.brandId);
            cmd.Parameters.AddWithValue("@itemRate", i.itemRate);
            cmd.Parameters.AddWithValue("@itemStock", i.itemStock);
            cmd.Parameters.AddWithValue("@itemDescr", i.itemDescr);
            cmd.Parameters.AddWithValue("@itemPhoto", i.itemPhoto);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Delete from ItemMaster where itemId=@itemId";
            cmd.Parameters.AddWithValue("@itemId", id);
            int x = cmd.ExecuteNonQuery();
            return x;
        }

    }
}

