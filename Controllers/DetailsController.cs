using Newtonsoft.Json;
using PagedList;
using Pokemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pokemon.Controllers
{
    public class DetailsController : AsyncController
    {
        // GET: Details
        public async Task<ActionResult> IndexAsync(int? page)
        
        {
            if (TempData["Fav"] != null)
            {
                var count = TempData["Fav"] as List<int>;
                ViewBag.favourite = count.Count;
            }
            else
            {
                ViewBag.favourite = "0";
            }

            if (TempData["userSession"] != null)
            {
                ViewBag.firstname = TempData["userName"];
            }
            string endpoint = "https://localhost:7113/Pokeman";
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var pokemonListModel = JsonConvert.DeserializeObject<List<PokemonModel>>(await Response.Content.ReadAsStringAsync());
                        TempData["pokemanList"] = pokemonListModel;
                        TempData.Keep("pokemanList");
                        var Users = pokemonListModel.Take(10).ToList();     //At first i will show only 5 data per page so i had used Take(5)  
                        int UsersCount = Convert.ToInt32(Math.Ceiling((double)pokemonListModel.Count() / 10));
                        ViewData["pl"] = Users;
                        return View(pokemonListModel.ToPagedList(page ?? 1, 3));
                        // return RedirectToAction("Home");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return View();
                    }
                }
            }



            // return View();
        }

        [HttpGet]
        public async Task<JsonResult> PageAsync(int NumberOfData)   //the object name should be same as used in JQuerry above it will get the value of dropdown  
        {
            string endpoint = "https://localhost:7113/Pokeman";
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var pokemonListModel = JsonConvert.DeserializeObject<List<PokemonModel>>(await Response.Content.ReadAsStringAsync());
                        TempData["pokemanList"] = pokemonListModel;
                        TempData.Keep("pokemanList");
                        ViewData["pl"] = pokemonListModel;
                        int UsersCount = Convert.ToInt32(Math.Ceiling((double)pokemonListModel.Count() / NumberOfData));
                        var Result = new { user = pokemonListModel, CountUser = UsersCount };
                        return Json(Result, JsonRequestBehavior.AllowGet);
                        // return RedirectToAction("Home");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return Json("error");
                       //  return View();
                    }
                }
            }
           
        }


    }
}