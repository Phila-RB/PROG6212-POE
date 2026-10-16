using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PROGPOE.Models;
using System.Data;
using System.Data.SqlTypes;
using System.Security.Claims;
namespace PROGPOE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();

        List<Claim> claims;
        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles ="Organiser,Participant")]
        [HttpGet]
        [Route("auth/profile/{id:int}")]
        public IActionResult GetProfile(int id)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            SqlDataAdapter dap = new SqlDataAdapter($"SELECT * FROM Users WHERE UserID = {id}", con);
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
            string pass = HashPassword(newUser.Password);

            cmd.Parameters.AddWithValue("@Name", newUser.Name);
            cmd.Parameters.AddWithValue("@Surname", newUser.Surname);
            cmd.Parameters.AddWithValue("@Access", newUser.Access);
            cmd.Parameters.AddWithValue("@Password", pass);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return CreatedAtAction(nameof(GetProfile), new { id = newUser.UserID }, newUser);
        }

        public bool PassCheck(string storedHash, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword("", storedHash.Trim(), providedPassword);
            return result == PasswordVerificationResult.Success;
        }
        public string HashPassword(string password)
        {
            // Automatically salts and hashes the password
            string hashedPassword = _hasher.HashPassword("", password);
            return hashedPassword;
        }

        [HttpPost]
        [Route("/auth/login")]
        public async Task<IActionResult> Login(string name, string surname, string password)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE Name = '{name}' AND Surname = '{surname}'";
            using SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();

            dap.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                bool checkPass = PassCheck(dt.Rows[0]["Password"].ToString(), password);
                if (!checkPass)
                {
                    return Unauthorized();
                }
                // 2. Create user identity claims
                claims = new List<Claim> { new Claim(ClaimTypes.Sid, dt.Rows[0]["UserID"].ToString()), new Claim(ClaimTypes.Name, name), new Claim(ClaimTypes.Surname, surname), new Claim(ClaimTypes.Role, dt.Rows[0]["Access"].ToString())};
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Issue the session cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Ok("logged in");
            }
            return NotFound();
        }

        [Authorize(Roles = "Organiser,Participant")]
        [HttpPut]
        [Route("auth/profile")]
        public IActionResult UpdateProfile(User userUpd)
        {
            if (userUpd == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE UserID = {Convert.ToInt32(claims[0])}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            using SqlCommand cmd = new SqlCommand("UPDATE Users SET Name = @NAME, Surname = @Surname, Access = @ACCESS, Password = @Password WHERE UserId = @ID", con);

            string name = (String.IsNullOrEmpty(userUpd.Name))? claims[1].ToString() : userUpd.Name;
            string surname = (String.IsNullOrEmpty(userUpd.Surname)) ? claims[2].ToString() : userUpd.Surname;
            string access = (String.IsNullOrEmpty(userUpd.Access)) ? claims[3].ToString() : userUpd.Access;
            string password = (String.IsNullOrEmpty(userUpd.Password)) ? dt.Rows[0]["Password"].ToString() : userUpd.Password;

            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(claims[0]));
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Surname", surname);
            cmd.Parameters.AddWithValue("@Access", access);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return CreatedAtAction(nameof(GetProfile), new { id = userUpd.UserID }, userUpd);
        }

        [Authorize(Roles = "Participant")]
        [HttpPost]
        [Route("event/{id:int}/participate")]//change to category/id/participate
        public IActionResult SignUP(int id, Signup signup)
        {
            if (signup == null)
            {
                return NotFound();
            }
            //string query = $"SELECT Signup.ParticipantID, Signup.EventID, Signup.CategoryID FROM Signup" +
            //    $" WHERE Signup.ParticipantID = {signup.ParticipantID} AND Signup.EventID = {signup.EventID} AND Signup.CategoryID = {signup.CategoryID}";
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            //SqlDataAdapter adp = new SqlDataAdapter(query, con);
            //DataTable dt = new DataTable();
            //adp.Fill(dt);
            //if (dt.Rows.Count == 0)
            //{
            //    return NotFound("sorry please double check the event or participant or category as what u entered may not exist");
            //}
            using SqlCommand cmd = new SqlCommand("INSERT INTO Signup (EventID, ParticipantID, CategoryID) VALUES (@EventID, @ParticipantID, @CategoryID)", con);

            cmd.Parameters.AddWithValue("@EventID", signup.EventID);
            cmd.Parameters.AddWithValue("@ParticipantID", signup.ParticipantID);
            cmd.Parameters.AddWithValue("@CategoryID", signup.CategoryID);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        [Authorize(Roles = "Participant")]
        [HttpGet]
        [Route("auth/profile/results")]
        public IActionResult GetResults()
        {
            List<ParticipantResults> results = new List<ParticipantResults>();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT PR.PRID, PR.ParticipantID, PR.EventID, PR.Placement, PR.Time FROM ParticipantResults AS PR " +
                $"INNER JOIN Participant ON PR.ParticipantID = Participant.ParticipantID " +
                $"INNER JOIN Event ON Event.EventID = PR.EventID " +
                $"INNER JOIN Users ON Users.UserID = Participant.UserID " +
                $"WHERE Users.UserID = {Convert.ToInt32(claims[0])}";
            using SqlDataAdapter adp = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ParticipantResults result = new();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result.PRID = Convert.ToInt32(dt.Rows[i]["PRID"]);
                    result.ParticipantID = Convert.ToInt32(dt.Rows[i]["ParticipantID"]);
                    result.EventID = Convert.ToInt32(dt.Rows[i]["EventID"]);
                    result.Placement = Convert.ToInt32(dt.Rows[i]["Placement"]);
                    result.Time = (TimeSpan)dt.Rows[i]["Time"];
                
                    results.Add(result);
                }
            }
            return Ok(results);
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
