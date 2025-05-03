using System.IO;

using Godot;

using LleuadNetworkSim.Scripts.Exceptions;

namespace LleuadNetworkSim.Utils.Validators;

public class FileValidator
{
    private const long MAX_FILE_SIZE = (25 * 1000 * 1000); //25MiB
    
    static public void ValidateFile(string _FilePath, string _ExpectedFileType, Node _Caller) {
        if (Path.GetExtension(_FilePath) != _ExpectedFileType)
        { ExceptionPopupWrapper.Throw(_Caller, new InvalidFileTypeException(_FilePath, ".lnmap")); }
        
        long FileSize = new FileInfo(_FilePath).Length;

        if (FileSize > MAX_FILE_SIZE)
        { ExceptionPopupWrapper.Throw(_Caller, new FileTooLargeException(FileSize)); }
    }
}
