namespace Employees.App.Commands
{
    internal interface ICommand
    {
        string Execute(params string[] args);
    }
}
