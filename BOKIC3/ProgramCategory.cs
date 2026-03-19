public partial class Program
{
    public static List<Category> categories { get; set; }

    public static void AddCategory()
    {
        Console.WriteLine("Введите название файла: ");

        string nameFile = Console.ReadLine().Trim();

        if (!File.Exists($"data/{nameFile}.txt")) categories.Add(new Category(nameFile));
    }

    public static void EditCategory()
    {
        Console.WriteLine("Введите название файла для редактирования: ");

        string nameFile = Console.ReadLine().Trim();

        if (categories.Contains(categories.Find(x => x.name.Equals(nameFile))))
        {
            Category category = categories.Find(x => x.name.Equals(nameFile));
            
            Console.WriteLine("Выберите действие: 0 - Выйти,\n Номер вопроса - изменить вопрос по номеру в списке:");

            for (int i = 0; i < category.questions.Count; i++) Console.WriteLine($"{i + 1} - {category.questions[i].content}");

            int choice = Convert.ToInt32(Console.ReadLine().Trim());

            if (choice > 0 && choice < category.questions.Count) category.questions[choice - 1].Edit();
        }
    }

    public static void RemoveCategory()
    {
        Console.WriteLine("Введите название файла для удаления:");

        string nameFile = Console.ReadLine().Trim();

        if (File.Exists($"data/{nameFile}.txt")) categories.Remove(categories.Find(x => x.name.Equals(nameFile)));
    }
}