using Microsoft.Win32;
using System;
using System.Diagnostics; // Добавлено для работы с системными логами
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _12._12_СП
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                string content = File.ReadAllText(currentFilePath);
                txtFileContent.Text = content;
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Поиск...")
            {
                txtSearch.Text = "";
                txtSearch.Opacity = 1;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск...";
                txtSearch.Opacity = 0.5;
            }
        }

        private void txtFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtFilter.Text == "Фильтр...")
            {
                txtFilter.Text = "";
                txtFilter.Opacity = 1;
            }
        }

        private void txtFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                txtFilter.Text = "Фильтр...";
                txtFilter.Opacity = 0.5;
            }
        }

        private void btnCreateFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.Create(saveFileDialog.FileName).Close();
                currentFilePath = saveFileDialog.FileName;
            }
        }

        private void btnDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("Сначала откройте файл для удаления.");
                return;
            }

            try
            {
                File.Delete(currentFilePath);
                MessageBox.Show("Файл успешно удален.");
                txtFileContent.Clear();
                currentFilePath = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении файла: {ex.Message}");
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("Сначала откройте файл для сохранения изменений.");
                return;
            }

            try
            {
                File.WriteAllText(currentFilePath, txtFileContent.Text);
                MessageBox.Show("Файл успешно сохранен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                int index = txtFileContent.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    txtFileContent.Select(index, searchText.Length);
                    txtFileContent.Focus();
                }
                else
                {
                    MessageBox.Show("Текст не найден.");
                }
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string filterText = txtFilter.Text;
            if (!string.IsNullOrEmpty(filterText) && filterText != "Фильтр...")
            {
                var lines = txtFileContent.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var filteredLines = lines.Where(line => line.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                txtFileContent.Text = string.Join(Environment.NewLine, filteredLines);
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            var lines = txtFileContent.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var sortedLines = lines.OrderBy(line => line).ToArray();
            txtFileContent.Text = string.Join(Environment.NewLine, sortedLines);
        }

        private void btnRefreshLogs_Click(object sender, RoutedEventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            listBoxLogs.Items.Clear();
            try
            {
                EventLog eventLog = new EventLog("Application");
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    listBoxLogs.Items.Add($"{entry.TimeGenerated}: {entry.Source} - {entry.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке логов: {ex.Message}");
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, txtFileContent.Text);
                MessageBox.Show("Данные успешно экспортированы.");
            }
        }
    }
}