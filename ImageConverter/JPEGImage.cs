using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace ImageConverters
{
    public class JPEGImage
    {
        /// <summary>
        /// Provides full file path to the converted file
        /// </summary>
        public string ConvertedFilePath { get; set; }
        /// <summary>
        /// If an error is thrown, it will be available here
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Converted image bytes
        /// </summary>
        byte[] Bytes { get; set; }
        string FilePath { get; set; }
        /// <summary>
        /// Constructor that takes the full file path of the file to convert
        /// </summary>
        /// <param name="imageFile"></param>
        public JPEGImage(string imageFile)
        {
            if (File.Exists(imageFile))
            {
                FilePath = imageFile;
            }
            else
            {
                throw new ArgumentException(String.Format("Specified file {0} does not exist.", imageFile));
            }
        }
        /// <summary>
        /// Saves JPEG file into the same folder as original file.
        /// </summary>
        /// <returns></returns>
        public bool ToFile()
        {
            try
            {
                Convert();
                var folder = Path.GetDirectoryName(FilePath);
                var file = Path.Combine(folder, Path.GetFileNameWithoutExtension(FilePath) + ".jpg");
                using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(Bytes, 0, Bytes.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Converts given file to JPEG
        /// </summary>
        private void Convert()
        {
            try
            {
                // Read image from file
                using (var image = new MagickImage(FilePath))
                {
                    // Sets the output format to jpeg
                    image.Format = MagickFormat.Jpeg;

                    // Create byte array that contains a jpeg file
                    Bytes = image.ToByteArray();
                }

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }
    }

}
