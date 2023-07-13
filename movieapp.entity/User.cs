using System;
using System.Collections.Generic;
using System.Text;

namespace movieapp.entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool isAdmin { get; set; }
        public List<UserWatched> UserWatched { get; set; }
    }
}
