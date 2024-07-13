using Evaluation_3.Models.CSVModel;
using Evaluation_3.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace Evaluation_3.Models.Entity.Additional
{
    public class CsvcommissionFunction
    {
        public ImportCsvResult<Csvcommission> GetCsvResult(List<CsvCommissionLine> lines)
        {
            Console.WriteLine("Nombre csv line: " + lines.Count);

            List<Csvcommission> listCommissions = new List<Csvcommission>();
            List<LineError> lineErrors = new List<LineError>();
            foreach (CsvCommissionLine line in lines)
            {
                try
                {
                    Csvcommission commission = new Csvcommission
                    {
                        Type = Validation.ValidateString(line.Type.Trim()),
                        Commission = Validation.ValidateDouble(line.Commission.Trim())
                    };

                    listCommissions.Add(commission);
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
            return new ImportCsvResult<Csvcommission>(listCommissions, lineErrors);
        }

        public async Task<ImportCsvResult<Csvcommission>> DispatchToTableAsync(mada_immoContext context, IWebHostEnvironment hostEnvironment, string csvFolder, IFormFile file)
        {
            Console.WriteLine("MIANTSO AN DISPATCH TO TABLE AN'I CSVCOMMISSION");

            ImportCsvResult<Csvcommission> result = new ImportCsvResult<Csvcommission>(new List<Csvcommission>(), new List<LineError>());
            bool uploaded = await Functions.UploadCsvFile(hostEnvironment, csvFolder, file);

            if (uploaded)
            {
                List<CsvCommissionLine> commissionsFromCsv = Functions.ReadCsv<CsvCommissionLine>(hostEnvironment, csvFolder, file.FileName);
                result = this.GetCsvResult(commissionsFromCsv);

                List<Csvcommission> csvCommissions = result.ListeObject;

                context.Csvcommissions.RemoveRange(context.Csvcommissions);
                context.Csvcommissions.AddRange(csvCommissions);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw(@"
                    INSERT INTO ""typebien""(""nom"",""commission"")
                    SELECT ""type"",""commission""
                    FROM(
                        SELECT ""type"", ""commission""
                        FROM ""csvcommission""
                        GROUP BY ""type"", ""commission""
                    ) as cscmn
                    WHERE NOT EXISTS(
                        SELECT 1 FROM ""typebien"" t
                        WHERE t.""nom"" = cscmn.""type""
                        AND t.""commission"" = cscmn.""commission""
                    );
                ");

                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();

            return result;
        }
    }
}
