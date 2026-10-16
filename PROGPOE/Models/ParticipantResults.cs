namespace PROGPOE.Models
{
    public class ParticipantResults
    {
        //participant result model
        public int PRID { get; set; }
        public int ParticipantID { get; set; }
        public int EventID { get; set; }
        public int Placement { get; set; }
        public TimeSpan Time { get; set; }
    }
}
