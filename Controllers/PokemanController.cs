using Pokemon.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Pokemon.Controllers
{
    public class PokemanController : AsyncController
    {
       

        public async Task<ActionResult> IndexAsync(int id)
        {
            string endpoint = "https://localhost:7113/Pokeman/" + id;
            using (HttpClient client = new HttpClient())
            {
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var pokemonListModel = JsonConvert.DeserializeObject<PokemonModel>(await Response.Content.ReadAsStringAsync());
                        var listPoke = new List<PokemonModel>();
                        listPoke.Add(pokemonListModel);
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

        [HttpPost]
        public async Task<ActionResult> IndexAsync(HttpPostedFileBase file, PokemonModel pokemonModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string path = Server.MapPath("~/Images/");

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        string fullPath = Path.Combine(path, fileName);
                        file.SaveAs(fullPath);
                        // string path1 = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
                        using (Image image = Image.FromFile(fullPath))
                        {
                            using (MemoryStream m = new MemoryStream())
                            {
                                image.Save(m, image.RawFormat);
                                byte[] imageBytes = m.ToArray();

                                // Convert byte[] to Base64 String
                                string base64String = Convert.ToBase64String(imageBytes);
                                pokemonModel.Name = file.FileName;
                                pokemonModel.Image = base64String;
                                string endpoint = "https://localhost:7113/Pokeman";
                                var content = JsonContent.Create(pokemonModel);
                                using (HttpClient client = new HttpClient())
                                {
                                    using (var Response = await client.PostAsync(endpoint, content))
                                    {
                                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            return RedirectToAction("../Home/Index");
                                        }
                                        else
                                        {
                                            ModelState.Clear();
                                            ModelState.AddModelError(string.Empty, "File upload not done succesfully");
                                            return View();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.FileStatus = ex.Message.ToString();
                    return View();
                }
            }
            return View();

        }
    }
}