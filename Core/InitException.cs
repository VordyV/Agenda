using System;

namespace Agenda.Core;

public class InitException : Exception
{
    public string Title { get; private set; }

    public InitException(string title, string text) : base(text)
    {
        this.Title = title;
    }
}