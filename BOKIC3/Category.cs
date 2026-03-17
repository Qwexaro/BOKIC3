using System;
using System.Collections.Generic;
using System.Text;

partial class Question
{
    public string content;
    public string answer;
    public bool correct;
    public bool is_resolved;
    public int point;

    public int countPoints()
    {
        if (!correct) return 0;
        else return point;
    }

    public Question(string content, string answer, int point)
    {
        this.content = content;
        this.answer = answer;
        correct = false;
        is_resolved = false;
        this.point = point;
    }
}
partial class Category
{
    string name;
    int points;
    List<Question> questions;
    bool is_finished;
    bool is_active;


    void showInfo()
    {
        Console.WriteLine(name);
        Console.WriteLine(questions.Count + " Вопросов");
    }

    void LoadFromFile(string filename)
    {

        foreach (string line in File.ReadAllLines($"{filename}.txt"))
        {
            string[] parts = line.Split(':');
            if (parts.Length != 3) continue; // защита от ошибок

            questions.Add(new Question(parts[0], parts[1], int.Parse(parts[2])));
        }
    }

    void SaveInFile()
    {
        foreach (Question question in questions)
        {
            
            if (File.ReadAllLines($"{name}.txt").Contains(question.content)) {
                string[] f = File.ReadAllLines($"{name}.txt");
                for (int i = 0; i < f.Length; i++) {
                    if (f[i].Contains(question.content))
                    {
                        f[i] = question.content + ":" + question.answer + ":" + question.point;
                        break;
                    }
                    File.WriteAllText($"{name}.txt", "");
                    File.WriteAllLines($"{name}.txt", f);
                }
            }
            else
            {
                string[] a = { question.content + ":" + question.answer + ":" + question.point};
                File.WriteAllLines($"{name}.txt", a);
            }
        }
    }

}

