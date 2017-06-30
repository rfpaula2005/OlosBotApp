using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace OlosBotApp.Models
{
    [Serializable]
    public class OlosActivityModel
    {

        public string channelId { get; set; }
        public string conversationId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string botId { get; set; }
        public string botName { get; set; }
        public string serviceUrl { get; set; }
        public string text { get; set; }


        public OlosActivityModel()
        {

        }

        public OlosActivityModel(string v_userId, string v_userName, string v_botId, string v_botName, string v_channelId, string v_conversationId, string v_text,   string v_serviceUrl)
        {
            userId = v_userId;
            userName = v_userName;
            botId = v_botId;
            botName = v_botName;
            channelId = v_channelId;
            conversationId = v_conversationId;
            text = v_text;
            serviceUrl = v_serviceUrl;
        }

    }
}