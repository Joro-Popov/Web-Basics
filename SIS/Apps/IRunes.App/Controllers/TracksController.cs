namespace IRunes.App.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;

    using Models.Domain;
    using Models.ViewModels.Track;

    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Attributes.Action;
    using SIS.HTTP.Exceptions;

    public class TracksController : BaseController
    {
        [HttpGet]
        [Authorize]
        public IActionResult Create(string albumId)
        {
            this.Model.Data["albumId"] = albumId;

            return this.View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateTrackViewModel model, string albumId)
        {
            var trackNameIsInvalid = string.IsNullOrWhiteSpace(model.TrackName);
            var linkIsInvalid = string.IsNullOrWhiteSpace(model.Link);
            var priceIsInvalid = string.IsNullOrWhiteSpace(model.Price);
            
            if (trackNameIsInvalid || linkIsInvalid || priceIsInvalid)
                return this.RedirectToAction($"/tracks/create?albumId={albumId}");
            
            var album = this.DbContext.Albums.FirstOrDefault(a => a.Id == albumId);

            var track = new Track()
            {
                Name = model.TrackName,
                Link = WebUtility.UrlDecode(model.Link),
                Price = decimal.Parse(model.Price)
            };

            var trackAlbum = new TrackAlbum()
            {
                Track = track,
                AlbumId = albumId
            };

            album.TrackAlbums.Add(trackAlbum);

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction($"/Albums/Details?albumId={albumId}");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(string albumId, string trackId)
        {
            var track = this.DbContext.Albums
                .FirstOrDefault(a => a.Id == albumId)?.TrackAlbums
                .FirstOrDefault(t => t.Track.Id == trackId)
                .Track;

            this.Model.Data["albumId"] = albumId;
            this.Model.Data["track"] = track.Link;
            this.Model.Data["name"] = WebUtility.UrlDecode(track.Name);
            this.Model.Data["price"] = track.Price.ToString(CultureInfo.InvariantCulture);

            return this.View();
        }
    }
}
