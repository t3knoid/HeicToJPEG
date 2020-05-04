# HeicToJPEG
HeicToJPEG provides an easy way to convert HEIC images into JPG. This project uses the [Magick.NET](https://github.com/dlemstra/Magick.NET) library to perform conversions.

# Projects
There are several projects within this solution.

## ImageConverters
This project provides the main methods that performs the image conversion. The image conversion rely on the Magick.Net library. 

### JPEGImage Class
This class provides the necessary properties and methods to convert a given image to JPEG. To use this class, instantiate it with the location of the file you want to convert.

```csharp
JPEGImage myimage = new JPEGImage(filePath);
``` 

A call to the ToFile() method will convert and write the given file to its JPEG equivalent in the same folder of the original file.

```csharp
myimage.ToFile();
```

## HeicToJPEG
This project provides a Win32 Windows GUI that allows selection of a HEIC file to convert.

## HeicToJPEG-cmd
This project provides a Win32 command-line interface that allows conversion of a HEIC file to JPEG.

```batch
usage: HeicToJPEG-cmd -file pathToHEIC
```

# Releases
Builds are created using [AppVeyor](https://ci.appveyor.com/project/t3knoid/heictojpeg). 

Download the [latest build](https://github.com/t3knoid/HeicToJPEG/releases/latest/).
