using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Olos.BotProtocol
{
    public class SuggestionAction
    {
        public string Image { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        public SuggestionAction()
        {
            Image = "";
            Title = "";
            Value = "";

        }

        public SuggestionAction(string image, string title, string value)
        {
            Image = image;
            Title = title;
            Value = value;

        }

        public static SuggestedActions ConvertToSuggestedActions(List<SuggestionAction> Lista)
        {
            SuggestedActions SA = new SuggestedActions();

            List<CardAction> Actions = new List<CardAction>();

            foreach (SuggestionAction item in Lista)
            {
                CardAction CA = new CardAction(ActionTypes.ImBack, item.Title, item.Image, null, item.Value, null);
                Actions.Add(CA);
            }

            SA.Actions = Actions;

            return SA;
        }


        public static List<SuggestionAction> ConvertToSuggestionAction(SuggestedActions S)
        {
            List<SuggestionAction> Lista = new List<SuggestionAction>();

            foreach (CardAction item in S.Actions)
            {
                SuggestionAction SA = new SuggestionAction();
                Lista.Add(SA);
            }

            return Lista;
        }


    }
}