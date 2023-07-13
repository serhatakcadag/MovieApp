using System;
using System.Collections.Generic;
using System.Text;

namespace movieapp.entity
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<UserWatched> UserWatched { get; set; }
    }
}
