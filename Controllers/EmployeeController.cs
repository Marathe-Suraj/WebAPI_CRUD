using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_CRUD.Model;
using WebAPI_CRUD.Service.Interface;

namespace WebAPI_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService repo;
        public EmployeeController(IEmployeeService repo)
        {
            this.repo = repo;

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var _list = await this.repo.GetAll();
            if (_list != null)
            {
                return Ok(_list);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetbyID/{ID}")]
        public async Task<IActionResult> GetbyCode(int ID)
        {
            var _list = await this.repo.GetbyID(ID);
            if (_list != null)
            {
                return Ok(_list);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            var _result = await this.repo.Create(employee);
            return Ok(_result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Employee employee, int ID)
        {
            var _result = await this.repo.Update(employee, ID);
            return Ok(_result);
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(int ID)
        {
            var _result = await this.repo.Remove(ID);
            return Ok(_result);
        }
    }
}
