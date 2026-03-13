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

        public Note(string title, string content) 
        {
            Id = Guid.NewGuid(); 
            Title = title; 
            Content = content; 
            Tags = new HashSet<string>();
        }

        public NoteMemento CreateMemento() => new NoteMemento(Title, Content, new HashSet<string>(Tags));

        public void RestoreMemento(NoteMemento memento)
        {
            Title = memento.Title;
            Content = memento.Content;
            Tags = new HashSet<string>(memento.Tags);
        }
    }
}
