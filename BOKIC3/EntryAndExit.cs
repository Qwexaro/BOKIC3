using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class EntryAndExit
{
    public static List<User> LoadUsers()
    {
        List<User> users = new List<User>();
        if (!File.Exists("users.txt"))
        {
            File.WriteAllLines("users.txt", new[]
            {
                "admin:1234:admin",
                "user:1234:user"
            });
        }

        foreach (string line in File.ReadAllLines("users.txt"))
        {
            string[] parts = line.Split(':');
            if (parts.Length != 3) continue;
            users.Add(new User
            {
                Login = parts[0],
                Password = parts[1],
                Role = parts[2]
            });
        }
        return users;
    }

    public static bool RegisterUser(string login, string password)
    {
        var users = LoadUsers();
        if (users.Any(u => u.Login == login))
        {
            Console.WriteLine("Пользователь с таким логином уже существует!");
            return false;
        }
        string newUserLine = $"{login}:{password}:user";
        string existingContent = File.ReadAllText("users.txt");
        if (existingContent.Length > 0 && !existingContent.EndsWith(Environment.NewLine))
        {
            File.AppendAllText("users.txt", Environment.NewLine);
        }
        File.AppendAllText("users.txt", newUserLine + Environment.NewLine);
        return true;
    }

    public static User Login(List<User> users)
    {
        Console.Clear();
        Console.WriteLine("=== Вход в систему ===");
        Console.Write("Логин: ");
        string login = Console.ReadLine();
        Console.Write("Пароль: ");
        string password = Console.ReadLine();

        foreach (var user in users)
        {
            if (user.Login == login && user.Password == password)
                return user;
        }
        Console.WriteLine("Неверный логин или пароль!");
        Console.ReadKey();
        return null;
    }

    private static int ShowStartMenu()
    {
        string[] menuItems = { "Вход", "Регистрация", "Выход" };
        int selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Добро пожаловать в систему тестирования ===\n");
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                    Console.WriteLine("> " + menuItems[i]);
                else
                    Console.WriteLine("  " + menuItems[i]);
            }

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == menuItems.Length - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    return selectedIndex;
            }
        }
    }

    public static void RunApp()
    {
        while (true)
        {
            int choice = ShowStartMenu();

            if (choice == 2) // Выход
            {
                Console.Clear();
                Console.WriteLine("До свидания!");
                break;
            }

            if (choice == 0) // Вход
            {
                var users = LoadUsers();
                User currentUser = Login(users);
                if (currentUser == null) continue;

                Program.CurrentUser = currentUser;
                // ПЕРЕЗАГРУЖАЕМ КАТЕГОРИИ ПРИ КАЖДОМ ВХОДЕ
                Program.categories = CategoryLoader.LoadAllCategories();

                string result;
                if (currentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    result = Menu.AdminMenu();
                else
                    result = Menu.UserMenu();

                if (result.Equals("Выйти из аккаунта", StringComparison.OrdinalIgnoreCase))
                {
                    Program.CurrentUser = null;
                    continue;
                }
                if (result.Equals("Выход", StringComparison.OrdinalIgnoreCase))
                {
                    Program.CurrentUser = null;
                    break;
                }
            }
            else if (choice == 1) // Регистрация
            {
                Console.Clear();
                Console.WriteLine("=== Регистрация нового пользователя ===\n");
                Console.Write("Придумайте логин: ");
                string login = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(login))
                {
                    Console.WriteLine("Логин не может быть пустым.");
                    Console.ReadKey();
                    continue;
                }
                Console.Write("Придумайте пароль: ");
                string password = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Пароль не может быть пустым.");
                    Console.ReadKey();
                    continue;
                }

                if (RegisterUser(login, password))
                {
                    Console.WriteLine("Регистрация успешна! Выполняется автоматический вход...");
                    var newUser = new User { Login = login, Password = password, Role = "user" };
                    Program.CurrentUser = newUser;
                    Program.categories = CategoryLoader.LoadAllCategories(); // перезагружаем категории
                    Console.ReadKey();

                    string result = Menu.UserMenu();
                    if (result.Equals("Выйти из аккаунта", StringComparison.OrdinalIgnoreCase))
                    {
                        Program.CurrentUser = null;
                        continue;
                    }
                    if (result.Equals("Выход", StringComparison.OrdinalIgnoreCase))
                    {
                        Program.CurrentUser = null;
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Регистрация не удалась.");
                    Console.ReadKey();
                }
            }
        }
    }
}