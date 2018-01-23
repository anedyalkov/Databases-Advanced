namespace Employees.App.Commands
{
    using Employees.Services;
    using System.Linq;

    class SetAddressCommand : ICommand
    {
        private readonly EmployeeService employeeService;

        public SetAddressCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        //<employeeId> <address> 
        public string Execute(params string[] args)
        {
            var employeeId = int.Parse(args[0]);

            var addressName = args.Skip(1).ToArray();

            var employeeName = employeeService.SetAddress(employeeId, addressName);

            return $"{employeeName}'s address was set to {string.Join(" ", addressName)}.";
        }
    }
}
