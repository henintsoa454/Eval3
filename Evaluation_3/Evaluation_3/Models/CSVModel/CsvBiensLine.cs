using CsvHelper.Configuration.Attributes;

namespace Evaluation_3.Models.CSVModel
{
    public class CsvBiensLine
    {
        [Index(0)]
        public string reference { get; set; }
        [Index(1)]
        public string nom { get; set; }
        [Index(2)]
        public string Description { get; set; }
        [Index(3)]
        public string Type { get; set; }
        [Index(4)]
        public string region { get; set; }
        [Index(5)]
        public string loyer_mensuel { get; set; }
        [Index(6)]
        public string Proprietaire { get; set; }
    }
}
