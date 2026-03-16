using System.Collections.Generic;
using System.Linq;
using NoteManager.Models;
using NoteManager.Services;

namespace NoteManager.Services.Strategies
{
    public class SortByDate : ISortStrategy
    {
        public List<Note> Sort(List<Note> notes)
            => notes.OrderByDescending(n => n.CreatedAt).ToList();
    }
}
