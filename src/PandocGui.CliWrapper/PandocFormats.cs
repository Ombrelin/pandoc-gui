using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PandocGui.CliWrapper;

public record InputFormat(string DisplayName, string Format, string Category, IReadOnlyList<string> Extensions)
{
    public override string ToString() => DisplayName;
}

public record OutputFormat(string DisplayName, string Extension)
{
    public override string ToString() => DisplayName;
}

public static class PandocFormats
{
    public const string DefaultInputFormat = "markdown";

    // The full set of pandoc readers, ordered by real-world popularity (most common first).
    // Variants that share a file extension with a more common reader carry no extensions of
    // their own: they stay selectable in the combo but the popular reader wins auto-detection.
    public static IReadOnlyList<InputFormat> InputFormats { get; } = new List<InputFormat>
    {
        new("Markdown", "markdown", "Markdown", new[] { ".md", ".markdown", ".mdown", ".mkd", ".text", ".txt" }),
        new("GitHub Markdown", "gfm", "Markdown", Array.Empty<string>()),
        new("CommonMark", "commonmark", "Markdown", Array.Empty<string>()),
        new("CommonMark (extended)", "commonmark_x", "Markdown", Array.Empty<string>()),
        new("MultiMarkdown", "markdown_mmd", "Markdown", Array.Empty<string>()),
        new("PHP Markdown Extra", "markdown_phpextra", "Markdown", Array.Empty<string>()),
        new("Markdown (strict)", "markdown_strict", "Markdown", Array.Empty<string>()),
        new("Markua", "markua", "Markdown", Array.Empty<string>()),

        new("Word (docx)", "docx", "Word processing", new[] { ".docx" }),
        new("OpenDocument (odt)", "odt", "Word processing", new[] { ".odt" }),
        new("Rich Text (rtf)", "rtf", "Word processing", new[] { ".rtf" }),

        new("HTML", "html", "Web & XML", new[] { ".html", ".htm", ".xhtml" }),
        new("DocBook", "docbook", "Web & XML", new[] { ".dbk" }),
        new("JATS", "jats", "Web & XML", Array.Empty<string>()),
        new("BITS", "bits", "Web & XML", Array.Empty<string>()),
        new("OPML", "opml", "Web & XML", new[] { ".opml" }),

        new("LaTeX", "latex", "Typesetting", new[] { ".tex", ".latex", ".ltx" }),
        new("Typst", "typst", "Typesetting", new[] { ".typ" }),
        new("roff man", "man", "Typesetting", new[] { ".man" }),

        new("reStructuredText", "rst", "Markup", new[] { ".rst" }),
        new("Org mode", "org", "Markup", new[] { ".org" }),
        new("Textile", "textile", "Markup", new[] { ".textile" }),
        new("Muse", "muse", "Markup", new[] { ".muse" }),
        new("txt2tags", "t2t", "Markup", new[] { ".t2t" }),
        new("djot", "djot", "Markup", new[] { ".dj" }),

        new("EPUB", "epub", "Ebook", new[] { ".epub" }),
        new("FictionBook2", "fb2", "Ebook", new[] { ".fb2" }),

        new("Jupyter Notebook", "ipynb", "Notebook", new[] { ".ipynb" }),

        new("MediaWiki", "mediawiki", "Wiki", new[] { ".wiki" }),
        new("DokuWiki", "dokuwiki", "Wiki", Array.Empty<string>()),
        new("TikiWiki", "tikiwiki", "Wiki", Array.Empty<string>()),
        new("TWiki", "twiki", "Wiki", Array.Empty<string>()),
        new("Vimwiki", "vimwiki", "Wiki", Array.Empty<string>()),
        new("Jira", "jira", "Wiki", Array.Empty<string>()),
        new("Creole", "creole", "Wiki", Array.Empty<string>()),

        new("BibTeX", "bibtex", "Bibliography", new[] { ".bib" }),
        new("BibLaTeX", "biblatex", "Bibliography", Array.Empty<string>()),
        new("CSL JSON", "csljson", "Bibliography", Array.Empty<string>()),
        new("RIS", "ris", "Bibliography", new[] { ".ris" }),
        new("EndNote XML", "endnotexml", "Bibliography", Array.Empty<string>()),

        new("CSV", "csv", "Data", new[] { ".csv" }),
        new("TSV", "tsv", "Data", new[] { ".tsv" }),

        new("JSON (Pandoc AST)", "json", "Other", new[] { ".json" }),
        new("Native (Pandoc AST)", "native", "Other", new[] { ".native" }),
        new("Haddock markup", "haddock", "Other", Array.Empty<string>()),
    };

    // Output writers, ordered by real-world popularity. Each has a distinct extension so the
    // target writer is unambiguous from the file name pandoc receives via -o.
    public static IReadOnlyList<OutputFormat> OutputFormats { get; } = new List<OutputFormat>
    {
        new("PDF", ".pdf"),
        new("Word (docx)", ".docx"),
        new("HTML", ".html"),
        new("Markdown", ".md"),
        new("EPUB", ".epub"),
        new("OpenDocument (odt)", ".odt"),
        new("Rich Text (rtf)", ".rtf"),
        new("LaTeX", ".tex"),
        new("PowerPoint (pptx)", ".pptx"),
        new("reStructuredText", ".rst"),
        new("AsciiDoc", ".adoc"),
        new("Org mode", ".org"),
        new("Textile", ".textile"),
        new("MediaWiki", ".wiki"),
        new("Typst", ".typ"),
        new("DocBook", ".dbk"),
        new("Texinfo", ".texi"),
        new("roff man", ".man"),
        new("FictionBook2", ".fb2"),
        new("Plain text", ".txt"),
    };

    public static IReadOnlyList<string> InputExtensions { get; } =
        InputFormats.SelectMany(format => format.Extensions).Distinct().ToList();

    public static string DetectInputFormat(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return DefaultInputFormat;
        }

        var extension = Path.GetExtension(path);
        var match = InputFormats.FirstOrDefault(
            format => format.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
        return match?.Format ?? DefaultInputFormat;
    }

    public static bool IsSupportedInput(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var extension = Path.GetExtension(path);
        return InputExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}
