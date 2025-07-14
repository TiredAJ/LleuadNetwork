using System.IO;

using Godot;

using LleuadNetworkSim.Models.Exceptions;
using LleuadNetworkSim.Models.Exceptions.Validation.File;
using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Models.Validation;

static public class FileValidator
{
    private const long MAX_FILE_SIZE = (25 * 1000 * 1000); //25MiB

    static public void ValidateFile(string _FilePath, string _ExpectedFileType, Node _Caller) {
        if (Path.GetExtension(_FilePath) != _ExpectedFileType)
        { ExceptionPopupWrapper.Throw(_Caller, new InvalidFileTypeException(_FilePath, ".lnmap")); }

        if (!File.Exists(_FilePath))
        {
            ExceptionPopupWrapper.Throw(_Caller, new FileNotFoundException($"File could not be found at given path [{_FilePath}]."));
        }

        long FileSize = new FileInfo(_FilePath).Length;

        if (FileSize > MAX_FILE_SIZE)
        { ExceptionPopupWrapper.Throw(_Caller, new FileTooLargeException(FileSize)); }
    }
}
