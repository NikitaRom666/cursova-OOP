using System;
using System.Collections.Generic;
using NoteManager.Models;

namespace NoteManager.Controllers
{
    public class NoteController
    {
        private NoteStore _store = NoteStore.Instance;
        private Dictionary<Guid, Stack<NoteMemento>> _history = new Dictionary<Guid, Stack<NoteMemento>>();

        public event Action? DataChanged;

        public void CreateNewNote(string title, string content, List<string> tags)
        {
            var note = new Note(title, content) { Tags = new HashSet<string>(tags) };
            _store.AddNote(note);
            DataChanged?.Invoke();
        }

        public void UpdateNote(Note note, string title, string content, List<string> tags)
        {
            if (!_history.ContainsKey(note.Id))
                _history[note.Id] = new Stack<NoteMemento>();

            _history[note.Id].Push(note.CreateMemento());

            note.Title = title;
            note.Content = content;
            note.Tags = new HashSet<string>(tags);
            DataChanged?.Invoke();
        }

        public void UndoChanges(Note note)
        {
            if (_history.ContainsKey(note.Id) && _history[note.Id].Count > 0)
            {
                var memento = _history[note.Id].Pop();
                note.RestoreMemento(memento);
                DataChanged?.Invoke();
            }
        }

        public void DeleteNote(Note note) => _store.RemoveNote(note);

        public IEnumerable<Note> GetNotes(string query, ISortStrategy sortStrategy)
        {
            return sortStrategy.Sort(_store.Search(query));
        }
    }
}
