public partial class Program
{
    public static List<Category> categories;
    public static User CurrentUser;

    static void Main()
    {
        Directory.SetCurrentDirectory("..\\..\\..");
        EntryAndExit.RunApp();
    }
}