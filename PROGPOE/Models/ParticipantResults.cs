namespace PROGPOE.Models
{
    public class ParticipantResults
    {
        public int PRID { get; set; }
        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int Placement { get; set; }
        public TimeOnly Time { get; set; }
    }
}
