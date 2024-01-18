using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI_CRUD.Model;

namespace WebAPI_CRUD.Service.Interface
{
    public interface IEmployeeService
    {
        bool IsValidUser(User user);
        (string Token, ClaimsPrincipal Principal) Auth([FromBody] User user);
        Task<List<Employee>> GetAll();
        Task<Employee> GetbyID(int ID);
        Task<int> Create(Employee employee);
        Task<string> Update(Employee employee, int ID);
        Task<string> Remove(int ID);
        bool CheckIsAdmin(string Username);
    }
}
