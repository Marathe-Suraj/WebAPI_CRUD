using Microsoft.AspNetCore.Mvc;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Service.Interface;

namespace WebAPI_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeservice;
        public EmployeeController(IEmployeeService repo)
        {
            this.employeeservice = repo;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var _list = await this.employeeservice.GetAll();
            if (_list != null)
            {
                return Ok(_list);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetbyID/{ID}", Name = "GetEmployee")]
        public async Task<IActionResult> GetbyID(string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                var _list = await this.employeeservice.GetbyID(value);
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
                var createdEmployee = await this.employeeservice.Create(employee);
                return CreatedAtRoute("GetEmployee", new { id = createdEmployee }, createdEmployee);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Employee employee, string ID)
        {
            int value = 0;
            if (int.TryParse(ID, out value))
            {
                var _result = await this.employeeservice.Update(employee, value);
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
                var _result = await this.employeeservice.Remove(value);
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
                return BadRequest("Please enter valid input...");
            }
        }
    }
}
