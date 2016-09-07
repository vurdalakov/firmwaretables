namespace Vurdalakov
{
    using System;
    using System.Reflection;

    public abstract class DosToolsApplication
    {
        protected CommandLineParser _commandLineParser;

        public String ApplicationVersion
        {
            get
            {
                var parts = Assembly.GetExecutingAssembly().FullName.Split(',');
                return parts[1].Split('=')[1];
            }
        }

        public DosToolsApplication()
        {
            try
            {
                _commandLineParser = new CommandLineParser();

                if (_commandLineParser.IsOptionSet("?", "h", "help"))
                {
                    Help();
                }

                _silent = _commandLineParser.IsOptionSet("silent");
            }
            catch
            {
                Help();
            }
        }

        public void Run()
        {
            try
            {
                var result = Execute();
                Environment.Exit(result);
            }
            catch (Exception ex)
            {
                Print("{0}", ex.Message);
                Environment.Exit(1);
            }
        }

        protected abstract Int32 Execute();

        protected virtual void Help()
        {
            Environment.Exit(-1);
        }

        private Boolean _silent;

        protected void Print(String format, params Object[] args)
        {
            if (!_silent)
            {
                Console.WriteLine(String.Format(format, args));
            }
        }

        protected void Error(Int32 errorCode, String format, params Object[] args)
        {
            Print(format, args);
            Environment.Exit(errorCode);
        }
    }
}
