using System.Collections.Generic;

namespace NoteManager.Models
{
    public class NoteHistory
    {
        private readonly Stack<NoteMemento> _history = new Stack<NoteMemento>();

        public void SaveVersion(Note note)
        {
            _history.Push(note.CreateMemento());
        }

        public bool Undo(Note note)
        {
            if (_history.Count == 0) return false;

            var memento = _history.Pop();
            note.RestoreMemento(memento);
            return true;
        }
    }
}