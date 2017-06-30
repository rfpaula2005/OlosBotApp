using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Net;
using System.IO;

namespace OlosBotApp.Utils
{
    public class OlosFunctions
    {
        //Get the Http credentials from basic authentication
        public static KeyValuePair<string, string>[] getHttpCredentials(HttpContext httpContext)
        {
            string username;
            string password;
            string authHeader = httpContext.Request.Headers["Authorization"];
            KeyValuePair<string, string>[] retorno;

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                //the coding should be iso or you could use ASCII and UTF-8 decoder
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                username = usernamePassword.Substring(0, seperatorIndex);
                password = usernamePassword.Substring(seperatorIndex + 1);

                retorno = new[] {new KeyValuePair<string,string>("username",username),new KeyValuePair<string,string>("password",password)};
            }
            else
            {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            return retorno;
        }

        // Get the contents of a uri
        public static async Task<string> AccessTheWebAsync(string uri)
        {
            HttpClient client = new HttpClient();

            Task<string> getStringTask = client.GetStringAsync(uri);
            string urlContents = await getStringTask;

            return urlContents;
        }

        public static string PostJson(string url, string json)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri(url));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string parsedContent = json;
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            string  content = sr.ReadToEnd();

            return content;

            /*
            string result = "";
            Uri str_url = new Uri(url);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.UploadStringAsync(str_url, "POST", json);
                //result = client.UploadStringAsync(str_url, "POST", json);
            }
            
            return result;
            */
        }

        // Verify Basic Authentication
        public static bool verifyCredentials(KeyValuePair<string, string>[] v_Cretentials)
        {
            if (v_Cretentials.Length > 0 && v_Cretentials[0].Key == "username" && v_Cretentials[0].Value == ConfigurationManager.AppSettings["OlosRegisterAppUserName"] && v_Cretentials[1].Value == ConfigurationManager.AppSettings["OlosRegisterAppUserPasswd"])
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}