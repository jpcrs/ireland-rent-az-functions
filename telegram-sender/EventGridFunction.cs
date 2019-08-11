using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace telegram_sender
{
    public class EventGridFunction
    {
        [FunctionName("EventGridTelegramSender")]
        public async Task EventGridTest([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramKey"));

            var message = JsonConvert.DeserializeObject<MessageModel>(JsonConvert.SerializeObject(eventGridEvent.Data));


            var inlineButtons = new[] { InlineKeyboardButton.WithUrl("Link", message.Link) };
            var distanceMessage = message.WorkDistance != "" ? $"<b>-TravelPort Digital:</b> {message.WorkDistance}" : $"????";
            var msg = "";
            Message reply;
            if (message.Map != null)
            {
                msg = $@"<a href='{message.Map}'>Map</a>
<b>-Location:</b> {message.Location}
{distanceMessage}
<b>-Price:</b> {message.Price}";

                reply = await botClient.SendTextMessageAsync(Environment.GetEnvironmentVariable("ChatId"), msg, ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(inlineButtons));
            }
            else
            {
                msg = $@"<b>Mapa não localizado</b>
<b>-Location:</b> {message.Location}
{distanceMessage}
<b>-Price:</b> {message.Price}";

                reply = await  botClient.SendTextMessageAsync(Environment.GetEnvironmentVariable("ChatId"), msg, ParseMode.Html, true, replyMarkup: new InlineKeyboardMarkup(inlineButtons));
            }

            if (message.Photos.Count > 0)
            {
                IEnumerable<IAlbumInputMedia> inputMediaPhotos = message.Photos.Select(x => new InputMediaPhoto(new InputMedia(x))).Take(6);
                await botClient.SendMediaGroupAsync(inputMediaPhotos, Environment.GetEnvironmentVariable("ChatId"), replyToMessageId: reply.MessageId);
            }
        }
    }
}
