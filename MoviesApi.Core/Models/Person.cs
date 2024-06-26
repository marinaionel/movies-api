﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MoviesApi.Core.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Birth { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ICollection<Movie> DirectedMovies { get; set; } = new HashSet<Movie>();
        public ICollection<Movie> ActedInMovies { get; set; } = new HashSet<Movie>();
        [JsonIgnore]
        public ICollection<Account> Fans { get; set; } = new HashSet<Account>();
        [NotMapped]
        public bool? IsMyFavourite { get; set; } = null;
    }
}