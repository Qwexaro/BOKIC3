using System;
using System.Collections.Generic;
using System.IO;

public class EntryAndExit
{
    public static List<User> LoadUsers()
    {
        List<User> users = new List<User>();

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

    public static User Login(List<User> users)
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");
        Console.Write("Login: ");
        string login = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        foreach (var user in users)
        {
            if (user.Login == login && user.Password == password)
                return user;
        }

        Console.WriteLine("Wrong login or password!");
        Console.ReadKey();
        return null;
    }

    public static void RunApp()
    {
        while (true)
        {
            var users = LoadUsers();
            User currentUser = Login(users);

            if (currentUser == null)
                continue;

            string result;

            if (currentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                result = Menu.AdminMenu();
            else
                result = Menu.UserMenu();

            if (result.Equals("logout", StringComparison.OrdinalIgnoreCase))
                continue; // выход в login

            if (result.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                break; // завершить программу
        }
    }
}