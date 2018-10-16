using System.Collections.Generic;

namespace IRunes.Models.Domain
{
    public class Track : BaseEntity<string>
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<TrackAlbum> TrackAlbums { get; set; } = new HashSet<TrackAlbum>();
    }
}
