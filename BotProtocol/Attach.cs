using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Olos.BotProtocol
{
    public class Attach
    {
        public string ContentUrl { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }

        public Attach()
        {
            ContentUrl = "";
            ContentType = "";
            Name = "";
        }

        public Attach(string contentType, string contentUrl,  string name)
        {
            ContentUrl = contentUrl;
            ContentType = contentType;
            Name = name;
        }

        public static List<Attachment> ConvertToAttachment(List<Attach> Attachies)
        {
            List<Attachment> Lista = new List<Attachment>();

            foreach (Attach item in Attachies)
            {
                Attachment Anexo = new Attachment(item.ContentType, item.ContentUrl, null, item.Name, null);
                Lista.Add(Anexo);
            }
            return Lista;
        }

        public static List<Attach> ConvertToAttach (List<Attachment> ListaAttachment)
        {
            List<Attach> Lista = new List<Attach>();

            foreach (Attachment item in ListaAttachment)
            {
                Attach Anexo = new Attach(item.ContentType, item.ContentUrl, item.Name);
                Lista.Add(Anexo);
            }
            return Lista;
        }
    }
}
