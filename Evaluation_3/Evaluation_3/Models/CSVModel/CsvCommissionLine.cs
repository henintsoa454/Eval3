using CsvHelper.Configuration.Attributes;

namespace Evaluation_3.Models.CSVModel
{
    public class CsvCommissionLine
    {
        [Index(0)]
        public string Type { get; set; }
        [Index(1)]
        public string Commission { get;set;}
    }
}
