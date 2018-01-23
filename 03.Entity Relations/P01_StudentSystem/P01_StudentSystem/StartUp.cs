namespace P01_StudentSystem
{
    using System;
    using P01_StudentSystem.Data;
    using P01_StudentSystem.Data.Models;

    class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new StudentSystemContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
