namespace P03_SalesDatabase.Data.Models
{
    using System.Collections.Generic;

    public class Customer
    {
        public Customer()
        {
            Sales = new List<Sale>();
        }

        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CreditCardNumber  { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
