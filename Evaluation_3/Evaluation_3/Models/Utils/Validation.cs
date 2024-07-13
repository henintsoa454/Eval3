using System.Text.RegularExpressions;

namespace Evaluation_3.Models.Utils
{
    public class Validation
    {
        // ----------- control regex ------------
        public static bool IsValidPassword(string password)
        {
            string atLeastSixChars = @"^.{6,}$";
            string atLeastSixCharsContainsUpperAndLower = @"^(?=.*[a-z])(?=.*[A-Z]).{6,}$";
            string atLeastSixCharsContainsUpperAndLowerAndDigit = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$";
            string atLeastSixCharsContainsUpperAndLowerAndDigitAndSpecialChars = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$";

            return Regex.IsMatch(password, atLeastSixChars);
        }
        public static bool IsValidPhoneNumber(string number)
        {
            string telmaRegex = @"^(034|038)\d{7}$";
            string orangeRegex = @"^(032|037)\d{7}$";
            string airtelRegex = @"^033\d{7}$";

            string allRegex = @"^(032|033|034|037|038)\d{7}$";

            return Regex.IsMatch(number, allRegex);
        }

        public static bool IsValidEmail(string email)
        {
            string emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailRegex);
        }

        public static bool IsValidDate(string date)
        {
            string regexFormat1 = @"^\d{4}-\d{2}-\d{2}$";
            string regexFormat2 = @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$";
            string regexFormat3 = @"^\d{4}/\d{2}/\d{2}$";
            string regexFormat4 = @"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}$";
            string regexFormat5 = @"^\d{2}-\d{2}-\d{4}$";
            string regexFormat6 = @"^\d{2}/\d{2}/\d{4}$";

            bool matchRegex =  Regex.IsMatch(date, regexFormat1);
            if (matchRegex)
            {
                DateTime parsedDate;
                string[] formats = new[]
                {
                    "yyyy-MM-dd",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy/MM/dd",
                    "yyyy/MM/dd HH:mm:ss",
                    "dd-MM-yyyy",
                    "dd-MM-yyyy HH:mm:ss",
                    "dd/MM/yyyy",
                    "dd/MM/yyyy HH:mm:ss",
                };
                if(DateTime.TryParseExact(date, formats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    return true;
                }
            }
            return false;
        }

        public static double ValidateDouble(string value)
        {
            Console.WriteLine($"Value to parse to DOUBLE '{value}'");

            value = value.Replace("%", "");
            value = value.Replace(".", ",");

            Console.WriteLine($"Value AFTER REPLACE '{value}'");

            double doubleValue = Math.Round(double.Parse(value), 2, MidpointRounding.AwayFromZero);

            Console.WriteLine($"Value AFTER PARSE '{doubleValue}'");

            return doubleValue;
        }

        public static int ValidateInt(string value)
        {
            Console.WriteLine($"Value to parse to INT '{value}'");

            value = value.Replace("%", "");
            value = value.Replace(".", ",");

            Console.WriteLine($"Value AFTER REPLACE '{value}'");

            int intValue = int.Parse(value);

            Console.WriteLine($"Value AFTER PARSE '{intValue}'");

            return intValue;
        }

        public static string ValidateString(string value) => value.Trim();

        public static DateTime FormatDate(string date)
        {
            DateTime dateTime;
            string[] formats = new[]
            {
                "yyyy/MM/dd",
                "yyyy/MM/dd HH:mm:ss",
                "dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "d/M/yyyy",
                "d/MM/yyyy",
                "dd/M/yyyy",
                "yyyy/M/d",
                "yyyy/M/dd",
                "yyyy/MM/d",
                "yyyy/M/dd HH:mm:ss",
                "yyyy/MM/d HH:mm:ss",
                "d/M/yyyy HH:mm:ss",
                "d/MM/yyyy HH:mm:ss",
                "dd/M/yyyy HH:mm:ss",
                "d/M/yyyy HH:mm:ss",
            };

            try
            {
                dateTime = DateTime.ParseExact(date, formats, null, System.Globalization.DateTimeStyles.None);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: ", e);
                Console.Error.WriteLine("ERROR: ", e.StackTrace);
                throw new Exception($"Date invalide '{date}'");
            }

            return dateTime;
        }

        public static DateTime FormatDate2(string date)
        {
            DateTime dateTime; 
            string[] formats = new[]
            {
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/MM/dd",
                "yyyy/MM/dd HH:mm:ss",
                "dd-MM-yyyy",
                "dd-MM-yyyy HH:mm:ss",
                "dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
            };

            try
            {
                dateTime = DateTime.ParseExact(date, formats, null, System.Globalization.DateTimeStyles.None);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("ERROR: ",e);
                Console.Error.WriteLine("ERROR: ",e.StackTrace);
                throw new Exception($"Date invalide '{date}'");
            }

            return dateTime;
        }

        public static void Main(string[] args)
        {
            string password = "       ";
            Console.WriteLine($"Valid Password {password} => {IsValidPassword(password)}");

            string number = "0342502525";
            Console.WriteLine($"Valid Phone Number {number} => {IsValidPhoneNumber(number)}");

            string email = "sanda25@gmail.com";
            Console.WriteLine($"Valid Date Format {email} => {IsValidEmail(email)}");

            string date = "2024-02-29";
            Console.WriteLine($"Valid Date Format {date} => {IsValidDate(date)}");
        }

        // --------------------------------------
    }
}
