using System;
using System.Collections.Generic;

namespace NoteManager.Models
{
    public class NoteMemento
    {
        public string Title { get; }
        public string Content { get; }
        public string Category { get; }
        public HashSet<string> Tags { get; }
        public DateTime SavedAt { get; } = DateTime.Now;

        public NoteMemento(string title, string content, string category, HashSet<string> tags)
        {
            Title = title;
            Content = content;
            Category = category;
            Tags = new HashSet<string>(tags);
        }
    }
}


