using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NETCore.Models;
using NETCore.Repositories.Data;
using NETCore.ViewModels;

namespace NETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IConfiguration _configuration;
        DynamicParameters parameters = new DynamicParameters();
        private readonly EmployeeRepository _repository;

        public UsersController
            (
            IConfiguration config, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            RoleManager<IdentityRole> roleManager,
            EmployeeRepository employeeRepository
            )
        {
            _configuration = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _repository = employeeRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(EmployeeVM employeeVM)
        {
            if (ModelState.IsValid)
            {
                var user = new User { };
                user.Id = Guid.NewGuid().ToString();
                user.UserName = employeeVM.Email;
                user.Email = employeeVM.Email;
                user.PasswordHash = employeeVM.Password;

                var result = await _userManager.CreateAsync(user, employeeVM.Password);
                result = await _userManager.AddToRoleAsync(user, "Member");
                if (result.Succeeded)
                {
                    var emp = await _repository.InsertEmployee(employeeVM);
                    if (emp != null)
                    {
                        return Ok("Register success");
                    }
                    return BadRequest("Failed to register employee");
                }
                else
                {
                    return BadRequest("Failed to register");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Get(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(userVM.Username, userVM.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(userVM.Username);
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("MyNetCoreConnection")))
                    {
                        var spName = "SP_GetInfo";
                        parameters.Add("@Id", user.Id);
                        IEnumerable<UserVM> data = connection.Query<UserVM>(spName, parameters, commandType: CommandType.StoredProcedure);
                        foreach (UserVM users in data)
                        {
                            userVM.Role = users.Role;
                        }
                    }

                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            //Create claims details based on the user information
                            //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("Username", user.UserName),
                            new Claim("Email", user.Email),
                            new Claim("Role", userVM.Role)
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        return BadRequest("Username/Password Wrong");
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("LogOut")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Log Out Success");
        }


        [HttpPost]
        [Route("AddRole")]
        public async Task<ActionResult> AddRole(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole();
                role.Name = userVM.Username;

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return Ok("Register success");
                }
                else
                {
                    return BadRequest("Failed to register");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            else
            {
                await _userManager.DeleteAsync(user);
                return Ok("User deleted");
            }
        }

        [HttpGet("{id}")]
        public async Task<User> Check(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            return user;
        }
    }
}