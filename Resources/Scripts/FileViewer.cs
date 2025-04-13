using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace AutoSorter.Resources.Scripts
{
    public class FileViewer
    {
        public List<string> GetFileNamesFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                MessageBox.Show("Указанная директория не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return Directory.GetFiles(directory)
                            .Select(Path.GetFileName)
                            .ToList();
        }

        public List<string> GetFileNamesFromDirectoryByPattern(string directory, string pattern)
        {
            List<string> fileNames = GetFileNamesFromDirectory(directory);
            List<string> patternedFiles = fileNames.Where(fileName => fileName.Contains(pattern)).ToList();

            if (patternedFiles.Count <= 0)
            {
                MessageBox.Show($"Не удалось найти файлы в дериктории {directory}, с указанным шаблоном {pattern}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return patternedFiles;
        }

        public List<string> GetFilePairsWithTimeDifferenceByPattern(string directory, string pattern, int maxTimeDifference)
        {
            List<string> filteredFiles = GetFileNamesFromDirectoryByPattern(directory, pattern);
            List<KeyValuePair<string, DateTime>> sortedFiles = SortFilesByDate(filteredFiles);
            List<string> resultPairs = new List<string>();

            for (int i = 0; i < sortedFiles.Count - 1; i++)
            {
                string file1 = sortedFiles[i].Key;
                string file2 = sortedFiles[i + 1].Key;
                TimeSpan difference = sortedFiles[i + 1].Value - sortedFiles[i].Value;

                if (difference.TotalSeconds > maxTimeDifference)
                {
                    resultPairs.Add($"{file1}\n{file2}\n(Отличаются по времени на {FormatTimeDifference(difference)}) ({difference.TotalSeconds} сек.)\n");
                }
            }

            return resultPairs;
        }

        private DateTime? ExtractDateFromFileName(string fileName)
        {
            Regex dateRegex = new Regex(@"\d{8}_\d{6}");

            Match match = dateRegex.Match(fileName);
            if (match.Success)
            {
                string dateTimeStr = match.Value;
                string datePart = dateTimeStr.Substring(0, 8);
                string timePart = dateTimeStr.Substring(9, 6);

                if (DateTime.TryParseExact(datePart + timePart, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime fileDate))
                {
                    return fileDate;
                }
            }

            return null;
        }

        private List<KeyValuePair<string, DateTime>> SortFilesByDate(List<string> fileNames)
        {
            Dictionary<string, DateTime> fileDateMap = new Dictionary<string, DateTime>();

            foreach (string file in fileNames)
            {
                DateTime? extractedDate = ExtractDateFromFileName(file);
                if (extractedDate.HasValue)
                {
                    fileDateMap[file] = extractedDate.Value;
                }
            }

            return fileDateMap.OrderBy(kv => kv.Value).ToList();
        }

        private string FormatTimeDifference(TimeSpan difference)
        {
            return $"{difference.Hours} ч {difference.Minutes} мин {difference.Seconds} сек";
        }
    }
}
