using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class CategoryLoader
{
    public static List<Category> LoadAllCategories()
    {
        List<Category> categories = new List<Category>();
        Directory.CreateDirectory("data");

        string[] categoryFiles = Directory.GetFiles("data", "*.txt");
        if (categoryFiles.Length == 0)
        {
            Console.WriteLine("Папка data пуста. Нет доступных категорий.");
            return categories;
        }

        Console.Clear();
        Console.WriteLine("=== Загрузка категорий ===\n");

        int loaded = 0;
        int errors = 0;

        foreach (string filePath in categoryFiles)
        {
            string categoryName = Path.GetFileNameWithoutExtension(filePath);
            Category category = new Category(categoryName);
            try
            {
                category.LoadFromFile(categoryName);
                // Загружаем даже если вопросов 0
                categories.Add(category);
                loaded++;
                Console.WriteLine($"[ОК] {categoryName} ({category.questions.Count} вопросов)");
            }
            catch (Exception ex)
            {
                errors++;
                Console.WriteLine($"[ОШИБКА] {categoryName}: {ex.Message}");
            }
        }

        Console.WriteLine($"\nЗагружено категорий: {loaded} из {categoryFiles.Length}");
        if (errors > 0) Console.WriteLine($"Ошибок: {errors}");
        Console.ReadKey();

        return categories;
    }

    public static Category LoadCategory(string categoryName)
    {
        string filePath = $"data/{categoryName}.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Категория '{categoryName}' не найдена");
            return null;
        }
        Category category = new Category(categoryName);
        category.LoadFromFile(categoryName);
        return category;
    }

    public static List<string> GetAvailableCategories()
    {
        Directory.CreateDirectory("data");
        List<string> categories = new List<string>();
        foreach (string filePath in Directory.GetFiles("data", "*.txt"))
        {
            categories.Add(Path.GetFileNameWithoutExtension(filePath));
        }
        return categories;
    }

    public static bool CategoryExists(string categoryName)
    {
        return File.Exists($"data/{categoryName}.txt");
    }

    public static void DeleteCategoryFile(string categoryName)
    {
        string filePath = $"data/{categoryName}.txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"Файл категории {categoryName} удален");
        }
    }
}