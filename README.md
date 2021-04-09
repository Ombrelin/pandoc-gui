# Pandoc GUI

This software aims to provide a clean and modern GUI to generate PDF documents from markdown using Pandoc, with the ability to pass several options, like : 

- Custom code highlithing theme
- Headings numbers
- Font
- Link color
- Margin
- PDF Engine
- Table of contents

Pandoc GUI runs on .NET 5.

## Screenshots

![Main UI](./screenshots/main-ui.png)

## Prerequisites

### Windows

Check that you have Pandoc and MikTex installed.

#### Using Chocolatey

```bash
choco install pandoc miktex -y
```

#### Using winget

```bash
winget install Pandoc MiKTex
```

### Linux

```bash
sudo apt-get install pandoc texlive-latex-extra texlive-latex-recommended
```

### MacOs

```bash
brew install pandoc
brew install --cask miktex-console
```

## Install Release

### Windows

 - [Download Installer](https://github.com/Ombrelin/pandoc-gui/releases/download/v1.0/pandoc-gui-setup.exe)
 - Just run the installer

No need to install the .NET 5 Runtime, it has been bundled in the executable.

### MacOS

WIP

### Linux

WIP

## Run Dev

Make sure you gave the .NET 5 SDK installed.

```bash
git clone https://github.com/Ombrelin/pandoc-gui
cd pandoc-gui/PandocGui
dotnet run
```

## Special Thanks

- AvaloniaUI
- ReactiveUI
- Pandoc
