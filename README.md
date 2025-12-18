# HeicToJPEG

HeicToJPEG is a comprehensive Windows application suite that provides easy conversion of HEIC images to JPEG format. The project uses the [Magick.NET](https://github.com/dlemstra/Magick.NET) library for image processing and supports three different user interfaces: a GUI application, command-line tool, and Windows service.

## Project Overview

This is a C# solution consisting of four main components:

1. **ImageConverter** - Core conversion library
2. **HeicToJPEG** - Windows Forms GUI application
3. **HeicToJPEG-cmd** - Command-line utility
4. **HeicToJPEG-service** - Windows service for batch/automated conversion
5. **Installer** - NSIS-based Windows installer

## Architecture

### ImageConverter (Library)

The core image conversion logic is encapsulated in the `ImageConverter` project, which serves as a shared library used by all UI implementations.

#### JPEGImage Class

This class handles the actual conversion logic and provides the following functionality:

- **Constructor**: Takes a file path to a HEIC image and validates file existence
- **ToFile() Method**: Performs conversion and writes the JPEG output to the same directory as the original file, using the same filename with a `.jpg` extension
- **ConvertedFilePath Property**: Returns the full path to the converted file after successful conversion
- **Error Property**: Contains error messages if conversion fails

**Usage Example:**
```csharp
JPEGImage myimage = new JPEGImage(filePath);
myimage.ToFile(); // Converts file and saves as JPEG in same directory
```

**Conversion Process:**
1. Reads the HEIC image file
2. Uses ImageMagick to convert to JPEG format
3. Writes bytes to output file in the original directory
4. Returns success/failure status

### HeicToJPEG (GUI Application)

A Windows Forms application providing a user-friendly interface for image conversion:

- **File Browser**: Browse and select HEIC files for conversion
- **Convert Button**: Initiates conversion process
- **User Feedback**: Success/error message boxes display conversion results
- **Menu Integration**: Additional menu options for file operations

**Usage:** Launch the application, browse to select a HEIC file, and click Convert.

### HeicToJPEG-cmd (Command-Line Utility)

A console application for automated or scripted image conversion:

- **Command Syntax**: `HeicToJPEG-cmd -file <path_to_HEIC>`
- **File Validation**: Checks file existence before processing
- **Exit Codes**: Returns status codes for success/failure
- **Batch Processing**: Suitable for integration with scripts and batch operations

**Example Usage:**
```batch
HeicToJPEG-cmd -file "C:\Images\photo.heic"
```

### HeicToJPEG-service (Windows Service)

A Windows service implementation for unattended/background conversion:

- **Service Architecture**: Runs as a Windows service under ServiceBase framework
- **Dual Mode**: Can execute in interactive mode (for testing) or as a true Windows service
- **Worker Pattern**: Uses a Worker class to handle conversion logic
- **Installation**: Includes ProjectInstaller for easy service registration/removal

**Features:**
- Can be installed/started via `sc` command or Services.msc
- Allows unattended batch conversion
- Supports automated workflows

## Build and Deployment

### Build System
- **Solution File**: `HeicToJPEG.sln`
- **CI/CD**: Automated builds via [AppVeyor](https://ci.appveyor.com/project/t3knoid/heictojpeg)
- **Build Configuration**: Visual Studio 2017, Any CPU, Release mode

### Installation
- **Installer Type**: NSIS (Nullsoft Scriptable Install System)
- **Output**: `HeicToJPEG_setup.exe`
- **Deployment**: GitHub Releases integration for automated deployment

### Dependencies
- **Magick.NET**: Image conversion library (NuGet)
- **ImageMagick**: Underlying image processing engine

## Releases

Builds are automatically created using [AppVeyor](https://ci.appveyor.com/project/t3knoid/heictojpeg) and deployed to GitHub releases.

Download the [latest build](https://github.com/t3knoid/HeicToJPEG/releases/latest/).

## Use Cases

- **Desktop Users**: Use the GUI application for occasional conversions
- **Developers/Power Users**: Use the command-line tool for scripting and batch operations
- **System Administrators**: Use the Windows service for automated, scheduled conversions
- **Batch Processing**: Integrate the CLI utility into automation workflows

## Project Structure

```
HeicToJPEG/
├── ImageConverter/          # Core conversion library
├── HeicToJPEG/             # Windows Forms GUI
├── HeicToJPEG-cmd/         # Command-line interface
├── HeicToJPEG-service/     # Windows Service
├── Installer/              # NSIS installer scripts
└── _TestImages/            # Test image resources
```
