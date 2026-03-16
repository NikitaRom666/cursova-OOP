using System;
using System.Collections.Generic;
using System.Linq;
using NoteManager.Models;
using NoteManager.Services;

namespace NoteManager.Services.Strategies
{
    public class SortByTitle : ISortStrategy
    {
        public List<Note> Sort(List<Note> notes)
            => notes.OrderBy(n => n.Title, StringComparer.OrdinalIgnoreCase).ToList();
    }
}
