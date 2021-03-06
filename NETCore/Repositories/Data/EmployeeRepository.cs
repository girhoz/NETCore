﻿using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using NETCore.Context;
using NETCore.Models;
using NETCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NETCore.Repositories.Data
{
    public class EmployeeRepository : GeneralRepository<Employee, MyContext>
    {
        DynamicParameters parameters = new DynamicParameters();
        IConfiguration _configuration { get; }
        private readonly MyContext _myContext;

        public EmployeeRepository(MyContext myContext, IConfiguration configuration) : base(myContext)
        {
            _configuration = configuration;
            _myContext = myContext;
        }
        
        public async Task<IEnumerable<EmployeeVM>> GetAll()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
            {
                var spName = "SP_GetAllEmployee";

                var data = await connection.QueryAsync<EmployeeVM>(spName, commandType: CommandType.StoredProcedure);
                return data;
            }
        }

        public async Task<IEnumerable<EmployeeVM>> GetById(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
            {
                var spName = "SP_GetEmployeeById";
                parameters.Add("@Id", Id);
                var data = await connection.QueryAsync<EmployeeVM>(spName, parameters, commandType: CommandType.StoredProcedure);
                return data;
            }
        }

        public async Task<IEnumerable<EmployeeVM>> InsertEmployee(EmployeeVM employee)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
            {
                var spName = "SP_InsertEmployee";
                parameters.Add("@Email", employee.Email);
                parameters.Add("@First", employee.FirstName);
                parameters.Add("@Last", employee.LastName);
                parameters.Add("@Birth", employee.BirthDate);
                parameters.Add("@Phone", employee.PhoneNumber);
                parameters.Add("@Address", employee.Address);
                parameters.Add("@Dept_Id", employee.Department_Id);
                var create = await connection.QueryAsync<EmployeeVM>(spName, parameters, commandType: CommandType.StoredProcedure);
                return create;
            }
        }

        public async Task<IEnumerable<ChartVM>> GetChart()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
            {
                var spName = "SP_GetChartEmployee";
                var data = await connection.QueryAsync<ChartVM>(spName, commandType: CommandType.StoredProcedure);
                return data;
            }
        }
    }
}
