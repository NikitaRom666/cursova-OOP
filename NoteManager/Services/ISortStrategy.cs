using System.Collections.Generic;
using NoteManager.Models;

namespace NoteManager.Services
{
    public interface ISortStrategy
    {
        List<Note> Sort(List<Note> notes);
    }
}
