using AllInMailTHRASHER.PluginManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace AllInMailTHRASHER
{
    class Program
    {
        private static Dictionary<string, string> KeyDictionary;
        private static Dictionary<string, IPlugin> PluginsDictionary;
        public static List<string> blacklisteddomains = new List<string>();
        private static string v = "0.9";
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        static void Main(string[] args)
        {
            Console.Title = "Mail Refresher | Made by xPolish";
            WebClient w = new WebClient();
            if (!(w.DownloadString("http://auth.xpolish.pl/MailRefresher/v.txt") == v))
            {
                while (true)
                {
                    Console.WriteLine("You're using an old version do you want to update?\r\nYes or No");
                    string response = Console.ReadLine();
                    Console.Clear();
                    if (response.ToLower() == "yes")
                    {
                        try
                        {
                            w.DownloadFile("http://auth.xpolish.pl/MailRefresher/MailRefresher.rar", @Environment.CurrentDirectory + "\\MailRefresher.rar");
                            Console.WriteLine("Finished downloading , the archive is at " + @Environment.CurrentDirectory + "\\MailRefresher.rar");
                            Console.ReadKey();
                            return;
                        }
                        catch { }
                    }
                    else if (response.ToLower() == "no")
                    {
                        Console.ReadKey();
                        return;
                    }
                    Console.Clear();
                }
            }
            if (!File.Exists("combo.txt"))
            {
                Console.WriteLine("Combo.txt not found");
                Console.Read();
                return;
            }
            {
                string[] xx = new WebClient().DownloadString("http://auth.xpolish.pl/MailRefresher/blacklist.txt").Split(',');
                foreach(var x in xx){
                    blacklisteddomains.Add(x);
                }
            }
            if (!Directory.Exists("configs"))
                Directory.CreateDirectory("configs");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Made by xPolish | https://www.nulled.to/user/950279- | Discord: https://discord.gg/B2jgkD7");
            Console.WriteLine("==========================================================================================");
            PluginLoader pLoader = new PluginLoader();
            var plugins = pLoader.GetPlugins();
            PluginsDictionary = new Dictionary<string, IPlugin>();
            foreach (var p in plugins)
            {
                PluginsDictionary[p.GetName()] = p;
            }
            int counter = 0;
            KeyDictionary = new Dictionary<string, string>();
            if (PluginsDictionary.Count <= 0)
            {
                Console.WriteLine("No configs detected, refer to join discord server.");
                Console.Read();
                return;
            }
            Console.WriteLine($"                                           Configs loaded: {plugins.Count}");
            Console.WriteLine("==========================================================================================");
            foreach (var plug in PluginsDictionary)
            {
                counter++;

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (plug.Value.GetSettings().Author == "")
                {
                    Console.WriteLine($"                                  -> ({plug.Key}) [{counter}]  <-                                  ");
                }
                else
                {
                    Console.WriteLine($"                                  -> ({plug.Key}) by {plug.Value.GetSettings().Author} <{counter}>  <-                                  ");
                }
                KeyDictionary[counter.ToString()] = plug.Key;
                Thread.Sleep(300);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("==========================================================================================");
            Console.WriteLine("If u need a config made contact Choempie#0001 at discord");
            Console.WriteLine("==========================================================================================");
            Console.WriteLine("Type below the number of the config you want to use");
            Console.WriteLine("==========================================================================================");
            Console.Write("                                                 ");
            string str = Console.ReadLine();

            try
            {
                LoadModule(KeyDictionary[str]);
            }
            catch
            {

            }
            Console.Read();
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        static public void LoadModule(string pName)
        {
            var p = PluginsDictionary[pName];
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            if (p.GetSettings().Author == "")
            {
                Console.WriteLine($"You selected Module: {pName}");
            }
            else
            {
                Console.WriteLine($"You selected Module: {pName} by {p.GetSettings().Author}");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Thread.Sleep(500);
            string thr = "";
            do
            {
                Console.Clear();
                Console.WriteLine("Input the date below (It will get all emails from the date u put to todays)\r\nFormat: [YYYY-MM-DD] Example: (2018-1-1) can be (2018-01-01) too");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("-----------------------------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                thr = Console.ReadLine();
            } while (thr == "");
            Thread.Sleep(500);
            Console.Clear();
            if (p.GetSettings().Author == "")
            {
                Console.WriteLine($"You started Cracking: {pName}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("-----------------------------------------");
                ConsoleColor consoleColor = ConsoleColor.Green;
                try
                {
                    consoleColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), p.GetSettings().ConsoleColor, true);
                }
                catch { }
                Console.ForegroundColor = consoleColor;
            }
            else
            {
                Console.WriteLine($"You started Cracking: {pName} by {p.GetSettings().Author}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("-----------------------------------------");
                ConsoleColor consoleColor = ConsoleColor.Green;
                try
                {
                    consoleColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), p.GetSettings().ConsoleColor, true);
                }
                catch{ }
                Console.ForegroundColor = consoleColor;
            }
            p.Start(thr);
            string var1 = "";
            do
            {
                var1 = Console.ReadLine();
                if (var1.Contains("clear"))
                {
                    Console.Clear();
                    continue;
                }
                if (var1.Contains("stop"))
                {
                    p.Stop();
                    break;
                }
                if (var1.Contains("help"))
                {
                    Console.WriteLine("");
                    continue;
                }

            } while (true);

        }

    }
}