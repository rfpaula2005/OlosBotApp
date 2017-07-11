using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Net;
using OlosBotApp.Utils;
using Olos.BotProtocol;
using System.Security.Claims;
using System.Web;
using System.Linq;


namespace OlosBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        //Initialize class properties with web.config values
        protected string PartitionKey = (ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] != null) ? ConfigurationManager.AppSettings["OlosBotStorageOlosBotCredentialsPartitionKey"] : "BotCredential";
        protected string GatewayUrl = (ConfigurationManager.AppSettings["GatewayUrl"] != null) ? ConfigurationManager.AppSettings["GatewayUrl"] : "";
        protected string OlosEngineUri = (ConfigurationManager.AppSettings["OlosEngineUri"] != null) ? ConfigurationManager.AppSettings["OlosEngineUri"] : "";
        protected bool UseLocalCredentials = ((ConfigurationManager.AppSettings["UseLocalCredentials"] != null) ? ConfigurationManager.AppSettings["UseLocalCredentials"] : "false") == "false" ? false : true;

        //Get the AppEntity related to the current AppId
        protected AppEntity ObjAppEntity;

        public Task StartAsync(IDialogContext context)
        {
            Utils.Log.Info("================== RootDialog::StartAsync ================== ");
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            DateTime dt_inicio = DateTime.Now;
            Utils.Log.Info("================== RootDialog::MessageReceivedAsync ================== ");

            //Patern vars
            var activity = await result as Activity;
            //Strore de Message data;
            string messageJson;

            try
            {
                Utils.Log.Info("[RootDialog::MessageReceivedAsync] MessageReceivedAsync Initiated");

                var lc_appId = ((ClaimsIdentity)HttpContext.Current.User.Identity).GetCredentialsFromClaims().MicrosoftAppId;

                //Check if ObjAppEntity have already created
                if (ObjAppEntity == null)
                {
                    Utils.Log.Info("[RootDialog::MessageReceivedAsync] Recovering ObjAppEntity");
                    //Check appId on AzureTable OlosBotCredentials and return the data
                    ObjAppEntity = Utils.AzureCloudStorageTable.getAppEntityData("OlosBotCredentials", PartitionKey, lc_appId);
                }


                Utils.Log.Info("[RootDialog::MessageReceivedAsync] Converting activity");
                Message ObjMessage = Message.ConvertToMessage(activity);
                ObjMessage.AppId = lc_appId;
                ObjMessage.GatewayUrl = GatewayUrl;
                messageJson = ObjMessage.GetJsonN();

                Utils.Log.Info("[RootDialog::MessageReceivedAsync] Activity converted");
                Utils.Log.Warn("[RootDialog::MessageReceivedAsync] Olos Message \n\n", ObjMessage);
                try
                {
                    //Set OlosEngineUri follow de web.config definitions (use local or remote credentials)
                    string v_OlosEngineUri = (UseLocalCredentials) ? OlosEngineUri : ObjAppEntity.OlosEngineUri;

                    //Utils.Log.Info("[RootDialog::MessageReceivedAsync] Conecting to Olos Bot Receiver " + ObjAppEntity.OlosEngineUri);
                    Utils.Log.Info("[RootDialog::MessageReceivedAsync] Conecting to Olos Bot Receiver " + v_OlosEngineUri);

                    //This code was moved from MessagesController to here and commented - it was OK!
                    /*
                    if (activity.Type == ActivityTypes.ConversationUpdate)
                    {
                        Utils.Log.Info("[MessagesController::Post] Activity ConversationUpdate Received");
                        IConversationUpdateActivity update = activity;
                        var client = new ConnectorClient(new Uri(update.ServiceUrl), lc_appId);

                        if (update.MembersAdded.Any())
                        {
                            var reply = activity.CreateReply();
                            foreach (var newMember in update.MembersAdded)
                            {
                                if (newMember.Id != activity.Recipient.Id)
                                {
                                    reply.Text = $"Bem-vindo {newMember.Name}!";
                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                        }
                    }
                    */

                    string content;
                    DateTime dt_inicioPostJson = DateTime.Now;
                    WebResponse responseFromServer = OlosFunctions.PostJson(v_OlosEngineUri, messageJson);
                    DateTime dt_fimPostJson = DateTime.Now;
                    Utils.Log.Info("[RootDialog::MessageReceivedAsync] PostJson Duration:" + Math.Round(Convert.ToDecimal((dt_fimPostJson - dt_inicioPostJson).TotalMilliseconds)).ToString() + " ms");

                    Utils.Log.Info("[RootDialog::MessageReceivedAsync] Olos Bot Receiver response", ((System.Net.HttpWebResponse)responseFromServer).StatusCode);
                    switch (((System.Net.HttpWebResponse)responseFromServer).StatusCode)
                    {
                        case HttpStatusCode.Accepted:
                            Utils.Log.Info("[RootDialog::MessageReceivedAsync] Recovering content");
                            content = OlosFunctions.GetResponseStrContent(responseFromServer);
                            //Log
                            await context.PostAsync(HttpStatusCode.Accepted + "->" + content);
                            //context.Wait(MessageReceivedAsync);
                            break;
                        case HttpStatusCode.OK:
                            Utils.Log.Info("[RootDialog::MessageReceivedAsync] Recovering content");
                            content = OlosFunctions.GetResponseStrContent(responseFromServer);
                            //Return to user
                            Utils.Log.Info("[RootDialog::MessageReceivedAsync] Sending content do user");
                            await context.PostAsync(HttpStatusCode.OK + "->" + content);
                            //context.Wait(MessageReceivedAsync);
                            break;
                        default:
                            //LogErros
                            Utils.Log.Error("[RootDialog::MessageReceivedAsync] Http Error", ((System.Net.HttpWebResponse)responseFromServer).StatusCode);
                            break;
                    }

                    DateTime dt_fim = DateTime.Now;
                    Utils.Log.Info("[RootDialog::MessageReceivedAsync] Duration:" + Math.Round(Convert.ToDecimal((dt_fim - dt_inicio).TotalMilliseconds)).ToString() + " ms");

                }
                catch (WebException wex)
                {
                    Utils.Log.Error("[RootDialog::MessageReceivedAsync] Exception \n\n", wex);
                    await context.PostAsync($"Por favor me desculpe, no momento estamos passando por algumas dificuldades técnicas. Retorne mais tarde e teremos o maior prazer em ajudá-lo com a sua solicitação.\n\n Execption:\n\n\n\n" + ObjAppEntity.OlosEngineUri + "\n\n\n\n" + wex.Message);
                    //context.Wait(MessageReceivedAsync);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error("[RootDialog::MessageReceivedAsync] Exception \n\n", ex);
                await context.PostAsync($"Por favor me desculpe, no momento estamos passando por algumas dificuldades técnicas. Retorne mais tarde e teremos o maior prazer em ajudá-lo com a sua solicitação.\n\n Execption:\n\n\n\n" + ObjAppEntity.OlosEngineUri + "\n\n\n\n" + ex.Message);
                //context.Wait(MessageReceivedAsync);
            }
            finally
            {
                context.Wait(MessageReceivedAsync);
            }

            /*
            Utils.Log.Info("[RootDialog::MessageReceivedAsync] Recovering content");
            //Return to user
            Utils.Log.Info("[RootDialog::MessageReceivedAsync] Sending content do user");
            DateTime dt_fim = DateTime.Now;
            await context.PostAsync("[Tempo:" + (dt_fim - dt_inicio).TotalMilliseconds.ToString() + "]" + HttpStatusCode.OK + "->" + DateTime.Now + " userId: " + activity.From.Id);
            context.Wait(MessageReceivedAsync);
            */


        }
    }
}