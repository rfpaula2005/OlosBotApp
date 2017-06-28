using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;

namespace OlosBotApp
{
    public class Functions
    {
        public static async Task<string> AccessTheWebAsync(string uri)
        {
            HttpClient client = new HttpClient();

            Task<string> getStringTask = client.GetStringAsync(uri);
            string urlContents = await getStringTask;

            return urlContents;
        }
    }
}