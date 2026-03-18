using System;
using System.Collections.Generic;
using System.Text;

public partial class Category
{
    string name;
    int points;
    List<Question> questions;
    bool is_finished;
    bool is_active;

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

        foreach (string line in File.ReadAllLines($"data/{filename}.txt"))
        {
            string[] parts = line.Split(':');
            if (parts.Length != 3) continue; // защита от ошибок

            questions.Add(new Question(parts[0], parts[1], int.Parse(parts[2])));
        }
    }

    public void SaveInFile()
    {
        foreach (Question question in questions)
        {
            
            if (File.ReadAllLines($"data/{name}.txt").Contains(question.content)) {
                string[] f = File.ReadAllLines($"{name}.txt");
                for (int i = 0; i < f.Length; i++) {
                    if (f[i].Contains(question.content))
                    {
                        f[i] = question.content + ":" + question.answer + ":" + question.point;
                        break;
                    }
                    File.WriteAllText($"data/{name}.txt", "");
                    File.WriteAllLines($"data/{name}.txt", f);
                }
            }
            else
            {
                string[] a = { question.content + ":" + question.answer + ":" + question.point};
                File.WriteAllLines($"data/{name}.txt", a);
            }
        }
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

