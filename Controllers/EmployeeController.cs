using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Service.Interface;

namespace WebAPI_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        public string Username = "";
        public EmployeeController(IEmployeeService repo)
        {
            employeeService = repo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            (string JWTToken, ClaimsPrincipal Principal) = employeeService.Auth(user);
            //HttpContext.User.AddIdentity(Principal.Identities.First());
            //var userNameClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, Principal);

            //if (userNameClaim != null)
            //{
            //    Username = userNameClaim.Value;
            //}
            if (JWTToken != "")
            {
                return Ok(JWTToken);
            }
            else
            {
                return Unauthorized();
            }
        }

        //[Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var obj = HttpContext.User;
            bool IsAdmin = employeeService.CheckIsAdmin(Username);
            if (IsAdmin)
            {
                var _list = await employeeService.GetAll();
                if (_list != null)
                {
                    return Ok(_list);
                }
            }
            else
            {
                return Unauthorized();
            }
            return NotFound();
        }

        //[Authorize]
        [HttpGet("GetbyID/{ID}", Name = "GetEmployee")]
        public async Task<IActionResult> GetbyID(string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                var _list = await employeeService.GetbyID(value);
                if (_list != null)
                {
                    return Ok(_list);
                }
                else
                {
                    return NotFound("User not found...");
                }
            }
            else
            {
                return BadRequest("Please enter valid input...");
            }
        }

        //[Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (employee.Username == "" || employee.Password == "")
            {
                return BadRequest("Please enter valid input...");
            }
            else
            {
                bool IsAdmin = employeeService.CheckIsAdmin(Username);
                if (IsAdmin)
                {
                    var createdEmployee = await employeeService.Create(employee);
                    return CreatedAtRoute("GetEmployee", new { id = createdEmployee }, createdEmployee);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        //[Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Employee employee, string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                var _result = await employeeService.Update(employee, value);
                if (_result != null)
                {
                    return Ok(_result);
                }
                else
                {
                    return NotFound("User not found...");
                }
            }
            else
            {
                return BadRequest("Please enter valid input...");
            }
        }

        //[Authorize]
        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                bool IsAdmin = employeeService.CheckIsAdmin(Username);
                if (IsAdmin)
                {
                    var _result = await employeeService.Remove(value);
                    if (_result != null)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound("User not found...");
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest("Please enter valid input...");
            }
        }
    }
}