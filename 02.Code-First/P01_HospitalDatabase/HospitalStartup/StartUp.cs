namespace HospitalStartup
{
    using System;
    using P01_HospitalDatabase;
    using P01_HospitalDatabase.Data.Models;

    class StartUp
    {
        static void Main(string[] args)
        {
            using (var db = new HospitalContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
