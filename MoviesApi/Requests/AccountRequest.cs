using System;

namespace MoviesApi.Requests
{
    public class AccountRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
