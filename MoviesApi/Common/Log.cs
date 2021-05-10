using movies_api.Core.Logging;

namespace movies_api.Common
{
    internal class Log : LogBase<Log>
    {
        public Log() : base("MoviesApi") { }
    }
}
