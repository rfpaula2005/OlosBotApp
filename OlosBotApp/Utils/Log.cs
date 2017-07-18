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
        public enum DetailLevel
        {
            None,
            Information,
            Warnings,
            Errors,
            All
        };

        public static DetailLevel StringToDetailLevel(string Level)
        {
            DetailLevel result;
            switch (Level)
            {
                case "None":
                    result =  DetailLevel.None;
                    break;
                case "Information":
                    result = DetailLevel.Information;
                    break;
                case "Warnings":
                    result = DetailLevel.Warnings;
                    break;
                case "Errors":
                    result = DetailLevel.Errors;
                    break;
                case "All":
                    result = DetailLevel.All;
                    break;
                default:
                    result = DetailLevel.None;
                    break;
            }
            return result;
        }

        public static DetailLevel level = DetailLevel.None;

        static Log()
        {
            //Initialize logLevel
            Utils.Log.level = Utils.Log.StringToDetailLevel(((System.Configuration.ConfigurationManager.AppSettings["LogLevel"] != null) ? System.Configuration.ConfigurationManager.AppSettings["LogLevel"] : "All"));
        }


        private static LogMessage GetMessage(string message, object debugData)
        {
            return new LogMessage() { message = message, json = debugData };
        }

        public static void Info(string message, object debugData = null)
        {
            //Trace.TraceInformation(GetMessage(message, debugData).ToString());
            //Debug.WriteLineIf((level >= DetailLevel.Information), GetMessage(message, debugData).ToString());
            if (level >= DetailLevel.Information)
            {
                Trace.TraceInformation(GetMessage(message, debugData).ToString());
            }
        }

        public static void Warn(string message, object debugData = null)
        {
            //Trace.TraceWarning(GetMessage(message, debugData).ToString());
            //Debug.WriteLineIf((level >= DetailLevel.Warnings), GetMessage(message, debugData).ToString());
            if (level >= DetailLevel.Warnings)
            {
                Trace.TraceWarning(GetMessage(message, debugData).ToString());
            }
        }

        public static void Error(string message, object debugData = null)
        {
            //Trace.TraceError(GetMessage(message, debugData).ToString());
            //Debug.WriteLineIf((level >= DetailLevel.Errors), GetMessage(message, debugData).ToString());
            if (level >= DetailLevel.Errors)
            {
                Trace.TraceError(GetMessage(message, debugData).ToString());
            }
        }
    }
}