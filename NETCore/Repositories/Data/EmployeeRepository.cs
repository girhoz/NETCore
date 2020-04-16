using Dapper;
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

        public EmployeeRepository(MyContext myContext, IConfiguration configuration) : base(myContext)
        {
            _configuration = configuration;
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

        public async Task<IEnumerable<EmployeeVM>> GetById(int Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
            {
                var spName = "SP_GetEmployeeById";
                parameters.Add("@Id", Id);
                var data = await connection.QueryAsync<EmployeeVM>(spName, parameters, commandType: CommandType.StoredProcedure);
                return data;
            }
        }
    }
}
