using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ShawInterviewExercise.API.Models
{
    public class Authenticate
    {
        public static string GetAuthToken(string userid)
        {
            userid = userid ?? "";
            if (userid.Equals("")) return null;

            string now = String.Format("{0:u}", DateTime.Now);
            string plain_token = userid + " , " + now;
            byte[] toBytes = Encoding.ASCII.GetBytes(plain_token);

            return Convert.ToBase64String(toBytes);
        }

        //YWRtaW4gLCAyMDE1LTEyLTAxIDExOjUyOjQ5Wg==
        public static Boolean Auth(string token)
        {
            string plain_token = Encoding.ASCII.GetString(Convert.FromBase64String(token));
            //Debug.WriteLine("plain_token=" + plain_token + ";token=" + token);
            string[] matches = Regex.Split(plain_token, ",");

            if (matches.Length == 2)
            {
                string user = matches[0];
                string timeStamp = matches[1];
                if (user.TrimEnd().Equals("admin"))
                    return true;
            }
            return false;
        }

        public static Boolean Auth(HttpRequestMessage request)
        {
            IEnumerable<Object> tokens = request.Headers.GetValues("show-access-token");
            if (tokens == null)
                return false;

            var securityToken = tokens.First();
            if (tokens == null)
                return false;

            return Authenticate.Auth(securityToken.ToString());
        }

    }
}