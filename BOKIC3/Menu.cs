using System;
using System.Collections.Generic;

public class Menu
{
    public static string RunMenu(List<string> menu)
    {
        int position = 0;
        ConsoleKey key;

        while (true)
        {
            Console.Clear();
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

                    if (selected.Equals("logout", StringComparison.OrdinalIgnoreCase) ||
                        selected.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Clear();
                        Console.WriteLine($"Confirm {selected}? (Y/N)");
                        var confirm = Console.ReadKey(true).Key;
                        if (confirm == ConsoleKey.Y || confirm == ConsoleKey.Enter)
                            return selected;
                    }
                    else
                    {
                        // Вызов соответствующих методов из Program
                        switch (selected)
                        {
                            case "Add_category":
                                Program.AddCategory();
                                break;
                            case "Change_category":
                                Program.EditCategory();
                                break;
                            case "Delete_category":
                                Program.RemoveCategory();
                                break;
                            case "Save_category":
                                Program.SaveAllCategories();
                                break;
                            case "Load_category":
                                Program.LoadAllCategories();
                                break;
                            case "Choose_category":
                                Program.SelectCategory();
                                break;
                        }
                        // После выполнения метода ждём нажатия клавиши, чтобы вернуться в меню
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
            "Choose_category",
            "Load_category",
            "logout",
            "Exit"
        };
        return RunMenu(menu);
    }

    public static string AdminMenu()
    {
        List<string> menu = new List<string>()
        {
            "Add_category",
            "Change_category",
            "Delete_category",
            "Save_category",
            "Load_category",
            "logout",
            "Exit"
        };
        return RunMenu(menu);
    }
}