namespace AllInMailTHRASHER.PluginManager
{
    public interface IPlugin
    {
        void Start(string s, string ss);
        void Hook();
        void Stop();
        void Check(string email, string pass, string date);
        string GetName();
        PSettings GetSettings();
    }
}
