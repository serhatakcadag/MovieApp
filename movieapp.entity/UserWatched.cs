using System;
using System.Collections.Generic;
using System.Text;

namespace movieapp.entity
{
    public class UserWatched
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
