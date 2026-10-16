using Microsoft.AspNetCore.Identity;

namespace PROGPOE.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Access { get; set; }
        public string Password { get; set; }
    }
}
