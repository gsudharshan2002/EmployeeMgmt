namespace EmployeeMgmt.Core;

public interface IEmployeeRepository
{
    void Add(Employee e);
    Employee? GetById(int id);
    IEnumerable<Employee> GetAll();
    void Remove(int id);
    int Count { get; }
}
