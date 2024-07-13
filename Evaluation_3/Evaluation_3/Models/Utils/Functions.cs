using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Evaluation_3.Models.Utils
{
    public class Functions
    {

        public static string GenerateFileName(IFormFile file)
        {
            if (file == null)
            {
                return "no_image.jpg";
            }
            else
            {
                return Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            }
        }
        public static async void UpdloadImage(IWebHostEnvironment hostEnvironment, string yourFolder, string imageName, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                var wwwRootPath = hostEnvironment.WebRootPath;
                var imageUploadFolder = Path.Combine(wwwRootPath, yourFolder);
                if (!Directory.Exists(imageUploadFolder))
                {
                    Directory.CreateDirectory(imageUploadFolder);
                }
                var imageFilePath = Path.Combine(imageUploadFolder, imageName);
                using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        private static void TrimStringProperties<T>(T obj)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = (string)property.GetValue(obj);
                    if (value != null)
                    {
                        value = value.Trim();
                        property.SetValue(obj, value);
                    }
                }
            }
        }


        // --------- CSV FILES ----------
        public static async Task<bool> UploadCsvFile(IWebHostEnvironment hostEnvironment, string yourFolder, IFormFile file)
        {
            try
            {
                string fileDir = Path.Combine(hostEnvironment.WebRootPath, yourFolder);
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string fileName = Path.Combine(fileDir, file.FileName);
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR UPLOAD: "+e.StackTrace);
                throw e;
            }
            return true;
        }

        public static bool CreateNewCsv<T>(IWebHostEnvironment hostEnvironment, string yourFolder, string fileName, List<T> lines)
        {
            var path = Path.Combine(hostEnvironment.WebRootPath, yourFolder, "New", fileName);
            using (var write = new StreamWriter(path))
            using (var csv = new CsvWriter(write, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords<T>(lines);
            }
            return true;
        }

        public static List<T> ReadCsv<T>(IWebHostEnvironment hostEnvironment, string csvFolder,string fileName)
        {
            List<T> csvLines = new List<T>();

            var path = Path.Combine(hostEnvironment.WebRootPath, csvFolder, fileName);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };
            Console.WriteLine($"PATH DU CSV {fileName} => {path}");
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var csvLine = csv.GetRecord<T>();
                    TrimStringProperties(csvLine);
                    Console.WriteLine($"Add CSV LINE: {csvLine}");
                    csvLines.Add(csvLine);
                }
            }
            Console.WriteLine("**** NOMBRE FINAL ==>"+csvLines.Count);

            return csvLines;
        }

        // ------------------------------

        public static object? GetKeyValue(object obj,Type objType)
        {
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                bool isKey = property.IsDefined(typeof(KeyAttribute), inherit: false);
                if (isKey)
                {
                    return property.GetValue(obj);
                }
            }
            return null;
        }

        public static void AffectUpdatedField<T>(object from, object to)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object? propValue = property.GetValue(from);
                if (propValue != null)
                {
                    if(propValue.GetType().IsEquivalentTo(typeof(DateTime)))
                    {
                        if (!propValue.Equals(DateTime.MinValue))
                        {
                            property.SetValue(to, propValue);
                        }
                    }else if (propValue.GetType().IsClass)
                    {
                        Console.WriteLine($"{property.Name} de {nameof(T)} est une class");
                        if(GetKeyValue(propValue,propValue.GetType()) != GetKeyValue(to, to.GetType()))
                        {
                            Console.WriteLine($"Valeur differente du precedent pour le champ {property.Name}");
                            property.SetValue(to, propValue);
                        }
                    }else
                    {
                        if (propValue != property.GetValue(to) && !String.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            property.SetValue(to, propValue);
                        }
                    }
                }
            }
        }

        public static List<T> Trier<T>(IQueryable<T> query, string triColumn, string orderType)
        {
            if (String.IsNullOrWhiteSpace(orderType))
            {
                return query.OrderBy(triColumn + " asc").ToList();
            }
            else
            {
                if (orderType.Equals("desc"))
                {
                    return query.OrderBy(triColumn + " desc").ToList();
                }
                else
                {
                    return query.OrderBy(triColumn + " asc").ToList();
                }
            }
        }

        public static string HashPassword(string word)
        {
            PasswordHasher<object> hasher = new PasswordHasher<object>();
            return hasher.HashPassword("", word);
        }
        public static bool VerifyPassword(string hashed,string provided)
        {
            PasswordHasher<object> hasher = new PasswordHasher<object>();
            PasswordVerificationResult result = hasher.VerifyHashedPassword("", hashed, provided);

            return result == PasswordVerificationResult.Success;
        }

        

        public static void Main(string[] args)
        {
            string password = "123";
            string hashed = HashPassword(password);
            Console.WriteLine($"True Password: {VerifyPassword(hashed, "123")}");

            // ------ Utilisation Session --------
            /*
             * 
            public static void SetObject<T>(this ISession session, string key, T value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }

            public static T GetObject<T>(this ISession session, string key)
            {
                var value = session.GetString(key);
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }


            HttpContext.Session.SetObject("NomDeLaCle", objetAStocker);
            var objetRécupéré = HttpContext.Session.GetObject < TypeDeL'Objet>("NomDeLaCle");

            *
            */
            // ----------------------------
        }

        //public DateTime getDateDisponibilité()
        //{
        //    Console.WriteLine("Nombre de location: " + this.LocationIdbienNavigations.Count);
        //    if (this.LocationIdbienNavigations.Count > 0)
        //    {
        //        DateTime result = this.LocationIdbienNavigations.First().Datedebut.AddMonths(this.LocationIdbienNavigations.First().Duree);
        //        foreach (Location location in this.LocationIdbienNavigations)
        //        {
        //            if (result <= location.Datedebut.AddMonths(location.Duree))
        //            {
        //                result = location.Datedebut.AddMonths(location.Duree);
        //            }
        //        }
        //        result = result.AddMonths(1);
        //        if (result > DateTime.Now)
        //        {
        //            return result;
        //        }
        //        else
        //        {
        //            return DateTime.Now;
        //        }
        //    }
        //    else
        //    {
        //        return DateTime.Now;
        //    }
        //}
    }
}
