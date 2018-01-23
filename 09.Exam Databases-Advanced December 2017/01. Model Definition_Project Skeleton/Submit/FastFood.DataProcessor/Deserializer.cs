using System;
using FastFood.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using FastFood.DataProcessor.Dto.Import;
using Newtonsoft.Json;
using FastFood.Models;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using FastFood.Models.Enums;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
            var sb = new StringBuilder();

            var deserializedUsers = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

            var validEmployees = new List<Employee>();

            foreach (var employeeDto in deserializedUsers)
            {
                if (!isValid(employeeDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Position position = context.Positions.SingleOrDefault(p => p.Name == employeeDto.Position);


                if (position == null)
                {
                    position = new Position
                    {
                        Name = employeeDto.Position
                    };

                    context.Positions.Add(position);
                    context.SaveChanges();
                }
                
                var employee = new Employee()
                {
                    Name = employeeDto.Name,
                    Age = employeeDto.Age,
                    Position = position

                };

                validEmployees.Add(employee);
                sb.AppendLine(string.Format(SuccessMessage, employeeDto.Name));
            }


            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
            var sb = new StringBuilder();

            var deserializedItems = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);

            var validItems = new List<Item>();

            foreach (var itemDto in deserializedItems)
            {
                if (!isValid(itemDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var itemAlreadyExists = validItems.Any(i => i.Name == itemDto.Name);

                if (itemAlreadyExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Category category = context.Categories.SingleOrDefault(c => c.Name == itemDto.Category);

                if (category == null)
                {
                    category = new Category()
                    {
                        Name = itemDto.Category,
                    };

                    context.Categories.Add(category);
                    context.SaveChanges();
                }

                var item = new Item()
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Category = category
                };

                validItems.Add(item);
                sb.AppendLine(string.Format(SuccessMessage, itemDto.Name));
            }
            context.Items.AddRange(validItems);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
            var serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));
            var deserializedOrders = (OrderDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();

            var validOrders = new List<Order>();

            foreach (var orderDto in deserializedOrders)
            {
                if (!isValid(orderDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var employee = context.Employees.SingleOrDefault(e => e.Name == orderDto.Employee);

                if (employee == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var dateTime = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var type = Enum.Parse<OrderType>(orderDto.Type);

                var orderItems = orderDto.Items.Select(oi => new OrderItem
                {
                    Item = context.Items.SingleOrDefault(i => i.Name == oi.Name),
                    Quantity = oi.Quantity

                }).ToArray();

                var order = new Order()
                {
                    Customer = orderDto.Customer,
                    DateTime = dateTime,
                    Employee = employee,
                    Type = type,
                    OrderItems = orderItems

                };

                validOrders.Add(order);
                sb.AppendLine($"Order for {orderDto.Customer} on {orderDto.DateTime} added");
            }
            context.Orders.AddRange(validOrders);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static bool isValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return isValid;
        }
    }
}