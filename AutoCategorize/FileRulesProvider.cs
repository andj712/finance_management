using System.Text.Json;

namespace finance_management.AutoCategorize
{
    public class FileRulesProvider : IRulesProvider
    {
        private readonly string _rulesFilePath;

        public FileRulesProvider(string rulesFilePath)
        {
            _rulesFilePath = rulesFilePath;
        }

        public RulesList GetRules()
        {
            //cita i desrijalizuje json fajl u RulesList objekat
            var json = File.ReadAllText(_rulesFilePath);
            return JsonSerializer.Deserialize<RulesList>(json)
                   ?? throw new InvalidDataException("Unable to deserialize rules list.");
        }
    }

}
