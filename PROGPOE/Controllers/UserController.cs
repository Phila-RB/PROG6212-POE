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
        //private readonly User loggedIn = new User();
        private readonly User loggedIn = new() {UserID = 1, Name = "Thabo", Surname = "Mokoena", Access = "Organiser", Password = "******" };

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("auth/profile/{id:int}")]
        public IActionResult GetProfile(int id)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            SqlDataAdapter dap = new SqlDataAdapter($"SELECT * FROM Users WHERE UserID = id", con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            User user = new User();
            if (dt.Rows.Count > 0) {
                user.UserID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                user.Name = Convert.ToString(dt.Rows[0]["Name"]);
                user.Surname = Convert.ToString(dt.Rows[0]["Surname"]);
                user.Access = Convert.ToString(dt.Rows[0]["Access"]);
                user.Password = Convert.ToString(dt.Rows[0]["Password"]);
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("auth/register")]
        public IActionResult Register(User newUser)
        {
            if (newUser == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlCommand cmd = new SqlCommand("INSERT INTO Users (Name, Surname, Access, Password) VALUES (@Name, @Surname, @Access, @Password)", con);

            cmd.Parameters.AddWithValue("@Name", newUser.Name);
            cmd.Parameters.AddWithValue("@Surname", newUser.Surname);
            cmd.Parameters.AddWithValue("@Access", newUser.Access);
            cmd.Parameters.AddWithValue("@Password", newUser.Password);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return CreatedAtAction(nameof(GetProfile), new { id = newUser.UserID }, newUser);
        }
        
        [HttpPost]
        [Route("/auth/login")]
        public IActionResult GetUser(string name, string surname, string password)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE Name = {name} AND Surname = {surname} AND Password = {password}";
            using SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                loggedIn.UserID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                loggedIn.Name = Convert.ToString(dt.Rows[0]["Name"]);
                loggedIn.Surname = Convert.ToString(dt.Rows[0]["Surname"]);
                loggedIn.Access = Convert.ToString(dt.Rows[0]["Access"]);
                loggedIn.Password = "*****************";

                return Ok("logged in");
            }
            return NotFound();
        }

        [HttpPut]
        [Route("auth/profile")]

        public IActionResult UpdateProfile(User userUpd)
        {
            if (userUpd == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE UserID = {loggedIn.UserID}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            using SqlCommand cmd = new SqlCommand("UPDATE Users SET Name = @NAME, Surname = @Surname, Access = @ACCESS, Password = @Password WHERE UserId = @ID", con);

            string name = (String.IsNullOrEmpty(userUpd.Name))?loggedIn.Name : userUpd.Name;
            string surname = (String.IsNullOrEmpty(userUpd.Surname)) ? loggedIn.Surname : userUpd.Surname;
            string access = (String.IsNullOrEmpty(userUpd.Access)) ? loggedIn.Access : userUpd.Access;
            string password = (String.IsNullOrEmpty(userUpd.Password)) ? dt.Rows[0]["Password"].ToString() : userUpd.Password;

            cmd.Parameters.AddWithValue("@ID", loggedIn.UserID);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Surname", surname);
            cmd.Parameters.AddWithValue("@Access", access);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return CreatedAtAction(nameof(GetProfile), new { id = userUpd.UserID }, userUpd);
        }

        //[HttpPost("/addstudent")]
        //public IActionResult CreatStu(Student newStu)
        //{
        //    if (newStu == null)
        //    {
        //        return NotFound();
        //    }
        //    StuDB.Add(newStu);
        //    return CreatedAtAction(nameof(GetStu), new { id = newStu.Id }, newStu);
        //}

        //[HttpGet]
        //[Route("users")]
        //public IActionResult GetAllUsers()
        //{
        //    List<User> list = new List<User>();

        //    SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
        //    SqlDataAdapter dap = new SqlDataAdapter("SELECT * FROM Users", con);
        //    DataTable dt = new DataTable();
        //    dap.Fill(dt);
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            User user = new User();
        //            user.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
        //            user.Name = Convert.ToString(dt.Rows[i]["Name"]);
        //            user.Surname = Convert.ToString(dt.Rows[i]["Surname"]);
        //            user.Access = Convert.ToString(dt.Rows[i]["Access"]);
        //            user.Password = Convert.ToString(dt.Rows[i]["Password"]);
        //            list.Add(user);
        //        }
        //    }
        //    con.Close();
        //    return Ok(list);
        //}
    }
}
