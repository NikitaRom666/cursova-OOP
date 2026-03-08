using System.Windows;
using System.Windows.Controls;
using NoteManager.Controllers;
using NoteManager.Models;
namespace NoteManager.Views {
    public partial class MainWindow : Window {
        private NoteController _controller = new NoteController();
        private Note? _selectedNote;
        public MainWindow() {
            InitializeComponent(); _controller.DataChanged += RefreshList; RefreshList();
        }
        private void RefreshList() { NotesList.ItemsSource = null; NotesList.ItemsSource = _controller.GetAllNotes(); }
        private void SaveNoteButton_Click(object sender, RoutedEventArgs e) { _controller.CreateNote(TitleBox.Text, ContentBox.Text, TagsBox.Text); ClearFields(); }
        private void UpdateNoteButton_Click(object sender, RoutedEventArgs e) {
            if (_selectedNote != null) { _controller.UpdateNote(_selectedNote, TitleBox.Text, ContentBox.Text, TagsBox.Text); MessageBox.Show("Зміни збережено!"); }
        }
        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e) {
            if (_selectedNote != null && MessageBox.Show("Видалити нотатку?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                _controller.DeleteNote(_selectedNote); ClearFields();
            }
        }
        private void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            _selectedNote = NotesList.SelectedItem as Note;
            if (_selectedNote != null) {
                TitleBox.Text = _selectedNote.Title; ContentBox.Text = _selectedNote.Content; TagsBox.Text = string.Join(", ", _selectedNote.Tags);
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e) { NotesList.ItemsSource = _controller.SearchNotes(SearchBox.Text); }
        private void SortTitleButton_Click(object sender, RoutedEventArgs e) => _controller.SetSorting(new SortByTitle());
        private void SortDateButton_Click(object sender, RoutedEventArgs e) => _controller.SetSorting(new SortByDate());
        private void ClearFields() { TitleBox.Clear(); ContentBox.Clear(); TagsBox.Clear(); _selectedNote = null; NotesList.SelectedItem = null; }
    }
}