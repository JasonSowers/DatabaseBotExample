using System;
using System.Linq;
using System.Threading.Tasks;
using BotDatabaseExample.Entities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotDatabaseExample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            string reply;
            var activity = await result as Activity;
            using (TableEntities db = new TableEntities())
            {
                

                switch (activity.Text.ToLower())
                {
                    case "update":
                        var updateItem = (from each in db.Tables
                            where each.Id == 1
                            select each).FirstOrDefault();
                        updateItem.Quantity += 1;
                        db.SaveChanges();
                        reply = $" you selected {updateItem.Item} quantity is {updateItem.Quantity}";
                        break;
                    case "select":
                        var selectItem = (from each in db.Tables
                            where each.Id == 1
                            select each).FirstOrDefault();
                        reply = $"{selectItem.Item} quantity is now {selectItem.Quantity}";
                        break;
                    default:
                        reply = "Please enter \"select\" or \"update\"";
                        break;
                }
            }
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }
    }
}