using Microsoft.Bot.Connector;

namespace Olos.BotProtocol
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }


        public Account()
        {
            Id = "";
            Name = "";
        }

        public Account (string id, string name)
        {
            Id = id;
            Name = name;
        }

        public ChannelAccount ConvertToChannelAccount()
        {
            ChannelAccount ObjChannelAccount = new ChannelAccount(this.Id, this.Name);
            return ObjChannelAccount;
        }

        public static Account ConvertToAccount(ChannelAccount CA)
        {
            Account ObjAccount = new Account(CA.Id, CA.Name);
            return ObjAccount;
        }
    }
}
