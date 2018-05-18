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
        private static Dictionary<string, string> KeyDictionaty;
        private static Dictionary<string, IPlugin> PluginsDictionary;
        private static string v = "0.2";
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
            if (!Directory.Exists("configs"))
                Directory.CreateDirectory("configs");
            Console.WriteLine("Made by xPolish | https://www.nulled.to/user/950279- | Discord: https://discord.gg/B2jgkD7");
            PluginLoader pLoader = new PluginLoader();
            var plugins = pLoader.GetPlugins();
            PluginsDictionary = new Dictionary<string, IPlugin>();
            foreach (var p in plugins)
            {
                PluginsDictionary[p.GetName()] = p;
            }
            int counter = 0;
            KeyDictionaty = new Dictionary<string, string>();
            if (PluginsDictionary.Count <= 0)
            {
                Console.WriteLine("No configs detected, refer to join discord server.");
                Console.Read();
                return;
            }
            foreach (var plug in PluginsDictionary)
            {
                counter++;

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (plug.Value.GetSettings().Author == "")
                {
                    Console.WriteLine($"-> ({plug.Key}) [{counter}]                                     ");
                }
                else
                {
                    Console.WriteLine($"-> ({plug.Key}) by {plug.Value.GetSettings().Author} [{counter}]                                     ");
                }
                KeyDictionaty[counter.ToString()] = plug.Key;
                Thread.Sleep(300);
            }
            string str = Console.ReadLine();

            try
            {
                LoadModule(KeyDictionaty[str]);
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
            Console.Clear();
            string thr = "";
            do
            {
                Console.WriteLine("Input the date");
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
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine($"You started Cracking: {pName} by {p.GetSettings().Author}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("-----------------------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            p.Start(p.GetSettings().Email, thr);
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