using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NETCore.Models;
using NETCore.ViewModels;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class UsersController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44398/api/")
        };

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Member")
            {
                return View();
            }
            return RedirectToAction("AccessDenied", "Users");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserVM userVM)
        {
            var myContent = JsonConvert.SerializeObject(userVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = client.PostAsync("Users/Login", byteContent).Result;
            if (result.IsSuccessStatusCode)
            {
                //Get token and role from token
                var data = result.Content.ReadAsStringAsync().Result;
                var token = "Bearer " + data;
                var role = GetRole(data);
                var email = GetEmail(data);
                //Add token to session and role to session
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("JWToken", token);
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "Employees");
                }
                else
                {
                    return RedirectToAction("Index", "Users");
                }

            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("Role");
            return RedirectToAction("Login", "Users");
        }

        //Decode token to get role
        protected string GetRole(string token)
        {
            string secret = "sdfsdfsjdbf78sdyfssdfsdfbuidfs98gdfsdbf";
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            IEnumerable<Claim> data = claims.Claims;
            return data.SingleOrDefault(p => p.Type == "Role")?.Value;
        }

        protected string GetEmail(string token)
        {
            string secret = "sdfsdfsjdbf78sdyfssdfsdfbuidfs98gdfsdbf";
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            IEnumerable<Claim> data = claims.Claims;
            return data.SingleOrDefault(p => p.Type == "Email")?.Value;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public JsonResult LoadEmployee()
        {
            var email = HttpContext.Session.GetString("Email");
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44398/api/")
            };
            //Get the session with token and set authorize bearer token to API header
            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("JWToken"));
            var responseTask = client.GetAsync("Employees/" + email); //Access data from employees API
            responseTask.Wait(); //Waits for the Task to complete execution.
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode) // if access success
            {
                var readTask = result.Content.ReadAsAsync<IList<EmployeeVM>>(); //Get all the data from the API
                readTask.Wait();
                return Json(readTask.Result[0]);
            }
            else
            {
                return Json(result);
            }
        }

        public IActionResult Update(EmployeeVM employeeVM)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44398/api/")
            };
            //Get the session with token and set authorize bearer token to API header
            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("JWToken"));
            //Convert to Json input
            var myContent = JsonConvert.SerializeObject(employeeVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = client.PutAsync("Employees/" + employeeVM.Email, byteContent).Result;
            return RedirectToAction("Index", "Users");
        }
    }
}