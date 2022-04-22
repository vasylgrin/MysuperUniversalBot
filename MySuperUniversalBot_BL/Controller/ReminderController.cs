using MySuperUniversalBot_BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MySuperUniversalBot_BL.Controller
{
    public class ReminderController : List<Reminder>
    {
        BotController botController = new BotController();

        /// <summary>
        /// Напоминане.
        /// </summary>
        public Reminder CurrentReminder { get; set; }

        List<Reminder> Reminders { get; set; }


        /// <summary>
        /// Содание нового напоминания.
        /// </summary>
        /// <param name="name">Тема напоминания.</param>
        /// <param name="description">Описание напоминания.</param>
        /// <param name="dateTime">Дата напоминания.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ReminderController(long chatId, string? topic, DateTime dateTime)
        {
            #region Перевірка на null
            if (chatId <= 0)
            {
                botController.PrintMessage("Id чата не може бути пустим або бути рівне нулю...");
                return;
            }
            else if (string.IsNullOrWhiteSpace(topic))
            {
                botController.PrintMessage("Тема не може бути пустою...");
                return;
            }
            else if (dateTime < DateTime.Now)
            {
                botController.PrintMessage("Дата не може бути з минулого...");
                return;
            }
            #endregion

            CurrentReminder = new Reminder(chatId, topic, dateTime);

            SaveDataBase(CurrentReminder);
        }


        public ReminderController()
        {

        }


        /// <summary>
        /// Зберігання нагадування в базу данних.
        /// </summary>
        /// <param name="reminder">Нагадування.</param>
        void SaveDataBase(Reminder reminder)
        {
            using (DataBaseContext dataBaseContext = new())
            {
                if (dataBaseContext.Database.EnsureCreated())
                {
                    Console.WriteLine("База даных создана.");
                    if (dataBaseContext.Database.CanConnect())
                        Console.WriteLine("База данних доступна");
                    else
                        Console.WriteLine("База данних недоступна");
                }
                else
                {
                    Console.WriteLine("База даных уже существует.");
                    if (dataBaseContext.Database.CanConnect())
                        Console.WriteLine("База данних доступна");
                    else
                        Console.WriteLine("База данних недоступна");
                }

                dataBaseContext.Reminders.Add(reminder);
                dataBaseContext.SaveChanges();

                Reminders = dataBaseContext.Reminders.Where(x => x == reminder).ToList();

                if (CurrentReminder!=null)
                {
                    botController.PrintMessage($"Ваше нагадування збережено:\nТема: {reminder.Topic}\nДата та час: {reminder.DateTime}",reminder.ChatId);
                }
            }
        }


        /// <summary>
        /// Збереження нагадування.
        /// </summary>
        /// <param name="chatId">Чат Id</param>
        /// <param name="topic">Тема нагадування.</param>
        /// <param name="description">Опис нагадування.</param>
        /// <param name="dateTime">Дата нагадування.</param>
        public void SaveReminder(long chatId, string topic, DateTime date, CancellationToken cancellationToken)
        {
            if (chatId < 0)
            {
                botController.PrintMessage("Некоректний id чату...");
            }
            if (string.IsNullOrWhiteSpace(topic))
            {
                botController.PrintMessage("Тема не може бути пустою...");
            }
            if (date == DateTime.MinValue)
            {
                botController.PrintMessage($"Дата не може бути пустою...");
            }
            if (date < DateTime.Now)
            {
                botController.PrintMessage("Дата не може бути з минулого...");
            }

            ReminderController reminderController = new(chatId, topic, date);
        }


        /// <summary>
        /// Виведення дійсних нагадувань.
        /// </summary>
        /// <param name="chatId">Id чату.</param>
        public void OutputReminders(long chatId)
        {
            using (DataBaseContext db = new())
            {
                Reminders = db.Reminders.Where(x => x.ChatId == chatId).ToList();
                if (Reminders != null && Reminders.Count > 0)
                {
                    foreach (var reminder in Reminders)
                    {
                        botController.PrintMessage($"Ваше нагадування: \nТема:{reminder.Topic} Дата та час: {reminder.DateTime}");
                    }
                }
                else
                {
                    botController.PrintMessage($"У вас не має нагадувань.");
                }
            }
        }


        /// <summary>
        /// Метод для потоку
        /// </summary>
        public void GetReminderForThread()
        {
            using (DataBaseContext db = new())
            {
                while (true)
                {
                    var dateNow = DateTime.Now;
                    Reminders = db.Reminders.Where(x => x.DateTime <= DateTime.Now).ToList();
                    if (Reminders != null)
                    {
                        foreach (var reminder in Reminders)
                        {
                            botController.PrintMessage($"Ваше нагадування: \nТема:{reminder.Topic}", reminder.ChatId);
                            db.Reminders.Remove(reminder);
                            db.SaveChanges();
                        }
                    }
                    Thread.Sleep(1000);
                }           
            }
        }
    }
}
