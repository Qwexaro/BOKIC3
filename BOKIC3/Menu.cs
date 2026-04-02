using System;
using System.Collections.Generic;

public class Menu
{
    public static string RunMenu(List<string> menu, string title)
    {
        int position = 0;
        ConsoleKey key;

        while (true)
        {
            Console.Clear();
            // Выводим заголовок в рамке
            Console.WriteLine($"=== {title} ===\n");
            for (int i = 0; i < menu.Count; i++)
            {
                if (i == position)
                    Console.WriteLine("> " + menu[i]);
                else
                    Console.WriteLine("  " + menu[i]);
            }

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    position = (position == 0) ? menu.Count - 1 : position - 1;
                    break;
                case ConsoleKey.DownArrow:
                    position = (position == menu.Count - 1) ? 0 : position + 1;
                    break;
                case ConsoleKey.Enter:
                    string selected = menu[position];

                    if (selected.Equals("Выйти из аккаунта", StringComparison.OrdinalIgnoreCase) ||
                        selected.Equals("Выход", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Clear();
                        Console.WriteLine($"Подтвердите {selected}? (Y/N)");
                        var confirm = Console.ReadKey(true).Key;
                        if (confirm == ConsoleKey.Y || confirm == ConsoleKey.Enter)
                            return selected;
                    }
                    else
                    {
                        switch (selected)
                        {
                            case "Добавить категорию":
                                Program.AddCategory();
                                break;
                            case "Изменить категорию":
                                Program.EditCategory();
                                break;
                            case "Удалить категорию":
                                Program.RemoveCategory();
                                break;
                            case "Сохранить категории":
                                Program.SaveAllCategories();
                                break;
                            case "Загрузить категории":
                                Program.LoadAllCategories();
                                break;
                            case "Выбрать категорию":
                                Program.SelectCategory();
                                break;
                            case "Мои результаты":
                                Program.ShowUserResults();
                                break;
                            case "Сбросить результаты":
                                Program.ResetUserResults();
                                break;
                        }
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                    }
                    break;
            }
        }
    }

    public static string UserMenu()
    {
        List<string> menu = new List<string>()
    {
        "Выбрать категорию",
        "Загрузить категории",
        "Мои результаты",
        "Сбросить результаты",  // новый пункт
        "Выйти из аккаунта",
        "Выход"
    };
        return RunMenu(menu, "Меню пользователя");
    }

    public static string AdminMenu()
    {
        List<string> menu = new List<string>()
    {
        "Добавить категорию",
        "Изменить категорию",
        "Удалить категорию",
        "Сохранить категории",
        "Загрузить категории",
        "Выйти из аккаунта",
        "Выход"
    };
        return RunMenu(menu, "Меню администратора");
    }
}