using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using xNet;

namespace AllInMailTHRASHER.PluginManager
{
    public class PSettings
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public int Threads { get; set; }
        public string Capture { get; set; }
        public string FileOutput { get; set; }
        public string FileName { get; set; }
        public List<Regexx> list = new List<Regexx>();
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static PSettings GetSettings(string file)
        {
            string s = File.ReadAllText(file);
            PSettings p = new PSettings
            {
                Name = s.GetData("Name"),
                Author = s.GetData("Author"),
                Email = s.GetData("Domain"),
                Threads = Convert.ToInt32(s.GetData("Threads")),
                Capture = s.GetData("Capture"),
                FileOutput = s.GetData("FileOutput"),
                FileName = s.GetData("FileName"),
                list = x(s)
            };
            return p;
            }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static List<Regexx> x(string s)
        {
            List<Regexx> list = new List<Regexx>();
            MatchCollection mm = Regex.Matches(s, "<Regex>(.*)<\\/Regex>");
            foreach (Match m in mm) {
                var r = m.Groups[1].Value.Split(new string[] { "||||" }, StringSplitOptions.None);
                list.Add(new Regexx
                {
                    no = r[0],
                    regexpos = r[1],
                    regex = r[2],
                    group = Regex.Match(r[3], "\\[(.*)\\]").Groups[1].Value

                });

            }
            return list;
        }
        
        }
    }
public class Regexx
{
    public string no { get; set; }
    public string regexpos { get; set; }
    public string regex { get; set; }
    public string group { get; set; }
}
public static class Help
    {
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public static string GetData(this string source, string Tok)
        {
            return Parse(source, Tok);
        }

    [Obfuscation(Feature = "virtualization", Exclude = false)]
    private static string Parse(string s, string p)
        {
            return s.Substring($"<{p}>", $"</{p}>", 0);
        }
    }
