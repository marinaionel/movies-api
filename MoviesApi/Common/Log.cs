using MoviesApi.Core.Logging;

namespace MoviesApi.Common
{
    internal class Log : LogBase<Log>
    {
        public Log() : base("MoviesApi") { }
    }
}
