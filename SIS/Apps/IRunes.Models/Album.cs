using System;
using System.Linq;

namespace IRunes.Models
{
    using System.Collections.Generic;

    public class Album : BaseEntity<string>
    {
        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price =>
            Math.Round(this.TrackAlbums.Sum(t => t.Track.Price) - (this.TrackAlbums.Sum(t => t.Track.Price) * 0.13m), 2);

        public virtual ICollection<TrackAlbum> TrackAlbums { get; set; } = new HashSet<TrackAlbum>();
    }
}
