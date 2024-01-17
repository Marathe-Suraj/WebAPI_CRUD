using System.Data;
using WebAPI_CRUD.Model.Data;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Service.Interface;
using Dapper;
using System;

namespace WebAPI_CRUD.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DapperDBContext context;
        public EmployeeService(DapperDBContext context)
        {
            this.context = context;
        }

        public async Task<int> Create(Employee employee)
        {
            int response = 0;
            var parameters = new DynamicParameters();
            parameters.Add("Username", employee.Username, DbType.String);
            parameters.Add("Password", employee.Password, DbType.String);
            parameters.Add("IsAdmin", employee.IsAdmin, DbType.Boolean);
            parameters.Add("Age", employee.Age, DbType.Int64);
            parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (var conn = context.CreateConnection())
            {
                try
                {
                    await conn.ExecuteAsync("uspInsertUser", parameters, commandType: CommandType.StoredProcedure);
                    int generatedId = parameters.Get<int>("@Id");
                    response = generatedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return response;
        }

        public async Task<List<Employee>> GetAll()
        {
            using (var conn = context.CreateConnection())
            {
                try
                {
                    var emplist = await conn.QueryAsync<Employee>("uspGetAllUsers");
                    return emplist.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<Employee>();
                }
            }
        }

        public async Task<Employee> GetbyID(int ID)
        {
            using (var conn = context.CreateConnection())
            {
                var emp = new Employee();
                try
                {
                    emp = await conn.QuerySingleOrDefaultAsync<Employee>("uspGeUserByID", new { ID }, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return emp;
            }
        }

        public async Task<string> Remove(int ID)
        {
            string response = string.Empty;
            using (var conn = context.CreateConnection())
            {
                try
                {
                    await conn.ExecuteAsync("uspDeleteUser", new { ID }, commandType: CommandType.StoredProcedure);
                    response = "pass";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return response;
        }

        public async Task<string> Update(Employee employee, int ID)
        {
            string response = string.Empty;
            var parameters = new DynamicParameters();
            parameters.Add("ID", ID, DbType.Int64);
            parameters.Add("Username", employee.Username, DbType.String);
            parameters.Add("Password", employee.Password, DbType.String);
            parameters.Add("isAdmin", employee.IsAdmin, DbType.Boolean);
            parameters.Add("Age", employee.Age, DbType.Int64);
            using (var conn = context.CreateConnection())
            {
                try
                {
                    await conn.ExecuteAsync("uspUpdateUser", parameters, commandType: CommandType.StoredProcedure);
                    response = "pass";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return response;
        }
    }
}
