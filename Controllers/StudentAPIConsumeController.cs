using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestfulAPIs_Asp.netCore.Models;

namespace ConsumeWebAPIs.Controllers
{
    public class StudentAPIConsumeController : Controller
    {
        private string LocalURL = "https://localhost:44351";
        public IActionResult Index()
        {
            List<StudentModel> Data = new List<StudentModel>();
            try
            {
                using (HttpClient Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(LocalURL);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsons"));
                    HttpResponseMessage response = Client.GetAsync("/Student/GetAllStudents").Result;
                    Client.Dispose();
                    if (response.IsSuccessStatusCode)
                    {
                        string StringData = response.Content.ReadAsStringAsync().Result;
                        Data = JsonConvert.DeserializeObject<List<StudentModel>>(StringData);
                    }
                    else
                    {
                        TempData["Error"] = $"{response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message; 
            }
            return View(Data);
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(StudentModel StdModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (HttpClient Client = new HttpClient())
                    {
                        Client.BaseAddress = new Uri(LocalURL);
                        var Data = JsonConvert.SerializeObject(StdModel);
                        var ContentData = new StringContent(Data, Encoding.UTF8, "application/json");
                        if (StdModel.StudentID == 0)
                        {
                            HttpResponseMessage response = Client.PostAsync("/Student/AddStudent", ContentData).Result;
                            TempData["Success"] = "Record added successfully!";
                            // TempData["Success"] = response.Content.ReadAsStringAsync().Result;
                        }                      
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Model State is not valid");
                    return View(StdModel);
                }
            }
            catch (Exception Ex)
            {
                TempData["Exception"] = Ex.Message;
            }
            return RedirectToAction("Index");
        }

    }
}
