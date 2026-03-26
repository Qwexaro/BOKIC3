using System;
using System.Collections.Generic;
using System.IO;

public partial class Program
{
    public static List<Category> categories;

    static void Main()
    {
        Directory.SetCurrentDirectory("..\\..\\..");
        EntryAndExit.RunApp();
    }
}