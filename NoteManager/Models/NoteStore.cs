using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NoteManager.Models
{
    public class NoteStore
    {
        private static NoteStore? _instance;
        public static NoteStore Instance => _instance ??= new NoteStore();

        private List<Note> _notes = new();

        private static readonly string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notes.json");

        private NoteStore() { Load(); }

        public void AddNote(Note note)
        {
            if (note == null) return;
            _notes.Add(note);
            Save();
        }

        public void DeleteNote(Guid id)
        {
            _notes.RemoveAll(n => n.Id == id);
            Save();
        }

        public void RemoveNote(Note note) => DeleteNote(note.Id);

        public IEnumerable<Note> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _notes;
            return _notes.Where(n =>
                n.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                n.Content.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                n.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        public void Save()
        {
            try
            {
                var dtos = _notes.Select(n => new NoteDto
                {
                    Id        = n.Id,
                    Title     = n.Title,
                    Content   = n.Content,
                    CreatedAt = n.CreatedAt,
                    IsPinned  = n.IsPinned,
                    Category  = n.Category,
                    Tags      = n.Tags.ToList()
                }).ToList();
                File.WriteAllText(SavePath,
                    JsonSerializer.Serialize(dtos,
                        new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NoteStore.Save() failed: {ex.Message}");
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(SavePath)) return;
                var dtos = JsonSerializer.Deserialize<List<NoteDto>>(File.ReadAllText(SavePath));
                if (dtos == null) return;
                _notes = dtos.Select(d =>
                {
                    var n = new Note(d.Id, d.Title, d.Content, d.CreatedAt) { IsPinned = d.IsPinned, Category = d.Category };
                    foreach (var t in d.Tags) n.Tags.Add(t);
                    return n;
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NoteStore.Load() failed: {ex.Message}");
                _notes = new();
            }
        }

        private class NoteDto
        {
            public Guid         Id        { get; set; }
            public string       Title     { get; set; } = "";
            public string       Content   { get; set; } = "";
            public DateTime     CreatedAt { get; set; }
            public bool         IsPinned  { get; set; }
            public string       Category  { get; set; } = "Загальне";
            public List<string> Tags      { get; set; } = new();
        }
    }
}

