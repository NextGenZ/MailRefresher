using AllInMailTHRASHER.Email;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

internal struct ClientConfig
{
    private static ClientConfig[] _configs;
    static ClientConfig()
    {
        WebClient wc = new WebClient();
        _configs = JsonConvert.DeserializeObject<ClientConfig[]>(wc.DownloadString("http://auth.xpolish.pl/MailRefresher/mailservers.json"));
        return;
    }
    public string[] Domains { get; set; }
    public string Type { get; set; }
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string SocketType { get; set; }
    public string UserName { get; set; }
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public static IMailClient GetMailClient(string domain)
    {
        var conf = GetConfig(domain);
        if (conf.Type == "imap")
            return new ImapMailClient(conf);
        return new Pop3MailClient(conf);
    }
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public static ClientConfig GetConfig(string domain)
    {
        ClientConfig config;
        try
        {
            config = _configs.Where(x => x.Domains.Any(xx => xx == domain)).ToArray()[0];
        }
        catch
        {
            config = new ClientConfig
            {
                Port = 993,
                Hostname = "imap." + domain,
                SocketType = "SSL",
                Type = "imap"
            };
        }
        return config;
    }
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public bool IsNull()
    {
        return string.IsNullOrWhiteSpace(this.Hostname);
    }
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public override string ToString()
    {
        string text = this.Hostname;
        if (text.Length > 33)
        {
            text = text.Substring(0, 30) + "...";
        }
        return text;
    }
}