public partial class Question
{
    public void Edit()
    {
        Console.WriteLine("Выберите как изменить вопрос\n1 - изменить вопрос\n2 - изменить ответ\n3 - изменить очки\n0 - Выход");

        int choice = int.Parse(Console.ReadLine().Trim());

        switch (choice)
        {
            case 1:
                Console.WriteLine("Введите новый вопрос: ");
                content = Console.ReadLine().Trim();
                break;
            case 2:
                Console.WriteLine("Введите новый ответ: ");
                answer = Console.ReadLine().Trim();
                break;
            case 3:
                Console.WriteLine("Введите новые очки: ");
                point = int.Parse(Console.ReadLine().Trim());
                break;
        }
    }
}