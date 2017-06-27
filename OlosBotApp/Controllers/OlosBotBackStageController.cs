using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;


namespace OlosBotApp.Controllers
{
    public class OlosBotBackStageController : ApiController
    {
        [Route("api/OlosBotBackStage")]

        //[HttpGet,HttpPost]

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] {"Esta api disponibiliza somente o metodo POST!"};
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                //if (!string.IsNullOrEmpty(ConversationStarter.fromId))
                //{
                //await ConversationStarter.Resume(ConversationStarter.conversationId, ConversationStarter.channelId); //We don't need to wait for this, just want to start the interruption here

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent($"<html><body>Mensagem recebida</body></html>", System.Text.Encoding.UTF8, @"text/html");
                return resp;
                //}
                //else
                //{
                //    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                //    resp.Content = new StringContent($"<html><body>You need to talk to the bot first so it can capture your details.</body></html>", System.Text.Encoding.UTF8, @"text/html");
                //    return resp;
                //}
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string value)
        {

            try
            {
                //if (!string.IsNullOrEmpty(ConversationStarter.fromId))
                //{
                //await ConversationStarter.Resume(ConversationStarter.conversationId, ConversationStarter.channelId); //We don't need to wait for this, just want to start the interruption here

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent($"<html><body>Mensagem recebida</body></html>", System.Text.Encoding.UTF8, @"text/html");
                return resp;
                //}
                //else
                //{
                //    var resp = new HttpResponseMessage(HttpStatusCode.OK);
                //    resp.Content = new StringContent($"<html><body>You need to talk to the bot first so it can capture your details.</body></html>", System.Text.Encoding.UTF8, @"text/html");
                //    return resp;
                //}
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }


        }


    }
}