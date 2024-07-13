using Evaluation_3.Models.CSVModel;
using Evaluation_3.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace Evaluation_3.Models.Entity.Additional
{
    public class CsvLocationFunction
    {
        public ImportCsvResult<Csvlocation> GetCsvResult(List<CsvLocationLine> lines)
        {
            Console.WriteLine("Nombre csv line: " + lines.Count);

            List<Csvlocation> listLocations = new List<Csvlocation>();
            List<LineError> lineErrors = new List<LineError>();
            foreach (CsvLocationLine line in lines)
            {
                try
                {
                    Csvlocation location = new Csvlocation
                    {
                        Reference = Validation.ValidateString(line.Reference.Trim()),
                        DateDebut = Validation.FormatDate(line.DateDebut.Trim()),
                        DureeMois = Validation.ValidateInt(line.DureeMois.Trim()),
                        Client = Validation.ValidateString(line.Client.Trim())
                    };

                    listLocations.Add(location);
                }
                catch (Exception e)
                {
                    Console.WriteLine("----------------------------- ERROR -------------------------------");
                    Console.Error.WriteLine($"LINE PARSE ERROR => Ligne {lines.IndexOf(line) + 1}, {e.Message}");
                    Console.Error.WriteLine($"LINE PARSE ERROR => Ligne {lines.IndexOf(line) + 1}, {e.StackTrace}");
                    Console.WriteLine("----------------------------- ERROR -------------------------------");
                    lineErrors.Add(new LineError(lines.IndexOf(line) + 1, e.Message));
                    continue;
                }
            }
            return new ImportCsvResult<Csvlocation>(listLocations, lineErrors);
        }

        public async Task<ImportCsvResult<Csvlocation>> DispatchToTableAsync(mada_immoContext context, IWebHostEnvironment hostEnvironment, string csvFolder, IFormFile file)
        {
            Console.WriteLine("MIANTSO AN DISPATCH TO TABLE AN'I CSVLOCATION");

            ImportCsvResult<Csvlocation> result = new ImportCsvResult<Csvlocation>(new List<Csvlocation>(), new List<LineError>());
            bool uploaded = await Functions.UploadCsvFile(hostEnvironment, csvFolder, file);

            if (uploaded)
            {
                List<CsvLocationLine> locationsFromCsv = Functions.ReadCsv<CsvLocationLine>(hostEnvironment, csvFolder, file.FileName);
                result = this.GetCsvResult(locationsFromCsv);

                List<Csvlocation> csvLocations = result.ListeObject;

                context.Csvlocations.RemoveRange(context.Csvlocations);
                context.Csvlocations.AddRange(csvLocations);
                await context.SaveChangesAsync();

                var clientSql = @"
                    INSERT INTO ""client""(""nom"", ""email"")
                    SELECT ""client"",""client""
                    FROM(
                        SELECT ""client""
                        FROM ""csvlocation""
                        GROUP BY ""client""
                    ) as csvlo
                    WHERE NOT EXISTS(
                        SELECT 1 FROM ""client"" c
                        WHERE c.""email"" = csvlo.""client""
                    );
                ";

                var locationSql = @"
                    INSERT INTO ""location""(""referencebien"", ""datedebut"", ""duree"",""idclient"", ""idbien"")
                    SELECT l.""reference"",l.""date_debut"",l.""duree_mois"",c.""id"",b.""id""
                    FROM ""csvlocation"" l
                    JOIN ""client"" c on c.""email"" = l.""client""
                    JOIN ""bien"" b on  b.""reference"" = l.""reference""
                    WHERE NOT EXISTS
                    (
                        SELECT 1 FROM ""location"" lo
                        WHERE lo.""referencebien"" = l.""reference""
                        AND lo.""datedebut"" = l.""date_debut""
                        AND lo.""duree"" = l.""duree_mois""
                        AND lo.""idclient"" = c.""id""
                        AND lo.""idbien"" = b.""id""
                    );
                ";

                context.Database.ExecuteSqlRaw(clientSql);
                context.Database.ExecuteSqlRaw(locationSql);

                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();

            return result;
        }
    }
}
