namespace IRunes.Models
{
    using System.Collections.Generic;

    public class Track : BaseEntity<string>
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<TrackAlbum> TrackAlbums { get; set; } = new HashSet<TrackAlbum>();
    }
}
