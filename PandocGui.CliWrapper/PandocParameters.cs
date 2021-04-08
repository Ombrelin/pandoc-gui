﻿using System;

namespace PandocGui.CliWrapper
{
    public class PandocParameters
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public bool HighlightTheme { get; set; } = false;
        public string HighlightThemeSource { get; set; }
        public bool NumberedHeader { get; set; } = false;
        public bool CustomFont { get; set; } = false;
        public string CustomFontName { get; set; }
        public bool CustomMargin { get; set; } = false;
        public decimal CustomMarginValue { get; set; }
        public bool CustomPdfEngine { get; set; } = false;
        public string CustomPdfEngineValue { get; set; }
        public bool TableOfContents { get; set; } = false;
        public bool LogToFile { get; set; } = false;
        public string LogFilePath { get; set; }
    }
}