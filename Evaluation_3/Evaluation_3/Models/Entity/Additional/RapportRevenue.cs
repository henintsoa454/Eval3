using System.Globalization;

namespace Evaluation_3.Models.Entity.Additional
{
    public class RapportRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string client { get; set; }
        public string bien { get; set; }
        public int ordreMois { get; set; }
        public double Revenue { get; set; }
        public double Loyer { get; set; }
        public double Commission { get; set; }

        public string GetMonthName()
        {
            if (this.Month < 1 || this.Month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(this.Month));
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            return culture.DateTimeFormat.GetMonthName(this.Month);
        }
    }
}
