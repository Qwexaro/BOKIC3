using System;
using System.Collections.Generic;
using System.IO;

public partial class Category
{
    public string name;
    public int points;
    public List<Question> questions;
    public bool is_finished;
    public bool is_active;

    public Category(string name)
    {
        this.name = name;
        points = 0;
        questions = new List<Question>();
        is_finished = false;
        is_active = false;
    }

    public Category(string name, List<Question> questions)
    {
        this.name = name;
        points = 0;
        this.questions = questions;
        is_finished = false;
        is_active = false;
    }

    public void showInfo()
    {
        Console.WriteLine(name);
        Console.WriteLine(questions.Count + " Вопросов");
    }

    public void LoadFromFile(string filename)
    {
        string path = $"data/{filename}.txt";
        if (!File.Exists(path)) return;

        foreach (string line in File.ReadAllLines(path))
        {
            string[] parts = line.Split(':');
            if (parts.Length != 3) continue;

            questions.Add(new Question(parts[0], parts[1], int.Parse(parts[2])));
        }
    }

    public void SaveInFile()
    {
        string path = $"data/{name}.txt";
        List<string> lines = new List<string>();
        foreach (Question q in questions)
        {
            lines.Add($"{q.content}:{q.answer}:{q.point}");
        }
        File.WriteAllLines(path, lines);
    }

    public void checkAnswers()
    {
        points = 0;
        int completed = 0;
        foreach (Question question in questions)
        {
            if (question.correct) points += question.point;
            if (question.is_resolved) completed++;
        }
        Console.WriteLine("Points gained: " + points);
        Console.WriteLine("Questions finished: " + completed);
        if (completed == questions.Count)
        {
            is_finished = true;
            Console.WriteLine($"Category {name} is done");
        }
    }
}