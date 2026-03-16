using System;
using System.Collections.Generic;

namespace NoteManager.Models
{
    public class Note
    {
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public HashSet<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsPinned { get; set; } = false;

        public string Category { get; set; } = "Загальне";

        public Note(string title, string content)
        {
            Id      = Guid.NewGuid();
            Title   = title;
            Content = content;
            Tags    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public Note(Guid id, string title, string content, DateTime createdAt)
        {
            Id        = id;
            Title     = title;
            Content   = content;
            CreatedAt = createdAt;
            Tags      = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public NoteMemento CreateMemento()
            => new NoteMemento(Title, Content, Category, new HashSet<string>(Tags, StringComparer.OrdinalIgnoreCase));

        public void RestoreMemento(NoteMemento memento)
        {
            Title   = memento.Title;
            Content = memento.Content;
            Category = memento.Category;
            Tags    = new HashSet<string>(memento.Tags, StringComparer.OrdinalIgnoreCase);
        }
    }
}

