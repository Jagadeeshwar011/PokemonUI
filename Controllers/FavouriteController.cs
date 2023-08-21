using Newtonsoft.Json;
using Pokemon.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace Pokemon.Controllers
{
    public class FavouriteController :AsyncController
    {
        // GET: Favourite
        public async Task<ActionResult> IndexAsync()
        {
            var userDetails = Session["userSession"];
            var details = Session["userSession"] as LoginResponse;
            string endpoint = "https://localhost:7113/UserFavorite?userId=" + details.Id;
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var pokemonListModel = JsonConvert.DeserializeObject<List<UserFavourite>>(await Response.Content.ReadAsStringAsync());
                        var listPoke = new List<PokemonModel>();
                        foreach (var item in pokemonListModel)
                        {
                            var itemVal = new PokemonModel();
                            itemVal.Name = item.Pokeman.Name;
                            itemVal.Description = item.Pokeman.Description;
                            itemVal.Image = item.Pokeman.Image;
                            itemVal.Id = item.Pokeman.Id;
                           
                            listPoke.Add(itemVal);
                        }
                        
                        ViewData["pl"] = listPoke;
                        return View();
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
            return View();
        }

        public async Task AddFavouriteAsync(int id)
        {
            var userDetails = Session["userSession"];
            if (userDetails != null)
            {
                var details = Session["userSession"] as LoginResponse;
                string endpoint = "https://localhost:7113/UserFavorite?userId=" + details.Id + "&pokemanId=" + id;
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
                                
                                    await RemoveFavAsync(details.Id, id);
                                ViewBag.favourite = pokemonListModel.Count - 1;
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
        private async Task RemoveFavAsync(int userId, int pokemonId)
        {
            UserFavourite user = new UserFavourite()
            {
                UserId = userId,
                PokemanId = pokemonId
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            // UserFavorite?userId=7&pokemanId=1
            string endpoint = "https://localhost:7113/UserFavorite?userId=" + userId + "&pokemanId=" + pokemonId;
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