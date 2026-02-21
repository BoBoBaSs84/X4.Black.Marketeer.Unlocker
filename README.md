[![CI](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/github-code-scanning/codeql/badge.svg?branch=main)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/github-code-scanning/codeql)
[![Dependabot](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/dependabot/dependabot-updates/badge.svg?branch=main)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/actions/workflows/dependabot/dependabot-updates)

[![.NET](https://img.shields.io/badge/net8.0-5C2D91?logo=.NET&labelColor=gray)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker)
[![C#](https://img.shields.io/badge/C%23-13.0-239120)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker)
[![Issues](https://img.shields.io/github/issues/BoBoBaSs84/X4.Black.Marketeer.Unlocker)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/issues)
[![Commit](https://img.shields.io/github/last-commit/BoBoBaSs84/X4.Black.Marketeer.Unlocker)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/commit/main)
[![Size](https://img.shields.io/github/repo-size/BoBoBaSs84/X4.Black.Marketeer.Unlocker)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker)
[![License](https://img.shields.io/github/license/BoBoBaSs84/X4.Black.Marketeer.Unlocker)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/blob/main/LICENSE)
[![Release](https://img.shields.io/github/v/release/BoBoBaSs84/X4.Black.Marketeer.Unlocker)](https://github.com/BoBoBaSs84/X4.Black.Marketeer.Unlocker/releases/latest)

# X4.Black.Marketeer.Unlocker

Unlock all black marketeers in your X4: Foundations save file with a single command-line tool.

## Overview

**X4 Black Marketeer Unlocker** is a .NET 8 console application designed to modify your X4: Foundations save file, instantly unlocking all black marketeers. The tool safely creates a backup of your save file before making any changes, ensuring your original data is preserved.

## Features

- **Automatic Backup:** Creates a `.bak` backup of your save file before modification.
- **Unlock All Black Marketeers:** Scans and updates all relevant NPCs in your save file.
- **Safe and Transparent:** Reports the number of marketeers found and details of each unlock.
- **Cross-Platform:** Runs anywhere .NET 8 is supported.

## Usage

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.

### Running the Application

1. Build the project using Visual Studio 2022 or the .NET CLI:

```csharp
dotnet build
```

2. Run the application from the command line, providing the path to your X4 save file:

```cmd
X4.Black.Marketeer.Unlocker.exe <path-to-save-file.xml.gz>
```

Example:

```cmd
X4.Black.Marketeer.Unlocker.exe "C:\Users\<YourAccount>\Documents\Egosoft\<YourSteamId>\X4\save\save_001.xml.gz"
```

### What Happens

- The tool creates a backup of your save file (`<save-file>.bak`) if one does not already exist.
- It decompresses and parses the save file, finds all black marketeer NPCs, and unlocks them by updating their traits.
- The modified save file is compressed and saved, overwriting the original.

## License

This project is licensed under the [MIT License](LICENSE).

## Disclaimer

This tool modifies your X4: Foundations save files. Always keep backups of your original saves. Use at your own risk.
