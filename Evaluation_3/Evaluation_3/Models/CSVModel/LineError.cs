namespace Evaluation_3.Models.CSVModel
{
    [Serializable]
    public class LineError
    {
        public int Line { get; set; }
        public string Error { get; set; } = "";
        public string? StackTrace { get; set; }
        public Exception Exception { get; set; }

        public LineError()
        {
        }
        public LineError(int line,string error)
        {
            Line = line;
            Error = error;
        }
        public LineError(int line, string error,string? stackTrace)
        {
            Line = line;
            Error = error;
            StackTrace = stackTrace;
        }
        public LineError(int line, string error,string? stackTrace,Exception ex)
        {
            Line = line;
            Error = error;
            StackTrace = stackTrace;
            Exception = ex;
        }
    }
}
