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

                    else if (selected.Equals("Add_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //AddCategory
                    }
                    else if (selected.Equals("Change_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //ChangeCategory
                    }
                    else if (selected.Equals("Delete_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //DeleteCategory
                    }
                    else if (selected.Equals("Save_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //SaveCategory
                    }
                    else if (selected.Equals("Load_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //LoadCategory
                    }
                    else if (selected.Equals("Choose_category", StringComparison.OrdinalIgnoreCase))
                    {
                        //ChooseCategory
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