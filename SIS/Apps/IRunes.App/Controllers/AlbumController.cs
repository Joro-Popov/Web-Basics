using SIS.Framework.ActionResults.Contracts;

namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Models;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Enums;
    using SIS.WebServer.Results;

    public class AlbumController : BaseController
    {
        private const string EmptyAlbumsCollection = "There are currently no albums!";
        
        public IActionResult ListAlbums()
        {
            if (!this.IsAuthenticated(this.Request)) this.View("/");
            
            var username = this.Request.Session.GetParameter("username").ToString();

            var albums = this.DbContext.Users.FirstOrDefault(u => u.Username == username).Albums.ToList();

            if (albums.Count == 0)
            {
                this.ViewBag["albums"] = EmptyAlbumsCollection;
                return this.View();
            }

            this.ViewBag["albums"] = ConvertAlbumNamesToHtml(albums);

            return this.View();
        }
        
        public IActionResult CreateAlbum()
        {
            return !this.IsAuthenticated(this.Request) ? this.View("/") : this.View();
        }
        
        public IActionResult CreateAlbumPost()
        {
            if (!this.IsAuthenticated(this.Request)) return this.View("/");

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Request.Session.GetParameter("username").ToString());
            var albumName = this.Request.FormData["albumName"].ToString();
            var cover = this.Request.FormData["cover"].ToString();

            var albumNameIsInvalid = string.IsNullOrWhiteSpace(albumName);
            var albumNameExists = this.DbContext.Albums.Any(a => a.Name == albumName);
            var albumCoverIsInvalid = string.IsNullOrWhiteSpace(cover);
            
            if(albumCoverIsInvalid || albumNameExists || albumNameIsInvalid) return this.View("/Albums/Created");

            var album = new Album
            {
                Name = albumName,
                Cover = cover,
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

            return this.View("/Albums/All");
        }
        
        public IActionResult AlbumDetails()
        {
            if (!this.IsAuthenticated(this.Request)) return this.View("/");
            
            var albumId = this.Request.QueryData["albumId"].ToString();
            var username = this.Request.Session.GetParameter("username").ToString();
            var album = this.DbContext.Users.FirstOrDefault(u => u.Username == username)?.Albums.FirstOrDefault(a => a.Id == albumId);
            
            this.ViewBag["albumId"] = albumId;
            this.ViewBag["cover"] = WebUtility.UrlDecode(album.Cover);
            this.ViewBag["name"] = WebUtility.UrlDecode(album.Name);
            this.ViewBag["price"] = album.Price.ToString(CultureInfo.InvariantCulture);
            this.ViewBag["tracks"] = ConvertTrackNamesToHtml(album, albumId);

            return this.View();
        }
        
        public IActionResult CreateTrack()
        {
            if (!this.IsAuthenticated(this.Request)) return this.View("/");

            this.ViewBag["albumId"] = this.Request.QueryData["albumId"].ToString();
            return this.View();
        }
        
        public IActionResult CreateTrackPost()
        {
            if (!this.IsAuthenticated(this.Request)) return this.View("/");

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
            if (!this.IsAuthenticated(this.Request)) return this.View("/");

            var albumId = this.Request.QueryData["albumId"].ToString();
            var trackId = this.Request.QueryData["trackId"].ToString();

            var track = this.DbContext.Albums
                .FirstOrDefault(a => a.Id == albumId)?.TrackAlbums
                .FirstOrDefault(t => t.Track.Id == trackId)
                .Track;

            this.ViewBag["track"] = track.Link;
            this.ViewBag["name"] = WebUtility.UrlDecode(track.Name);
            this.ViewBag["price"] = track.Price.ToString(CultureInfo.InvariantCulture);
            this.ViewBag["albumId"] = albumId;

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
