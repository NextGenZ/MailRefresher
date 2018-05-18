using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AE.Net.Mail;
using xNet;

namespace AllInMailTHRASHER.Email {
interface IMailClient
{
    bool IsConnected { get; }

    bool IsAuthenticated { get; }
    void SetProxy(ProxyType proxyType, string proxy);
    void SetProxy(ProxyClient proxyClient);
    bool Connect(string host, int port, string type);
    bool Connect();
    bool Login(string mail, string password);
    MailMessage[] GetMessages(string from, DateTime time);
}
class ImapMailClient : IMailClient
{

    public bool IsConnected => _imapClient.IsConnected;

    public bool IsAuthenticated => _imapClient.IsAuthenticated;
    private readonly ImapClient _imapClient;
    private readonly ClientConfig _clientConfig;
    private ProxyClient _proxyClient;
    public ImapMailClient()
    {
        this._imapClient = new ImapClient
        {
            AuthMethod = AuthMethods.Login,
            ServerTimeout = 30000
        };
    }
    public ImapMailClient(ClientConfig config)
    {
        this._imapClient = new ImapClient
        {
            AuthMethod = AuthMethods.Login,
            ServerTimeout = 30000
        };
        _clientConfig = config;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Connect(string host, int port, string type)
    {
        if (_proxyClient != null)
        {
            _imapClient.Connect(_proxyClient.CreateConnection(host, port), host, port, type == "SSL");
            return _imapClient.IsConnected;
        }
        _imapClient.Connect(host, port, type == "SSL");
        return _imapClient.IsConnected;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Connect()
    {
        if (_proxyClient != null)
        {
            _imapClient.Connect(_proxyClient.CreateConnection(_clientConfig.Hostname, _clientConfig.Port), _clientConfig.Hostname, _clientConfig.Port, _clientConfig.SocketType == "SSL");
            return _imapClient.IsConnected;
        }
        _imapClient.Connect(_clientConfig.Hostname, _clientConfig.Port, _clientConfig.SocketType == "SSL");
        return _imapClient.IsConnected;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public MailMessage[] GetMessages(string from, DateTime time)
    {
        this._imapClient.Encoding = Encoding.UTF8;
        string criteria = string.Format("BODY \"If you were prompted for a verification code\" FROM {0}", "account-update@amazon.com");
        criteria = $"FROM {from}";

        List<AE.Net.Mail.MailMessage> list = new List<MailMessage>();
        try
        {
            string[] array2 = this._imapClient.Search(criteria);
            int i = array2.Length - 1;
            while (array2.Length != 0 && i >= 0)
            {
                AE.Net.Mail.MailMessage mailMessage = this._imapClient.GetMessage(array2[i]);
                if (mailMessage.Date >= time)
                {
                    list.Add(mailMessage);
                }
                i--;
            }
        }
        catch
        {
            return list.ToArray();
        }
        return list.ToArray();
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Login(string mail, string password)
    {
        bool result = false;
        try
        {
            this._imapClient.Login(mail, password);
            result = _imapClient.IsAuthenticated;
        }
        catch (Exception ex)
        {
            if (ex is ImapClientException || ex is IOException)
            {
                throw;
            }
            if (ex.Message.Contains("MBR1236"))
            {
                result = false;
            }
            else if (!ex.Message.Contains("Application-specific password required") && !ex.Message.Contains("Please log in via your web browser") && !ex.Message.Contains("profil.wp.pl"))
            {
                result = false;
            }
            else
            {
                result = false;
            }
        }
        return result;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void SetProxy(ProxyType proxyType, string proxy)
    {
        if (proxyType != ProxyType.Http)
            _proxyClient = ProxyClient.Parse(proxyType, proxy);
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void SetProxy(ProxyClient proxyClient)
    {
        if (proxyClient?.Type != ProxyType.Http)
            _proxyClient = proxyClient;
    }
}
class Pop3MailClient : IMailClient
{
    private readonly Pop3Client _pop3Client;
    private readonly ClientConfig _clientConfig;
    private ProxyClient _proxyClient;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool IsConnected => _pop3Client.IsConnected;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool IsAuthenticated => _pop3Client.IsAuthenticated;
    public Pop3MailClient()
    {
        _pop3Client = new Pop3Client
        {
            ServerTimeout = 30000
        };
    }
        public Pop3MailClient(ClientConfig config)
    {
        _pop3Client = new Pop3Client
        {
            ServerTimeout = 30000
        };
        _clientConfig = config;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Connect(string host, int port, string type)
    {
        if (_proxyClient != null)
        {
            _pop3Client.Connect(_proxyClient.CreateConnection(host, port), host, port, type == "SSL");
            return _pop3Client.IsConnected;
        }
        _pop3Client.Connect(host, port, type == "SSL");
        return _pop3Client.IsConnected;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Connect()
    {
        if (_proxyClient != null)
        {
            _pop3Client.Connect(_proxyClient.CreateConnection(_clientConfig.Hostname, _clientConfig.Port), _clientConfig.Hostname, _clientConfig.Port, _clientConfig.SocketType == "SSL");
            return _pop3Client.IsConnected;
        }
        _pop3Client.Connect(_clientConfig.Hostname, _clientConfig.Port, _clientConfig.SocketType == "SSL");
        return _pop3Client.IsConnected;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public MailMessage[] GetMessages(string from, DateTime time)
    {

        int messageCount = this._pop3Client.GetMessageCount();

        List<MailMessage> list = new List<MailMessage>();

        for (int i = 0; i < messageCount; i++)
        {
            try
            {
                AE.Net.Mail.MailMessage mailMessage = this._pop3Client.GetMessage(i, false);

                if (mailMessage.Date >= time && mailMessage.From?.Address.ToLower() == from)
                {
                    list.Add(mailMessage);
                }

            }
            catch
            {
            }
        }

        return list.ToArray();
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool Login(string mail, string password)
    {
        bool result = false;
        try
        {
            this._pop3Client.Login(mail, password);
            result = _pop3Client.IsAuthenticated;
        }
        catch (Exception ex)
        {
            if (ex is IOException)
            {
                throw;
            }
            if (ex.Message.Contains("POP3 blocked"))
            {
                result = false;
            }
            if (ex.Message.Contains("POP3 disabled"))
            {
                result = false;
            }
            else
            {
                result = false;
            }
        }
        return result;
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void SetProxy(ProxyType proxyType, string proxy)
    {
        if (proxyType != ProxyType.Http)
            _proxyClient = ProxyClient.Parse(proxyType, proxy);
    }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void SetProxy(ProxyClient proxyClient)
    {
        if (proxyClient?.Type != ProxyType.Http)
            _proxyClient = proxyClient;
    }
}
}