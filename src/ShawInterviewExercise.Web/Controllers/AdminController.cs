using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShawInterviewExercise.Web.Models;
using ShawInterviewExercise.API.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;

namespace ShawInterviewExercise.Web.Controllers
{
    public class AdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string webApiEntry = WebConfigurationManager.AppSettings["ShowWebApiEntry"];
        private static string showWsEntry = "api/show/";

        //
        // GET: /Shows/
        public ActionResult Index()
        {
            var token = Session["show-access-token"];
            if (token==null) 
                return View("Login");

            string securityToken = token.ToString(); 
             if (securityToken == null || securityToken.Equals(""))
             {
                return View("Login");
             }

            var urlGet = showWsEntry;
            List<Show> shows = null;
            try
            {
                shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);
            }
            catch (UnableConnectShowAPIException e)
            {
                logger.Error(e);
                 return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "AdminController", "Index"));
                //return HttpNotFound();
            }

            if (shows == null)
            {
                Exception e= new HttpException(404, "Page not Found");
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "AdminController", "Index"));
                //return HttpNotFound();
            }
            return View(shows);
        }

        [HttpPost]
        public ActionResult Login(AdminLogin adminLogin)
        {
            string UserName=adminLogin.UserName;
            string Password=adminLogin.Password;

            //if (UserName.ToLower().Equals("admin") && Password.ToLower().Equals("admin"))
            if (UserName.ToLower().Equals("admin") )
            {
                string securityToken=Authenticate.GetAuthToken(UserName);
                Session["show-access-token"] = securityToken;
                //ShowsServices.SetAccessTokenToCookie(Response, securityToken);
                return RedirectToAction("Index");
            }
            return View("Login");
       }

        //
        // GET: /Shows/Details/5
        public ActionResult Details(int id = 0)
        {
            string urlGet = showWsEntry + id.ToString();
            List<Show> shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);
            if (shows == null)
            {
                Exception e = new HttpException(404, "Page not Found");
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "AdminController", "Index"));
            }
            return View(shows[0]);

        }

        //
        // GET: /Shows/Create
        public ActionResult Create()
        {
            TblShow tblshow = new TblShow();
            return View(tblshow);
        }

        //
        // POST: /Shows/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblShow tblShow)
        {
            if (ModelState.IsValid)
            {
                HttpContent content = null;
                Show show = new Show();

                show.Name = tblShow.Name;
                show.Title = tblShow.Title;
                show.Description = tblShow.Title;
                show.VideoUrl = tblShow.VideoUrl;
                show.Enabled = tblShow.Enabled;
                show.ShowDate = tblShow.ShowDate;
                show.Memo = tblShow.Memo;
                show.UpdatedAt = System.DateTime.Now;
                show.UpdatedBy = "ychen";

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase myfile = Request.Files[0];
                    String mypath = HttpContext.Server.MapPath("~/Images/");
                    String myFilename = myfile.FileName ?? "";
                    if (myFilename.Length > 1)
                    {
                        show.ImageFile = myfile.FileName;
                        content = ShowsServices.CreateMultipartContent(show, myfile, myfile.FileName);
                    }
                }

                if (content == null)
                {
                    content = ShowsServices.CreateMultipartContent(show, null, show.ImageFile);
                }

                HttpClient client = new HttpClient();
                var urlPost = webApiEntry + showWsEntry + show.Id.ToString();
                client.BaseAddress = new Uri(webApiEntry);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("show-access-token", "YWRtaW4gLCAyMDE1LTEyLTAxIDExOjUyOjQ5Wg==");

                var response = client.PostAsync(urlPost, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    Debug.WriteLine("Error");
                }

            }

            return View(tblShow);
        }

        //
        // GET: /Shows/Edit/5

        public ActionResult Edit(int id = 0)
        {
            string urlGet = showWsEntry + id.ToString();
            List<Show> shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);

            if (shows == null)
            {
                Exception e = new HttpException(404, "Page not Found");
                return View("~/Views/Shared/ShowError.cshtml", new HandleErrorInfo((Exception)e, "AdminController", "Index"));
                //return HttpNotFound();
            }

            return View(shows[0]);

        }

        //
        // POST: /Shows/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Show show)
        {
            if (ModelState.IsValid)
            {
                HttpContent content = null;
                show.UpdatedAt = System.DateTime.Now;
                show.UpdatedBy = "Ychen";

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase myfile = Request.Files[0];
                    String mypath = HttpContext.Server.MapPath("~/Images/");
                    String myFilename = myfile.FileName ?? "";
                    if (myFilename.Length > 1)
                    {
                        show.ImageFile = myfile.FileName;
                        content = ShowsServices.CreateMultipartContent(show, myfile, myfile.FileName);
                    }
                }

                if (content == null)
                {
                    content = ShowsServices.CreateMultipartContent(show, null, show.ImageFile);
                }

                HttpClient client = new HttpClient();
                var urlPut = webApiEntry + showWsEntry + show.Id.ToString();
                client.BaseAddress = new Uri(webApiEntry);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("show-access-token", "YWRtaW4gLCAyMDE1LTEyLTAxIDExOjUyOjQ5Wg==");
                var response = client.PutAsync(urlPut, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    Debug.WriteLine("Error");
                }
                return View(show);
            }

            return View(show);

        }

        //
        // GET: /Shows/Delete/5

        public ActionResult Delete(int id = 0)
        {
            string urlGet = showWsEntry + id.ToString();
            List<Show> shows = ShowsServices.CallGetApi<Show>(webApiEntry, urlGet);

            if (shows == null)
            {
                return HttpNotFound();
            }

            return View(shows[0]);
        }

        //
        // POST: /Shows/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string urlDelete = showWsEntry + id.ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(webApiEntry);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("show-access-token", "YWRtaW4gLCAyMDE1LTEyLTAxIDExOjUyOjQ5Wg==");
            //ShowsServices.SetAccessTokenToHead(client.DefaultRequestHeaders);

            HttpResponseMessage response = client.DeleteAsync(urlDelete).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }


}

