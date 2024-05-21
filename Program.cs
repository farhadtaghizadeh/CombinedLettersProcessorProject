using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LetterProcessor
{
    public interface ILetterProcessor
    {
        void CreateSampleInputData();
        void CombineLetters();
        void GenerateReport();
        void ArchiveFiles();
    }

    public class LetterProcessor : ILetterProcessor
    {
        private readonly string _rootFolder = "CombinedLetters";
        private readonly string _inputFolder = Path.Combine("CombinedLetters", "Input");
        private readonly string _archiveFolder = Path.Combine("CombinedLetters", "Archive");
        private readonly string _outputFolder = Path.Combine("CombinedLetters", "Output");

        public void CreateSampleInputData()
        {
            Console.WriteLine("Creating sample input data...");
            string admissionFolder = Path.Combine(_inputFolder, "Admission", "20230518");
            string scholarshipFolder = Path.Combine(_inputFolder, "Scholarship", "20230518");

            // Create directories if they do not exist
            Directory.CreateDirectory(admissionFolder);
            Directory.CreateDirectory(scholarshipFolder);

            // Create sample admission letters
            CreateFile(Path.Combine(admissionFolder, "admission-12345678.txt"), "Admission Letter for Student 12345678");
            CreateFile(Path.Combine(admissionFolder, "admission-87654321.txt"), "Admission Letter for Student 87654321");

            // Create sample scholarship letters
            CreateFile(Path.Combine(scholarshipFolder, "scholarship-12345678.txt"), "Scholarship Letter for Student 12345678");
            CreateFile(Path.Combine(scholarshipFolder, "scholarship-56781234.txt"), "Scholarship Letter for Student 56781234");

            Console.WriteLine("Sample input files created successfully.");
        }

        private void CreateFile(string path, string content)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(content);
            }
        }

        public void CombineLetters()
        {
            Console.WriteLine("Starting to combine letters...");
            var combinedLetters = new Dictionary<string, List<string>>();

            foreach (var folder in Directory.GetDirectories(_inputFolder))
            {
                foreach (var dateFolder in Directory.GetDirectories(folder))
                {
                    var files = Directory.GetFiles(dateFolder, "*.txt");
                    Console.WriteLine($"Found {files.Length} files in folder: {dateFolder}");
                    foreach (var file in files)
                    {
                        var studentId = Path.GetFileName(file).Split('-')[1].Split('.')[0];
                        if (!combinedLetters.ContainsKey(studentId))
                        {
                            combinedLetters[studentId] = new List<string>();
                        }
                        combinedLetters[studentId].Add(file);
                    }
                }
            }

            var outputDateFolder = Path.Combine(_outputFolder, DateTime.Now.ToString("yyyyMMdd"));
            Directory.CreateDirectory(outputDateFolder);

            if (combinedLetters.Count == 0)
            {
                Console.WriteLine("No letters to combine.");
            }

            foreach (var studentId in combinedLetters.Keys)
            {
                if (combinedLetters[studentId].Count > 1)
                {
                    var combinedFilePath = Path.Combine(outputDateFolder, $"combined-{studentId}.txt");
                    Console.WriteLine($"Combining letters for student {studentId} into {combinedFilePath}");
                    using (var writer = new StreamWriter(combinedFilePath))
                    {
                        foreach (var file in combinedLetters[studentId])
                        {
                            writer.WriteLine(File.ReadAllText(file));
                        }
                    }
                }
            }

            Console.WriteLine("Combining letters completed.");
        }

        public void GenerateReport()
        {
            Console.WriteLine("Generating report...");
            var outputDateFolder = Path.Combine(_outputFolder, DateTime.Now.ToString("yyyyMMdd"));
            var reportPath = Path.Combine(outputDateFolder, "report.txt");

            var combinedFiles = Directory.GetFiles(outputDateFolder, "combined-*.txt");

            using (var writer = new StreamWriter(reportPath))
            {
                writer.WriteLine($"Processing Date: {DateTime.Now:yyyy-MM-dd}");
                writer.WriteLine($"Total Combined Letters: {combinedFiles.Length}");
                foreach (var file in combinedFiles)
                {
                    var studentId = Path.GetFileName(file).Split('-')[1].Split('.')[0];
                    writer.WriteLine(studentId);
                }
            }
            Console.WriteLine("Report generation completed.");
        }

        public void ArchiveFiles()
        {
            Console.WriteLine("Starting to archive files...");
            foreach (var folder in Directory.GetDirectories(_inputFolder))
            {
                foreach (var dateFolder in Directory.GetDirectories(folder))
                {
                    var dateFolderName = Path.GetFileName(dateFolder);
                    var archiveDateFolder = Path.Combine(_archiveFolder, dateFolderName);

                    if (!Directory.Exists(archiveDateFolder))
                    {
                        Directory.CreateDirectory(archiveDateFolder);
                    }

                    var files = Directory.GetFiles(dateFolder);
                    if (files.Length == 0)
                    {
                        Console.WriteLine($"No files to archive in folder: {dateFolder}");
                    }
                    foreach (var file in files)
                    {
                        var destFile = Path.Combine(archiveDateFolder, Path.GetFileName(file));
                        Console.WriteLine($"Archiving file: {file} to {destFile}");
                        File.Move(file, destFile);
                    }
                }
            }
            Console.WriteLine("Archiving completed.");
        }

        static void Main(string[] args)
        {
            var processor = new LetterProcessor();
            processor.CreateSampleInputData();
            processor.CombineLetters();
            processor.GenerateReport();
            processor.ArchiveFiles();
        }
    }
}
