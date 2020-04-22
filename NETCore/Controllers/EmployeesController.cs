using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using NETCore.Base;
using NETCore.Models;
using NETCore.Repositories.Data;
using NETCore.ViewModels;

namespace NETCore.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController<Employee, EmployeeRepository>
    {
        private readonly EmployeeRepository _repository;

        public EmployeesController(EmployeeRepository employeeRepository) : base(employeeRepository)
        {
            this._repository = employeeRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<EmployeeVM>> Get()
        {
            return await _repository.GetAll();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeVM>> Get(string id)
        {
            var get = await _repository.GetById(id);
            if (get == null)
            {
                return NotFound();
            }
            return Ok(get);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, Employee entity)
        {
            var put = await _repository.GetEmployee(id);
            if (put == null)
            {
                return BadRequest();
            }
            if (entity.FirstName != null) {
                put.FirstName = entity.FirstName;
            }
            if (entity.LastName != null)
            {
                put.LastName = entity.LastName;
            }
            if (entity.Email != null)
            {
                put.Email = entity.Email;
            }
            if (entity.Address != null)
            {
                put.Address = entity.Address;
            }
            if (entity.BirthDate != default(DateTime))
            {
                put.BirthDate = entity.BirthDate;
            }
            if (entity.PhoneNumber != null)
            {
                put.PhoneNumber = entity.PhoneNumber;
            }
            if (entity.Department_Id != 0)
            {
                put.Department_Id = entity.Department_Id;
            }
            put.UpdateDate = DateTimeOffset.Now;
            await _repository.Put(put);
            return Ok("Update Succesfull");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> Delete(string id)
        {
            var delete = await _repository.GetEmployee(id);
            if (delete == null)
            {
                return NotFound();
            }
            delete.DeleteDate = DateTimeOffset.Now;
            delete.IsDelete = true;
            await _repository.Put(delete);
            return delete;
        }
    }
}