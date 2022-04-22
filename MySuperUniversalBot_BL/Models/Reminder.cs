using MySuperUniversalBot_BL.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySuperUniversalBot_BL.Models
{
    public class Reminder 
    {
        BotController botController = new();
       
        
        /// <summary>
        /// Идентификатор напоминания.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Id чата.
        /// </summary>
        public long ChatId { get; set; }


        /// <summary>
        /// Имя темы.
        /// </summary>
        public string? Topic { get; set; }


        /// <summary>
        /// Дата напоминания.
        /// </summary>
        public DateTime DateTime { get; set; }


        /// <summary>
        /// Создание напоминания.
        /// </summary>
        /// <param name="name">Имя темы.</param>
        /// <param name="description">Описание темы.</param>
        /// <param name="dateTime">Дата напоминания.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Reminder(long сhatId, string topic, DateTime dateTime)
        {
            if (сhatId <= 0)
                botController.PrintMessage("Id чата не может быть пустым или быть равен нулю.");
            if (string.IsNullOrWhiteSpace(topic))
                botController.PrintMessage("Тема не может быть пустая.");
            if (dateTime < DateTime.Now)
                botController.PrintMessage("Дата не може бути з минулого.");

            ChatId = сhatId;
            Topic = topic;
            DateTime = dateTime;
        } 

        public Reminder()
        {

        }
    }
}
