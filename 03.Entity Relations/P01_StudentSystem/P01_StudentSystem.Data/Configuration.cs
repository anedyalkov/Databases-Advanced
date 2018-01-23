using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    class Configuration
    {
        internal static string ConnectionString { get; set; } = @"Server=(localdb)\MSSQLLocalDB;Database=StudentSystem;Integrated Security=True";
    }
}
