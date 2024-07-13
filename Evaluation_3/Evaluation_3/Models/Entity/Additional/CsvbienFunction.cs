using Evaluation_3.Models.CSVModel;
using Evaluation_3.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace Evaluation_3.Models.Entity.Additional
{
    public class CsvbienFunction
    {
        public ImportCsvResult<Csvbien> GetCsvResult(List<CsvBiensLine> lines)
        {
            Console.WriteLine("Nombre csv line: " + lines.Count);

            List<Csvbien> listBiens = new List<Csvbien>();
            List<LineError> lineErrors = new List<LineError>();
            foreach (CsvBiensLine line in lines)
            {
                try
                {
                    Csvbien bien = new Csvbien
                    {
                        Reference = Validation.ValidateString(line.reference.Trim()),
                        Nom = Validation.ValidateString(line.nom.Trim()),
                        Description = Validation.ValidateString(line.Description.Trim()),
                        Type = Validation.ValidateString(line.Type.Trim()),
                        Region = Validation.ValidateString(line.region.Trim()),
                        LoyerMensuel = Validation.ValidateDouble(line.loyer_mensuel.Trim()),
                        Proprietaire = Validation.ValidateString(line.Proprietaire.Trim())
                    };

                    listBiens.Add(bien);
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
            return new ImportCsvResult<Csvbien>(listBiens, lineErrors);
        }

        public async Task<ImportCsvResult<Csvbien>> DispatchToTableAsync(mada_immoContext context, IWebHostEnvironment hostEnvironment, string csvFolder, IFormFile file)
        {
            Console.WriteLine("MIANTSO AN DISPATCH TO TABLE AN'I CSVBIEN");

            ImportCsvResult<Csvbien> result = new ImportCsvResult<Csvbien>(new List<Csvbien>(), new List<LineError>());
            bool uploaded = await Functions.UploadCsvFile(hostEnvironment, csvFolder, file);

            if (uploaded)
            {
                List<CsvBiensLine> biensFromCsv = Functions.ReadCsv<CsvBiensLine>(hostEnvironment, csvFolder, file.FileName);
                result = this.GetCsvResult(biensFromCsv);

                List<Csvbien> csvEtapes = result.ListeObject;

                context.Csvbiens.RemoveRange(context.Csvbiens);
                context.Csvbiens.AddRange(csvEtapes);
                await context.SaveChangesAsync();

                var regionSql = @"
                    INSERT INTO ""region""(""nom"")
                    SELECT ""region""
                    FROM(
                        SELECT ""region""
                        FROM ""csvbiens""
                        GROUP BY ""region""
                    ) as csvbi
                    WHERE NOT EXISTS(
                        SELECT 1 FROM ""region"" re
                        WHERE re.""nom"" = csvbi.""region""
                    );
                ";

                var proprietaireSql = @"
                    INSERT INTO ""proprietaire""(""nom"", ""numerotel"")
                    SELECT ""proprietaire"",""proprietaire""
                    FROM(
                        SELECT ""proprietaire""
                        FROM ""csvbiens""
                        GROUP BY ""proprietaire""
                    ) as csvbi
                    WHERE NOT EXISTS(
                        SELECT 1 FROM ""proprietaire"" p
                        WHERE p.""numerotel"" = csvbi.""proprietaire""
                    );
                ";

                var bienSql = @"
                    INSERT INTO ""bien""(""reference"", ""nom"", ""description"",""idtype"", ""idregion"", ""loyermensuel"", ""idproprietaire"")
                    SELECT b.""reference"",b.""nom"",b.""description"",t.""id"",r.""id"",b.""loyer_mensuel"",p.""id""
                    FROM ""csvbiens"" b
                    JOIN ""typebien"" t on t.""nom"" = b.""type""
                    JOIN ""region"" r on  r.""nom"" = b.""region""
                    JOIN ""proprietaire"" p on p.""numerotel"" = b.""proprietaire""
                    WHERE NOT EXISTS
                    (
                        SELECT 1 FROM ""bien"" bi
                        WHERE bi.""reference"" = b.""reference""
                    );
                ";

                context.Database.ExecuteSqlRaw(regionSql);
                context.Database.ExecuteSqlRaw(proprietaireSql);
                context.Database.ExecuteSqlRaw(bienSql);

                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();

            return result;
        }
    }
}
