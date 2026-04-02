using System;
using System.Collections.Generic;
using System.IO;

public partial class Program
{
    private static (bool finished, int earnedPoints) GetCategoryStatusAndPoints(string categoryName)
    {
        if (Program.CurrentUser == null) return (false, 0);
        string file = Path.Combine("results", $"results_{Program.CurrentUser.Login}.txt");
        if (!File.Exists(file)) return (false, 0);
        string[] lines = File.ReadAllLines(file);
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            string[] parts = lines[i].Split('|');
            if (parts.Length == 5 && parts[1].Equals(categoryName, StringComparison.OrdinalIgnoreCase))
            {
                bool finished = bool.Parse(parts[4]);
                int earned = int.Parse(parts[2]);
                return (finished, earned);
            }
        }
        return (false, 0);
    }

    public static void SelectCategory()
    {
        if (categories == null || categories.Count == 0)
        {
            Console.WriteLine("Нет доступных категорий.");
            Console.ReadKey();
            return;
        }

        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Выбор категории ===\n");

            int maxNameLen = 0;
            foreach (var cat in categories)
                if (cat.name.Length > maxNameLen) maxNameLen = cat.name.Length;
            maxNameLen = Math.Max(maxNameLen, 15);

            for (int i = 0; i < categories.Count; i++)
            {
                var cat = categories[i];
                var (finished, earnedPoints) = GetCategoryStatusAndPoints(cat.name);
                int maxPoints = 0;
                foreach (var q in cat.questions) maxPoints += q.point;

                string status;
                string pointsDisplay;

                if (cat.questions.Count == 0)
                {
                    status = "нет вопросов";
                    pointsDisplay = "";
                }
                else if (finished)
                {
                    status = "пройдена";
                    pointsDisplay = $"{earnedPoints}/{maxPoints}";
                }
                else if (earnedPoints > 0 || File.Exists(Path.Combine("results", $"results_{Program.CurrentUser.Login}.txt")))
                {
                    // Проверяем, есть ли файл и запись для этой категории
                    bool hasRecord = false;
                    string file = Path.Combine("results", $"results_{Program.CurrentUser.Login}.txt");
                    if (File.Exists(file))
                    {
                        string[] lines = File.ReadAllLines(file);
                        foreach (string linee in lines)
                        {
                            string[] parts = linee.Split('|');
                            if (parts.Length == 5 && parts[1].Equals(cat.name, StringComparison.OrdinalIgnoreCase))
                            {
                                hasRecord = true;
                                break;
                            }
                        }
                    }
                    if (hasRecord)
                    {
                        status = "не завершена";
                        pointsDisplay = $"{earnedPoints}/{maxPoints}";
                    }
                    else
                    {
                        status = "доступна";
                        pointsDisplay = "";
                    }
                }
                else
                {
                    status = "доступна";
                    pointsDisplay = "";
                }

                string line = $"{cat.name.PadRight(maxNameLen)} | {status,-12} | {pointsDisplay}";
                if (i == selectedIndex)
                    Console.WriteLine("> " + line);
                else
                    Console.WriteLine("  " + line);
            }

            Console.WriteLine("\n[ESC] - Отмена");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? categories.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == categories.Count - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    Category selected = categories[selectedIndex];
                    if (selected.questions.Count == 0)
                    {
                        Console.WriteLine($"В теме '{selected.name}' нет вопросов.");
                        Console.ReadKey();
                        continue;
                    }
                    var (finishedFlag, _) = GetCategoryStatusAndPoints(selected.name);
                    if (finishedFlag)
                    {
                        Console.WriteLine($"Тема '{selected.name}' уже пройдена на 100%.");
                        Console.ReadKey();
                        continue;
                    }
                    PlayCategory(selected);
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    public static void PlayCategory(Category category)
    {
        var (finished, _) = GetCategoryStatusAndPoints(category.name);
        if (finished)
        {
            Console.WriteLine($"Тема '{category.name}' уже пройдена на 100%.");
            Console.ReadKey();
            return;
        }

        if (category.questions.Count == 0)
        {
            Console.WriteLine($"В теме '{category.name}' нет вопросов.");
            Console.ReadKey();
            return;
        }

        // Сбрасываем состояние вопросов перед прохождением (если категория не завершена)
        foreach (var q in category.questions)
        {
            q.is_resolved = false;
            q.correct = false;
        }

        Console.WriteLine($"\n--- {category.name} ---\n");
        for (int i = 0; i < category.questions.Count; i++)
        {
            var q = category.questions[i];
            // Теперь вопросы всегда не решены, так что можно убрать проверку
            Console.WriteLine($"Вопрос {i + 1}: {q.content}");
            Console.Write("Ваш ответ: ");
            string userAnswer = Console.ReadLine().Trim();

            if (userAnswer.Equals(q.answer, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Правильно! +{q.point} очков.");
                q.correct = true;
            }
            else
            {
                Console.WriteLine($"Неправильно. Правильный ответ: {q.answer}");
                q.correct = false;
            }
            q.is_resolved = true;
            Console.WriteLine();
        }

        category.checkAnswers();
        SaveResults(category);
        category.SaveInFile();
        Console.ReadKey();
    }

    public static void SaveResults(Category category)
    {
        if (Program.CurrentUser == null) return;
        int maxPoints = 0;
        foreach (var q in category.questions) maxPoints += q.point;
        string dir = "results";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string fileName = $"results_{Program.CurrentUser.Login}.txt";
        string file = Path.Combine(dir, fileName);
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{category.name}|{category.points}|{maxPoints}|{category.is_finished}";
        File.AppendAllText(file, line + Environment.NewLine);
    }
}