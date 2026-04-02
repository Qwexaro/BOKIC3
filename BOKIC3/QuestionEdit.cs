public partial class Question
{
    public void Edit()
    {
        bool editing = true;
        while (editing)
        {
            string[] menuItems = { "Изменить вопрос", "Изменить ответ", "Изменить очки", "Выйти" };
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine("=== Редактирование вопроса ===\n");
                Console.WriteLine($"Текущий вопрос: {content}");
                Console.WriteLine($"Текущий ответ: {answer}");
                Console.WriteLine($"Текущие очки: {point}\n");

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedIndex)
                        Console.WriteLine("> " + menuItems[i]);
                    else
                        Console.WriteLine("  " + menuItems[i]);
                }

                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex == menuItems.Length - 1) ? 0 : selectedIndex + 1;
                        break;
                }
            } while (key != ConsoleKey.Enter);

            switch (selectedIndex)
            {
                case 0: // Изменить вопрос
                    Console.Write("Введите новый вопрос: ");
                    content = Console.ReadLine().Trim();
                    break;
                case 1: // Изменить ответ
                    Console.Write("Введите новый ответ: ");
                    answer = Console.ReadLine().Trim();
                    break;
                case 2: // Изменить очки
                    int newPoint;
                    Console.Write("Введите новые очки: ");
                    while (!int.TryParse(Console.ReadLine().Trim(), out newPoint) || newPoint <= 0)
                    {
                        Console.Write("Очки должны быть положительным целым числом. Попробуйте снова: ");
                    }
                    point = newPoint;
                    break;
                case 3: // Выйти
                    editing = false;
                    break;
            }

            if (editing && selectedIndex != 3)
            {
                Console.WriteLine("\nИзменение применено. Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }
}