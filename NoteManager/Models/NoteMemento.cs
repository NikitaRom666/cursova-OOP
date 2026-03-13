using System.Collections.Generic;

namespace NoteManager.Models
{
    public class NoteMemento
    {
        public string Title { get; }
        public string Content { get; }
        public HashSet<string> Tags { get; }

        public NoteMemento(string title, string content, HashSet<string> tags)
        {
            Title = title;
            Content = content;
            Tags = new HashSet<string>(tags);
        }
    }
}
