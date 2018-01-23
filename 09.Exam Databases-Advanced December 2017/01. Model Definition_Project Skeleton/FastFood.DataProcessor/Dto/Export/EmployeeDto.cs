using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastFood.DataProcessor.Dto.Export
{
    public class EmployeeDto
    {
        public string Name { get; set; }

        public OrderDto[] Orders { get; set; }

        public decimal TotalMade
        {
            get { return this.Orders.Sum(o => o.TotalPrice);}
        }
    }
}
