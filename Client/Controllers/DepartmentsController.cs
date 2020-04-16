using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Client.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NETCore.Models;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class DepartmentsController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44398/api/")
        };

        // GET: Departments
        public IActionResult Index()
        {
            //return View();
            return View(LoadDepartment()); //Tampilkan data berdasarkan fungsi loaddepartment
        }

        public JsonResult LoadDepartment()
        {
            DepartmentJson departmentVM = null;
            var responseTask = client.GetAsync("Departments"); //Access data from department API
            responseTask.Wait(); //Waits for the Task to complete execution.
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode) // if access success
            {
                var json = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                departmentVM = JsonConvert.DeserializeObject<DepartmentJson>(json); //Tampung setiap data didalam departments
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error");
            }
            return Json(departmentVM);  
        }

        public JsonResult InsertOrUpdate(Department department)
        {
            var myContent = JsonConvert.SerializeObject(department);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (department.Id == 0)
            {
                var result = client.PostAsync("Departments", byteContent).Result;
                return Json(result);
            }
            else
            {
                var result = client.PutAsync("Departments/" + department.Id, byteContent).Result;
                return Json(result);
            }
        }

        public JsonResult GetById(int Id)
        {
            DepartmentVM departmentVM = null;
            var responseTask = client.GetAsync("Departments/" + Id); //Access data from department API
            responseTask.Wait(); //Waits for the Task to complete execution.
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode) // if access success
            {
                var json = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                departmentVM = JsonConvert.DeserializeObject<DepartmentVM>(json); //Tampung setiap data didalam departments
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error");
            }
            return Json(departmentVM);
        }

        public JsonResult Delete(int Id)
        {
            var result = client.DeleteAsync("Departments/" + Id).Result;
            return Json(result);
        }


    }
}