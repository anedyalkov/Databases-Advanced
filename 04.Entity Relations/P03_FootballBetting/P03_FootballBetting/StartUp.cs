namespace P03_FootballBetting
{
    using System;
    using P03_FootballBetting.Data;
    using P03_FootballBetting.Data.Models;

    class StartUp
    {
        static void Main(string[] args)
        {
            using (var db = new FootballBettingContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
