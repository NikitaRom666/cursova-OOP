using System;
using System.Collections.Generic;
using System.Linq;
using NoteManager.Models;
using NoteManager.Services;
using NoteManager.Services.Strategies;

namespace NoteManager.Controllers
{
    public class NoteController
    {
        private NoteStore _store = NoteStore.Instance;
        private Dictionary<Guid, Stack<NoteMemento>> _history = new();

        public event Action? DataChanged;

        public void CreateNewNote(string title, string content, List<string> tags)
        {
            var note = NoteFactory.CreateWithTags(title, content, tags);
            _store.AddNote(note);
            DataChanged?.Invoke();
        }

        public void UpdateNote(Note note, string title, string content, List<string> tags)
        {
            if (!_history.ContainsKey(note.Id))
                _history[note.Id] = new Stack<NoteMemento>();

            _history[note.Id].Push(note.CreateMemento());

            note.Title   = title;
            note.Content = content;
            note.Tags    = new HashSet<string>(tags);
            DataChanged?.Invoke();
        }

        public List<NoteMemento> GetHistory(Note note)
        {
            if (_history.ContainsKey(note.Id))
                return _history[note.Id].ToList();
            return new List<NoteMemento>();
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

        public void DeleteNote(Note note)
        {
            _store.DeleteNote(note.Id);
            DataChanged?.Invoke();
        }

        public IEnumerable<Note> GetNotes(string query, ISortStrategy sortStrategy)
            => sortStrategy.Sort(_store.Search(query).ToList());
    }
}


