using System;
using System.Collections.Generic;
using System.Linq;
namespace NoteManager.Models {
    public interface ISortStrategy { List<Note> Sort(List<Note> notes); }
    public class SortByTitle : ISortStrategy { public List<Note> Sort(List<Note> notes) => notes.OrderBy(n => n.Title).ToList(); }
    public class SortByDate : ISortStrategy { public List<Note> Sort(List<Note> notes) => notes.OrderByDescending(n => n.Id).ToList(); }

    public class NoteCollection {
        private static NoteCollection _instance;
        public static NoteCollection Instance => _instance ?? (_instance = new NoteCollection());
        private NoteCollection() { }

        private List<Note> _notes = new List<Note>();
        private ISortStrategy _sortStrategy;

        public void AddNote(Note note) => _notes.Add(note);
        public void DeleteNote(Note note) => _notes.Remove(note);
        public void SetSortStrategy(ISortStrategy strategy) => _sortStrategy = strategy;
        public List<Note> GetSortedNotes() => _sortStrategy == null ? _notes : _sortStrategy.Sort(_notes);
        
        // Оновлений пошук: шукає збіги у Title АБО в Tags
        public List<Note> Search(string query) => _notes.Where(n => 
            (n.Title != null && n.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) || 
            (n.Tags != null && n.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)))
        ).ToList();
    }
}