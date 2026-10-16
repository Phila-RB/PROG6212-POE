using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PROGPOE.Models;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace PROGPOE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EventController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [Route("categories")]
        public IActionResult CreateCategory(Category newCat)
        {
            if (newCat == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlCommand cmd = new SqlCommand("INSERT INTO Category (CategoryName) VALUES (@Name)", con);

            cmd.Parameters.AddWithValue("@Name", newCat.CategoryName);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
            //return CreatedAtAction(nameof(GetCategory), new { id = newCat.CategoryID }, newCat);
        }

        [HttpGet]
        [Route("categories")]
        public IActionResult GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Category", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            Category category = new Category();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    category.CategoryID = Convert.ToInt32(dt.Rows[i]["CategoryID"]);
                    category.CategoryName = Convert.ToString(dt.Rows[i]["CategoryName"]);
                    categories.Add(category);
                }

            }
            return Ok(categories);
        }

        [HttpPost]
        [Route("/events")]
        public IActionResult CreateEvents(Event newEvent)
        {
            if (newEvent == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlCommand cmd = new SqlCommand("INSERT INTO Event (CategoryID, EventName, Location, Description, Date, Type) VALUES (@catID, @name, @loc, @des, @date, @type)", con);

            cmd.Parameters.AddWithValue("@catID", newEvent.CategoryID);
            cmd.Parameters.AddWithValue("@name", newEvent.EventName);
            cmd.Parameters.AddWithValue("@loc", newEvent.Location);
            cmd.Parameters.AddWithValue("@des", newEvent.Description);
            cmd.Parameters.AddWithValue("@date", newEvent.Date);
            cmd.Parameters.AddWithValue("@type", newEvent.Type);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        [HttpGet]
        [Route("events/{id:int}")]
        public IActionResult GetEvent(int id)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Event WHERE EventID = {id}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            Event newEvent = new Event();
            if (dt.Rows.Count > 0)
            {
                newEvent.EventID = Convert.ToInt32(dt.Rows[0]["EventID"]);
                newEvent.CategoryID = Convert.ToInt32(dt.Rows[0]["CategoryID"]);
                newEvent.EventName = Convert.ToString(dt.Rows[0]["EventName"]);
                newEvent.Location = Convert.ToString(dt.Rows[0]["Location"]);
                newEvent.Description = Convert.ToString(dt.Rows[0]["Description"]);
                newEvent.Date = Convert.ToDateTime(dt.Rows[0]["Date"]);
                newEvent.Type = Convert.ToString(dt.Rows[0]["Type"]);
                return Ok(newEvent);
            }
            return NotFound();
        }

        [HttpPut]
        [Route("events/{id:int}")]
        public IActionResult UpdateEvent(int id, Event upd)
        {
            Console.Out.WriteLine(upd);
            if (upd == null)
            {
                return NotFound();
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT * FROM Event WHERE EventID = {id}";
            SqlDataAdapter dap = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            using SqlCommand cmd = new SqlCommand("UPDATE Event SET CategoryID = @CategoryID, EventName = @EventName, Location = @Location," +
                " Description = @Description, Date = @Date, Type = @Type WHERE EventID = @ID", con);

            int categoryID = (upd.CategoryID != 0) ? upd.CategoryID : (int)dt.Rows[0]["CategoryID"];
            string eventName = (!String.IsNullOrEmpty(upd.EventName)) ? upd.EventName : dt.Rows[0]["EventName"].ToString();
            string location = (!String.IsNullOrEmpty(upd.Location)) ? upd.Location : dt.Rows[0]["Location"].ToString();
            string description = (!String.IsNullOrEmpty(upd.Description)) ? upd.Description : dt.Rows[0]["Description"].ToString();
            DateTime Date = (upd.Date == null || upd.Date == (DateTime)dt.Rows[0]["Date"] ) ? upd.Date : (DateTime)dt.Rows[0]["Date"];
            string Type = (!String.IsNullOrEmpty(upd.Type)) ? upd.Type : dt.Rows[0]["Type"].ToString();

            cmd.Parameters.AddWithValue("@ID", id);
            cmd.Parameters.AddWithValue("@CategoryID", categoryID);
            cmd.Parameters.AddWithValue("@EventName", eventName);
            cmd.Parameters.AddWithValue("@Location", location);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Date", Date);
            cmd.Parameters.AddWithValue("@Type", Type);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        [HttpGet]
        [Route("/events")]
        public IActionResult GetAllEvents()
        {
            List<Event> events = new List<Event>();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            using SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Event", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            Event newEvent = new Event();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    newEvent.EventID = Convert.ToInt32(dt.Rows[i]["EventID"]);
                    newEvent.CategoryID = Convert.ToInt32(dt.Rows[i]["CategoryID"]);
                    newEvent.EventName = Convert.ToString(dt.Rows[i]["EventName"]);
                    newEvent.Location = Convert.ToString(dt.Rows[i]["Location"]);
                    newEvent.Description = Convert.ToString(dt.Rows[i]["Description"]);
                    newEvent.Date = Convert.ToDateTime(dt.Rows[i]["Date"]);
                    newEvent.Type = Convert.ToString(dt.Rows[i]["Type"]);
                    events.Add(newEvent);
                }
                
            }
            return Ok(events);
        }

        [HttpPost]
        [Route("results")]
        public IActionResult CreateResults(ParticipantResults results)
        {
            if (results == null)
            {
                return NotFound();
            }
            string query = $"SELECT * FROM Participant, Event WHERE EventID = {results.EventID} AND ParticipantID = {results.ParticipantID};";
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            SqlDataAdapter adp = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count == 0) {
                return NotFound("sorry please double check the event or participant as what u entered may not exist");
            }
            using SqlCommand cmd = new SqlCommand("INSERT INTO ParticipantResults (ParticipantID, EventID, Placement, Time) VALUES(@ParticipantID, @EventID, @Placement, @Time)", con);

            cmd.Parameters.AddWithValue("@ParticipantID", results.ParticipantID);
            cmd.Parameters.AddWithValue("@EventID", results.EventID);
            cmd.Parameters.AddWithValue("@Placement", results.Placement);
            cmd.Parameters.AddWithValue("@Time", results.Time);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return Created();
        }

        [HttpGet]
        [Route("api/events/{id:int}/participants")]
        public IActionResult GetAllParticipants(int id)
        {
            List<Participant> participants = new List<Participant>();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            string query = $"SELECT Participant.ParticipantID, Participant.UserID FROM Signup " +
                $"INNER JOIN Participant ON Participant.ParticipantID = Signup.ParticipantID " +
                $"INNER JOIN Event ON Event.EventID = Signup.EventID " +
                $"WHERE Event.EventID = {id}";
            using SqlDataAdapter adp = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            Participant participant = new Participant();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    participant.ParticipantID = Convert.ToInt32(dt.Rows[i]["ParticipantID"]);
                    participant.UserID = Convert.ToInt32(dt.Rows[i]["UserID"]);
                    participants.Add(participant);
                }

            }
            return Ok(participants);
        }

        [HttpDelete]
        [Route("api/events/{id:int}")]
        public IActionResult DeleteEvent(int id)
        {
            string query = $"DELETE FROM Event WHERE EventID = {id}";
            SqlConnection con = new SqlConnection(_config.GetConnectionString("RaceDayCon").ToString());
            
            using SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            return NoContent();
          
        }
    }
}
