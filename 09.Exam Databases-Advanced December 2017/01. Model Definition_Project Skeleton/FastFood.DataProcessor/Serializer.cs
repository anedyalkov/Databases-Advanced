using System;
using System.IO;
using FastFood.Data;
using System.Linq;
using FastFood.Models.Enums;
using FastFood.Models;
using FastFood.DataProcessor.Dto.Export;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace FastFood.DataProcessor
{
    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var employeeDto = new EmployeeDto()
            {
                Name = employeeName,

                Orders = context.Employees
                .SingleOrDefault(e => e.Name == employeeName)
                .Orders
                .Where(o => o.Type == type)
                .Select(o => new OrderDto
                {
                    Customer = o.Customer,
                    Items = o.OrderItems.Select(oi => new ItemDto
                    {
                        Name = oi.Item.Name,
                        Price = oi.Item.Price,
                        Quantity = oi.Quantity,
                    }).ToArray(),

                    TotalPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.Item.Price)
                })
                .OrderByDescending(e => e.TotalPrice)
                .ThenByDescending(e => e.Items.Count())
                .ToArray(),
            };

            var json = JsonConvert.SerializeObject(employeeDto, Formatting.Indented);
            return json;

        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categoriesNames = categoriesString.Split(",").ToArray();

            var categories = context.Categories
                .Where(c => categoriesNames.Contains(c.Name))
                .Select(c => new
                {
                    Name = c.Name,
                    Items = c.Items.ToArray()

                }).ToArray()
                .Select(c => new
                {
                    Name = c.Name,
                    MostPopularItem = c.Items
                    .Select(i => new
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(oi => oi.Quantity * i.Price),
                        TotalSum = i.OrderItems.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(i => i.TotalMade)
                    //.ThenByDescending(i => i.TotalSum)
                    .FirstOrDefault()
                })
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    MostPopularItem = new MostPopularItemDto()
                    {
                        Name = c.MostPopularItem.Name,
                        TotalMade = c.MostPopularItem.TotalMade,
                        TimesSold = c.MostPopularItem.TotalSum
                    }
                })
                .OrderByDescending(c => c.MostPopularItem.TotalMade)
                .ThenByDescending(c => c.MostPopularItem.TimesSold)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));
            serializer.Serialize(new StringWriter(sb), categories, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
        }
	}
}