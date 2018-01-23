namespace BookShop
{
    using System;
    using System.Linq;
    using System.Globalization;
    using BookShop.Data;
    using BookShop.Initializer;
    using BookShop.Models;
    using System.Text.RegularExpressions;
    using System.Text;

    public class StartUp
    {
        static void Main()
        {
            using (var db = new BookShopContext())
            {

                //DbInitializer.ResetDatabase(db);

                //var input = Console.ReadLine();

                var result = GetMostRecentBooks(db);

                Console.WriteLine(result);

                //IncreasePrices(db);
                
            }
        }

        //1.	Age Restriction

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            int enumValue = -1;

            switch (command.ToLower())
            {
                case "minor":
                    enumValue = 0;
                    break;
                case "teen":
                    enumValue = 1;
                    break;
                case "adult":
                    enumValue = 2;
                    break;
            }

            var titles = context.Books
                .Where(b => b.AgeRestriction == (AgeRestriction)enumValue)
                .Select(b => b.Title)
                .OrderBy(t => t).ToArray();

            string result = String.Join(Environment.NewLine, titles);
            return result;
        }

        //2.	Golden Books

        public static string GetGoldenBooks(BookShopContext context)
        {

            var titles = context.Books
                .Where(b => b.EditionType == EditionType.Gold)
                .Where(b => b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            string result = String.Join(Environment.NewLine, titles);

            return result;
        }
        //3.	Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
            .Where(b => b.Price > 40)
            .OrderByDescending(b => b.Price)
            .Select(b => $"{b.Title} - ${b.Price:f2}")
            .ToArray();

            string result = String.Join(Environment.NewLine, books);

            return result;
        }
        //4. Not Released In
        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var titles = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            string result = String.Join(Environment.NewLine, titles);

            return result;
        }
        //5.	Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var titles = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            string result = String.Join(Environment.NewLine, titles);

            return result;
        }

        //6.	Released Before Date

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var givenDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < givenDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                .ToArray();

            string result = String.Join(Environment.NewLine, books);

            return result;
        }
        //7.	Author Search

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string pattern = $"^.*{input.ToLower()}$";

            var authors = context.Books
                .Where(b => Regex.Match(b.Author.FirstName.ToLower(), pattern).Success)
                .Select(b => $"{b.Author.FirstName} {b.Author.LastName}")
                .OrderBy(a => a)
                .Distinct()
                .ToArray();

            string result = String.Join(Environment.NewLine, authors);

            return result;
        }

        //8.	Book Search

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string pattern = $"^.*{input.ToLower()}.*$";

            var authors = context.Books
                .Where(b => Regex.Match(b.Title.ToLower(), pattern).Success)
                .Select(b => $"{b.Title}")
                .OrderBy(b => b)
                .ToArray();

            string result = String.Join(Environment.NewLine, authors);

            return result;
        }

        //9.	Book Search by Author

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            string pattern = $"^{input.ToLower()}.*$";

            var books = context.Books
                .Where(b => Regex.Match(b.Author.LastName.ToLower(), pattern).Success)
                .OrderBy(b => b.BookId)
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})");

            string result = String.Join(Environment.NewLine, books);

            return result;
        }

        //10.	Count Books

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksNumber = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            var result = booksNumber;

            return result;
        }

        //11.	Total Book Copies

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var author = context.Authors
                .Select(a => new
                {
                    Name = $"{a.FirstName} {a.LastName}",
                    NumberOfCopies = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(a => a.NumberOfCopies)
                .ToArray();

            //var authorStrings = author.Select(a => $"{a.Name} - {a.NumberOfCopies}").ToArray();

            //string result = String.Join(Environment.NewLine, authorStrings);

            var sb = new StringBuilder();

            foreach (var a in author)
            {
                sb.AppendLine($"{a.Name} - {a.NumberOfCopies}");
            }

            return sb.ToString();
        }

        //12.	Profit by Category

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesProfit = context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(a => a.TotalProfit)
                .ThenBy(a => a.Category)
                .ToArray();

            //var categoriesProfitAsString = categoriesProfit.Select(b => $"{b.Category} ${b.TotalProfit}").ToArray();

            //var result = String.Join(Environment.NewLine, categoriesProfitAsString);

            var sb = new StringBuilder();

            foreach (var cp in categoriesProfit)
            {
                sb.AppendLine($"{cp.Category} ${cp.TotalProfit}");
            }

            return sb.ToString();
        }
        //13.	Most Recent Books

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks.Select(cb => cb.Book)
                    .OrderByDescending(b => b.ReleaseDate).Take(3)
                }).ToArray();

            var builder = new StringBuilder();

            foreach (var category in categories)
            {
                builder.AppendLine($"--{category.Name}");

                foreach (var book in category.Books)
                {
                    string year = null;

                    if (book.ReleaseDate == null)
                    {
                        year = "no release date";
                       
                    }
                    else
                    {
                        year = book.ReleaseDate.Value.Year.ToString();
                    }

                    builder.AppendLine($"{book.Title} ({year})");
                }
            }
            return builder.ToString();
        }

        //14.	Increase Prices

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //15.	Remove Books

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.
                Where(b => b.Copies < 4200);

            int result = books.Count();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return result;
        }
    }
}
