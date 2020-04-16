using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Client.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NETCore.Models;
using NETCore.ViewModels;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class EmployeesController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44398/api/")
        };
        // GET: Division
        public ActionResult Index()
        {
            return View(LoadEmployee()); //Tampilkan data berdasarkan fungsi loaddivision
        }

        public JsonResult LoadEmployee()
        {
            IEnumerable<EmployeeVM> employee = null;
            var responseTask = client.GetAsync("Employees"); //Access data from department API
            responseTask.Wait(); //Waits for the Task to complete execution.
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode) // if access success
            {
                var readTask = result.Content.ReadAsAsync<IList<EmployeeVM>>(); //Get all the data from the API
                readTask.Wait();
                employee = readTask.Result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error");
            }
            return Json(employee);
        }

        public JsonResult InsertOrUpdate(EmployeeVM employeeVM)
        {
            var myContent = JsonConvert.SerializeObject(employeeVM);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (employeeVM.Id == 0)
            {
                var result = client.PostAsync("Employees", byteContent).Result;
                return Json(result);
            }
            else
            {
                var result = client.PutAsync("Employees/" + employeeVM.Id, byteContent).Result;
                return Json(result);
            }
        }

        public JsonResult GetById(int Id)
        {
            IEnumerable<EmployeeVM> employee = null;
            var responseTask = client.GetAsync("Employees/" + Id); //Access data from department API
            responseTask.Wait(); //Waits for the Task to complete execution.
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode) // if access success
            {
                var readTask = result.Content.ReadAsAsync<IList<EmployeeVM>>(); //Get all the data from the API
                readTask.Wait();
                employee = readTask.Result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error");
            }
            return Json(employee);
        }

        public JsonResult Delete(int Id)
        {
            var result = client.DeleteAsync("Employees/" + Id).Result;
            return Json(result);
        }
    }
}