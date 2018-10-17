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
    using SIS.Framework.Services.Contracts;

    public class TracksController : BaseController
    {
        public TracksController(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        [HttpGet]
        public IActionResult Create(string albumId)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/home/index");

            this.Model.Data["albumId"] = albumId;

            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreateTrackViewModel model, string albumId)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/home/index");
            
            var price = decimal.Parse(model.Price);

            var trackNameIsInvalid = string.IsNullOrWhiteSpace(model.TrackName);
            var linkIsInvalid = string.IsNullOrWhiteSpace(WebUtility.UrlDecode(model.Link));
            var priceIsInvalid = price < 0;

            if (trackNameIsInvalid || linkIsInvalid || priceIsInvalid) return this.RedirectToAction("/tracks/create");

            var album = this.DbContext.Albums.FirstOrDefault(a => a.Id == albumId);

            var track = new Track()
            {
                Name = model.TrackName,
                Link = WebUtility.UrlDecode(model.Link),
                Price = price
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
                //return new ServerErrorResult(e.Message, HttpResponseStatusCode.InternalServerError);
            }

            return this.RedirectToAction($"/Albums/Details?albumId={albumId}");
        }

        [HttpGet]
        public IActionResult Details(string albumId, string trackId)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/home/index");
            
            var track = this.DbContext.Albums
                .FirstOrDefault(a => a.Id == albumId)?.TrackAlbums
                .FirstOrDefault(t => t.Track.Id == trackId)
                .Track;

            this.Model.Data["track"] = track.Link;
            this.Model.Data["name"] = WebUtility.UrlDecode(track.Name);
            this.Model.Data["price"] = track.Price.ToString(CultureInfo.InvariantCulture);
            this.Model.Data[albumId] = albumId;;

            return this.View();
        }
    }
}
