using HW02.Helpers;

namespace HW02.LoggerContext.DB
{
    public class LoggerDBContext
    {

        private readonly string[] _paths = { "..", "..", "..", "LoggerContext", "DB", "Storage", "Log.txt" };
        private readonly string _filePath;

        public LoggerDBContext()
        {
            _filePath = Path.Combine(_paths);
            FileHelper.CreateFile(_filePath);
        }


        // TODO: replace type 'object' with your data model
        public void WriteLog(string log)
        {
            using StreamWriter sw = File.AppendText(_filePath);
            // TODO: when your data model is ready, uncomment following line
            sw.WriteLine(log);
        }
    }
}
