using System;
using System.IO;
using System.Linq;   // если хотите оставить Find, но Any заменён на цикл

public partial class Program
{
    public static void AddCategory()
    {
        Console.WriteLine("Введите название категории: ");
        string name = Console.ReadLine().Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Название не может быть пустым.");
            return;
        }

        // Проверка на существование категории без LINQ
        bool exists = false;
        foreach (var c in categories)
        {
            if (c.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                exists = true;
                break;
            }
        }
        if (exists)
        {
            Console.WriteLine("Категория с таким именем уже существует.");
            return;
        }

        Category newCat = new Category(name);
        categories.Add(newCat);
        newCat.SaveInFile();
        Console.WriteLine($"Категория '{name}' добавлена.");
    }

    public static void EditCategory()
    {
        if (categories.Count == 0)
        {
            Console.WriteLine("Нет категорий для редактирования.");
            return;
        }

        Console.WriteLine("Введите название категории для редактирования: ");
        string nameFile = Console.ReadLine().Trim();
        Category category = categories.Find(x => x.name.Equals(nameFile, StringComparison.OrdinalIgnoreCase));
        if (category == null)
        {
            Console.WriteLine("Категория не найдена.");
            return;
        }

        Console.WriteLine("Выберите действие: 0 - Выйти, \nНомер вопроса - изменить вопрос по номеру в списке:");
        for (int i = 0; i < category.questions.Count; i++)
            Console.WriteLine($"{i + 1} - {category.questions[i].content}");

        if (int.TryParse(Console.ReadLine().Trim(), out int choice) && choice > 0 && choice <= category.questions.Count)
        {
            category.questions[choice - 1].Edit();
            category.SaveInFile(); // сохраняем изменения
            Console.WriteLine("Изменения сохранены.");
        }
    }

    public static void RemoveCategory()
    {
        if (categories.Count == 0)
        {
            Console.WriteLine("Нет категорий для удаления.");
            return;
        }

        Console.WriteLine("Введите название категории для удаления: ");
        string nameFile = Console.ReadLine().Trim();
        Category category = categories.Find(x => x.name.Equals(nameFile, StringComparison.OrdinalIgnoreCase));
        if (category == null)
        {
            Console.WriteLine("Категория не найдена.");
            return;
        }
        categories.Remove(category);
        CategoryLoader.DeleteCategoryFile(category.name);
        Console.WriteLine($"Категория '{category.name}' удалена.");
    }

    public static void SaveAllCategories()
    {
        if (categories == null || categories.Count == 0)
        {
            Console.WriteLine("Нет категорий для сохранения.");
            return;
        }
        CategoryStorageManager.SaveAllCategories(categories);
    }

    public static void LoadAllCategories()
    {
        categories = CategoryLoader.LoadAllCategories();
        Console.WriteLine("Категории перезагружены из файлов.");
    }
}   