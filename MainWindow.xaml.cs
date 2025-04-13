using AutoSorter.Resources.Scripts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using winForms = System.Windows.Forms;

namespace AutoSorterExtended
{
    public partial class MainWindow : Window
    {
        private FileViewer fileViewer;
        private string selectedFolderPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            fileViewer = new FileViewer();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new winForms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == winForms.DialogResult.OK)
                {
                    selectedFolderPath = dialog.SelectedPath;
                    SelectedFolderTextBox.Text = selectedFolderPath;
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedFolderPath))
            {
                MessageBox.Show("Выберите папку для поиска файлов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsTextBoxEmpty(FileNamePatternTextBox, "Шаблон наименования файла не может быть пустым.") ||
                IsTextBoxEmpty(TimeDifferenceTextBox, "Вы не ввели разницу во времени для вывода нужных файлов.") ||
                !IsValidNumber(TimeDifferenceTextBox, "Разница во времени должна быть числом."))
            {
                return;
            }

            List<string> files = fileViewer.GetFilePairsWithTimeDifferenceByPattern(selectedFolderPath,FileNamePatternTextBox.Text,int.Parse(TimeDifferenceTextBox.Text));

            ResultsTextBox.Text = files.Count > 0
                ? string.Join(Environment.NewLine, files)
                : "Файлы не найдены.";
        }

        private bool IsTextBoxEmpty(TextBox textBox, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            return false;
        }

        private bool IsValidNumber(TextBox textBox, string errorMessage)
        {
            if (!int.TryParse(textBox.Text, out _))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
