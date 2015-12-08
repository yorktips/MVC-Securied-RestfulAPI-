using ShawInterviewExercise.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShawInterviewExercise.API.Models;
using System.Diagnostics;
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace ShawInterviewExercise.Web.Controllers
{
    public class ShowsController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string webApiEntry = WebConfigurationManager.AppSettings["ShowWebApiEntry"];
        private static string showWsEntry = "api/show/";
        

        /*
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

            filterContext.Result = new ViewResult()
            {
                ViewName = "~/Views/Shared/ShowError.cshtml",
                ViewData= new HandleErrorInfo(filterContext.Exception, "ShowsController", "")
                ViewData = new ViewDataDictionary(model)
            };

        }
        */

        //
        // GET: /Shows/

        public ActionResult Index()
        {
            var urlGet = showWsEntry;
            List<Show> shows = null;
            try
            {
                shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);
            }
            catch (UnableConnectShowAPIException e)
            {
                logger.Error(e);
                //Debug.WriteLine(e);
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "ShowsController", "Index"));
                //return HttpNotFound();
            }

            if (shows == null)
            {
                return HttpNotFound();
            }
            return View(shows);

        }

        // GET api/<controller>/id
        public ActionResult Filter(string id)
        {
            var urlGet = showWsEntry;
            List<Show> shows = null;
            try
            {
                shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);
            }
            catch (UnableConnectShowAPIException e)
            {
                logger.Error(e);
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "ShowsController", "Index"));
            }

            if (shows == null)
            {
                return HttpNotFound();
            }

            if (id.ToLower().Equals("all")) 
                return View("Index",shows);
        

            List<Show> results = new List<Show>();

            string sPattern = "^[a-zA-Z]+$";
            foreach (Show s in shows) {
                string name = s.Name.ToLower();
                if (id.ToLower().Equals("aster"))
                {
                    if ( !Regex.IsMatch(name,sPattern) ) 
                        results.Add(s);
                }else{
                    if (name.ToLower().StartsWith(id.ToLower()))
                        results.Add(s);
                }
            }

            return View("Index",results);
        }


        public ActionResult Details(int id)
        {
            //TODO: Change this to look up show from API
            try
            {
                string urlGet = showWsEntry + id.ToString();
                List<Show> shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);
                if (shows == null)
                {
                    return HttpNotFound();
                }
                return View(shows[0]);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "ShowsController", "Details"));
            }

        }

    }
}
