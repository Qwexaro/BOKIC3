using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace QuizSystem
{
    public static class CategoryStorageManager
    {
        private static string dataPath = "data";
        private static string backupPath = "data/backups";
        private static string exportPath = "data/exports";
        private static string tempPath = "data/temp";

        // Инициализация директорий
        static CategoryStorageManager()
        {
            InitializeDirectories();
        }

        private static void InitializeDirectories()
        {
            Directory.CreateDirectory(dataPath);
            Directory.CreateDirectory(backupPath);
            Directory.CreateDirectory(exportPath);
            Directory.CreateDirectory(tempPath);
        }


        // Сохранение всех категорий
        public static void SaveAllCategories(List<Category> categories)
        {
            if (categories == null || categories.Count == 0)
            {
                Console.WriteLine("Нет категорий для сохранения!");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Сохранение всех категорий ===\n");

            int saved = 0;
            int failed = 0;

            foreach (var category in categories)
            {
                try
                {
                    category.SaveInFile();
                    saved++;
                    Console.WriteLine($"✓ Сохранена: {category.name}");
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine($"✗ Ошибка сохранения {category.name}: {ex.Message}");
                }
            }

            Console.WriteLine($"\nРезультат: сохранено {saved}, ошибок {failed}");
            Console.ReadKey();
        }

        //Сохранение конкретной категории
        public static bool SaveCategory(Category category)
        {
            try
            {
                category.SaveInFile();
                Console.WriteLine($"✓ Категория '{category.name}' сохранена");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка сохранения '{category.name}': {ex.Message}");
                return false;
            }
        }

        // Экспорт всех категорий в JSON
        public static void ExportAllToJson(List<Category> categories)
        {
            if (categories == null || categories.Count == 0)
            {
                Console.WriteLine("Нет категорий для экспорта!");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Экспорт категорий в JSON ===\n");

            int exported = 0;

            foreach (var category in categories)
            {
                if (ExportToJson(category))
                {
                    exported++;
                }
            }

            Console.WriteLine($"\n✓ Экспортировано категорий: {exported}");
            Console.WriteLine($"Папка экспорта: {exportPath}");
            Console.ReadKey();
        }

        // Экспорт одной категории в JSON
        public static bool ExportToJson(Category category)
        {
            try
            {
                string jsonPath = $"{exportPath}/{category.name}.json";

                var categoryData = new
                {
                    name = category.name,
                    points = category.points,
                    is_finished = category.is_finished,
                    is_active = category.is_active,
                    lastModified = category.lastModified,
                    questions = category.questions.Select(q => new
                    {
                        content = q.content,
                        answer = q.answer,
                        point = q.point,
                        is_resolved = q.is_resolved,
                        correct = q.correct
                    }).ToList()
                };

                string jsonString = JsonSerializer.Serialize(categoryData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                File.WriteAllText(jsonPath, jsonString, Encoding.UTF8);
                Console.WriteLine($"✓ Экспорт JSON: {category.name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка экспорта {category.name}: {ex.Message}");
                return false;
            }
        }

        // Экспорт в CSV формат
        public static void ExportToCsv(List<Category> categories)
        {
            if (categories == null || categories.Count == 0)
            {
                Console.WriteLine("Нет категорий для экспорта!");
                return;
            }

            string csvPath = $"{exportPath}/categories_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            try
            {
                using (var writer = new StreamWriter(csvPath, false, Encoding.UTF8))
                {
                    // Заголовок CSV
                    writer.WriteLine("Категория;Вопрос;Ответ;Очки;Решен;Правильно;Статус");

                    foreach (var category in categories)
                    {
                        foreach (var question in category.questions)
                        {
                            writer.WriteLine($"\"{category.name}\";\"{EscapeCsv(question.content)}\";\"{EscapeCsv(question.answer)}\";{question.point};{question.is_resolved};{question.correct};{category.is_finished}");
                        }
                    }
                }

                Console.WriteLine($"✓ Данные экспортированы в CSV: {csvPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка экспорта в CSV: {ex.Message}");
            }

            Console.ReadKey();
        }

        private static string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\"", "\"\"");
        }


        // Импорт категорий из JSON
        public static List<Category> ImportFromJson()
        {
            List<Category> importedCategories = new List<Category>();

            if (!Directory.Exists(exportPath))
            {
                Console.WriteLine("Папка с экспортированными файлами не найдена!");
                return importedCategories;
            }

            var jsonFiles = Directory.GetFiles(exportPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Console.WriteLine("Нет JSON файлов для импорта!");
                return importedCategories;
            }

            Console.Clear();
            Console.WriteLine("=== Импорт категорий из JSON ===\n");

            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonFile, Encoding.UTF8);
                    var jsonDoc = JsonDocument.Parse(jsonString);
                    var root = jsonDoc.RootElement;

                    string categoryName = root.GetProperty("name").GetString();

                    Category category = new Category(categoryName);

                    // Загрузка данных из JSON
                    if (root.TryGetProperty("points", out var pointsElement))
                        category.points = pointsElement.GetInt32();

                    if (root.TryGetProperty("is_finished", out var finishedElement))
                        category.is_finished = finishedElement.GetBoolean();

                    if (root.TryGetProperty("is_active", out var activeElement))
                        category.is_active = activeElement.GetBoolean();

                    if (root.TryGetProperty("questions", out var questionsElement))
                    {
                        foreach (var q in questionsElement.EnumerateArray())
                        {
                            var question = new Question(
                                q.GetProperty("content").GetString(),
                                q.GetProperty("answer").GetString(),
                                q.GetProperty("point").GetInt32()
                            );

                            if (q.TryGetProperty("is_resolved", out var resolvedElement))
                                question.is_resolved = resolvedElement.GetBoolean();

                            if (q.TryGetProperty("correct", out var correctElement))
                                question.correct = correctElement.GetBoolean();

                            category.questions.Add(question);
                        }
                    }

                    importedCategories.Add(category);
                    Console.WriteLine($"✓ Импортирована: {categoryName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Ошибка импорта {Path.GetFileName(jsonFile)}: {ex.Message}");
                }
            }

            Console.WriteLine($"\nИмпортировано категорий: {importedCategories.Count}");
            Console.ReadKey();

            return importedCategories;
        }

        // Создание резервной копии категории
        public static bool CreateBackup(Category category)
        {
            try
            {
                string backupFilePath = $"{backupPath}/{category.name}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string currentPath = $"{dataPath}/{category.name}.txt";

                if (File.Exists(currentPath))
                {
                    File.Copy(currentPath, backupFilePath, true);

                    // Очищаем старые резервные копии (оставляем только 10 последних)
                    var backups = Directory.GetFiles(backupPath, $"{category.name}_*.txt")
                        .OrderByDescending(f => f)
                        .Skip(10)
                        .ToList();

                    foreach (var oldBackup in backups)
                    {
                        File.Delete(oldBackup);
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //Восстановление из резервной копии
        public static bool RestoreFromBackup(string categoryName, DateTime backupDate)
        {
            string backupFileName = $"{categoryName}_{backupDate:yyyyMMdd_HHmmss}.txt";
            string backupFilePath = $"{backupPath}/{backupFileName}";

            if (!File.Exists(backupFilePath))
            {
                Console.WriteLine($"Резервная копия {backupFileName} не найдена!");
                return false;
            }

            try
            {
                string targetPath = $"{dataPath}/{categoryName}.txt";
                File.Copy(backupFilePath, targetPath, true);
                Console.WriteLine($"✓ Категория {categoryName} восстановлена из резервной копии {backupDate}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка восстановления: {ex.Message}");
                return false;
            }
        }

        // Просмотр доступных резервных копий
        public static void ShowBackups(string categoryName)
        {
            if (!Directory.Exists(backupPath))
            {
                Console.WriteLine("Резервные копии не найдены!");
                return;
            }

            var backups = Directory.GetFiles(backupPath, $"{categoryName}_*.txt")
                .OrderByDescending(f => f)
                .ToList();

            if (backups.Count == 0)
            {
                Console.WriteLine($"Нет резервных копий для категории {categoryName}");
                return;
            }

            Console.WriteLine($"\n=== Резервные копии категории {categoryName} ===\n");

            for (int i = 0; i < backups.Count; i++)
            {
                var fileInfo = new FileInfo(backups[i]);
                string fileName = Path.GetFileNameWithoutExtension(backups[i]);
                string dateStr = fileName.Replace($"{categoryName}_", "");
                Console.WriteLine($"{i + 1}. {dateStr} ({fileInfo.Length} байт, {fileInfo.LastWriteTime})");
            }

            Console.Write("\nВыберите номер для восстановления (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= backups.Count)
            {
                var selectedBackup = backups[choice - 1];
                var backupFile = new FileInfo(selectedBackup);
                string dateStr = Path.GetFileNameWithoutExtension(selectedBackup).Replace($"{categoryName}_", "");

                Console.Write($"Восстановить из копии {dateStr}? (Y/N): ");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    DateTime backupDate = backupFile.LastWriteTime;
                    RestoreFromBackup(categoryName, backupDate);
                }
            }
        }


        // Очистка старых файлов
        public static void CleanupOldFiles(int daysToKeep)
        {
            Console.Clear();
            Console.WriteLine($"=== Очистка старых файлов (старше {daysToKeep} дней) ===\n");

            int deleted = 0;
            DateTime cutoff = DateTime.Now.AddDays(-daysToKeep);

            // Очистка резервных копий
            if (Directory.Exists(backupPath))
            {
                var oldBackups = Directory.GetFiles(backupPath, "*.txt")
                    .Where(f => File.GetLastWriteTime(f) < cutoff)
                    .ToList();

                foreach (var file in oldBackups)
                {
                    File.Delete(file);
                    deleted++;
                    Console.WriteLine($"Удален: {Path.GetFileName(file)}");
                }
            }

            // Очистка экспортированных файлов
            if (Directory.Exists(exportPath))
            {
                var oldExports = Directory.GetFiles(exportPath, "*.json")
                    .Where(f => File.GetLastWriteTime(f) < cutoff)
                    .ToList();

                foreach (var file in oldExports)
                {
                    File.Delete(file);
                    deleted++;
                    Console.WriteLine($"Удален: {Path.GetFileName(file)}");
                }

                var oldCsv = Directory.GetFiles(exportPath, "*.csv")
                    .Where(f => File.GetLastWriteTime(f) < cutoff)
                    .ToList();

                foreach (var file in oldCsv)
                {
                    File.Delete(file);
                    deleted++;
                    Console.WriteLine($"Удален: {Path.GetFileName(file)}");
                }
            }

            Console.WriteLine($"\n✓ Удалено файлов: {deleted}");
            Console.ReadKey();
        }

        // Получение статистики по хранилищу
        public static void ShowStorageStats()
        {
            Console.Clear();
            Console.WriteLine("=== Статистика хранилища ===\n");

            // Статистика по папке data
            if (Directory.Exists(dataPath))
            {
                var categoryFiles = Directory.GetFiles(dataPath, "*.txt")
                    .Where(f => !f.Contains("backups") && !f.Contains("exports") && !f.Contains("temp"))
                    .ToList();

                long totalSize = categoryFiles.Sum(f => new FileInfo(f).Length);

                Console.WriteLine($"Категорий: {categoryFiles.Count}");
                Console.WriteLine($"Общий размер: {FormatFileSize(totalSize)}");
            }

            // Статистика по резервным копиям
            if (Directory.Exists(backupPath))
            {
                var backups = Directory.GetFiles(backupPath, "*.txt");
                long backupSize = backups.Sum(f => new FileInfo(f).Length);

                Console.WriteLine($"\nРезервные копии: {backups.Count}");
                Console.WriteLine($"Размер резервных копий: {FormatFileSize(backupSize)}");
            }

            // Статистика по экспортам
            if (Directory.Exists(exportPath))
            {
                var exports = Directory.GetFiles(exportPath, "*.*");
                long exportSize = exports.Sum(f => new FileInfo(f).Length);

                Console.WriteLine($"\nЭкспортированные файлы: {exports.Count}");
                Console.WriteLine($"Размер экспортов: {FormatFileSize(exportSize)}");
            }

            // Информация о свободном месте
            try
            {
                DriveInfo drive = new DriveInfo(Directory.GetCurrentDirectory());
                Console.WriteLine($"\nСвободно на диске: {FormatFileSize(drive.AvailableFreeSpace)}");
            }
            catch { }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "Б", "КБ", "МБ", "ГБ" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }


        private static bool autoSaveEnabled = true;
        private static int autoSaveInterval = 300; // секунд
        private static DateTime lastAutoSave = DateTime.Now;

        // Включение/отключение автосохранения
        public static void SetAutoSave(bool enabled, int intervalSeconds = 300)
        {
            autoSaveEnabled = enabled;
            autoSaveInterval = intervalSeconds;
            Console.WriteLine($"Автосохранение {(enabled ? "включено" : "отключено")}");
            if (enabled)
                Console.WriteLine($"Интервал: {intervalSeconds} секунд");
        }

        // Проверка необходимости автосохранения
        public static void CheckAutoSave(List<Category> categories)
        {
            if (!autoSaveEnabled) return;

            if ((DateTime.Now - lastAutoSave).TotalSeconds >= autoSaveInterval)
            {
                Console.WriteLine("\n[Автосохранение]");
                SaveAllCategories(categories);
                lastAutoSave = DateTime.Now;
            }
        }


        // Валидация файла категории
        public static bool ValidateCategoryFile(string categoryName)
        {
            string filePath = $"{dataPath}/{categoryName}.txt";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл категории {categoryName} не найден");
                return false;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                int lineNumber = 0;

                foreach (var line in lines)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(':');
                    if (parts.Length != 3 && parts.Length != 5)
                    {
                        Console.WriteLine($"Ошибка в строке {lineNumber}: неверный формат");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка валидации: {ex.Message}");
                return false;
            }
        }

        // Создание отчета о категориях

        public static void GenerateReport(List<Category> categories)
        {
            string reportPath = $"{exportPath}/report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            try
            {
                using (var writer = new StreamWriter(reportPath, false, Encoding.UTF8))
                {
                    writer.WriteLine("=== ОТЧЕТ ПО КАТЕГОРИЯМ ===");
                    writer.WriteLine($"Дата: {DateTime.Now}");
                    writer.WriteLine($"Всего категорий: {categories.Count}");
                    writer.WriteLine();

                    int totalQuestions = 0;
                    int totalPoints = 0;
                    int completedCategories = 0;

                    foreach (var category in categories)
                    {
                        int categoryPoints = category.questions.Sum(q => q.correct ? q.point : 0);
                        int resolvedQuestions = category.questions.Count(q => q.is_resolved);

                        totalQuestions += category.questions.Count;
                        totalPoints += categoryPoints;
                        if (category.is_finished) completedCategories++;

                        writer.WriteLine($"Категория: {category.name}");
                        writer.WriteLine($"  Вопросов: {category.questions.Count}");
                        writer.WriteLine($"  Решено вопросов: {resolvedQuestions}/{category.questions.Count}");
                        writer.WriteLine($"  Набрано очков: {categoryPoints}");
                        writer.WriteLine($"  Статус: {(category.is_finished ? "Завершена" : "В процессе")}");
                        writer.WriteLine();
                    }

                    writer.WriteLine("=== ИТОГО ===");
                    writer.WriteLine($"Всего вопросов: {totalQuestions}");
                    writer.WriteLine($"Всего очков: {totalPoints}");
                    writer.WriteLine($"Завершено категорий: {completedCategories}/{categories.Count}");
                }

                Console.WriteLine($"✓ Отчет создан: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка создания отчета: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}