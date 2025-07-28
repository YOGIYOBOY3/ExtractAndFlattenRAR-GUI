using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ExtractAndFlattenRAR
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourcePathBox.Text = dialog.SelectedPath;
            }
        }

        private void BrowseDest_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DestPathBox.Text = dialog.SelectedPath;
            }
        }

        private void StartExtract_Click(object sender, RoutedEventArgs e)
        {
            string source = SourcePathBox.Text;
            string dest = DestPathBox.Text;
            bool flatten = FlattenCheck.IsChecked == true;

            var rarFiles = Directory.GetFiles(source, ""*.rar"");
            ProgressBar.Maximum = rarFiles.Length;
            ProgressBar.Value = 0;

            foreach (var rar in rarFiles)
            {
                string archiveName = Path.GetFileNameWithoutExtension(rar);
                string archiveFolder = Path.Combine(dest, archiveName);

                Directory.CreateDirectory(archiveFolder);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ""C:\Program Files\7-Zip\7z.exe"",
                    Arguments = $""x \\""{rar}\\"" -o\\"" {archiveFolder}\\"" -y"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                process.WaitForExit();

                if (flatten)
                {
                    var files = Directory.GetFiles(archiveFolder, ""*.*"", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        string destFile = Path.Combine(archiveFolder, Path.GetFileName(file));
                        if (file != destFile)
                        {
                            File.Move(file, destFile, true);
                        }
                    }

                    foreach (var dir in Directory.GetDirectories(archiveFolder, ""*"", SearchOption.AllDirectories))
                    {
                        if (Directory.Exists(dir)) Directory.Delete(dir, true);
                    }
                }

                ProgressBar.Value += 1;
            }

            MessageBox.Show(""? Done! All .rar files extracted."");
        }
    }
}
