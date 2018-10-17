namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Models.Domain;
    using Models.ViewModels.Album;

    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Services.Contracts;

    public class AlbumsController : BaseController
    {
        private const string EMPTY_ALBUMS_COLLECTION = "There are currently no albums!";

        public AlbumsController(IAuthenticationService authenticationService) : base(authenticationService)
        {

        }

        [HttpGet]
        public IActionResult All()
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/Home/Index");

            var username = this.Request.Session.GetParameter("username").ToString();
            
            var albums = this.DbContext.Users.FirstOrDefault(u => u.Username == username).Albums.ToList();

            if (albums.Count == 0)
            {
                this.Model.Data["albums"] = EMPTY_ALBUMS_COLLECTION;

                return this.View();
            }

            this.Model.Data["albums"] = this.ConvertAlbumNamesToHtml(albums);

            return this.View();
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            if (this.AuthenticationService.IsAuthenticated(this.Request))
            {
                return this.View();
            }

            return this.RedirectToAction("/home/index");
        }
        
        [HttpPost]
        public IActionResult Create(CreateAlbumViewModel model)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/Home/Index");

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Request.Session.GetParameter("username").ToString());
            
            var albumNameIsInvalid = string.IsNullOrWhiteSpace(model.AlbumName);
            var albumNameExists = this.DbContext.Albums.Any(a => a.Name == model.AlbumName);
            var albumCoverIsInvalid = string.IsNullOrWhiteSpace(model.Cover);
            
            if(albumCoverIsInvalid || albumNameExists || albumNameIsInvalid) return this.RedirectToAction("/Albums/Create");

            var album = new Album
            {
                Name = model.AlbumName,
                Cover = model.Cover,
            };
            
            user.Albums.Add(album);

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                //return this.ServerErrorResult(e.Message);
            }

            return this.RedirectToAction("/Albums/All");
        }
        
        [HttpGet]
        public IActionResult Details(string albumId)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/Home/Index");
            
            var username = this.Request.Session.GetParameter("username").ToString();
            var album = this.DbContext.Users.FirstOrDefault(u => u.Username == username)?.Albums.FirstOrDefault(a => a.Id == albumId);

            this.Model.Data["albumId"] = albumId;
            this.Model.Data["cover"] = WebUtility.UrlDecode(album.Cover);
            this.Model.Data["name"] = WebUtility.UrlDecode(album.Name);
            this.Model.Data["price"] = album.Price.ToString(CultureInfo.InvariantCulture);
            this.Model.Data["tracks"] = ConvertTrackNamesToHtml(album, albumId);

            return this.View();
        }
        
        private string ConvertAlbumNamesToHtml(IEnumerable<Album> albums)
        {
            var convertedAlbums = new StringBuilder();

            foreach (var album in albums)
            {
                var albumName = WebUtility.UrlDecode(album.Name);

                convertedAlbums.AppendLine($"<p><a href=\"/Albums/Details?albumId={album.Id}\">{albumName}</a></p>");
            }

            return convertedAlbums.ToString();
        }

        private string ConvertTrackNamesToHtml(Album album, string albumId)
        {
            var tracks = new StringBuilder();

            var index = 1;

            foreach (var track in album.TrackAlbums)
            {
                var trackName = WebUtility.UrlDecode(track.Track.Name);

                tracks.AppendLine($"<li>{index}. <a href=\"/Tracks/Details?albumId={albumId}&trackId={track.Track.Id}\">{trackName}<a/></li>");

                index++;
            }

            return tracks.ToString();
        }
    }
}
