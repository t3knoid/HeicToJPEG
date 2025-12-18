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

### GitHub Actions Workflows

This project uses GitHub Actions for automated build and release:

#### Build Workflow (`build.yml`)

- **Trigger**: Pushes to `main`, `master`, or `develop` branches; pull requests
- **Actions**:
  - Restores NuGet dependencies
  - Builds solution in Release configuration
  - Uploads build artifacts (DLLs and EXEs)

#### Release Workflow (`release.yml`)

- **Trigger**: Git tag push (format: `v*`, e.g., `v1.0.0`)
- **Actions**:
  - Updates all AssemblyInfo.cs files with version from git tag
  - Builds the solution
  - Creates NSIS Windows installer with version in filename
  - Publishes release to GitHub with all artifacts
  - Artifacts include: GUI app, CLI tool, and Windows installer

### Versioning Strategy

The project uses a **VERSION file** as the single source of truth:

1. **VERSION File** (`VERSION` in root): Contains base version (e.g., `1.0.0`)
2. **Build Number**: Scripts append commit count (e.g., `1.0.0.{commit-count}`)
3. **Release Tags**: Must match VERSION file (e.g., `v1.0.0`)
4. **Validation**: Release workflow validates tag matches VERSION file content

**Version Flow:**

```
Development builds (branch push):
  - Read VERSION file (e.g., 1.0.0)
  - Append commit count (e.g., 1.0.0.42)
  - Update AssemblyInfo.cs files
  - Build with dev version

Release builds (tag push v1.0.0):
  - ValidateReleaseTag checks: v1.0.0 == VERSION file content
  - If mismatch: Release fails with error
  - If match: Continue with build and create release
```

**UpdateVersion.ps1 Script** (`scripts/UpdateVersion.ps1`):
- Reads VERSION file
- Calculates build number from commit count
- Updates all AssemblyInfo.cs files
- Supports dry-run mode (`-DryRun` flag)

**ValidateReleaseTag.ps1 Script** (`scripts/ValidateReleaseTag.ps1`):
- Extracts version from git tag (removes `v` prefix)
- Compares with VERSION file
- Fails release if versions don't match
- Prevents accidental mismatched releases

### Creating a Release

To create a new release, follow these steps:

1. **Update VERSION file** in the project root with the new version:
   ```
   1.2.0
   ```

2. **Commit the version change:**
   ```powershell
   git add VERSION
   git commit -m "Bump version to 1.2.0"
   git push origin main
   ```

3. **When ready to package a stable build, create and push a matching version tag:**
   ```powershell
   git tag v1.2.0
   git push origin v1.2.0
   ```
   
   ⚠️ **Important:** Only create a tag when you're ready to release a stable build. Tags trigger the full release workflow and will be publicly available.

4. **GitHub Actions automatically:**
   - Validates that tag (`v1.2.0`) matches VERSION file (`1.2.0`)
   - Fails if versions don't match (preventing mistakes)
   - Builds the code with version 1.2.0
   - Creates installer: `HeicToJPEG_1.2.0_setup.exe`
   - Publishes GitHub Release with all artifacts

**Important:** The tag must match the VERSION file exactly (minus the `v` prefix). If they don't match, the release workflow will fail with a validation error.

**Example workflow:**
```
1. VERSION file: 1.2.0
2. Tag created: v1.2.0        ✓ Match → Release succeeds
   
   OR

1. VERSION file: 1.2.0
2. Tag created: v1.3.0        ✗ Mismatch → Release fails with validation error
```

### Installation

- **Installer Type**: NSIS (Nullsoft Scriptable Install System)
- **Output**: `HeicToJPEG_setup.exe`
- **Deployment**: GitHub Releases integration for automated deployment

### Dependencies

- **Magick.NET**: Image conversion library (NuGet)
- **ImageMagick**: Underlying image processing engine
### Downloading Releases

Download the [latest release](https://github.com/t3knoid/HeicToJPEG/releases/latest/) from GitHub.

Available artifacts:

- **HeicToJPEG_setup.exe** - Complete Windows installer (recommended)
- **HeicToJPEG.exe** - GUI application executable
- **HeicToJPEG-cmd.exe** - Command-line tool executable

## Use Cases

- **Desktop Users**: Use the GUI application for occasional conversions
- **Developers/Power Users**: Use the command-line tool for scripting and batch operations
- **System Administrators**: Use the Windows service for automated, scheduled conversions
- **Batch Processing**: Integrate the CLI utility into automation workflows

## Project Structure

```bash
HeicToJPEG/
├── ImageConverter/          # Core conversion library
├── HeicToJPEG/             # Windows Forms GUI
├── HeicToJPEG-cmd/         # Command-line interface
├── HeicToJPEG-service/     # Windows Service
├── Installer/              # NSIS installer scripts
└── _TestImages/            # Test image resources
```
