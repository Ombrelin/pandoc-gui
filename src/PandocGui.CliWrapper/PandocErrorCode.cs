namespace PandocGui.CliWrapper;

public enum PandocErrorCode
{
    PandocFailOnWarningError = 3,
    PandocAppError = 4,
    PandocTemplateError = 5,
    PandocOptionError = 6,
    PandocUnknownReaderError = 21,
    PandocUnknownWriterError = 22,
    PandocUnsupportedExtensionError = 23,
    PandocCiteprocError = 24,
    PandocEpubSubdirectoryError = 31,
    PandocPDFError = 43,
    PandocXMLError = 44,
    PandocPDFProgramNotFoundError = 47,
    PandocHttpError = 61,
    PandocShouldNeverHappenError = 62,
    PandocSomeError = 63,
    PandocParseError = 64,
    PandocParsecError = 65,
    PandocMakePDFError = 66,
    PandocSyntaxMapError = 67,
    PandocFilterError = 83,
    PandocMacroLoop = 91,
    PandocUTF8DecodingError = 92,
    PandocIpynbDecodingError = 93,
    PandocUnsupportedCharsetError = 94,
    PandocCouldNotFindDataFileError = 97,
    PandocResourceNotFound = 99
}