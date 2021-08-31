namespace SimpleModder
{
    public static class Logger
    {
        public delegate void MessageDelegate(string msg);

        public static event MessageDelegate OnMessage = delegate { };

        public static void Log(string msg)
        {
            OnMessage(msg);
        }
    }
}
