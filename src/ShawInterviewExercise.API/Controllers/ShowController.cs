using ShawInterviewExercise.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Diagnostics;

namespace ShawInterviewExercise.API.Controllers
{

    public class ShowController : ApiController
    {

        //readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IShowRepository repository;
        string StoragePath = "";
        string root = HttpContext.Current.Server.MapPath("~/Images");

        public ShowController(ShowRepository repository_)
        {
            repository = repository_;
            root = HttpContext.Current.Server.MapPath("~/Images");
            StoragePath = root.Replace("ShawInterviewExercise.API", "ShawInterviewExercise.Web") + "\\";
            Debug.WriteLine("StoragePath=" + StoragePath);
        }

        public ShowController()
        {
            repository = new ShowRepository();
            root = HttpContext.Current.Server.MapPath("~/Images");
            StoragePath = root.Replace("ShawInterviewExercise.API", "ShawInterviewExercise.Web") + "\\";
            Debug.WriteLine("StoragePath=" + StoragePath);
        }


        // GET api/<controller>
        public string Get()
        {
            IEnumerable<Show> shows = repository.GetAllShows();
            return JsonConvert.SerializeObject(shows);
        }


        // GET api/<controller>/1
        public string Get(int id)
        {
            Show show = repository.GetShowById(id);
            if (show == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return JsonConvert.SerializeObject(show);

        }



        // POST api/<controller>        
        //public Show Post([FromBody]Show show)
        public async Task<HttpResponseMessage> Post()
        {
            //Need to pass Auth
            if (!Authenticate.Auth(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            Show show = new Show();
            string imageGuid= Guid.NewGuid().ToString();
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            StringBuilder sb = new StringBuilder(); // Holds the response body

            // Read the form data and return an async task.
            await Request.Content.ReadAsMultipartAsync(provider);

            // This illustrates how to get the form data.
            foreach (var key in provider.FormData.AllKeys)
            {
                foreach (var val in provider.FormData.GetValues(key))
                {
                    string jsonString = val;
                    Debug.WriteLine(jsonString);
                    var varShows = CommonServices.JsonDeserialize<Show>(jsonString);
                    List<Show> shows = (List<Show>)varShows;
                    show = shows[0];
                }
            }

            foreach (var file in provider.FileData)
            {
                show.Image = System.IO.File.ReadAllBytes(file.LocalFileName);
                show.ImageGuid = imageGuid + CommonServices.GetFileAttrName(show.ImageFile);
                CommonServices.FileCopy(file.LocalFileName, StoragePath +  show.ImageGuid);
                //CommonServices.FileCopy(file.LocalFileName, StoragePath + show.ImageFile);
            }

            var response = repository.AddShow(show);

            return new HttpResponseMessage()
            {
                Content = new StringContent(response)
            };

        }



        // PUT api/<controller>/5
        //public async Task<HttpResponseMessage> Put(int id, [FromBody]Show show)
        //public void Put(int id)
        public async Task<HttpResponseMessage> Put(int id)
        {
            if (!Authenticate.Auth(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            Show show = new Show();

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            StringBuilder sb = new StringBuilder(); // Holds the response body

            // Read the form data and return an async task.
            await Request.Content.ReadAsMultipartAsync(provider);

            // This illustrates how to get the form data.
            foreach (var key in provider.FormData.AllKeys)
            {
                foreach (var val in provider.FormData.GetValues(key))
                {
                    string jsonString = val;
                    Debug.WriteLine(jsonString);
                    var varShows = CommonServices.JsonDeserialize<Show>(jsonString);
                    List<Show> shows = (List<Show>)varShows;
                    show = shows[0];
                }
            }


            // This illustrates how to get the file names for uploaded files.
            foreach (var file in provider.FileData)
            {
                show.Image = System.IO.File.ReadAllBytes(file.LocalFileName);
                CommonServices.FileCopy(file.LocalFileName, StoragePath + show.ImageGuid);
                //CommonServices.FileCopy(file.LocalFileName, StoragePath + show.ImageFile);
            }

            var response = repository.EditShow(show);

            return new HttpResponseMessage()
            {
                Content = new StringContent(sb.ToString())
            };

        }



        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            if (!Authenticate.Auth(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            Show show = repository.GetShowById(id);
            if (show == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            repository.DeleteShow(id);

            //Delete image file etc
            string image=StoragePath + show.ImageGuid;
            CommonServices.DeleteImageFile(image);
                        
            Debug.WriteLine(id.ToString() + ", " + image + " has been deleted succesfully");
        }

    }

}
