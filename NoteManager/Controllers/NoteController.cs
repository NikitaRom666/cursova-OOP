using System;
using System.Collections.Generic;
using NoteManager.Models;
namespace NoteManager.Controllers {
    public class NoteController {
        private NoteCollection _noteCollection = NoteCollection.Instance;
        public event Action? DataChanged;

        public void CreateNote(string title, string content, string tagsStr) {
            var note = new Note(title, content); UpdateTags(note, tagsStr);
            _noteCollection.AddNote(note); DataChanged?.Invoke();
        }
        public void UpdateNote(Note note, string title, string content, string tagsStr) {
            if (note == null) return;
            note.Title = title; note.Content = content; UpdateTags(note, tagsStr);
            DataChanged?.Invoke();
        }
        public void DeleteNote(Note note) { _noteCollection.DeleteNote(note); DataChanged?.Invoke(); }
        private void UpdateTags(Note note, string tagsStr) {
            note.Tags = new List<string>(tagsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            for(int i=0; i<note.Tags.Count; i++) note.Tags[i] = note.Tags[i].Trim();
        }
        public List<Note> GetAllNotes() => _noteCollection.GetSortedNotes();
        public List<Note> SearchNotes(string query) => string.IsNullOrWhiteSpace(query) ? GetAllNotes() : _noteCollection.Search(query);
        public void SetSorting(ISortStrategy strategy) { _noteCollection.SetSortStrategy(strategy); DataChanged?.Invoke(); }
    }
}