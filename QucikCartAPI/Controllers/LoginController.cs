using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QucikCartAPI.Models;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QucikCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
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
        private const string JwtKey = "QuickCartShoppingAngularProjectAshiwiniJadhav";
        private const string JwtIssuer = "https://localhost:7092/Login";




        private string GenerateJwtToken(string email, string role)
        {
            var keyBytes = Encoding.UTF8.GetBytes(JwtKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
             {
                new Claim("email", email),
                new Claim("role", role)
            };


            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("adminlogin")]
        public IActionResult AdminLogin([FromBody] AdminLogin m)
        {
            string adminUsername = "admin";
            string adminPassword = "12345";

            if (m.Username == adminUsername && m.Password == adminPassword)
            {
                string token = GenerateJwtToken(m.Username, "Admin");
              

                return Ok(new { success = true, message = "Login successful", token, role = "Admin", });
            }
            else
            {
                return Ok(new { success = false, message = "Invalid credentials" });
            }
        }

        [HttpPost("companylogin")]
        public IActionResult CompanyLogin([FromBody] CompLogin m)
        {
            OpenConn();

            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Company WHERE compEmail=@email AND compPassword=@password";

            cmd.Parameters.AddWithValue("@email", m.compEmail);
            cmd.Parameters.AddWithValue("@password", m.compPassword);

            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                string token = GenerateJwtToken(m.compEmail, "Company");
              
                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    compId = Convert.ToInt32(dr["compId"]),
                    compNm = Convert.ToString(dr["compNm"]),
                    token,
                    role = "Company",
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = "Invalid Email or Password"
                });
            }
        }
        [HttpPost("customerlogin")]
        public IActionResult CustomerLogin([FromBody] CustLogin m)
        {
            OpenConn();
            cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT * FROM Customer WHERE custEmail=@email AND custPassword=@password";
            cmd.Parameters.AddWithValue("@email", m.custEmail);
            cmd.Parameters.AddWithValue("@password", m.custPassword);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                string token = GenerateJwtToken(m.custEmail, "Customer");
                return Ok(new { success = true, message = "Login successful", token, role = "Customer", custId = Convert.ToInt32(dr["custId"]),custNm= Convert.ToString(dr["custNm"])});
            }
            else
            {
                return Ok(new { success = false, message = "Invalid Email or Password" });
            }
        }
    }
}