# AutoInteractiveTool - Excel HTTP POST Sender

A cross-platform .NET MAUI application that reads Excel files and sends each row's data via HTTP POST requests to a specified API endpoint.

## Features

- **Cross-Platform Support**: Runs on Windows, macOS, Android, and iOS
- **Excel File Processing**: Supports both .xlsx and .xls file formats
- **HTTP POST Integration**: Sends data to any REST API endpoint with Bearer token authentication
- **Progress Tracking**: Real-time progress bar and detailed logging
- **Input Validation**: Validates URLs, file types, and required fields
- **Error Handling**: Comprehensive error handling for file operations and HTTP requests

## Requirements

- .NET 9.0 or higher
- Visual Studio 2022 or Visual Studio Code with .NET MAUI workload

## Getting Started

### 1. Build and Run
```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run on specific platform
dotnet build -f net9.0-android
dotnet build -f net9.0-ios
dotnet build -f net9.0-maccatalyst
dotnet build -f net9.0-windows10.0.19041.0
```

### 2. Using the Application

1. **Configure API Settings**:
   - Enter the target API URL (e.g., `https://api.example.com/data`)
   - Provide a valid Bearer token for authentication

2. **Select Excel File**:
   - Click "Select Excel File" button
   - Choose a .xlsx or .xls file from your device
   - The app will validate the file format

3. **Process Data**:
   - Click "Start Processing" to begin sending data
   - Monitor progress through the progress bar and log
   - View detailed results for each row processed

### 3. File Format Requirements

The Excel file should contain text data where:
- Each row represents one data entry to be sent
- All columns in a row are concatenated with spaces
- Empty rows are skipped
- The first worksheet is used for processing

## Architecture

### Services

- **IExcelProcessor**: Handles Excel file reading and validation
- **IHttpService**: Manages HTTP POST requests with authentication
- **MainPage**: UI logic and user interaction handling

### Key Components

- **EPPlus**: For Excel file processing (non-commercial license)
- **CommunityToolkit.Maui**: For enhanced UI components and file picker
- **HttpClient**: For REST API communication
- **Dependency Injection**: For service management and testability

### Platform-Specific Features

- **Android**: File access permissions configured
- **iOS/macOS**: File type associations for Excel files
- **Windows**: Native file picker integration

## Technical Details

### Data Processing Flow

1. **File Selection**: User selects Excel file via platform file picker
2. **Validation**: File extension validation (.xlsx/.xls)
3. **Reading**: EPPlus reads Excel data row by row
4. **HTTP Requests**: Each row sent as JSON POST request with Bearer auth
5. **Logging**: Real-time status updates and error reporting

### HTTP Request Format

```json
{
  "data": "Row text content concatenated with spaces"
}
```

### Headers
```
Authorization: Bearer {your-token}
Content-Type: application/json
```

## Error Handling

The application handles various error scenarios:

- **Invalid file types**: Clear validation messages
- **Network errors**: Timeout and connection error handling
- **Authentication failures**: HTTP status code reporting
- **File read errors**: Excel file corruption or access issues
- **Missing inputs**: Form validation before processing

## Security Considerations

- Bearer tokens are marked as password fields (masked input)
- HTTP requests use proper authentication headers
- File access follows platform security guidelines
- No sensitive data is logged or persisted

## Dependencies

```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.0" />
<PackageReference Include="EPPlus" Version="7.0.0" />
<PackageReference Include="CommunityToolkit.Maui" Version="7.0.1" />
<PackageReference Include="Microsoft.Maui.Essentials" Version="$(MauiVersion)" />
```

## License

This project uses EPPlus under the non-commercial license. For commercial use, please ensure you have the appropriate EPPlus license.

## Troubleshooting

### Common Issues

1. **File picker not opening**: Check platform-specific permissions
2. **Excel file not reading**: Ensure file is not corrupted or password-protected
3. **HTTP requests failing**: Verify URL format and network connectivity
4. **Authentication errors**: Check Bearer token validity

### Platform-Specific Notes

- **Android**: Requires storage permissions for file access
- **iOS**: May require user permission for file access on first use
- **Windows**: File picker has native integration
- **macOS**: Similar to iOS with permission requirements

## Contributing

This is a complete, functional .NET MAUI application ready for use or further customization based on specific requirements.