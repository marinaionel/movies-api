using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Core.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        [NotMapped]
        public int Age
        {
            get
            {
                DateTime zeroTime = new(1, 1, 1);

                DateTime a = Birthday;
                DateTime b = DateTime.Now;

                TimeSpan span = b - a;
                int years = (zeroTime + span).Year - 1;
                return years;
            }
        }

    }
}
