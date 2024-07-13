namespace Evaluation_3.Models.CSVModel
{
    public class ImportCsvResult<T>
    {
        public List<T> ListeObject { get; set; } = new List<T>();
        public List<LineError> LineErrors { get; set; } = new List<LineError>();
        public ImportCsvResult(List<T> listeObject, List<LineError> lineErrors)
        {
            ListeObject = listeObject;
            LineErrors = lineErrors;
        }
    }
}
