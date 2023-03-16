using HW02.Helpers;
using System.Text.Json;

namespace HW02.AnalyticalDataContext.DB
{
    public class AnalyticalDBContext
    {
        private readonly string[] _paths = { "..", "..", "..", "AnalyticalDataContext", "DB", "Storage", "AnalyticalData.json" };
        private readonly string _filePath;

        public AnalyticalDBContext()
        {
            _filePath = Path.Combine(_paths);
            FileHelper.CreateFile(_filePath);
        }

        // TODO: replace type List<object> in functions headers to the appropriate data model -> List<YourDataModel>
        public void SaveAnalyticalData(List<AnalyticData> log)
        {
            string jsonString = JsonSerializer.Serialize(log);
            using StreamWriter outputFile = new StreamWriter(_filePath);
            outputFile.WriteLine(jsonString);
        }

        public List<AnalyticData> ReadAnalyticalData()
        {
            string? line;
            using (StreamReader inputFile = new StreamReader(_filePath))
            {
                line = inputFile.ReadLine();
            }

            if (line == null)
            {
                return new List<AnalyticData>();
            }

            var model = JsonSerializer.Deserialize<List<AnalyticData>>(line);
            return model;
        }
    }
}
