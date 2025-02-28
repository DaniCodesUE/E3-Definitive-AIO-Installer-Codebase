using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using MessageBox = System.Windows.MessageBox;
using System.Net.Http;
using System.Diagnostics;
using System.IO.Compression;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Archives;
using Path = System.IO.Path;
using System.Threading;
using SevenZipExtractor;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.Win32.SafeHandles;
using System.Text.RegularExpressions;
using System.Globalization;
using Application = System.Windows.Application;
using System.Windows.Threading;

namespace E3_Definitive_Mod_Demo_Launcher
{
    public partial class DyingLight2Downgrader : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, IntPtr lpPipeAttributes, uint nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetHandleInformation(IntPtr hObject, int dwMask, int dwFlags);

        private const int HANDLE_FLAG_INHERIT = 1;

        private const string DemoRarUrl = "https://www.dropbox.com/scl/fi/akzb0off1czgamcd8736e/DEMO.rar?rlkey=ff853rwz8qlg1r15nk6aa4zds&st=a4azct0y&dl=1";
        private const string AudioFixUrl = "https://www.dropbox.com/scl/fi/woyx6zsgd2xqdcclvvcxo/AudioFixDl2_1.12.1.7z?rlkey=ht3mqnxxcsq9raqdgkk46xru5&st=njal8sp2&dl=1";
        private const string SteamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
        private DispatcherTimer logMonitorTimer;
        private bool installationStartedLogged = false; // Prevents duplicate logs


        public DyingLight2Downgrader()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            System.Windows.Controls.ProgressBar progressBar = this.ProgressBar;
            InitializeComponent();
            StartLogMonitor();
        }

        private HashSet<string> processedLogs = new HashSet<string>(); 

        private void StartLogMonitor()
        {
            logMonitorTimer = new DispatcherTimer();
            logMonitorTimer.Interval = TimeSpan.FromMilliseconds(100);
            logMonitorTimer.Tick += CheckLogForTrigger;
            logMonitorTimer.Start();
        }

        private void CheckLogForTrigger(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string[] logLines = Log.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string line in logLines)
                {
                    if (line.Contains("Waiting for user info") && !processedLogs.Contains(line))
                    {
                        AppendLog("Depot Installation Started successfully.");
                        processedLogs.Add(line); 
                    }
                }
            });
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = sender as System.Windows.Controls.TextBox;

            if (textbox != null && textbox.Tag == null)
            {
                textbox.Text = string.Empty;
                textbox.Tag = "clicked";
            }
        }

        private string GetSelectedAuthMethod()
        {
            if (ComboBox_AuthType.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
            }
            return "Steam Email Code"; 
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string steamCmdPath = Path.Combine(Directory.GetCurrentDirectory(), "steamcmd", "steamcmd.exe");
            string steamCmdContentPath = Path.Combine(Directory.GetCurrentDirectory(), "steamcmd", "steamapps", "content", "app_534380");
            string username = UserBox.Text;
            string password = PassBox.Password;
            string installDir = InstallLink.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(installDir))
            {
                MessageBox.Show("Please make sure you've filled out all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            bool isSteamGuardEnabled = SteamGuardCheckbox.IsChecked ?? true;
            string authMethod = GetSelectedAuthMethod();

            AppendLog($"SteamGuardCheckbox.IsChecked: {isSteamGuardEnabled}, Selected Auth Method: {authMethod}");

            try
            {
                string appId = "534380";
                (string depotId, string manifestId)[] depotsAndManifests =
                {
            ("534381", "8397059556255747146"),
            ("534382", "2610088083322243488"),
            ("534383", "6553645018477200440"),
            ("534384", "3809152703080297962"),
            ("534385", "8328607143729447236")
        };

                bool depotsReady = AreDepotsReady(steamCmdContentPath, installDir, depotsAndManifests);
                AppendLog($"Checking depots... {(depotsReady ? "Ready. Proceeding to cleanup and extraction." : "Not ready. Starting download.")}");

                if (depotsReady)
                {
                    DeleteDepotFolders(steamCmdContentPath);
                    await DownloadAndExtractDemo(installDir);
                    AppendLog("Demo setup complete.");
                    return;
                }

                if (!File.Exists(steamCmdPath))
                {
                    AppendLog("SteamCMD not found. Attempting download...");
                    await DownloadAndExtractSteamCMD();
                }

                if (!File.Exists(steamCmdPath))
                {
                    MessageBox.Show("Failed to set up SteamCMD.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await InitializeSteamCmdAsync(steamCmdPath);
                AppendLog("Starting SteamCMD login...");

                if (authMethod == "Steam Email Code")
                {
                    string loginCommand = $"steamcmd +login {username} {password}";
                    await RunSteamCMD(loginCommand, true);
                }

                int totalDepots = depotsAndManifests.Length;
                int completedDepots = 0;

                foreach (var (depotId, manifestId) in depotsAndManifests)
                {
                    bool success = false;

              
       
                   success = await Task.Run(() =>
                            RunSteamCmdForDepot(steamCmdPath, username, password, appId, depotId, manifestId, installDir, isSteamGuardEnabled, authMethod));

                    if (success)
                    {
                        string depotPath = Path.Combine(steamCmdContentPath, $"depot_{depotId}");

                        if (Directory.Exists(depotPath))
                        {
                            AppendLog($"Depot {depotId} downloaded successfully. Copying files...");
                            CopyFiles(depotPath, installDir);
                        }
                        else
                        {
                            AppendLog($"Depot {depotId} downloaded but depot folder was not found.");
                        }
                    }
                    else
                    {
                        AppendLog($"Depot {depotId} failed to download. Skipping...");
                    }

                    completedDepots++;
                    UpdateProgressBar(completedDepots, totalDepots);
                }

                AppendLog("All depots processed. Starting cleanup and demo setup...");
                DeleteDepotFolders(steamCmdContentPath);
                await DownloadAndExtractDemo(installDir);
                AppendLog("Demo setup complete.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        

        private string PromptForSteamAppCode()
        {
            string steamAppCode = null;

            Dispatcher.Invoke(() =>
            {
                var steamAppCodeWindow = new SteamGuardWindow();
                if (steamAppCodeWindow.ShowDialog() == true)
                {
                    steamAppCode = steamAppCodeWindow.SteamGuardCode;
                }
            });

            return steamAppCode;
        }

        private void DeleteDepotFolders(string depotPath)
        {
            var deletedFolders = Directory.GetDirectories(depotPath).ToList();
            int deletedCount = 0;

            foreach (var dir in deletedFolders)
            {
                try
                {
                    Directory.Delete(dir, true);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    AppendLog($"Failed to delete {dir}: {ex.Message}");
                }
            }

            AppendLog($"Deleted {deletedCount}/{deletedFolders.Count} depot folders.");
        }

        private async Task InitializeSteamCmdAsync(string steamCmdPath)
        {
            AppendLog("Initializing SteamCMD for auto-update...");

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = steamCmdPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.OutputDataReceived += (sender, args) => AppendLog(args.Data);
                process.ErrorDataReceived += (sender, args) => AppendLog(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await Task.Delay(10000);

                if (!process.HasExited)
                {
                    process.StandardInput.WriteLine("quit");
                    AppendLog("Sent +quit command to SteamCMD.");
                    var exitTask = Task.Run(() => process.WaitForExit());
                    var timeoutTask = Task.Delay(5000);

                    await Task.WhenAny(exitTask, timeoutTask);

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    else
                    {
                        AppendLog("SteamCMD exited gracefully.");
                    }
                }
            }
        }

        private void MergeDirectories(string sourceDir, string targetDir)
        {
            string[] sourceFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

            foreach (var sourceFile in sourceFiles)
            {
                string relativePath = GetRelativePath(sourceDir, sourceFile);
                string targetFile = Path.Combine(targetDir, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                File.Copy(sourceFile, targetFile, true);
            }

            AppendLog($"Merged {sourceFiles.Length} files into {targetDir}.");
        }


        private string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
        private bool AreDepotsReady(string steamCmdContentPath, string installDir, (string depotId, string manifestId)[] depotsAndManifests)
        {
            foreach (var (depotId, _) in depotsAndManifests)
            {
                string depotPath = Path.Combine(steamCmdContentPath, $"depot_{depotId}");
                if (!Directory.Exists(depotPath))
                {
                    AppendLog($"Depot {depotId} is not downloaded.");
                    return false;
                }

                string installDepotPath = Path.Combine(installDir, $"depot_{depotId}");
                if (!Directory.Exists(installDepotPath))
                {
                    AppendLog($"Depot {depotId} is not copied to install directory.");
                    return false;
                }

                if (!AreFilesIdentical(depotPath, installDepotPath))
                {
                    AppendLog($"Files for depot {depotId} are not identical between source and install directories.");
                    return false;
                }
            }

            AppendLog("All depots are downloaded and extracted.");
            return true;
        }

        private bool AreFilesIdentical(string sourceDir, string targetDir)
        {
            var sourceFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            var targetFiles = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);

            var sourceFileSet = new HashSet<string>(sourceFiles.Select(f => GetRelativePath(sourceDir, f)));
            var targetFileSet = new HashSet<string>(targetFiles.Select(f => GetRelativePath(targetDir, f)));

            return sourceFileSet.SetEquals(targetFileSet);
        }


        private void CopyFiles(string sourceDir, string targetDir)
        {
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDir = System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(dir));
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                CopyFiles(dir, destDir);
            }
        }

        private async Task DownloadAndExtractDemo(string installDir)
        {
            string tempRarPath = Path.Combine(installDir, "demo.rar");
            string targetPhPath = Path.Combine(installDir, "ph");

            try
            {
                AppendLog("Downloading demo.rar...");
                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.GetAsync(DemoRarUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var fs = new FileStream(tempRarPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
                AppendLog($"demo.rar downloaded to {tempRarPath}.");

                ExtractWithWinRAR(tempRarPath, installDir);

                string dllPath = Path.Combine(installDir, "ph", "work", "bin", "x64", "nvngx_dlssg.dll");
                string renamedDllPath = Path.Combine(installDir, "ph", "work", "bin", "x64", "nvngx_dlssg.dll1");
                if (File.Exists(dllPath))
                {
                    File.Move(dllPath, renamedDllPath);
                    AppendLog("Renamed nvngx_dlssg.dll to nvngx_dlssg.dll1.");
                }
                else
                {
                    AppendLog("nvngx_dlssg.dll not found. Skipping rename.");
                }

                AppendLog("'ph' folder extracted successfully.");
                ShowCompletionPopup();
            }
            catch (Exception ex)
            {
                AppendLog($"Error during demo.rar processing: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempRarPath)) File.Delete(tempRarPath);
            }
        }


        private async Task ResumeDownload(string url, string filePath)
        {
            long existingLength = 0;

            if (File.Exists(filePath))
                existingLength = new FileInfo(filePath).Length;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        private void ExtractDemoRar(string rarPath, string installDir)
        {
            try
            {
                AppendLog("Extracting 'ph' folder from demo.rar...");
                using (var archive = RarArchive.Open(rarPath))
                {
                    int totalEntries = archive.Entries.Count();
                    int processedEntries = 0;

                    foreach (var entry in archive.Entries.Where(e => !e.IsDirectory && e.Key.StartsWith("DEMO/ph/")))
                    {
                        string relativePath = entry.Key.Substring(5);
                        string destinationPath = Path.Combine(installDir, relativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.WriteToFile(destinationPath, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });

                        processedEntries++;
                        AppendLog($"Extracting: {processedEntries}/{totalEntries}");
                    }
                }
                AppendLog("Extraction complete!");
                ShowCompletionPopup();
            }
            catch (Exception ex)
            {
                AppendLog($"Extraction error: {ex.Message}");
            }
        }

        private void ExtractWithWinRAR(string rarPath, string installDir)
        {
            try
            {
                AppendLog("Extracting using WinRAR...");

                string winrarPath = @"C:\Program Files\WinRAR\WinRAR.exe"; 

                if (!File.Exists(winrarPath))
                {
                    AppendLog("WinRAR not found! Asking user to locate it...");

                    var openFileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Title = "Select WinRAR.exe",
                        Filter = "WinRAR Executable (WinRAR.exe)|WinRAR.exe",
                        InitialDirectory = @"C:\Program Files"
                    };

                    bool? result = openFileDialog.ShowDialog(); 

                    if (result == true)
                    {
                        winrarPath = openFileDialog.FileName;
                        AppendLog($"User selected WinRAR path: {winrarPath}");
                    }
                    else
                    {
                        AppendLog("WinRAR selection was canceled. Extraction aborted.");
                        return;
                    }
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = winrarPath,
                    Arguments = $"x -y \"{rarPath}\" \"{installDir}\\\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    AppendLog("Extraction complete!");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"WinRAR extraction error: {ex.Message}");
            }
        }


        private void ShowCompletionPopup()
        {
            string message = "Installation complete! You can now add 'DyingLightGame_x64_rwdi.exe' to Steam. " +
                             "Follow the custom launch tutorial for further instructions.";
            string caption = "Installation Complete";

            System.Windows.Forms.MessageBox.Show(message, caption,
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }



        private void AppendLog(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Dispatcher.Invoke(() =>
                {
                    Log.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
                });
            }
        }


        private void OverwriteMatchingFiles(string sourceDir, string targetDir)
        {
            foreach (var sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(sourceDir, sourceFile);
                string targetFile = Path.Combine(targetDir, relativePath);

                if (File.Exists(targetFile))
                {
                    File.Copy(sourceFile, targetFile, true);
                    AppendLog($"Overwritten: {relativePath}");
                }
                else
                {
                    AppendLog($"Skipped (no match): {relativePath}");
                }
            }
        }

        private async Task RunSteamCMD(string command, bool isSteamGuardEnabled)
        {
            AppendLog("Initializing SteamCMD process...");

            string steamCmdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamcmd.exe");
            if (!File.Exists(steamCmdPath))
            {
                AppendLog("SteamCMD executable not found.");
                throw new FileNotFoundException("SteamCMD executable not found.");
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = steamCmdPath,
                Arguments = command,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();

                var outputTask = Task.Run(() => ReadStream(process.StandardOutput));
                var errorTask = Task.Run(() => ReadStream(process.StandardError));

                string steamGuardCode = null;
                bool steamGuardSubmitted = false;
                if (isSteamGuardEnabled)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Task.Delay(8000);
                        var steamGuardWindow = new SteamGuardWindow();
                        if (steamGuardWindow.ShowDialog() == true) 
                        {
                            steamGuardCode = steamGuardWindow.SteamGuardCode; 
                            AppendLog($"Steam Guard code captured: {steamGuardCode}");
                        }
                        else
                        {
                            AppendLog("Steam Guard input canceled. Terminating process.");
                            process.Kill(); 
                        }
                    });

                    if (!string.IsNullOrEmpty(steamGuardCode))
                    {
                        process.StandardInput.WriteLine(steamGuardCode);

                        await Task.Delay(8000);

                        process.StandardInput.WriteLine($"+set_steam_guard_code {steamGuardCode}");
                        AppendLog("Steam Guard code submitted and re-sent for verification.");
                    }
                    else
                    {
                        AppendLog("Steam Guard code was empty or not captured. Terminating process.");
                        process.Kill(); 
                    }
                }
            }
        }

        private void ReadStream(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                AppendLog(line);
            }
        }


        private async Task<bool> RunSteamCmdForDepot(string steamCmdPath, string username, string password, string appId, string depotId, string manifestId, string installDir, bool isSteamGuardEnabled, string authMethod)
        {
            try
            {
                string command = $"+force_install_dir \"{installDir}\" +login {username} {password} +download_depot {appId} {depotId} {manifestId} -noupdate +quit";

                AppendLog($"Starting SteamCMD for depot {depotId}...");

                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = steamCmdPath,
                        Arguments = command,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    StringBuilder outputBuilder = new StringBuilder();
                    process.OutputDataReceived += (sender, args) => AppendLog(args.Data);
                    process.ErrorDataReceived += (sender, args) => AppendLog(args.Data);

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (isSteamGuardEnabled == true && authMethod == "Steam App Code")
                    { 
                        await Task.Delay(10000);

                        string steamGuardCode = null;
                        Dispatcher.Invoke(() =>
                        {
                            var steamGuardWindow = new SteamGuardWindow();
                            if (steamGuardWindow.ShowDialog() == true)
                            {
                                steamGuardCode = steamGuardWindow.SteamGuardCode;
                            }
                        });

                        if (!string.IsNullOrEmpty(steamGuardCode))
                        {
                            await Task.Delay(2500);
                            process.StandardInput.WriteLine(steamGuardCode);
                            AppendLog("Steam Guard code submitted.");
                            await Task.Delay(2500);
                        }
                        else
                        {
                            AppendLog("Steam Guard code was empty. Depot download may fail or be incomplete.");
                        }
                    }

                    process.WaitForExit();
                    AppendLog($"SteamCMD for depot {depotId} exited with code {process.ExitCode}");
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error running SteamCMD for depot {depotId}: {ex.Message}");
                return false;
            }
        }

        private async Task DownloadAndExtractSteamCMD()
        {
            try
            {
                string zipPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "steamcmd.zip");
                string extractPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "steamcmd");

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(SteamCmdUrl);
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                        AppendLog("SteamCMD downloaded.");
                    }
                }

                if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);

                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(zipPath);
                AppendLog("SteamCMD extracted.");
            }
            catch (Exception ex)
            {
                AppendLog($"Error downloading or extracting SteamCMD: {ex.Message}");
            }
        }

        private void FindDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    InstallLink.Text = dialog.SelectedPath;
                }
            }
        }

        private void UpdateProgressBar(int completed, int total)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)completed / total * 100;
            });
        }

        private void CloseButt_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming in a future update", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string installDir = InstallLink.Text;

            if (string.IsNullOrWhiteSpace(installDir))
            {
                MessageBox.Show("Please specify the installation directory first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppendLog("Starting debug demo download...");
            await DownloadAndExtractDemo(installDir);
            AppendLog("Debug demo download completed.");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_AuthType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void InstallLink_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string installDir = InstallLink.Text;

            if (string.IsNullOrWhiteSpace(installDir))
            {
                MessageBox.Show("Please specify the installation directory first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppendLog("Starting debug demo download...");
            DownloadAndExtractDemo(installDir);
        }
    }
}
