using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;

namespace OlosBotApp.Utils
{
    public class LogMessage
    {
        public object json { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            if (json != null)
            {
                return message + " " + JsonConvert.SerializeObject(json, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
            }
            else
            {
                return message;
            }
        }
    }

    public static class Log
    {
        private static LogMessage GetMessage(string message, object debugData)
        {
            return new LogMessage() { message = message, json = debugData };
        }

        public static void Info(string message, object debugData = null)
        {
            Trace.TraceInformation(GetMessage(message, debugData).ToString());
        }

        public static void Warn(string message, object debugData = null)
        {
            Trace.TraceWarning(GetMessage(message, debugData).ToString());
        }

        public static void Error(string message, object debugData = null)
        {
            Trace.TraceError(GetMessage(message, debugData).ToString());
        }
    }
}