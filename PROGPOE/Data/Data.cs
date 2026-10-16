using Microsoft.EntityFrameworkCore;
using PROGPOE.Models;

namespace PROGPOE.Data
{
    public class Data : DbContext
    {
        //database config
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=THERUSSIAN2\\SQLEXPRESS08;Database=RacedayDB;Trusted_Connection=True;");
        }
        

        public DbSet<User> User { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Signup> Signups { get; set; } 
        public DbSet<ParticipantResults> ParticipantsResults { get; set; }
    }
}
