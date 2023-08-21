using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pokemon.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Pokemon.Controllers
{
    public class RegisterController : AsyncController
    {
        // GET: Register
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(UserModel userModel)
        {
            if (userModel == null)
                return View();
            
            // StringContent content = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            string endpoint = "https://localhost:7113/User";
            userModel.Role = "Admin";
            var content = JsonContent.Create(userModel);
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        return RedirectToAction("../Login/Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return View();
                    }
                }
            }
            // return RedirectToAction("../Home/Index");
        }
    }
}