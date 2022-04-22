using Microsoft.EntityFrameworkCore;
using MySuperUniversalBot_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MySuperUniversalBot_BL.Controller
{
    public class BotController
    {
        ReminderController reminderController;


        string[] commands = new[] 
        {   
            "/start",           // 0
            "Нагадування",      // 1
            "Додати",           // 2
            "Тема",             // 3 
            "Дата та Час",      // 4
            "Зберегти",         // 5 
            "Повернутись",      // 6
            "Переглянути",      // 7
            "Назад"             // 8
        };


        TelegramBotClient botClient = new("5268015233:AAFtYMakBaqz-SvLgrmN14IByvkLTP2-404");
        List<Reminder> Reminders { get; set; }
        
        string? topic, inputType;
        int i = 0;
        DateTime date;

        long chatId;
        CancellationToken cancellation;

        ///<summary>
        /// Перевірка команди.
        /// </summary>
        /// <param name="messageText">Текст повідомлення.</param>
        /// <param name="chatID">Чат Id</param>
        /// <param name="cancellationToken"></param>
        public void CheckAnswer(string messageText, long chatID, CancellationToken cancellationToken)
        {
            chatId = chatID;
            cancellation = cancellationToken;


            #region validations answer

            if (messageText == commands[0])
            {
                PrintMessage("Привіт");
                PrintKeyboard("Обирай: ", chatId, SetupKeyboard("Нагадування"), cancellationToken);
            }                                          // /start
            else if (messageText == commands[1] || messageText == commands[8])
            {
                PrintKeyboard("Обирай:", chatId, SetupKeyboard(commands[2], commands[7]), cancellationToken);
            }       // Нагадування та Назад
            else if (messageText == commands[2]) 
            {
                PrintKeyboard("Обирай:", chatId, SetupKeyboard(commands[3], commands[4], commands[5],commands[8]), cancellationToken);
            }                                     // Додати
            else if (messageText == commands[3]) 
            {
                PrintKeyboard("Пиши...", chatId, SetupKeyboard(commands[6]), cancellationToken);
                inputType = commands[3];
            }                                     // Тема
            else if (messageText == commands[4]) // Дата та час
            {
                PrintKeyboard("Формат дати дд.ММ.рррр 00:00:00", chatId, SetupKeyboard(commands[6]), cancellationToken);
                inputType = commands[4];
            }                                     // Дата та час
            else if (messageText == commands[5])
            {
                SaveReminder(chatID, topic, date, cancellationToken);
            }                                     // Зберегти
            else if (messageText == commands[6])
            {
                PrintKeyboard("Обирай:", chatId, SetupKeyboard(commands[3], commands[4], commands[5], commands[8]), cancellationToken);
            }                                     // Повернутись
            else if (messageText == commands[7])         
            {
                if (reminderController != null)
                {
                    reminderController.OutputReminders(chatID);
                }
                else
                {
                    PrintMessage("У тебе немає нагадувань.");
                }
            }                                     // Переглянути
            else
            {
                inputType += messageText;
            }

            #endregion


            if (string.IsNullOrWhiteSpace(inputType))
                return;
            else if (inputType.Contains(commands[3]))
            {
                inputType = inputType.Replace(commands[3], string.Empty);
                if (!string.IsNullOrWhiteSpace(inputType))
                {
                    topic = inputType;
                    PrintMessage($"тема {topic}");
                    PrintKeyboard("Обирай:", chatId, SetupKeyboard(commands[3], commands[4], commands[5], commands[8]), cancellationToken);
                }
                else
                {
                    inputType = commands[3];
                }
            }       // тема
            else if (inputType.Contains(commands[4]))
            {
                inputType = inputType.Replace(commands[4], string.Empty);
                if (!string.IsNullOrWhiteSpace(inputType))
                {
                    if (DateTime.TryParse(inputType,out date))
                    {
                        if(date < DateTime.Now && date != DateTime.MinValue)
                        {
                            PrintMessage($"Дата не може бути з минулого...");
                            inputType = commands[4];
                        }
                        else if(date == DateTime.MinValue)
                        {
                            PrintMessage($"Дата не може бути пустою...");
                        }
                        else
                        {
                            PrintMessage($"Дата {date}");
                            PrintKeyboard("Обирай:", chatId, SetupKeyboard(commands[3], commands[4], commands[5], commands[8]), cancellationToken);
                        }
                    }
                    else
                    {
                        PrintMessage("Невірний формат дати...");
                        inputType = commands[4];
                    }
                }
                else
                {
                    inputType = commands[4];
                }
            }       // дата
        }


        #region Перегрузка метода PrintMessage

        /// <summary>
        /// Вивід повідомлення.
        /// </summary>
        /// <param name="messageText">Повідомлення.</param>
        /// <param name="chatId">Id чату.</param>
        /// <param name="cancellationToken"></param>
        public async void PrintMessage(string messageText)
        {
            if (chatId == 0)
                return;
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cancellation);
        }


        /// <summary>
        /// Вивід повідомлення.
        /// </summary>
        /// <param name="messageText">Повідомлення.</param>
        /// <param name="chatId">Id чату.</param>
        /// <param name="cancellationToken"></param>
        public async void PrintMessage(string messageText,long chatId)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cancellation);
        }


        /// <summary>
        /// Вивід повідомлення.
        /// </summary>
        /// <param name="messageText">Повідомлення.</param>
        /// <param name="chatId">Id чату.</param>
        /// <param name="cancellationToken"></param>
        public async void PrintMessage(string messageText, long chatId, CancellationToken token)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: token);
        }

        #endregion


        #region Перегрузка метода SetupKeyboard

        public ReplyKeyboardMarkup SetupKeyboard(string button1)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[]{ button1 }
                    })
            { ResizeKeyboard = true };
            return replyKeyboardMarkup;
        }
        public ReplyKeyboardMarkup SetupKeyboard(string buttonName1, string buttonName2)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[]{ buttonName1, buttonName2}
                    })
            { ResizeKeyboard = true };
            return replyKeyboardMarkup;
        }
        
        public ReplyKeyboardMarkup SetupKeyboard(string buttonName1, string buttonName2, string buttonName3)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[]{buttonName1, buttonName2, buttonName3}
                    })
            { ResizeKeyboard = true };
            return replyKeyboardMarkup;
        }
        public ReplyKeyboardMarkup SetupKeyboard(string buttonName1, string buttonName2, string buttonName3, string buttonName4)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[]{ buttonName1, buttonName2, buttonName3, buttonName4 }
                    })
            { ResizeKeyboard = true };
            return replyKeyboardMarkup;
        }
        #endregion


        /// <summary>
        /// Вивід кнопок.
        /// </summary>
        /// <param name="messageText">Текст повідомлення.</param>
        /// <param name="chatId">Чат Id</param>
        /// <param name="replyKeyboardMarkup">Вид кнопок.</param>
        /// <param name="cancellationToken"></param>
        public async void PrintKeyboard(string messageText, long? chatId, ReplyKeyboardMarkup replyKeyboardMarkup, CancellationToken cancellationToken)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Вивід inline кнопок.
        /// </summary>
        /// <param name="messageText">Повідомлення.</param>
        /// <param name="chatId">Id чату.</param>
        /// <param name="inlineKeyboardMarkup">Вид inline кнопок.</param>
        /// <param name="cancellationToken"></param>
        public async void PrintInline(string messageText, long? chatId, InlineKeyboardMarkup inlineKeyboardMarkup, CancellationToken cancellationToken)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                replyMarkup: inlineKeyboardMarkup,
                cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Выдалення клавіатури.
        /// </summary>
        /// <param name="messageText">Текст повідомлення.</param>
        /// <param name="chatId">Чат Id.</param>
        /// <param name="cancellationToken"></param>
        public async void RemoveKeyboard(string messageText, long? chatId, CancellationToken cancellationToken)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Збереження нагадування.
        /// </summary>
        /// <param name="chatId">Чат Id</param>
        /// <param name="topic">Тема нагадування.</param>
        /// <param name="description">Опис нагадування.</param>
        /// <param name="dateTime">Дата нагадування.</param>
        public void SaveReminder(long chatId, string topic, DateTime date,CancellationToken cancellationToken)
        {
            if (chatId<0)
                PrintMessage("Некоректний id чату...");
            if (string.IsNullOrWhiteSpace(topic))
                PrintMessage("Тема не може бути пустою...");
            if (date<DateTime.Parse("20.04.2022"))
                PrintMessage("Дата не може бути з минулого...");
            ReminderController reminderController = new(chatId, topic, date);
        } 
    }
}
