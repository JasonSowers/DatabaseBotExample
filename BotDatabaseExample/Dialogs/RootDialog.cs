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
                    case "insert":
                        var insertItem = new Table()
                        {
                            Item = "new item",
                            Quantity = 10
                        };
                        db.Tables.Add(insertItem);
                        db.SaveChanges();
                        reply = $"{insertItem.Item} has been added the quantity is {insertItem.Quantity}";
                        break;
                    case "delete":
                        var deleteItem = (from each in db.Tables
                            where each.Id == 1
                            select each).FirstOrDefault();
                        db.Tables.Remove(deleteItem);
                        db.SaveChanges();
                        reply = $"{deleteItem.Item} has been deleted";
                        break;
                    default:
                        reply = "Please enter \"select\", \"update\", \"insert\", or \"delete\"";
                        break;
                }
            }
            // return our reply to the user
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }
    }
}