namespace IRunes.App.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;

    using Models.Domain;
    using Models.ViewModels.Album;
    using Models.ViewModels.Track;
    
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Attributes.Action;
    using SIS.HTTP.Exceptions;
    using SIS.Framework.ActionResults.Contracts;

    public class AlbumsController : BaseController
    {
        private const string EMPTY_ALBUMS_COLLECTION = "There are currently no albums!";
        
        [HttpGet]
        [Authorize]
        public IActionResult All()
        {
            var albums = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Identity.Username).Albums.ToList();

            if (!albums.Any())
            {
                this.Model.Data["Albums"] = EMPTY_ALBUMS_COLLECTION;

                return this.View();
            }

            var albumsData = albums.Select(a => new AlbumsViewModel()
            {
                Name = WebUtility.UrlDecode(a.Name),
                Id = a.Id
            });
            
            this.Model.Data["Albums"] = albumsData;

            return this.View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return this.View();
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateAlbumViewModel model)
        {
            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Identity.Username);
            
            var albumNameIsInvalid = string.IsNullOrWhiteSpace(model.AlbumName);
            var albumNameExists = this.DbContext.Albums.Any(a => a.Name == model.AlbumName);
            var albumCoverIsInvalid = string.IsNullOrWhiteSpace(model.Cover);
            
            if(albumCoverIsInvalid || albumNameExists || albumNameIsInvalid) return this.RedirectToAction("/Albums/Create");

            var album = new Album
            {
                Name = model.AlbumName,
                Cover = model.Cover,
            };

            if (user.Albums.Any(a => a.Name == album.Name))
            {
                return this.RedirectToAction("/albums/all");
            }

            user.Albums.Add(album);

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction("/Albums/All");
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Details(string albumId)
        {
            var album = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username)?.Albums.FirstOrDefault(a => a.Id == albumId);

            var albumData = album.TrackAlbums.Select(t =>  new TrackAlbumViewModel()
            {
                Name = WebUtility.UrlDecode(t.Track.Name),
                AlbumId = albumId,
                TrackId = t.TrackId
            });

            this.Model.Data["AlbumId"] = albumId;
            this.Model.Data["Cover"] = WebUtility.UrlDecode(album.Cover);
            this.Model.Data["Name"] = WebUtility.UrlDecode(album.Name);
            this.Model.Data["Price"] = album.Price.ToString(CultureInfo.InvariantCulture);
            this.Model.Data["TrackAlbum"] = albumData;

            return this.View();
        }
    }
}
