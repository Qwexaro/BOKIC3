using System;
using System.IO;

public partial class Program
{
    private static Category SelectCategory(string prompt)
    {
        if (categories == null || categories.Count == 0)
        {
            Console.WriteLine("Нет доступных категорий.");
            return null;
        }

        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== {prompt} ===\n");
            for (int i = 0; i < categories.Count; i++)
            {
                if (i == selectedIndex)
                    Console.WriteLine("> " + categories[i].name);
                else
                    Console.WriteLine("  " + categories[i].name);
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
                    return categories[selectedIndex];
                case ConsoleKey.Escape:
                    return null;
            }
        }
    }

    public static void ResetUserResults()
    {
        if (Program.CurrentUser == null)
        {
            Console.WriteLine("Пользователь не определён.");
            Console.ReadKey();
            return;
        }

        string file = Path.Combine("results", $"results_{Program.CurrentUser.Login}.txt");
        if (!File.Exists(file))
        {
            Console.WriteLine($"У пользователя {Program.CurrentUser.Login} нет сохранённых результатов.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== Сброс результатов для пользователя {Program.CurrentUser.Login} ===\n");
        Console.WriteLine("ВНИМАНИЕ! Все результаты пройденных тестов будут удалены без возможности восстановления.");
        Console.Write("Вы уверены? (Y/N): ");
        if (Console.ReadKey(true).Key == ConsoleKey.Y)
        {
            File.Delete(file);
            Console.WriteLine("\nРезультаты успешно сброшены.");
        }
        else
        {
            Console.WriteLine("\nСброс отменён.");
        }
        Console.ReadKey();
    }

    private static Question SelectQuestion(Category category, string prompt)
    {
        if (category.questions.Count == 0)
        {
            Console.WriteLine("В этой категории нет вопросов.");
            return null;
        }

        int selectedIndex = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== {prompt} ===\n");
            Console.WriteLine($"Категория: {category.name}\n");
            for (int i = 0; i < category.questions.Count; i++)
            {
                string preview = category.questions[i].content.Length > 50
                    ? category.questions[i].content.Substring(0, 47) + "..."
                    : category.questions[i].content;
                if (i == selectedIndex)
                    Console.WriteLine($"> {i + 1}. {preview}");
                else
                    Console.WriteLine($"  {i + 1}. {preview}");
            }
            Console.WriteLine("\n[ESC] - Отмена");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? category.questions.Count - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == category.questions.Count - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    return category.questions[selectedIndex];
                case ConsoleKey.Escape:
                    return null;
            }
        }
    }


    public static void AddCategory()
    {
        Console.Clear();
        Console.WriteLine("=== Добавление новой категории ===\n");

        string name = "";
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("Введите название категории: ");
            name = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(name))
                Console.WriteLine("Название не может быть пустым. Попробуйте снова.");
        }

        // Проверка на существование категории
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
            Console.WriteLine($"Категория '{name}' уже существует.");
            Console.ReadKey();
            return;
        }

        Category newCat = new Category(name);
        categories.Add(newCat);
        Console.WriteLine($"\nКатегория '{name}' создана.\n");

        // Предложение добавить вопросы
        bool adding = true;
        while (adding)
        {
            Console.WriteLine("--- Добавление вопроса ---");
            Console.WriteLine($"Всего вопросов в категории: {newCat.questions.Count}");

            Console.Write("Введите текст вопроса (или '0' для завершения): ");
            string content = Console.ReadLine().Trim();
            if (content == "0")
            {
                adding = false;
                break;
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Вопрос не может быть пустым.");
                continue;
            }

            Console.Write("Введите правильный ответ: ");
            string answer = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(answer))
            {
                Console.WriteLine("Ответ не может быть пустым.");
                continue;
            }

            int point = 0;
            bool validPoint = false;
            while (!validPoint)
            {
                Console.Write("Введите количество очков (целое число > 0): ");
                string pointInput = Console.ReadLine().Trim();
                if (int.TryParse(pointInput, out point) && point > 0)
                {
                    validPoint = true;
                }
                else
                {
                    Console.WriteLine("Очки должны быть целым положительным числом.");
                }
            }

            newCat.questions.Add(new Question(content, answer, point));
            Console.WriteLine($"\nВопрос добавлен! Текущее количество вопросов: {newCat.questions.Count}\n");
        }

        if (newCat.questions.Count == 0)
        {
            Console.WriteLine("Категория создана без вопросов.");
        }
        else
        {
            Console.WriteLine($"Добавление вопросов завершено. Всего вопросов: {newCat.questions.Count}");
        }

        newCat.SaveInFile();
        Console.WriteLine($"\nКатегория '{name}' сохранена в файл.");
        Console.ReadKey();
    }

    public static void EditCategory()
    {
        Category category = SelectCategory("Выбор категории для редактирования");
        if (category == null)
        {
            Console.WriteLine("Редактирование отменено.");
            Console.ReadKey();
            return;
        }

        Question question = SelectQuestion(category, "Выбор вопроса для редактирования");
        if (question == null)
        {
            Console.WriteLine("Редактирование отменено.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== Редактирование вопроса ===\n");
        Console.WriteLine($"Категория: {category.name}");
        Console.WriteLine($"Текущий вопрос: {question.content}\n");

        question.Edit();  // вызов существующего метода, который сам запрашивает изменения

        category.SaveInFile();
        Console.WriteLine("\nИзменения сохранены.");
        Console.ReadKey();
    }

    public static void RemoveCategory()
    {
        Category category = SelectCategory("Удаление категории");
        if (category == null)
        {
            Console.WriteLine("Удаление отменено.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"Вы действительно хотите удалить категорию '{category.name}'? (Y/N)");
        var confirm = Console.ReadKey(true).Key;
        if (confirm == ConsoleKey.Y)
        {
            categories.Remove(category);
            CategoryLoader.DeleteCategoryFile(category.name);
            Console.WriteLine($"\nКатегория '{category.name}' удалена.");
        }
        else
        {
            Console.WriteLine("\nУдаление отменено.");
        }
        Console.ReadKey();
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

    public static void ShowUserResults()
    {
        if (Program.CurrentUser == null)
        {
            Console.WriteLine("Пользователь не определён.");
            return;
        }

        string dir = "results";
        string fileName = $"results_{Program.CurrentUser.Login}.txt";
        string file = Path.Combine(dir, fileName);

        if (!File.Exists(file))
        {
            Console.WriteLine($"У пользователя {Program.CurrentUser.Login} пока нет пройденных тестов.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== Результаты пользователя: {Program.CurrentUser.Login} ===\n");

        string[] lines = File.ReadAllLines(file);
        if (lines.Length == 0)
        {
            Console.WriteLine("Нет записей.");
            Console.ReadKey();
            return;
        }

        // Заголовок таблицы
        Console.WriteLine("| {0,-19} | {1,-25} | {2,10} | {3,6} | {4,8} |",
            "Дата", "Категория", "Набрано/Макс", "Процент", "Статус");
        Console.WriteLine(new string('-', 19 + 25 + 10 + 6 + 8 + 15)); // примерная длина

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');
            if (parts.Length != 5) continue; // пропускаем старые записи, если есть

            string date = parts[0];
            string category = parts[1];
            int earned = int.Parse(parts[2]);
            int max = int.Parse(parts[3]);
            bool finished = bool.Parse(parts[4]);

            double percent = max > 0 ? (double)earned / max * 100 : 0;
            string status = finished ? "Завершён" : "Не завершён";

            Console.WriteLine("| {0,-19} | {1,-25} | {2,4}/{3,-4} | {4,5:F1}% | {5,8} |",
                date, category, earned, max, percent, status);
        }

        Console.WriteLine("\nНажмите любую клавишу...");
        Console.ReadKey();
    }
}