using System.Collections.Generic;
using System.IO;

namespace AllInMailTHRASHER.PluginManager
{

    public class PluginLoader
    {
        public List<IPlugin> GetPlugins()
        {

            List<IPlugin> plug = new List<IPlugin>();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\configs");
                foreach (var file in files)
                {
                    try
                    {
                        var plugin = new Container(PSettings.GetSettings(file));
                        plug.Add(plugin);
                    }
                    catch { }
                }
            return plug;
        }


    }
}
