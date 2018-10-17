using IRunes.Models.ViewModels.Album;
using SIS.Framework.ActionResults.Contracts.Base;
using SIS.Framework.Attributes.Methods;

namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Models.Domain;

    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Services.Contracts;

    public class AlbumController : BaseController
    {
        private const string EmptyAlbumsCollection = "There are currently no albums!";

        public AlbumController(IAuthenticationService authenticationService) : base(authenticationService)
        {

        }

        [HttpGet]
        public IActionResult All()
        {
            var username = this.Request.Session.GetParameter("username").ToString();

            if (!this.AuthenticationService.IsAuthenticated(this.Request)) this.RedirectToAction("/Home/Index");
            
            var albums = this.DbContext.Users.FirstOrDefault(u => u.Username == username).Albums.ToList();

            if (albums.Count == 0)
            {
                return this.View();
            }

            return this.View();
        }
        
        [HttpGet]
        public IActionResult CreateAlbum()
        {
            if (this.AuthenticationService.IsAuthenticated(this.Request))
            {
                return this.View();
            }

            return this.RedirectToAction("/home/index");
        }
        
        public IActionResult CreateAlbumPost(CreateAlbumViewModel model)
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.RedirectToAction("/Home/Index");

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Request.Session.GetParameter("username").ToString());
            
            var albumNameIsInvalid = string.IsNullOrWhiteSpace(model.AlbumName);
            var albumNameExists = this.DbContext.Albums.Any(a => a.Name == model.AlbumName);
            var albumCoverIsInvalid = string.IsNullOrWhiteSpace(model.Cover);
            
            if(albumCoverIsInvalid || albumNameExists || albumNameIsInvalid) return this.RedirectToAction("/Albums/Created");

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
                //return new ServerErrorResult(e.Message, HttpResponseStatusCode.InternalServerError);
            }

            return this.RedirectToAction("/Albums/All");
        }
        
        public IActionResult AlbumDetails(string albumId)
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
        
        public IActionResult CreateTrack()
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.View("/");

            //TODO: Inject Create TrackViewModel...

            //this.ViewBag["albumId"] = this.Request.QueryData["albumId"].ToString();
            return this.View();
        }
        
        public IActionResult CreateTrackPost()
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.View("/");

            var albumId = this.Request.QueryData["albumId"].ToString();
            var trackName = this.Request.FormData["trackName"].ToString();
            var link = WebUtility.UrlDecode(this.Request.FormData["link"].ToString());
            var price = decimal.Parse(this.Request.FormData["price"].ToString());

            var trackNameIsInvalid = string.IsNullOrWhiteSpace(trackName);
            var linkIsInvalid = string.IsNullOrWhiteSpace(link);
            var priceIsInvalid = price < 0;

            if(trackNameIsInvalid || linkIsInvalid || priceIsInvalid) return this.View("/");

            var album = this.DbContext.Albums.FirstOrDefault(a => a.Id == albumId);
            
            var track = new Track()
            {
                Name = trackName,
                Link = link,
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

            return this.View($"/Albums/Details?albumId={albumId}");
        }
        
        public IActionResult TrackDetails()
        {
            if (!this.AuthenticationService.IsAuthenticated(this.Request)) return this.View("/");

            var albumId = this.Request.QueryData["albumId"].ToString();
            var trackId = this.Request.QueryData["trackId"].ToString();

            var track = this.DbContext.Albums
                .FirstOrDefault(a => a.Id == albumId)?.TrackAlbums
                .FirstOrDefault(t => t.Track.Id == trackId)
                .Track;

            //TODO: Inject TrackDetailsViewModel...

            //this.ViewBag["track"] = track.Link;
            //this.ViewBag["name"] = WebUtility.UrlDecode(track.Name);
            //this.ViewBag["price"] = track.Price.ToString(CultureInfo.InvariantCulture);
            //this.ViewBag["albumId"] = albumId;

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

                tracks.AppendLine($"<li>{index}.<a href=\"/Tracks/Details?albumId={albumId}&trackId={track.Track.Id}\">{trackName}<a/></li>");

                index++;
            }

            return tracks.ToString();
        }
        
    }
}
