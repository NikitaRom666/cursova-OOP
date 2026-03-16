using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NoteManager.Controllers;
using NoteManager.Models;
using NoteManager.Services;
using NoteManager.Services.Strategies;

namespace NoteManager
{
    public partial class MainWindow : Window
    {
        private readonly NoteController _controller = new NoteController();
        private ISortStrategy _currentStrategy = new SortByDate();
        private string _searchQuery = "";
        private Note? _selectedNote;
        private bool _isDarkTheme = false;

        private static readonly string[] ChipColors = new[]
        {
            "#3498DB","#2ECC71","#E74C3C","#9B59B6",
            "#F39C12","#1ABC9C","#E67E22","#E91E63"
        };

        public MainWindow()
        {
            InitializeComponent();
            _controller.DataChanged += RefreshList;

            TitleInput.TextChanged += (s, ev) =>
            {
                if (_selectedNote != null && !Title.EndsWith("*"))
                    Title = "Note Manager Pro *";
            };

            ContentInput.TextChanged += (s, ev) =>
            {
                if (_selectedNote != null && !Title.EndsWith("*"))
                    Title = "Note Manager Pro *";
                var txt   = ContentInput.Text;
                var chars = txt.Length;
                var words = string.IsNullOrWhiteSpace(txt) ? 0 :
                    txt.Split(new char[]{' ','\n','\r','\t'},
                        StringSplitOptions.RemoveEmptyEntries).Length;
                CharCounter.Text = chars + " \u0441\u0438\u043c\u0432\u043e\u043b\u0456\u0432 | " + words + " \u0441\u043b\u0456\u0432";
            };

            RefreshList();
        }

        private void RefreshList()
        {
            bool filterByCategory = FilterCategoryBox?.SelectedIndex > 0;
            var selectedCategory  = (FilterCategoryBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var notes = _controller.GetNotes(_searchQuery, _currentStrategy)
                .OrderByDescending(n => n.IsPinned)
                .ToList();

            if (filterByCategory && !string.IsNullOrEmpty(selectedCategory))
                notes = notes.Where(n => n.Category == selectedCategory).ToList();

            NotesList.ItemsSource = null;
            NotesList.ItemsSource = notes;

            RefreshTagChipsPanel();

            var allN    = _controller.GetNotes("", new SortByTitle()).ToList();
            var pinnedN = allN.Count(n => n.IsPinned);
            var tagsN   = allN.SelectMany(n => n.Tags).Distinct().Count();
            StatusText.Text = "\u041d\u043e\u0442\u0430\u0442\u043e\u043a: " + allN.Count +
                              "  |  \u0417\u0430\u043a\u0440\u0456\u043f\u043b\u0435\u043d\u043e: " + pinnedN +
                              "  |  \u0422\u0435\u0433\u0456\u0432: " + tagsN +
                              "  |  " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void RefreshTagChipsPanel()
        {
            if (AllTagsPanel == null) return;
            AllTagsPanel.Children.Clear();
            var allTags = _controller.GetNotes("", new SortByTitle())
                .SelectMany(n => n.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();
            for (int i = 0; i < allTags.Count; i++)
            {
                var tag   = allTags[i];
                var color = ChipColors[i % ChipColors.Length];
                var chip  = new Border
                {
                    Background   = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(color)!),
                    CornerRadius = new CornerRadius(10),
                    Padding      = new Thickness(8, 3, 8, 3),
                    Margin       = new Thickness(0, 0, 4, 4),
                    Cursor       = Cursors.Hand
                };
                chip.Child = new TextBlock { Text = tag, Foreground = Brushes.White, FontSize = 10 };
                var t2 = tag;
                chip.MouseLeftButtonUp += (s, e) =>
                {
                    SearchInput.Text = t2;
                    _searchQuery     = t2;
                    FilterCategoryBox.SelectedIndex = 0;
                    var filtered = _controller.GetNotes("", new SortByTitle())
                        .Where(n => n.Tags.Contains(t2, StringComparer.OrdinalIgnoreCase))
                        .OrderByDescending(n => n.IsPinned).ToList();
                    NotesList.ItemsSource = null;
                    NotesList.ItemsSource = filtered;
                };
                AllTagsPanel.Children.Add(chip);
            }
        }

        private void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedItem is Note note)
            {
                _selectedNote     = note;
                TitleInput.Text   = note.Title;
                ContentInput.Text = note.Content;
                TagsInput.Text    = string.Join(", ", note.Tags);
                CategoryBox.Text  = note.Category ?? "\u0417\u0430\u0433\u0430\u043b\u044c\u043d\u0435";
                RenderNoteTagChips(note.Tags.ToList());
                Title = "Note Manager Pro";
            }
        }

        private void RenderNoteTagChips(List<string> tags)
        {
            TagChipsPanel.Children.Clear();
            for (int i = 0; i < tags.Count; i++)
            {
                var tag   = tags[i];
                var color = ChipColors[i % ChipColors.Length];
                var chip  = new Border
                {
                    Background   = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(color)!),
                    CornerRadius = new CornerRadius(12),
                    Padding      = new Thickness(10, 4, 10, 4),
                    Margin       = new Thickness(0, 0, 6, 4),
                    Cursor       = Cursors.Hand
                };
                chip.Child = new TextBlock { Text = tag, Foreground = Brushes.White, FontSize = 11, FontWeight = FontWeights.SemiBold };
                var t2 = tag;
                chip.MouseLeftButtonUp += (s, e) =>
                {
                    SearchInput.Text            = t2;
                    _searchQuery                = t2;
                    FilterCategoryBox.SelectedIndex = 0;
                    RefreshList();
                };
                TagChipsPanel.Children.Add(chip);
            }
        }

        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedNote          = null;
            NotesList.SelectedItem = null;
            TitleInput.Text        = "";
            ContentInput.Text      = "";
            TagsInput.Text         = "";
            CategoryBox.SelectedIndex = 0;
            TagChipsPanel.Children.Clear();
            Title = "Note Manager Pro";
            TitleInput.Focus();
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                MessageBox.Show(
                    "\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u043d\u0430\u0437\u0432\u0443 \u043d\u043e\u0442\u0430\u0442\u043a\u0438.",
                    "\u0423\u0432\u0430\u0433\u0430",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tags = TagsInput.Text
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            var category = (CategoryBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                           ?? "\u0417\u0430\u0433\u0430\u043b\u044c\u043d\u0435";

            if (_selectedNote == null)
            {
                var note = NoteFactory.CreateWithTags(TitleInput.Text, ContentInput.Text, tags);
                note.Category = category;
                _controller.CreateNewNote(TitleInput.Text, ContentInput.Text, tags);
                // Find the created note and set its category
                var allNotes = _controller.GetNotes("", new SortByTitle()).ToList();
                _selectedNote = allNotes.FirstOrDefault(n => n.Title == TitleInput.Text);
                if (_selectedNote != null)
                {
                    _selectedNote.Category = category;
                    NoteStore.Instance.Save();
                }
                RefreshList();
            }
            else
            {
                
                _controller.UpdateNote(_selectedNote, TitleInput.Text, ContentInput.Text, tags);
                _selectedNote.Category = category;
                NoteStore.Instance.Save();
                RenderNoteTagChips(tags);
            }

            Title = "Note Manager Pro";
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is not Note note) return;

            var result = MessageBox.Show(
                "\u0412\u0438\u0434\u0430\u043b\u0438\u0442\u0438 \u043d\u043e\u0442\u0430\u0442\u043a\u0443 \u00ab" + note.Title + "\u00bb?",
                "\u041f\u0456\u0434\u0442\u0432\u0435\u0440\u0434\u0436\u0435\u043d\u043d\u044f",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            _controller.DeleteNote(note);
            _selectedNote = null;
            TitleInput.Text   = "";
            ContentInput.Text = "";
            TagsInput.Text    = "";
            TagChipsPanel.Children.Clear();
            Title = "Note Manager Pro";
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is Note note)
            {
                _controller.UndoChanges(note);
                TitleInput.Text   = note.Title;
                ContentInput.Text = note.Content;
                TagsInput.Text    = string.Join(", ", note.Tags);
                CategoryBox.Text  = note.Category ?? "\u0417\u0430\u0433\u0430\u043b\u044c\u043d\u0435";
                RenderNoteTagChips(note.Tags.ToList());
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesList.SelectedItem is not Note note)
            {
                MessageBox.Show(
                    "\u0412\u0438\u0431\u0435\u0440\u0456\u0442\u044c \u043d\u043e\u0442\u0430\u0442\u043a\u0443.",
                    "\u0423\u0432\u0430\u0433\u0430",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            note.IsPinned = !note.IsPinned;
            NoteStore.Instance.Save();
            RefreshList();
            NotesList.SelectedItem = NotesList.Items.Cast<Note>().FirstOrDefault(n => n.Id == note.Id);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedNote == null)
            {
                MessageBox.Show(
                    "\u0412\u0438\u0431\u0435\u0440\u0456\u0442\u044c \u043d\u043e\u0442\u0430\u0442\u043a\u0443.",
                    "\u0423\u0432\u0430\u0433\u0430",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var history = _controller.GetHistory(_selectedNote);
            if (history.Count == 0)
            {
                MessageBox.Show(
                    "\u0406\u0441\u0442\u043e\u0440\u0456\u044f \u043f\u043e\u0440\u043e\u0436\u043d\u044f. \u0417\u0431\u0435\u0440\u0435\u0436\u0456\u0442\u044c \u043d\u043e\u0442\u0430\u0442\u043a\u0443 \u043a\u0456\u043b\u044c\u043a\u0430 \u0440\u0430\u0437\u0456\u0432.",
                    "\u0406\u0441\u0442\u043e\u0440\u0456\u044f",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\u0406\u0441\u0442\u043e\u0440\u0456\u044f: " + _selectedNote.Title);
            sb.AppendLine(new string('=', 40));
            for (int i = 0; i < history.Count; i++)
            {
                var m = history[i];
                sb.AppendLine("\n\u0412\u0435\u0440\u0441\u0456\u044f " + (history.Count - i) + " - " + m.SavedAt.ToString("dd.MM.yyyy HH:mm:ss"));
                sb.AppendLine("\u041d\u0430\u0437\u0432\u0430: " + m.Title);
                sb.AppendLine("\u0422\u0435\u0433\u0438: " + string.Join(", ", m.Tags));
                var preview = m.Content.Length > 80 ? m.Content.Substring(0, 80) + "..." : m.Content;
                sb.AppendLine("\u0417\u043c\u0456\u0441\u0442: " + preview);
                sb.AppendLine(new string('-', 40));
            }
            MessageBox.Show(sb.ToString(),
                "\u0406\u0441\u0442\u043e\u0440\u0456\u044f - " + history.Count + " \u0432\u0435\u0440\u0441\u0456\u0439",
                MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var notes = _controller.GetNotes("", new SortByTitle()).ToList();
            if (notes.Count == 0)
            {
                MessageBox.Show(
                    "\u041d\u0435\u043c\u0430\u0454 \u043d\u043e\u0442\u0430\u0442\u043e\u043a \u0434\u043b\u044f \u0435\u043a\u0441\u043f\u043e\u0440\u0442\u0443.",
                    "\u0415\u043a\u0441\u043f\u043e\u0440\u0442",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var lines = new System.Text.StringBuilder();
            lines.AppendLine("=== NOTE MANAGER PRO ===");
            lines.AppendLine("Date: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            lines.AppendLine("Total: " + notes.Count);
            lines.AppendLine(new string('=', 40));
            foreach (var n in notes)
            {
                lines.AppendLine();
                lines.AppendLine((n.IsPinned ? "[PIN] " : "") + n.Title);
                lines.AppendLine("Category: " + n.Category);
                lines.AppendLine("Date: " + n.CreatedAt.ToString("dd.MM.yyyy HH:mm"));
                if (n.Tags.Count > 0)
                    lines.AppendLine("Tags: " + string.Join(", ", n.Tags));
                lines.AppendLine(n.Content);
                lines.AppendLine(new string('-', 40));
            }
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notes_export.txt");
            System.IO.File.WriteAllText(path, lines.ToString(), System.Text.Encoding.UTF8);
            MessageBox.Show("OK: " + path, "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            System.Diagnostics.Process.Start("notepad.exe", path);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _searchQuery = SearchInput.Text;
            FilterCategoryBox.SelectedIndex = 0;
            RefreshList();
        }

        private void SortTitleButton_Click(object sender, RoutedEventArgs e)
        {
            _currentStrategy = new SortByTitle();
            _searchQuery     = "";
            SearchInput.Text = "";
            RefreshList();
        }

        private void SortDateButton_Click(object sender, RoutedEventArgs e)
        {
            _currentStrategy = new SortByDate();
            _searchQuery     = "";
            SearchInput.Text = "";
            RefreshList();
        }

        private void FilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _searchQuery     = "";
            SearchInput.Text = "";
            RefreshList();
        }

        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            var bg = _isDarkTheme ? "#1E1E2E" : "#F0F2F5";
            Background = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(bg)!);
        }
    }
}

