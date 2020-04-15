﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.Base;
using NETCore.Models;
using NETCore.Repositories.Data;
using NETCore.Repositories.Interface;

namespace NETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : BaseController<Department, DepartmentRepository>
    {
        private readonly DepartmentRepository _repository;

        public DepartmentsController(DepartmentRepository repository) : base (repository)
        {
            this._repository = repository;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Department entity)
        {
            var put = await _repository.Get(id);
            if (put == null)
            {
                return BadRequest();
            }
            put.Name = entity.Name;
            put.UpdateDate = DateTimeOffset.Now;
            await _repository.Put(put);
            return Ok("Update Succesfull");
        }


    }
}