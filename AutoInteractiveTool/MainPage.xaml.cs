using CommunityToolkit.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoInteractiveTool
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private readonly IExcelProcessor _excelProcessor;
        private readonly IHttpService _httpService;
        private string? _selectedFilePath;
        private bool _isProcessing;
        private StringBuilder _logBuilder;

        // UI element references
        private Label? _selectedFileLabel;
        private Label? _fileValidationLabel;
        private Button? _processButton;
        private ProgressBar? _processProgressBar;
        private Label? _progressLabel;
        private Label? _logLabel;
        private VerticalStackLayout? _progressSection;

        // Properties for data binding
        private string _url = "";
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
                ValidateInputs();
            }
        }

        private string _bearerToken = "";
        public string BearerToken
        {
            get => _bearerToken;
            set
            {
                _bearerToken = value;
                OnPropertyChanged();
                ValidateInputs();
            }
        }

        public MainPage(IExcelProcessor excelProcessor, IHttpService httpService)
        {
            InitializeComponent();
            _excelProcessor = excelProcessor;
            _httpService = httpService;
            _logBuilder = new StringBuilder();
            
            BindingContext = this;
            
            // Get UI element references
            InitializeUIReferences();
            
            // Initialize UI state
            ValidateInputs();
        }

        private void InitializeUIReferences()
        {
            _selectedFileLabel = this.FindByName<Label>("SelectedFileLabel");
            _fileValidationLabel = this.FindByName<Label>("FileValidationLabel");
            _processButton = this.FindByName<Button>("ProcessButton");
            _processProgressBar = this.FindByName<ProgressBar>("ProcessProgressBar");
            _progressLabel = this.FindByName<Label>("ProgressLabel");
            _logLabel = this.FindByName<Label>("LogLabel");
            _progressSection = this.FindByName<VerticalStackLayout>("ProgressSection");
        }

        /// <summary>
        /// Handles file selection button click
        /// </summary>
        private async void OnSelectFileClicked(object? sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } },
                        { DevicePlatform.Android, new[] { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                        { DevicePlatform.WinUI, new[] { ".xls", ".xlsx" } },
                        { DevicePlatform.macOS, new[] { "xls", "xlsx" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Select an Excel file",
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    // Validate file extension
                    if (_excelProcessor.IsValidExcelFile(result.FileName))
                    {
                        _selectedFilePath = result.FullPath;
                        if (_selectedFileLabel != null)
                        {
                            _selectedFileLabel.Text = $"✅ Selected: {result.FileName}";
                            _selectedFileLabel.TextColor = Color.FromArgb("#107C10"); // Success color
                        }
                        if (_fileValidationLabel != null)
                            _fileValidationLabel.IsVisible = false;
                        
                        AddToLog($"File selected: {result.FileName}");
                    }
                    else
                    {
                        _selectedFilePath = null;
                        if (_selectedFileLabel != null)
                        {
                            _selectedFileLabel.Text = "❌ Invalid file type";
                            _selectedFileLabel.TextColor = Color.FromArgb("#D13438"); // Error color
                        }
                        if (_fileValidationLabel != null)
                        {
                            _fileValidationLabel.Text = "Please select a valid Excel file (.xlsx or .xls)";
                            _fileValidationLabel.IsVisible = true;
                        }
                        
                        AddToLog("Invalid file type selected. Please choose an Excel file (.xlsx or .xls)");
                    }
                }
                
                ValidateInputs();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error selecting file: {ex.Message}", "OK");
                AddToLog($"Error selecting file: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles process button click
        /// </summary>
        private async void OnProcessClicked(object? sender, EventArgs e)
        {
            if (_isProcessing)
                return;

            try
            {
                _isProcessing = true;
                if (_processButton != null)
                {
                    _processButton.IsEnabled = false;
                    _processButton.Text = "⏳ Processing...";
                }
                if (_progressSection != null)
                    _progressSection.IsVisible = true;

                AddToLog("Starting processing...");

                // Read Excel file
                AddToLog($"Reading Excel file: {Path.GetFileName(_selectedFilePath!)}");
                var textData = await _excelProcessor.ReadExcelFileAsync(_selectedFilePath!);
                
                if (textData.Count == 0)
                {
                    AddToLog("No data found in Excel file.");
                    return;
                }

                AddToLog($"Found {textData.Count} rows to process");

                // Process each row
                int successCount = 0;
                int errorCount = 0;

                for (int i = 0; i < textData.Count; i++)
                {
                    var rowData = textData[i];
                    var progress = (double)(i + 1) / textData.Count;
                    
                    // Update progress
                    if (_processProgressBar != null)
                        _processProgressBar.Progress = progress;
                    if (_progressLabel != null)
                        _progressLabel.Text = $"Processing row {i + 1} of {textData.Count}";

                    AddToLog($"Processing row {i + 1}: {(rowData.Length > 50 ? rowData.Substring(0, 50) + "..." : rowData)}");

                    // Send HTTP request
                    var result = await _httpService.SendPostRequestAsync(Url, BearerToken, rowData);
                    
                    if (result.IsSuccess)
                    {
                        successCount++;
                        AddToLog($"✅ Row {i + 1} sent successfully (Status: {result.StatusCode})");
                    }
                    else
                    {
                        errorCount++;
                        AddToLog($"❌ Row {i + 1} failed: {result.Message}");
                    }

                    // Small delay to prevent overwhelming the server
                    await Task.Delay(100);
                }

                AddToLog($"\n🎉 Processing completed!");
                AddToLog($"✅ Success: {successCount}, ❌ Errors: {errorCount}");
                
                await DisplayAlert("Processing Complete", 
                    $"Processing finished!\n\n✅ Successful: {successCount}\n❌ Errors: {errorCount}", "OK");
            }
            catch (Exception ex)
            {
                AddToLog($"❌ Error during processing: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred during processing: {ex.Message}", "OK");
            }
            finally
            {
                _isProcessing = false;
                if (_processButton != null)
                {
                    _processButton.IsEnabled = true;
                    _processButton.Text = "▶️ Start Processing";
                }
                if (_progressSection != null)
                    _progressSection.IsVisible = false;
                if (_processProgressBar != null)
                    _processProgressBar.Progress = 0;
                
                ValidateInputs();
            }
        }

        /// <summary>
        /// Handles clear log button click
        /// </summary>
        private void OnClearLogClicked(object? sender, EventArgs e)
        {
            _logBuilder.Clear();
            if (_logLabel != null)
                _logLabel.Text = "Ready to process...";
        }

        /// <summary>
        /// Validates all inputs and enables/disables the process button
        /// </summary>
        private void ValidateInputs()
        {
            var isValid = !string.IsNullOrWhiteSpace(Url) &&
                         !string.IsNullOrWhiteSpace(BearerToken) &&
                         !string.IsNullOrWhiteSpace(_selectedFilePath) &&
                         !_isProcessing;

            if (_processButton != null)
                _processButton.IsEnabled = isValid;
        }

        /// <summary>
        /// Adds a message to the log
        /// </summary>
        private void AddToLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _logBuilder.AppendLine($"[{timestamp}] {message}");
            if (_logLabel != null)
                _logLabel.Text = _logBuilder.ToString();
        }

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
