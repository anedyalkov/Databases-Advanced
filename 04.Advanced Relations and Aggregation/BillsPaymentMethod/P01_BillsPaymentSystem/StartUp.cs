using System;
using P01_BillsPaymentSystem.Data;
using Microsoft.EntityFrameworkCore;
using P01_BillsPaymentSystem.Data.Models;
using System.Globalization;
using System.Linq;

namespace P01_BillsPaymentSystem
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (var db = new BillsPaymentSystemContext())
            {
                db.Database.Migrate();

                //Seed(db);
            }

            //var userId = int.Parse(Console.ReadLine());

            //using (var db = new BillsPaymentSystemContext())
            //{
            //    var user = db.Users
            //         .Where(u => u.UserId == userId)
            //         .Select(u => new
            //         {
            //             Name = $"{u.FirstName} {u.LastName}",

            //             BankAccounts = u.PaymentMethods
            //             .Where(pm => pm.Type == PaymentMethodType.BankAccount)
            //             .Select(pm => pm.BankAccount).ToList(),

            //             CreditCards = u.PaymentMethods
            //             .Where(pm => pm.Type == PaymentMethodType.CreditCard)
            //             .Select(pm => pm.CreditCard).ToList()
            //         }).FirstOrDefault();

            //    if (!db.Users.Any(u => u.UserId==userId))
            //    {
            //        Console.WriteLine($"User with id {userId} not found!");
            //        return;
            //    }

            //    Console.WriteLine($"User: {user.Name}");

            //    var bankAccounts = user.BankAccounts;

            //    if (bankAccounts.Any())
            //    {
            //        Console.WriteLine("Bank Accounts:");

            //        foreach (var ba in bankAccounts)
            //        {
            //            Console.WriteLine($"-- ID: {ba.BankAccountId}");
            //            Console.WriteLine($"--- Balance: {ba.Balance:f2}");
            //            Console.WriteLine($"--- Bank: {ba.BankName}");
            //            Console.WriteLine($"--- SWIFT: {ba.SwiftCode}");
            //        }
            //    }



            //    var creditCards = user.CreditCards;

            //    if (creditCards.Any())
            //    {
            //        Console.WriteLine("Credit Cards:");

            //        foreach (var cc in creditCards)
            //        {
            //            Console.WriteLine($"-- ID: {cc.CreditCardId}");
            //            Console.WriteLine($"--- Limit: {cc.Limit:f2}");
            //            Console.WriteLine($"--- Limit Left: {cc.LimitLeft:f2}");
            //            Console.WriteLine($"--- Expiration Date: {cc.ExpirationDate.ToString("yyyy/MM",CultureInfo.InvariantCulture)}");
            //        }
            //    }
            //}
        }


        private static void Seed(BillsPaymentSystemContext db)
        {
            using (db)
            {
                var user = new User()
                {
                    FirstName = "Georgi",
                    LastName = "Georgiev",
                    Email = "joro@abv.bg",
                    Password = "abcde"
                };

                var creditCards = new CreditCard[]
                    {
                         new CreditCard
                         {
                            ExpirationDate =  DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Limit = 2000m,
                            MoneyOwed = 100m
                         },

                        new CreditCard
                        {
                            ExpirationDate =  DateTime.ParseExact("24/01/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Limit = 1000m,
                            MoneyOwed = 200m
                        }

                    };

                var bankAccount = new BankAccount()
                {
                    Balance = 1600m,
                    BankName = "DSK",
                    SwiftCode = "DSKBANK"
                };

                var paymentMethods = new PaymentMethod[]
                    {
                        new PaymentMethod()
                        {
                            User = user,
                            CreditCard = creditCards[0],
                            Type = PaymentMethodType.CreditCard
                        },

                        new PaymentMethod()
                        {
                            User = user,
                            CreditCard = creditCards[1],
                            Type = PaymentMethodType.CreditCard
                        },

                        new PaymentMethod()
                        {
                            User = user,
                            BankAccount = bankAccount,
                            Type = PaymentMethodType.BankAccount
                        }
                    };

                db.Users.Add(user);
                db.CreditCards.AddRange(creditCards);
                db.BankAccounts.Add(bankAccount);
                db.PaymentMethods.AddRange(paymentMethods);

                db.SaveChanges();
            }
        }
    }
}
