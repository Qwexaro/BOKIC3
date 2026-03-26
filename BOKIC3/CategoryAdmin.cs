public partial class Category
{
    public void AddQuestion()
    {
        Console.WriteLine("Введите вопрос: ");
        string content = Console.ReadLine().Trim();

        Console.WriteLine("Введите ответ: ");
        string answer = Console.ReadLine().Trim();

        Console.WriteLine("Введите очки: ");
        int point = int.Parse(Console.ReadLine().Trim());

        questions.Add(new Question(content, answer, point));
    }

    public void RemoveQuestion()
    {
        if (questions.Count == 0) { Console.WriteLine("Вопросов нет."); return; }

        Console.WriteLine("Введите номер вопроса для удаления: ");
        int index = int.Parse(Console.ReadLine().Trim()) - 1;

        if (index >= 0 && index < questions.Count) questions.RemoveAt(index);
    }
}