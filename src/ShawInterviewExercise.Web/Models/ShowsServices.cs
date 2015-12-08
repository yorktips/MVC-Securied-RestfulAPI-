using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using ShawInterviewExercise.API.Models;

namespace ShawInterviewExercise.Web.Models
{

    public class ShowsServices
    {
        public static List<T> CallGetApi<T>(string webApiEntry, string urlGet)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(webApiEntry);
            HttpResponseMessage response = new HttpResponseMessage();

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                response = client.GetAsync(urlGet).Result;
            }
            catch (AggregateException e)
            {
                Debug.WriteLine(e.Message);
                throw (new UnableConnectShowAPIException("Unable to connect to " + webApiEntry));
            }

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync();

                string jsonString = json.Result.ToString();

                Debug.WriteLine("GetAPI:" + urlGet + ";jsonString=" + jsonString);

                return (List<T>)(CommonServices.JsonDeserialize<T>(jsonString));
            }
            else
            {
                Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            return null;
        }

        public static HttpContent CreateMultipartContent(Show show, HttpPostedFileBase file, string fileName)
        {
            MultipartContent content = new MultipartContent("form-data", Guid.NewGuid().ToString());

            StringContent jsonPart = new StringContent(JsonConvert.SerializeObject(show));
            jsonPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            jsonPart.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            content.Add(jsonPart);

            if (file != null)
            {
                StreamContent filePart = new StreamContent(file.InputStream);
                filePart.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                filePart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                filePart.Headers.ContentDisposition.FileName = fileName;
                filePart.Headers.ContentDisposition.Name = "attachment";
                content.Add(filePart);
            }

            return content;
        }

        public static string DoFilterLink(IEnumerable<Show> shows, string c)
        {
            string sPattern = "^[a-zA-Z]+$";

            if (c.ToLower().Equals("#")) {
                foreach (Show show in shows) {
                    string name=show.Name;
                    if (!Regex.IsMatch(name, sPattern)) 
                    {
                        return "";
                    }
                }
            }else{
                foreach (Show show in shows) 
                    if (show.Name.ToLower().StartsWith(c.ToLower())) return "";
            }

            return " disabled ";
        }

        public static HttpRequestHeaders SetAccessTokenToHead(HttpRequestHeaders headers)
        {
            headers.Add("show-access-token", "YWRtaW4gLCAyMDE1LTEyLTAxIDExOjUyOjQ5Wg==");
            return headers;
        }

        public static string GetAccessTokenFromCookie(HttpRequestBase request)
        {            

            return "";
        }

        public static void SetAccessTokenToCookie(HttpResponseBase response, string token)
        {
            return;
        }


    }

}