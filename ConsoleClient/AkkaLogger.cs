using System;
using Akka.Actor;
using Slalom.Stacks.Logging;

namespace ConsoleClient
{
    public class AkkaLogger : ILogger
    {
        private readonly ActorSystem _system;

        public AkkaLogger(ActorSystem system)
        {
            _system = system;
        }

        public void Dispose()
        {
        }

        public void Debug(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Debug(string template, params object[] properties)
        {
            _system.ActorSelection("akka.tcp://logging@localhost:8080/user/log").Tell(new LogMessage(template, properties));
        }

        public void Error(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Error(string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Fatal(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Information(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Information(string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Verbose(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Verbose(string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception exception, string template, params object[] properties)
        {
            throw new NotImplementedException();
        }

        public void Warning(string template, params object[] properties)
        {
            throw new NotImplementedException();
        }
    }
}