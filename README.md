# Pandoc GUI

![Icon](./PandocGui/Assets/avalonia-logo.ico)

This software aims to provide a clean and modern GUI to generate PDF documents from markdown using Pandoc, with the ability to pass several options, like : 

- Custom code highlithing theme
- Headings numbers
- Font
- Link color
- Margin
- PDF Engine
- Table of contents

Pandoc GUI runs on .NET 6.

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
winget install MiKTex
winget install Pandoc
```

### Linux

```bash
sudo apt-get install pandoc texlive-latex-extra texlive-latex-recommended
```

### MacOs

```bash
brew install pandoc
curl http://mirror.ctan.org/systems/mac/mactex/BasicTeX.pkg -o
sudo installer -pkg BasicTeX.pkg -target /
```

## Install Release

### Windows

 - [Download Installer](https://github.com/Ombrelin/pandoc-gui/releases/download/v1.0/pandoc-gui-setup.exe)
 - Just run the installer

No need to install the .NET 6 Runtime, it has been bundled in the executable.

### MacOS

WIP

### Linux

WIP

## Run Dev

Make sure you have the latest .NET 6 SDK installed.

```bash
git clone https://github.com/Ombrelin/pandoc-gui
cd pandoc-gui/PandocGui
dotnet run
```

> On Mac / Linux : `sudo dotnet run`

## Special Thanks

- AvaloniaUI
- ReactiveUI
- Pandoc
- [@Naaviiss](https://github.com/Naaviiss) for help with macOS run & deployment
- [@Grandkhan](https://github.com/Grandkhan) for the app icon
