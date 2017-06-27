using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OlosBotApp.Models
{
    [Serializable]
    public class OlosActivityModel
    {

        public static string channelId { get; set; }
        public static string conversationId { get; set; }
        public static string userId { get; set; }
        public static string userName { get; set; }
        public static string botId { get; set; }
        public static string botName { get; set; }
        public static string serviceUrl { get; set; }


        public OlosActivityModel()
        {

        }

        public OlosActivityModel(string v_conversationId, string v_userId, string v_channelId, string v_serviceUrl)
        {
            OlosActivityModel.channelId = v_channelId;
            OlosActivityModel.conversationId = v_conversationId;
            OlosActivityModel.userId = v_userId;
            OlosActivityModel.serviceUrl = v_serviceUrl;
        }

    }
}