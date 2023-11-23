using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Timers;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;

namespace TypingTest
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public int CharactersPerMinute { get; set; }
        public int CharactersPerSecond { get; set; }
    }

    public static class Record
    {
        private static List<User> users = new List<User>();

        private static string recordFile = "records.json";

        public static void AddRecord(User user)
        {
            users.Add(user);
        }

        public static void ShowTable()
        {
            Console.WriteLine("Таблица рекордов:");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Name} - {user.CharactersPerMinute} символов в минуту, {user.CharactersPerSecond} символов в секунду");
            }
        }

        public static void SaveJson()
        {
            using (FileStream fs = new FileStream(recordFile, FileMode.Create))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<User>));
                serializer.WriteObject(fs, users);
            }
        }

        public static void LoadJson()
        {
            if (File.Exists(recordFile))
            {
                using (FileStream fs = new FileStream(recordFile, FileMode.Open))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<User>));
                    users = (List<User>)serializer.ReadObject(fs);
                }
            }
        }
    }

    public class TextTyping
    {
        private string text;

        public TextTyping(string text)
        {
            this.text = text;
        }

        public void StartTyping(string userName)
        {
            Console.Clear();
            Console.WriteLine("Вот текст для набора:");
            Console.WriteLine(text);
            Console.WriteLine("Нажмите Enter, чтобы начать печатать.");

            Console.ReadLine();


            Thread timerThread = new Thread(new ThreadStart(() => TimerThread(userName)));
            timerThread.Start();

            DateTime startTime = DateTime.Now;

            string inputText = Console.ReadLine();

            DateTime endTime = DateTime.Now;
            int characters = inputText.Length;
            TimeSpan timeTaken = endTime - startTime;
            double minutes = timeTaken.TotalMinutes;
            double seconds = timeTaken.TotalSeconds;
            if (minutes > 0)
            {
                double charactersPerMinute = characters / minutes;
            }
            if (seconds > 0)
            {
                double charactersPerSecond = characters / seconds;
            }
            Record.AddRecord(new User { Name = userName, CharactersPerMinute = (int)minutes, CharactersPerSecond = (int)seconds });
            Record.ShowTable();
            Record.SaveJson();

        }
        static void TimerThread(string userName)
        {
            int timeInSeconds = 60;
            int topPosition = 9;

            while (timeInSeconds > 0)
            {
                Console.SetCursorPosition(0, topPosition);
                Console.WriteLine("Оставшееся время: " + timeInSeconds + " секунд");

                Thread.Sleep(1000);
                timeInSeconds--;
            }

            if (timeInSeconds == 0)
            {
                Console.SetCursorPosition(0, topPosition);
                Console.WriteLine("Время вышло. Ввод больше не доступен");
                Console.SetCursorPosition(0, topPosition + 1);
                TextTyping typing = new TextTyping("");
                typing.StartTyping(userName); // Automatically prompt next typing session
            }
        }
        
    }
    class Program
    {
        public static string UserName;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.Write("Введите ваше имя: ");

                UserName = Console.ReadLine();

                TextTyping textTyping = new TextTyping("Снег белый, потому что он отражает все видимые цвета света. Когда свет попадает на снежные кристаллы, он проходит через них и отражается от их внутренних поверхностей, затем выходит наружу. При этом лучи света рассеиваются в разные стороны и проходят через множество слоев снега, что приводит к тому, что все цвета в спектре видимого света равномерно отражаются и смешиваются вместе, создавая впечатление белого цвета. Важно отметить, что снег не всегда бывает абсолютно белым. Например, в условиях загрязненной атмосферы снег может приобретать серый или желтоватый оттенок из-за присутствия атмосферных загрязнений, таких как пыль, дым или выхлопные газы.");

                textTyping.StartTyping(UserName);
                Record.ShowTable();
                Record.SaveJson();
                Record.LoadJson();


                Console.WriteLine("Нажмите Enter, чтобы продолжить.");
                Console.ReadLine();

            }
        }
    }
}

        

