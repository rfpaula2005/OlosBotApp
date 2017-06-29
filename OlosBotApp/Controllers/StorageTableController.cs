using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OlosBotApp.Utils;


namespace OlosBotApp.Controllers
{
    [RoutePrefix("api/OlosRegisterApp")]
    public class StorageTableController : ApiController
    {

        [AcceptVerbs("POST")]
        [Route("insere")]
        public HttpResponseMessage Post(AppEntity AppData)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppData.PartitionKey))
                {

                    Utils.AzureCloudStorageTable.insAppEntity("OlosBotCredentials", AppData.PartitionKey, AppData.RowKey, AppData.AppPassword, AppData.BotId, AppData.OlosEngineUri);

                    //Return Success Message
                    var http_return = new { Code = "SUC-001", Message = "OK" };
                    return Request.CreateResponse(HttpStatusCode.OK, http_return);
                }
                else
                {
                    //Return Error Message
                    var http_return = new { Code = "ERR-102", Message = "Your AppEntity is not well formated." };
                    return Request.CreateResponse(HttpStatusCode.BadRequest, http_return);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
