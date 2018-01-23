namespace Employees.App.Commands
{
    using Employees.DtoModels;
    using Employees.Services;

    using System;
    class EmployeePersonalInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public EmployeePersonalInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        //<employeeId>
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            EmployeePersonalDto epDto = employeeService.PersonalById(employeeId);

            string birthday = "[no birthday specified]";

            if (epDto.Birthday != null)
            {
                birthday = epDto.Birthday.Value.ToString("dd-MM-yyyy");
            }

            string address = epDto.Address ?? "[no address specified]";

            string result = $"ID: {employeeId} - {epDto.FirstName} {epDto.LastName} - ${epDto.Salary:f2}" 
                + Environment.NewLine +
                $"Birthday: {birthday}"
                + Environment.NewLine +
                $"Address: {address}";

            return result;
        }
    }
}
