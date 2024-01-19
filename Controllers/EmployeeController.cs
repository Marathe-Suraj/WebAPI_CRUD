using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Service.Interface;

namespace WebAPI_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public string Role = "";
        public EmployeeController(IEmployeeService repo, IHttpContextAccessor httpContext)
        {
            employeeService = repo;
            httpContextAccessor = httpContext;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            IActionResult response = Unauthorized();
            string JWTToken = employeeService.Auth(user);

            if (JWTToken != "")
            {
                response = Ok(new { token = JWTToken });
            }
            return response;
        }

        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            if (httpContextAccessor.HttpContext != null)
            {
                Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            if (Role == "Admin")
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

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (employee.Username == "" || employee.Password == "")
            {
                return BadRequest("Please enter valid input...");
            }
            else
            {
                if (httpContextAccessor.HttpContext != null)
                {
                    Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                }
                if (Role == "Admin")
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

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                if (httpContextAccessor.HttpContext != null)
                {
                    Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                }
                if (Role == "Admin")
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