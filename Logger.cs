using System;
using System.IO;
using System.Threading.Tasks;
using Discord;

namespace SalgadoBot
{
    public class Logger
    {
        private static Logger _instance;
        public static Logger Instance => _instance ??= BuildInstance();

        private static Logger BuildInstance()
        {
            var instance = new Logger();
            instance.SetupLogger();
            return instance;
        }
        
        private const string LogBasePath = "DISCORD_LOG_";
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        public void SetupLogger()
        {
            var path = Path.Combine(Environment.CurrentDirectory, $"{LogBasePath}{DateTime.Now:yyyy-MM-dd}.txt");
            _fileStream = new FileStream(path,FileMode.Append, FileAccess.Write);
            _streamWriter = new StreamWriter(_fileStream);
        }

        public void CleanupLogger()
        {
            _streamWriter.Close();
            _fileStream.Close();
            Console.WriteLine("Logger finalized.");
        }

        public async Task Log(LogMessage message)
        {
            var msg = $"{message.ToString()}";
            Console.WriteLine(msg);
            await _streamWriter.WriteLineAsync(msg);
        }
    }
}