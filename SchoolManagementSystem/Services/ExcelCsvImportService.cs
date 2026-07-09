using Microsoft.AspNetCore.Http;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services
{
    public class ExcelCsvImportService
    {
        private readonly SchoolContext _context;

        public ExcelCsvImportService(SchoolContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, int RowsImported)> ImportStudentsFromCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "File is empty or missing.", 0);

            var studentsToInsert = new List<Student>();
            int rowCounter = 1;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());

                // Read the header line first to skip it (e.g., Name,Age,Gender,ClassId)
                var header = await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    rowCounter++;

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Split values by comma
                    var columns = line.Split(',');

                    // Strict validation rule: Ensure we have exactly the right amount of columns
                    if (columns.Length < 4)
                        return (false, $"Row {rowCounter} is missing columns.", 0);

                    string name = columns[0].Trim();
                    string ageRaw = columns[1].Trim();
                    string gender = columns[2].Trim();
                    string classIdRaw = columns[3].Trim();

                    // Try-Parse Pattern to eliminate runtime failures
                    if (!int.TryParse(ageRaw, out int age))
                        return (false, $"Row {rowCounter}: Age '{ageRaw}' must be a valid number.", 0);

                    if (!int.TryParse(classIdRaw, out int classId))
                        return (false, $"Row {rowCounter}: Class ID '{classIdRaw}' must be a valid number.", 0);

                    // Create our model instance manually
                    var student = new Student
                    {
                        Name = name,
                        Age = age,
                        Gender = gender,
                        ClassId = classId
                    };

                    studentsToInsert.Add(student);
                }

                // Database layer interaction
                await _context.Students.AddRangeAsync(studentsToInsert);
                await _context.SaveChangesAsync();

                // Commit the data safely to SQL Server
                await transaction.CommitAsync();
                return (true, "Import completed successfully!", studentsToInsert.Count);
            }
            catch (Exception ex)
            {
                // Abort the database save operation immediately
                await transaction.RollbackAsync();

                // IF EF Core threw a database update error, extract the direct SQL Server message
                if (ex.InnerException != null)
                {
                    return (false, $"Database Error on row {rowCounter}: {ex.InnerException.Message}", 0);
                }

                return (false, $"System Error on row {rowCounter}: {ex.Message}", 0);
            }
        }
    }
}