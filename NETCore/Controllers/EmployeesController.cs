using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.Base;
using NETCore.Models;
using NETCore.Repositories.Data;

namespace NETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController<Employee, EmployeeRepository>
    {
        private readonly EmployeeRepository _repository;

        public EmployeesController(EmployeeRepository employeeRepository) : base(employeeRepository)
        {
            this._repository = employeeRepository;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Employee entity)
        {
            var put = await _repository.Get(id);
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
            put.UpdateDate = DateTimeOffset.Now;
            await _repository.Put(put);
            return Ok("Update Succesfull");
        }
    }
}