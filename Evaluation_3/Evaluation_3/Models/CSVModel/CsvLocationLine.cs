using CsvHelper.Configuration.Attributes;

namespace Evaluation_3.Models.CSVModel
{
    public class CsvLocationLine
    {
        [Index(0)]
        public string Reference { get; set; }
        [Index(1)]
        public string DateDebut { get; set; }
        [Index(2)]
        public string DureeMois { get; set; }
        [Index(3)]
        public string Client { get; set; }
    }
}
