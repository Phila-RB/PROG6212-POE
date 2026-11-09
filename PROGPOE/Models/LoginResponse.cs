namespace PROGPOE.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Access { get; set; }
    }
}
