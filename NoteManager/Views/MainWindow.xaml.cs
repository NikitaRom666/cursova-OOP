using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NoteManager.Models;
using NoteManager.Controllers;

namespace NoteManager
{
    public partial class MainWindow : Window
    {
        private NoteController _controller = new NoteController();
        private ISortStrategy _currentStrategy = new SortByDate();
        private string _searchQuery = "";

        public MainWindow()
        {
            InitializeComponent();
            _controller.DataChanged += RefreshList;
            RefreshList();
        }

        private void RefreshList()
        {
            var notes = _controller.GetNotes(_searchQuery, _currentStrategy).ToList();
            NotesList.ItemsSource = notes;
        }

        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var tags = TagsInput.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(t => t.Trim()).ToList();
            _controller.CreateNewNote(TitleInput.Text, ContentInput.Text, tags);
            ClearInputs();
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is Note selectedNote)
            {
                var tags = TagsInput.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(t => t.Trim()).ToList();
                _controller.UpdateNote(selectedNote, TitleInput.Text, ContentInput.Text, tags);
            }
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is Note selectedNote)
            {
                _controller.DeleteNote(selectedNote);
                ClearInputs();
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is Note selectedNote)
            {
                _controller.UndoChanges(selectedNote);
                UpdateInputs(selectedNote);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _searchQuery = SearchInput.Text;
            RefreshList();
        }

        private void SortTitleButton_Click(object sender, RoutedEventArgs e)
        {
            _currentStrategy = new SortByTitle();
            RefreshList();
        }

        private void SortDateButton_Click(object sender, RoutedEventArgs e)
        {
            _currentStrategy = new SortByDate();
            RefreshList();
        }

        private void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedItem is Note selectedNote)
            {
                UpdateInputs(selectedNote);
            }
        }

        private void UpdateInputs(Note note)
        {
            TitleInput.Text = note.Title;
            ContentInput.Text = note.Content;
            TagsInput.Text = string.Join(", ", note.Tags);
        }

        private void ClearInputs()
        {
            TitleInput.Text = "";
            ContentInput.Text = "";
            TagsInput.Text = "";
        }
    }
}
