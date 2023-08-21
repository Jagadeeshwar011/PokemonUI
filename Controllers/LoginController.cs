using Newtonsoft.Json;
using Pokemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static System.Net.WebRequestMethods;

namespace Pokemon.Controllers
{
    public class LoginController : AsyncController
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> IndexAsync(string email, string password)
        {
            if (email == null)
                return View();
            UserModel user = new UserModel()
            {
                Email = email,
                Password = password
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            string endpoint = "https://localhost:7113/User/authenticate";
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var userDetails = JsonConvert.DeserializeObject<LoginResponse>(await Response.Content.ReadAsStringAsync());
                        TempData["userSession"] = email + Session.SessionID.ToString();
                        TempData.Keep("userSession");
                        TempData["userName"] = email;
                        TempData["userDetails"] = userDetails;
                        TempData.Keep("userDetails");
                        Session["userSession"] =userDetails;
                        return RedirectToAction("../Home/Index");
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

        //Logout
        public ActionResult Logout(string sessionId, string username)
        {
            foreach (var key in TempData.Keys.ToList())
            {
                TempData.Remove(key);
            }
            TempData["userSession"] = null;
            // SessionManagement.RemoveSession(sessionId);
            return RedirectToAction("../Login/Index");
        }
    }
}