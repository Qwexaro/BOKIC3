using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Program
{
    public static void SelectCategory()
    {
        if (categories == null || categories.Count == 0)
        {
            Console.WriteLine("Нет доступных категорий.");
            return;
        }

        for (int i = 0; i < categories.Count; i++)
        {
            var cat = categories[i];
            int maxPoints = 0;
            foreach (var q in cat.questions) maxPoints += q.point;
            string status;
            if (cat.questions.Count == 0)
                status = "нет вопросов";
            else if (cat.is_finished)
                status = "пройдена";
            else
                status = "доступна";

            Console.WriteLine($"{i + 1}. {cat.name} | {status} | очков: {cat.points}/{maxPoints}");
        }

        Console.Write("Выберите номер темы (0 - выход): ");
        int choice = int.Parse(Console.ReadLine().Trim());
        if (choice < 1 || choice > categories.Count) return;

        PlayCategory(categories[choice - 1]);
    }

    public static void PlayCategory(Category category)
    {
        if (category.questions.Count == 0)
        {
            Console.WriteLine($"В теме '{category.name}' нет вопросов.");
            return;
        }
        if (category.is_finished)
        {
            Console.WriteLine($"Тема '{category.name}' уже пройдена.");
            return;
        }

        Console.WriteLine($"\n--- {category.name} ---\n");
        for (int i = 0; i < category.questions.Count; i++)
        {
            var q = category.questions[i];
            if (q.is_resolved)
            {
                Console.WriteLine($"Вопрос {i + 1} уже решён. Пропускаем.");
                continue;
            }

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
    }

    public static void SaveResults(Category category)
    {
        string dir = "results";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string file = Path.Combine(dir, "results.txt");
        string line = $"{DateTime.Now}: {category.name} - очки: {category.points}, завершена: {category.is_finished}";
        File.AppendAllText(file, line + Environment.NewLine);
        Console.WriteLine($"Результат сохранён в файл {file}");
    }
}