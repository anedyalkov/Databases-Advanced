using System;
using FastFood.Data;
using System.Linq;

namespace FastFood.DataProcessor
{
    public static class Bonus
    {
	    public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
	    {
            var item = context.Items.SingleOrDefault(i => i.Name == itemName);

            var result = string.Empty;

            if (item == null)
            {
                result = $"Item {itemName} not found!";
                return result;
            }

            var oldPrice = item.Price;

            item.Price = newPrice;

            context.SaveChanges();

            result = $"{item.Name} Price updated from ${oldPrice:F2} to ${newPrice:F2}";

            return result;
           
	    }
    }
}
