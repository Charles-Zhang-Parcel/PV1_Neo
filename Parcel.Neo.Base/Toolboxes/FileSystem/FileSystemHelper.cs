using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parcel.Neo.Base.Toolboxes.FileSystem
{
    public class FetchParameters
    {
        public string InputFolder { get; set; }
        public string InputPattern { get; set; }

        public string[] OutputEntries { get; set; }
        public string[] OutputFolders { get; set; }
        public string[] OutputFiles { get; set; }
    }

    public class RenameParameters
    {
        public string[] InputPaths { get; set; }
        public string InputNamePattern { get; set; }
        public string InputReplacementPattern { get; set; }

        public string[] OutputOriginalPaths { get; set; }
        public string[] OutputNewPaths { get; set; }
    }

    public static class FileSystemHelper
    {
        #region Batch Processor
        public static void Fetch(FetchParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.InputFolder))
                throw new ArgumentException("Input folder is empty.");
            if (Directory.Exists(parameters.InputFolder))
                throw new ArgumentException("Input folder doesn't exist.");

            string[] files = Directory.GetFiles(parameters.InputFolder);
            string[] folders = Directory.GetDirectories(parameters.InputFolder);

            if (!string.IsNullOrWhiteSpace(parameters.InputPattern))
            {
                files = files
                    .Where(e => Regex.IsMatch(Path.GetFileName(e), parameters.InputPattern))
                    .ToArray();
                folders = folders
                    .Where(e => Regex.IsMatch(Path.GetFileName(e), parameters.InputPattern))
                    .ToArray();
            }

            parameters.OutputEntries = files.Concat(folders).ToArray();
            parameters.OutputFiles = files;
            parameters.OutputFolders = folders;
        }
        #endregion

        #region Default Processor

        public static void Rename(RenameParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.InputNamePattern))
                throw new ArgumentException("Input name pattern is empty.");
            if (string.IsNullOrWhiteSpace(parameters.InputReplacementPattern))
                throw new ArgumentException("Input replacement pattern is empty.");

            parameters.OutputOriginalPaths = parameters.InputPaths;
            parameters.OutputNewPaths = parameters.InputPaths
                .Select(path =>
                {
                    string name = Path.GetFileName(path);
                    string newName = Regex.Replace(name, parameters.InputNamePattern,
                        parameters.InputReplacementPattern);
                    string newPath = Path.Combine(Path.GetDirectoryName(path), newName);
                    return newPath;
                }).ToArray();
        }
        #endregion
    }
}