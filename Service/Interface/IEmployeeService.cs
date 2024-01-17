using WebAPI_CRUD.Model;

namespace WebAPI_CRUD.Service.Interface
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAll();
        Task<Employee> GetbyID(int ID);
        Task<string> Create(Employee employee);
        Task<string> Update(Employee employee, int ID);
        Task<string> Remove(int ID);
    }
}
