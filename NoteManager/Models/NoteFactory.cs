using System;
using System.Collections.Generic;

namespace NoteManager.Models
{
    public static class NoteFactory
    {
        public static Note Create(string title, string content)
            => new Note(title, content);

        public static Note CreateWithTags(string title, string content, IEnumerable<string> tags)
        {
            var note = new Note(title, content);
            foreach (var tag in tags) note.Tags.Add(tag);
            return note;
        }

        public static Note CreateEmpty()
            => new Note("Нова нотатка", string.Empty);
    }
}