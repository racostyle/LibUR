namespace LibUR.Logging
{
    public class LoggerFactory
    {
        public static ILogger Build(bool isDebugMode)
        {
            return isDebugMode ? new Logger() : new LoggerVoid();
        }
    }
}
