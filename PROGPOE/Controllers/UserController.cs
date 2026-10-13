using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PROGPOE.Models;
using System.Data;
using System.Data.SqlTypes;
namespace PROGPOE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("users")]
        public IActionResult GetAllUsers()
        {
            List<User> list = new List<User>();
           
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            Console.WriteLine(con);
            SqlDataAdapter dap = new SqlDataAdapter("SELECT * FROM Users", con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            if (dt.Rows.Count > 0) {
                for (int i = 0; i < dt.Rows.Count; i++) { 
                    User user = new User();
                    user.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                    user.Name = Convert.ToString(dt.Rows[i]["Name"]);
                    user.Surname = Convert.ToString(dt.Rows[i]["Surname"]);
                    user.Access = Convert.ToString(dt.Rows[i]["Access"]);
                    user.Password = Convert.ToString(dt.Rows[i]["Password"]);
                    list.Add(user);
                }
            }
            return Ok(list);
        }


    }
}
