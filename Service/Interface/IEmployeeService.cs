using WebAPI_CRUD.Model;

namespace WebAPI_CRUD.Service.Interface
{
    public interface IEmployeeService
    {
        (bool IsAuthenticated, bool IsAdmin) IsValidUser(User user);
        string Auth(User user);
        Task<List<Employee>> GetAll();
        Task<Employee> GetbyID(int ID);
        Task<int> Create(Employee employee);
        Task<string> Update(Employee employee, int ID);
        Task<string> Remove(int ID);
    }
}
