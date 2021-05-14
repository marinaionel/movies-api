﻿using MoviesApi.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

#nullable disable

namespace MoviesApi.Core.Model
{
    public class Movie : ISearchable
    {
        public Movie()
        {
            Directors = new HashSet<Director>();
            Stars = new HashSet<Star>();
        }
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public long? Year { get; set; }

        public virtual ICollection<Director> Directors { get; set; }
        public virtual ICollection<Star> Stars { get; set; }
        [NotMapped]
        public string IdString { get => "tt" + Id; }
        [NotMapped]
        public string SearchString { get => string.Join(',', Title); }
    }
}