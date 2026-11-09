using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROGPOE.Models;
using System.Data;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace PROGPOE.Controllers
{
    [Route("api")]
    [ApiController]

    public class UserController : ControllerBase 
    {
        private readonly IConfiguration _config; // config
        private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>(); //password hasher

        public UserController(IConfiguration config) //constructor 
        {
            _config = config;
        }

        [Authorize(Roles ="Organiser,Participant")] // role manager
        [HttpGet] //http request type
        [Route("auth/profile")] // route
        public IActionResult GetProfile() // this api gets the user profile
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString()); //sql connection
            SqlDataAdapter dap = new SqlDataAdapter($"SELECT * FROM Users WHERE UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value}", con); //data adapter
            DataTable dt = new DataTable(); //data table
            dap.Fill(dt); //opens and closes connection and populates data table
            User user = new User();
            if (dt.Rows.Count > 0) { // loops through data table and populates user object 
                user.UserID = Convert.ToInt32(dt.Rows[0]["UserID"]);
                user.Name = Convert.ToString(dt.Rows[0]["Name"]);
                user.Surname = Convert.ToString(dt.Rows[0]["Surname"]);
                user.Access = Convert.ToString(dt.Rows[0]["Access"]);
                user.Password = Convert.ToString(dt.Rows[0]["Password"]);
                return Ok(user); //returns data with 200 response
            }
            return NotFound(); //if user doesnt exist returns 404
        }

        [HttpPost]//post..does not require auth to use
        [Route("auth/register")]
        public IActionResult Register(User newUser)//this api registers a new user 
        {
            if (newUser == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlCommand cmd = new SqlCommand("INSERT INTO Users (Name, Surname, Access, Password) VALUES (@Name, @Surname, @Access, @Password)", con);
            string pass = HashPassword(newUser.Password);//hashing password

            cmd.Parameters.AddWithValue("@Name", newUser.Name);
            cmd.Parameters.AddWithValue("@Surname", newUser.Surname);
            cmd.Parameters.AddWithValue("@Access", newUser.Access);
            cmd.Parameters.AddWithValue("@Password", pass);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        public bool PassCheck(string storedHash, string providedPassword)//this function verifies passwords
        {
            var result = _hasher.VerifyHashedPassword("", storedHash.Trim(), providedPassword);
            return result == PasswordVerificationResult.Success;
        }
        public string HashPassword(string password)//this function hashes the registering users password
        {
            // Automatically salts and hashes the password
            string hashedPassword = _hasher.HashPassword("", password);
            return hashedPassword;
        }

        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> Login([FromBody] User user)//this api logs the user in and creates a session token to validate whether they are authenticated or not
        {
            Console.WriteLine("API LOGIN METHOD REACHED");

            Console.WriteLine("Name: " + user?.Name);
            Console.WriteLine("Surname: " + user?.Surname);
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE Name = '{user.Name}' AND Surname = '{user.Surname}'";
            Console.WriteLine(query);
            using SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();

            dap.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                //bool checkPass = PassCheck(dt.Rows[0]["Password"].ToString(), user.Password);
                //if (!checkPass)
                //{
                //    return Unauthorized();
                //}
               
                // 2. Create user identity claims
<<<<<<< HEAD
                var claims = new List<Claim> { new Claim(ClaimTypes.Sid, dt.Rows[0]["UserID"].ToString()), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Surname, user.Surname), new Claim(ClaimTypes.Role, dt.Rows[0]["Access"].ToString())};
                //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
=======
                var claims = new List<Claim> { new Claim(ClaimTypes.Sid, dt.Rows[0]["UserID"].ToString()), new Claim(ClaimTypes.Name, name), new Claim(ClaimTypes.Surname, surname), new Claim(ClaimTypes.Role, dt.Rows[0]["Access"].ToString())};
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
>>>>>>> 549c746d102548e62eebefb2ff95bc5cb842d85d

                //// 3. Issue the session cookie
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Read JWT settings
                var jwtSettings = _config.GetSection("Jwt");

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
                );

                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

                // Generate JWT
                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        Convert.ToDouble(jwtSettings["ExpiryMinutes"])
                    ),
                    signingCredentials: credentials
                );

                string tokenString = new JwtSecurityTokenHandler()
                    .WriteToken(token);

                // Return token and user information
                LoginResponse loginResponse = new LoginResponse() { Token = tokenString, UserID = (int)dt.Rows[0]["UserID"], Name = user.Name, Surname = user.Surname, Access = dt.Rows[0]["Access"].ToString() };
               
                return Ok(loginResponse);
            }
            return NotFound();
        }

        [Authorize(Roles = "Organiser,Participant")]
        [HttpPut]
        [Route("auth/profile")]
        public IActionResult UpdateProfile(User userUpd)// this api updates the users profile
        {
            if (userUpd == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Users WHERE UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            using SqlCommand cmd = new SqlCommand("UPDATE Users SET Name = @NAME, Surname = @Surname, Access = @ACCESS, Password = @Password WHERE UserId = @ID", con);
            
            string name = (string.IsNullOrEmpty(userUpd.Name))? HttpContext.User.FindFirst(ClaimTypes.Name)?.Value : userUpd.Name;
            string surname = (string.IsNullOrEmpty(userUpd.Surname)) ? HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value : userUpd.Surname;
            string access = (string.IsNullOrEmpty(userUpd.Access)) ? HttpContext.User.FindFirst(ClaimTypes.Role)?.Value : userUpd.Access;
            string password = (string.IsNullOrEmpty(userUpd.Password)) ? dt.Rows[0]["Password"].ToString() : userUpd.Password;

<<<<<<< HEAD
=======
            string name = (String.IsNullOrEmpty(userUpd.Name))? HttpContext.User.FindFirst(ClaimTypes.Name)?.Value : userUpd.Name;
            string surname = (String.IsNullOrEmpty(userUpd.Surname)) ? HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value : userUpd.Surname;
            string access = (String.IsNullOrEmpty(userUpd.Access)) ? HttpContext.User.FindFirst(ClaimTypes.Role)?.Value : userUpd.Access;
            string password = (String.IsNullOrEmpty(userUpd.Password)) ? dt.Rows[0]["Password"].ToString() : userUpd.Password;

>>>>>>> 549c746d102548e62eebefb2ff95bc5cb842d85d
            cmd.Parameters.AddWithValue("@ID", HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Surname", surname);
            cmd.Parameters.AddWithValue("@Access", access);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return CreatedAtAction(nameof(GetProfile), new { id = userUpd.UserID }, userUpd);
        }

        //[Authorize(Roles = "Participant")]
        [HttpPost]
        [Route("category/{id:int}/participate")]
        public IActionResult SignUP(int id, Signup signup)// this apie signs a user up to an event
        {
            Console.WriteLine("signup reached");
            if (signup == null)
            {
                return NotFound();
            }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT Participant.ParticipantID from Participant INNER JOIN Users ON Participant.UserID = Users.UserID where Users.UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);

            if (dt.Rows.Count == 0) {
                string q2 = $"INSERT INTO Participant (UserID) VALUES ({HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value})";
                using SqlCommand cmd2 = new SqlCommand(q2, con);
                con.Open();
                con.Close();
                string q3 = $"SELECT Participant.ParticipantID from Participant INNER JOIN Users ON Participant.UserID = Users.UserID where Users.UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value}";
                dap = new SqlDataAdapter(q3, con);
                dt = new DataTable();
                dap.Fill(dt);
            }

            using SqlCommand cmd = new SqlCommand("INSERT INTO Signup (EventID, ParticipantID, CategoryID) VALUES (@EventID, @ParticipantID, @CategoryID)", con);

            cmd.Parameters.AddWithValue("@EventID", signup.EventID);
            cmd.Parameters.AddWithValue("@ParticipantID", dt.Rows[0]["ParticipantID"]);
<<<<<<< HEAD
            cmd.Parameters.AddWithValue("@CategoryID", signup.CategoryID);
=======
            cmd.Parameters.AddWithValue("@CategoryID", id);
>>>>>>> 549c746d102548e62eebefb2ff95bc5cb842d85d

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        //[Authorize(Roles = "Participant")]
        [HttpGet]
        [Route("participating")]//added new rout and api to get events signed up for
        public IActionResult SignedUP()// this apie signs a user up to an event
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = "SELECT Event.EventName,'Joined' AS Status FROM Signup "+
            "INNER JOIN Event "+
            "ON Event.EventID = Signup.EventID "+
            "INNER JOIN Participant as PR "+
            "ON PR.ParticipantID = Signup.ParticipantID "+
            "INNER JOIN Users "+
            "ON Users.UserID = PR.UserID "+
            $"WHERE Users.UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value} ";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            List<SignedUpResponse> results = new List<SignedUpResponse>();
            if (dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SignedUpResponse result = new();
                    result.EventName = dt.Rows[i]["EventName"].ToString();
                    result.Status =  dt.Rows[i]["Status"].ToString();
                    results.Add(result);
                }
            }

            return Ok(results);
        }

        [Authorize(Roles = "Participant")]
        [HttpGet]
        [Route("auth/profile/results")]
        public IActionResult GetResults()//this api gets users aprticipation results
        {
            List<ParticipantResultsResponse> results = new List<ParticipantResultsResponse>();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
<<<<<<< HEAD
            string query = "SELECT Event.EventName, Event.Date, Category.CategoryName, PR.Time, PR.Placement FROM ParticipantResults AS PR " +
            "INNER JOIN Event " +
            "ON Event.EventID = PR.EventID " +
            "INNER JOIN Signup " +
            "ON Signup.ParticipantID = PR.ParticipantID AND Signup.EventID = PR.EventID " +
            "INNER JOIN Category " +
            "ON Category.CategoryID = Signup.CategoryID " +
            "INNER JOIN Participant " +
            "ON Participant.ParticipantID = PR.ParticipantID " +
            "INNER JOIN Users " +
            "ON Users.UserID = Participant.UserID " +
            $"WHERE Users.UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value} "+
            "ORDER BY Event.Date DESC";
=======
            string query = $"SELECT PR.PRID, PR.ParticipantID, PR.EventID, PR.Placement, PR.Time FROM ParticipantResults AS PR " +
                $"INNER JOIN Participant ON PR.ParticipantID = Participant.ParticipantID " + //this query tells the database to retrieve this specific data 
                $"INNER JOIN Event ON Event.EventID = PR.EventID " +
                $"INNER JOIN Users ON Users.UserID = Participant.UserID " +
                $"WHERE Users.UserID = {HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value}";
>>>>>>> 549c746d102548e62eebefb2ff95bc5cb842d85d
            using SqlDataAdapter adp = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ParticipantResultsResponse result = new();
                    result.EventName = dt.Rows[i]["EventName"].ToString();
                    result.Date = (DateTime)dt.Rows[i]["Date"];
                    result.Category = dt.Rows[i]["CategoryName"].ToString();
                    result.Time = (TimeSpan)(dt.Rows[i]["Time"]);
                    result.Placement = (int)dt.Rows[i]["Placement"];
                
                    results.Add(result);
                }
            }
            return Ok(results);
        }

    }
}
