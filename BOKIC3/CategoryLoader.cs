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

        foreach (string filePath in categoryFiles)
        {
            string categoryName = Path.GetFileNameWithoutExtension(filePath);
            Category category = new Category(categoryName);
            try
            {
                category.LoadFromFile(categoryName);
                if (category.questions.Count > 0)
                {
                    categories.Add(category);
                    Console.WriteLine($"✓ Загружена категория: {categoryName} ({category.questions.Count} вопросов)");
                }
                else
                {
                    Console.WriteLine($"⚠ Категория {categoryName} не содержит вопросов и пропущена");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка загрузки категории {categoryName}: {ex.Message}");
            }
        }
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