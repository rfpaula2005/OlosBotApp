﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OlosBotApp.Utils;
using System.Web;
using System.Collections.Generic;


namespace OlosBotApp.Controllers
{
    [RoutePrefix("api/OlosRegisterApp")]
    public class StorageTableController : ApiController
    {
        private HttpContext httpContext;

        [AcceptVerbs("POST")]
        [Route("insere")]
        public HttpResponseMessage Post(AppEntity AppData)
        {
            httpContext = HttpContext.Current;

            try
            {
                KeyValuePair<string, string>[] v_Cretentials = OlosFunctions.getHttpCredentials(httpContext);

                //Verify Credentials
                //if (v_Cretentials.Length > 0 && v_Cretentials[0].Key == "username" && v_Cretentials[0].Value == ConfigurationManager.AppSettings["OlosRegisterAppUserName"] && v_Cretentials[1].Value == ConfigurationManager.AppSettings["OlosRegisterAppUserPasswd"])
                if (OlosFunctions.verifyCredentials(v_Cretentials))
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
                } else
                {
                    //Handle what happens if that isn't the case
                    throw new Exception("The authorization username or password are not correct.");
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
