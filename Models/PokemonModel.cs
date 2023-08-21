using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Pokemon.Models
{
    public class PokemonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }
    }
}