using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Model.Data;
using WebAPI_CRUD.Service.Interface;

namespace WebAPI_CRUD.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DapperDBContext context;
        IConfiguration configuration;
        public EmployeeService(IConfiguration configuration, DapperDBContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public (string Token, ClaimsPrincipal Principal) Auth([FromBody] User user)
        {
            string Token = "";
            ClaimsPrincipal principal = null;
            if (user != null)
            {
                if (IsValidUser(user))
                {
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature
                    );

                    var subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    });

                    var expires = DateTime.UtcNow.AddMinutes(10);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = expires,
                        SigningCredentials = signingCredentials
                    };


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    Token = jwtToken;
                    principal = new ClaimsPrincipal(subject);
                }
            }
            return (Token, principal);
        }

        public bool IsValidUser(User user)
        {
            bool result = false;
            var parameters = new DynamicParameters();
            parameters.Add("Username", user.UserName, DbType.String);
            parameters.Add("Password", user.Password, DbType.String);
            parameters.Add("IsAuthenticated", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            using (var conn = context.CreateConnection())
            {
                try
                {
                    var RESULT = conn.Query("uspAuthorizeUser", parameters, commandType: CommandType.StoredProcedure);
                    bool IsAuthenticated = parameters.Get<bool>("@IsAuthenticated");
                    result = IsAuthenticated;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        public async Task<int> Create(Employee employee)
        {
            int response = 0;
            var parameters = new DynamicParameters();
            parameters.Add("Username", employee.Username, DbType.String);
            parameters.Add("Password", employee.Password, DbType.String);
            parameters.Add("IsAdmin", employee.IsAdmin, DbType.Boolean);
            parameters.Add("Age", employee.Age, DbType.Int64);
            parameters.Add("Hobbies", employee.HobbiesJson, DbType.String);
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
                    response = "User deleted successfully...";
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
            parameters.Add("Hobbies", employee.HobbiesJson, DbType.String);
            using (var conn = context.CreateConnection())
            {
                try
                {
                    await conn.ExecuteAsync("uspUpdateUser", parameters, commandType: CommandType.StoredProcedure);
                    response = "User details updated successfully...";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return response;
        }

        public bool CheckIsAdmin(string Username)
        {
            bool IsAdmin = false;
            var parameters = new DynamicParameters();
            parameters.Add("Username", Username, DbType.Int64);
            parameters.Add("IsAdmin", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            using (var conn = context.CreateConnection())
            {
                try
                {
                    var RESULT = conn.Query("uspCheckUserIsAdmin", parameters, commandType: CommandType.StoredProcedure);
                    IsAdmin = parameters.Get<bool>("@IsAdmin");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return IsAdmin;
        }
    }
}
