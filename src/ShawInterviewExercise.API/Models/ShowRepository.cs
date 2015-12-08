using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace ShawInterviewExercise.API.Models
{
    public class ShowRepository : IShowRepository
    {
        private ShawInterviewDbEntities db = new ShawInterviewDbEntities();

        private int _id = 3;

        public ShowRepository()
        {
        }

        public IEnumerable<Show> GetAllShows()
        {
            IQueryable<Show> shows = from s in db.Shows
                                     select s;

            return shows;
        }

        public string AddShow(Show show)
        {
            var maxIdShow = db.Shows.OrderByDescending(i => i.Id).FirstOrDefault();
            //var item = db.Shows.MaxBy(i => i.ID);
            if (maxIdShow == null)
                show.Id = 1;
            else
                show.Id = maxIdShow.Id + 1;

            db.Shows.Add(show);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(e);
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            return "User added";
        }

        public Show EditShow(Show show)
        {
            IQueryable<Show> shows = from s in db.Shows
                                     where s.Id == show.Id
                                     select s;

            foreach (Show _show in shows)
            {
                _show.Name = show.Name;
                _show.Description = show.Description;
                _show.Title = show.Title;
                _show.ImageGuid = show.ImageGuid;
                _show.Image = show.Image;
                _show.ImageFile = show.ImageFile;
                _show.VideoUrl = show.VideoUrl;
                _show.Enabled = show.Enabled;
                _show.ShowDate = show.ShowDate;
                _show.Memo = show.Memo;
                _show.UpdatedAt = show.UpdatedAt;
                _show.UpdatedBy = show.UpdatedBy;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(e);
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return show;
        }

        public Show GetShowById(int id)
        {
            IQueryable<Show> shows = from s in db.Shows
                                     select s;

            var show = shows.FirstOrDefault<Show>((p) => p.Id == id);

            if (show == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return show;
        }

        public void DeleteShow(int id)
        {
            var deleteShows = from shows in db.Shows
                              where shows.Id == id
                              select shows;

            foreach (Show show in deleteShows)
            {
                db.Shows.Remove(show);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine(e);
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }
    }

}