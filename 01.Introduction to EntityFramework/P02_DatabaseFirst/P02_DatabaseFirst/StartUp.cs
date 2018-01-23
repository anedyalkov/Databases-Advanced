using System;
using P02_DatabaseFirst.Data;
using System.Linq;
using P02_DatabaseFirst.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace P02_DatabaseFirst
{
    class StartUp
    {
        static void Main()
        {
            var context = new SoftUniContext();

            using (context)
            {
                EmployeesFullInformation(context);
                EmployeesWithSalaryOver50000(context);
                EmployeesFromResearchAndDevelopment(context);
                AddingNewAddressAndUpdatingEmployee(context);
                EmployeesAndProjects(context);
                AddressesByTown(context);
                Employee147(context);
                DepartmentsWithMoreThan5Employees(context);
                FindLatest10Projects(context);
                IncreaseSalaries(context);
                FindEmployeesByFirstNameStartingWithSa(context);
                DeleteProjectById(context);
            }


            //15.	Remove Towns

            //var input = Console.ReadLine();

            //using (var context = new SoftUniContext())
            //{
            //    var townToDelete = context.Towns
            //        .SingleOrDefault(t => t.Name == input);

            //    var addressesToDelete = context.Addresses
            //        .Where(a => a.Town.Name == input)
            //        .ToList();

            //    foreach (var e in context.Employees)
            //    {
            //        if (addressesToDelete.Contains(e.Address))
            //        {
            //            e.AddressId = null;
            //        }
            //    }
            //}

        }

        private static void DeleteProjectById(SoftUniContext context)
        {
            var projectId = 2;

            var employeeProjects = context.EmployeesProjects
                .Where(ep => ep.ProjectId == projectId);

            foreach (var ep in employeeProjects)
            {
                context.EmployeesProjects.Remove(ep);
            }

            var project = context.Projects.Find(2);

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                .Take(10);

            foreach (var p in projects)
            {
                Console.WriteLine($"{p.Name}");
            }
        }

        private static void FindEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            string pattern = $"^Sa.*$";

            var employees = context.Employees
                .Where(e => Regex.Match(e.FirstName, pattern).Success)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }
        }

        private static void IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering"||
                e.Department.Name == "Tool Design"||
                e.Department.Name == "Marketing"||
                e.Department.Name == "Information Services ")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employees)
            {
                e.Salary *= 1.12m;
            }

            context.SaveChanges();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

        }

        private static void FindLatest10Projects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var p in projects)
            {
                Console.WriteLine(p.Name);
                Console.WriteLine(p.Description);
                Console.WriteLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

        }

        private static void DepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DeprtmentName = d.Name,
                    ManagersFullName = $"{d.Manager.FirstName} {d.Manager.LastName}",
                    Employees = d.Employees.Select(e => new
                    {
                        EmployeeFirstName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        Job = e.JobTitle,
                    })
                        //.OrderBy(e => e.EmployeeFirstName)
                        //.ThenBy(e => e.EmployeeLastName)
                        //.ToList()
                    }).ToList();

            foreach (var d in departments)
            {
                Console.WriteLine($"{d.DeprtmentName} - {d.ManagersFullName}");

                foreach (var e in d.Employees.OrderBy(e => e.EmployeeFirstName).ThenBy(e => e.EmployeeLastName))
                {
                    Console.WriteLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.Job}");
                }

                Console.WriteLine(new string('-', 10));
            }
        }

        private static void Employee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    FullName = $"{e.FirstName} {e.LastName}",
                    Job = e.JobTitle,
                    Projects = e.EmployeesProjects.Select(ep => new
                    {
                        ProjectName = ep.Project.Name
                    })
                    .OrderBy(p => p.ProjectName)
                }).ToList();

            foreach (var item in employee)
            {
                Console.WriteLine($"{item.FullName} - {item.Job}");

                foreach (var p in item.Projects)
                {
                    Console.WriteLine($"{p.ProjectName}");
                }
            }
        }

        private static void AddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(a => new
                {
                    EmployeesCount = a.Employees.Count,
                    Town = a.Town.Name,
                    Address = a.AddressText
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.Town)
                .ThenBy(a => a.Address)
                .Take(10)
                .ToList();

            foreach (var a in addresses)
            {
                Console.WriteLine($"{a.Address}, {a.Town} - {a.EmployeesCount} employees");
            }
        }

        private static void EmployeesAndProjects(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e =>
                    e.EmployeesProjects.Any(ep =>
                    ep.Project.StartDate.Year >= 2001 &&
                    ep.Project.StartDate.Year <= 2003))
                .Take(30)
                .Select(e => new
                {
                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                    EmployeeManagerFullName = $"{e.Manager.FirstName} {e.Manager.LastName}",
                    Projects = e.EmployeesProjects.Select(ep => new
                    {
                        ep.Project,
                        ep.Project.StartDate,
                        ep.Project.EndDate
                    }),
                }).ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.EmployeeFullName} - Manager: {e.EmployeeManagerFullName}");

                foreach (var p in e.Projects)
                {
                    Console.Write($"--{p.Project.Name} - " +
                        $"{p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - ");

                    if (p.EndDate == null)
                    {
                        Console.WriteLine("not finished");
                    }
                    else
                    {
                        Console.WriteLine($"{p.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
                    }
                }
            }
        }

        private static void AddingNewAddressAndUpdatingEmployee(SoftUniContext context)
        {
            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4,
            };

            var employee = context.Employees.SingleOrDefault(e => e.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            foreach (var a in addresses)
            {
                Console.WriteLine(a);
            }
        }

        private static void EmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DepartmentName = e.Department.Name,
                    Salary = e.Salary,
                }).ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }
        }

        private static void EmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                 .Where(e => e.Salary > 50000)
                 .Select(e => new
                 {
                     e.FirstName
                 })
                 .OrderBy(e => e.FirstName)
                 .ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName}");
            }
        }

        private static void EmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
             .Select(e => new
             {
                 e.FirstName,
                 e.LastName,
                 e.MiddleName,
                 e.JobTitle,
                 e.Salary
             }).ToList();

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }
        }
    }
}
