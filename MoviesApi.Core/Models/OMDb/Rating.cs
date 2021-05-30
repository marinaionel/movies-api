using System;

namespace MoviesApi.Core.Models.OMDb
{
    [Obsolete]
    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
    }
}
