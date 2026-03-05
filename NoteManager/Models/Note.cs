using System;
using System.Collections.Generic;
namespace NoteManager.Models {
    public class Note {
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public Note(string title, string content) {
            Id = Guid.NewGuid(); Title = title; Content = content; Tags = new List<string>();
        }
    }
}