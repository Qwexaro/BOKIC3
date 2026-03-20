using System;
using System.Collections.Generic;
using System.Text;


public partial class Question
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

