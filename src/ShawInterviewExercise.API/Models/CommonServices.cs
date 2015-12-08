using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ShawInterviewExercise.API.Models
{
    public class UnableConnectShowAPIException : ApplicationException
    {
        public UnableConnectShowAPIException(string message)
            : base(message)
        {
        }
    }


    public static class CommonServices
    {
        public static Object JsonDeserialize<T>(string jsonString)
        {
            List<T> ret;

            jsonString = CleanJsonOutput(jsonString);

            try
            {
                var deserialized = JsonConvert.DeserializeObject<List<T>>(jsonString);
                ret = deserialized;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                var deserialized = JsonConvert.DeserializeObject<T>(jsonString);
                ret = new List<T>();
                ret.Add(deserialized);
            }

            return ret;
        }

        public static string BinaryToBase64(byte[] data)
        {
            string ret = "";
            try
            {
                //convert the bytes of data to a string using the Base64 encoding
                //This may increate 33% in buffer size
                ret = Convert.ToBase64String(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }


        public static byte[] Base64ToBinary(string base64)
        {
            byte[] ret = null;
            try
            {
                ret = Convert.FromBase64String(base64);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }


        public static string HttpPostedFileToBase64(HttpPostedFileBase dataFile)
        {
            string ret = "";
            try
            {
                //get the bytes from the content stream of the file
                byte[] data = new byte[dataFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(dataFile.InputStream))
                {
                    data = theReader.ReadBytes(dataFile.ContentLength);
                }

                ret = BinaryToBase64(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ret;
        }

        
        public static String CleanJsonOutput(String jsonStr)
        {
            string ret = jsonStr;
            string namespaceString = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">";
            string namespaceClosed = "</string>";

            if (ret.StartsWith(namespaceString))
            {
                ret = jsonStr.Replace(namespaceString, "");
                if (ret.EndsWith(namespaceClosed))
                {
                    ret = ret.Replace(namespaceClosed, "");
                }
            }

            ret = ret.Trim();

            if (ret.StartsWith("\"[") && ret.EndsWith("]\""))
            {
                ret = ret.Substring(1, ret.Length - 2);
                ret = ret.Replace("\"[", "[");
                ret = ret.Replace("]\"", "]");
            }

            ret = ret.Replace("\\\\", "\\");
            ret = ret.Replace("\\\"", "\"");

            if (ret.StartsWith("\"") && ret.EndsWith("\""))
            {
                ret = ret.Substring(1, ret.Length - 2);
            }
            return ret;
        }
        
        public static Boolean DeleteImageFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public static Boolean FileCopy(string src, string tar, Boolean deleteSrc = false)
        {
            try
            {
                if (!File.Exists(src))
                    return false;

                if (File.Exists(tar))
                    File.Delete(tar);

                File.Copy(src, tar);

                if (deleteSrc)
                    File.Delete(src);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public static string GetFileAttrName(string f)
        {
            int idx = f.LastIndexOf(".");
            if (idx < 0) return "";
            return f.Substring(idx, f.Length - idx);
        }

    }
}