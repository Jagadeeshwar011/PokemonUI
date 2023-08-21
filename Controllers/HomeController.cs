using Newtonsoft.Json;
using PagedList;
using Pokemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pokemon.Controllers
{
    public class HomeController : AsyncController
    {
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
                        var data = pokemonListModel.ToPagedList(page ?? 1, 5);
                        var count = data.Count;
                        TempData["pokemanList"] = data;
                        TempData.Keep("pokemanList");
                        ViewData["pl"] = data;
                        return View((pokemonListModel.ToPagedList(page ?? 1, 5)));
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public void CheckFavourite(string data)
        {
            ViewBag.Message = "Your application description page.";
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(string option, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewData["error"] = "";
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
                            //ViewData["pl"] = pokemonListModel;
                           
                            // return RedirectToAction("Home");
                        }
                    }
                }
                var list = TempData["pokemanList"] as List<PokemonModel>;
                search = search.ToLower().Trim();
                var filter = list.Find(x => x.Name.ToLower().Trim() == search);
                if (filter != null)
                {
                    var listPoke = new List<PokemonModel>();
                    listPoke.Add(filter);
                    ViewData["pl"] = listPoke;
                }
                else
                {
                    ViewData["pl"] = null;
                    ViewData["error"] = "no data found";
                }
            }
            else
            {
                var list = TempData["pokemanList"] as List<PokemonModel>;
                ViewData["pl"] = list;
            }
            return View();
            //if a user choose the radio button option as Subject  
           
        }

        public async Task AddFavouriteAsync(int id)
        {
            var userDetails = Session["userSession"];
            if (userDetails != null)
            {
                var details = Session["userSession"] as LoginResponse;
                string endpoint = "https://localhost:7113/UserFavorite"+ "?"+"userId=" + details.Id + "&pokemanId=" + id;
                using (HttpClient client = new HttpClient())
                {
                    using (var Response = await client.GetAsync(endpoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var pokemonListModel = JsonConvert.DeserializeObject<List<UserFavourite>>(await Response.Content.ReadAsStringAsync());
                            if (pokemonListModel != null)
                            {
                                var checkExist = pokemonListModel.Find(x => x.Pokeman.Id == id);
                                if (checkExist == null)
                                {
                                    await AddFavAsync(details.Id, id);
                                    ViewBag.favourite = pokemonListModel.Count + 1;
                                }
                                
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            /*
            
            if (TempData["Fav"] == null)
            {
                var ids = new List<int>();
                ids.Add(id);
                TempData["Fav"] = ids;
                TempData.Keep("Fav");
                ViewBag.favourite = ids.Count;
            }
            else
            {
                var list = TempData["Fav"] as IEnumerable<int>;
                if (list != null)
                {
                    if (list.Contains(id))
                    {
                        list = list.ToList();

                    }
                    else
                    {
                        var ids = new List<int>();
                        ids.Add(id);
                        TempData["Fav"] = ids;
                        TempData.Keep("Fav");
                        ViewBag.favourite = ids.Count;
                    }
                }
            }
            */
            
            // return Json("ok");
        }

        
        private async Task AddFavAsync(int userId, int pokemonId)
        {
            UserFavourite user = new UserFavourite()
            {
                UserId = userId,
                PokemanId = pokemonId
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            string endpoint = "https://localhost:7113/UserFavorite?userId=" + userId + "&pokemanId=" + pokemonId;
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.PostAsync(endpoint,null))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                       
                    }
                   
                }
            }
        }

        private async Task RemoveFavAsync(int userId, int pokemonId)
        {
            UserFavourite user = new UserFavourite()
            {
                UserId = userId,
                PokemanId = pokemonId
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            // UserFavorite?userId=7&pokemanId=1
            string endpoint = "https://localhost:7113/UserFavorite?userId="+userId + "&pokemanId=" + pokemonId;
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.DeleteAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                    }

                }
            }
        }
    }
}