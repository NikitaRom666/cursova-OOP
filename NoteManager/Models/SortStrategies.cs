using System.Collections.Generic;
using System.Linq;

namespace NoteManager.Models
{
    public interface ISortStrategy
    {
        IEnumerable<Note> Sort(IEnumerable<Note> notes);
    }

    public class SortByTitle : ISortStrategy
    {
        public IEnumerable<Note> Sort(IEnumerable<Note> notes)
        {
            return notes.OrderBy(n => n.Title);
        }
    }

    public class SortByDate : ISortStrategy
    {
        public IEnumerable<Note> Sort(IEnumerable<Note> notes)
        {
            return notes.OrderByDescending(n => n.CreatedAt);
        }
    }
}
