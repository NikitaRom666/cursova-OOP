using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteManager.Models
{
    public class NoteStore
    {
        private static NoteStore? _instance;
        public static NoteStore Instance => _instance ??= new NoteStore();

        private List<Note> _notes = new List<Note>();

        private NoteStore() { }

        public void AddNote(Note note) => _notes.Add(note);
        public void RemoveNote(Note note) => _notes.Remove(note);
        
        public IEnumerable<Note> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _notes;
            
            return _notes.Where(n => 
                n.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                n.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
}
