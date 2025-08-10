using System.IO;

using Godot;

using LleuadNetworkSim.Models.Exceptions.Validation.File;
using LleuadNetworkSim.Utils;

using static Common.Conf.Conf;

namespace LleuadNetworkSim.Models.Validation;

static public class FileValidator
{
    /// <summary>
    /// Validates a file at the given path.
    /// </summary>
    /// <param name="_FilePath">The path to the file to validate.</param>
    /// <param name="_ExpectedExtension">The expected file extension.</param>
    /// <param name="_Caller">The caller (a GD node so they can throw the extension wrapper).</param>
    static public void ValidateFile(string _FilePath, string _ExpectedExtension, Node _Caller) {
        
        if (!File.Exists(_FilePath))
        { ExceptionPopupWrapper.Throw(_Caller, new FileNotFoundException($"File could not be found at given path [{_FilePath}].")); }
        
        if (Path.GetExtension(_FilePath) != _ExpectedExtension)
        { ExceptionPopupWrapper.Throw(_Caller, new InvalidFileTypeException(_FilePath, MAP_EXTENSION)); }
        
        long FileSize = new FileInfo(_FilePath).Length;

        if (FileSize > MAX_FILE_SIZE)
        { ExceptionPopupWrapper.Throw(_Caller, new FileTooLargeException(FileSize)); }
    }
}
