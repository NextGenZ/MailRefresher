using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AllInMailTHRASHER.PluginManager
{

    public class PluginLoader
    {
        public List<IPlugin> GetPlugins()
        {

            List<IPlugin> plug = new List<IPlugin>();
            WebClient wc = new WebClient();
            MatchCollection mc = Regex.Matches(wc.DownloadString("http://auth.xpolish.pl/MailRefresher/configs/"), "unknown\"> <a href=\"([^\"]*)");
            foreach (Match m in mc)
            {
                try
                {
                    var plugin = new Container(PSettings.GetSettings(wc.DownloadString("http://auth.xpolish.pl" + m.Groups[1].Value), false));
                    plug.Add(plugin);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error parsing config:\r\n============================\r\n" +
                        "Mesage:\r\n" + ex.Message + "\r\n============================\r\n" +
                        "StackTrace:\r\n" + ex.StackTrace + "\r\n============================\r\n" +
                        "Source:\r\n" + ex.Source + "\r\n============================\r\n", $"[CLOUD] Error parsing config {m.Groups[1].Value.Split('/').Last()}");

                }
            }
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\configs", "*.json");
                foreach (var file in files)
                {
                    try
                    {
                        var plugin = new Container(PSettings.GetSettings(file));
                        plug.Add(plugin);
                    }
                    catch (Exception ex) {
                    MessageBox.Show("Error parsing config:\r\n============================\r\n" +
                        "Mesage:\r\n" + ex.Message + "\r\n============================\r\n" +
                        "StackTrace:\r\n" + ex.StackTrace + "\r\n============================\r\n" +
                        "Source:\r\n" + ex.Source + "\r\n============================\r\n", $"[LOCAL] Error parsing config {file.Split('\\').Last()}");

              }
                }
            return plug;
        }


    }
}
