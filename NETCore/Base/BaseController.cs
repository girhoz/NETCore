﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.Repositories.Interface;

namespace NETCore.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository _repository;

        public BaseController(TRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<TEntity>> Get()
        {
            var get = await _repository.Get();
            return Ok(new { data = get });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> Get(int id)
        {
            var get = await _repository.Get(id);
            if (get == null)
            {
                return NotFound();
            }
            return Ok(get);
        }

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            entity.CreateDate = DateTimeOffset.Now;
            await _repository.Post(entity);
            return Ok("Insert Success");
            //return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<TEntity>> Delete(int id)
        {
            
            var delete = await _repository.Delete(id);
            if (delete == null)
            {
                return NotFound();
            }
            return delete;
        }
    }
}