# WikiTools.Net

A .NET library and CLI tool for working with various wiki formats, with support for converting between WikidPad and Obsidian.

## Features

- **WikidPad Support**: Read and parse WikidPad `.wiki` files
- **Obsidian Support**: Read and parse Obsidian `.md` files
- **Format Conversion**: Convert WikidPad wikis to Obsidian format
- **CLI Tool**: Command-line interface for easy conversions

## Requirements

- .NET 9.0 SDK

## Building

```bash
dotnet restore
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Usage

### CLI Tool

Convert a WikidPad wiki to Obsidian format:

```bash
dotnet run --project src/WikiTools.CLI -- convert \
  --from wikidpad \
  --to obsidian \
  --source /path/to/wikidpad \
  --dest /path/to/obsidian
```

Or using short aliases:

```bash
dotnet run --project src/WikiTools.CLI -- convert \
  -f wikidpad \
  -t obsidian \
  -s /path/to/wikidpad \
  -d /path/to/obsidian
```

### Conversion Details

The converter handles the following WikidPad syntax:

| WikidPad Format | Obsidian Format | Example |
|----------------|-----------------|---------|
| Headers | Markdown headers | `+ Header` → `# Header` |
| WikiWords | Double brackets | `[WikiWord]` → `[[WikiWord]]` |
| Spaced links | Already compatible | `[[Link]]` → `[[Link]]` |
| Tags | Hashtags | `[tag:example]` → `#example` |
| Categories | Hashtags | `CategoryName` → `#Name` |
| File extension | Markdown | `.wiki` → `.md` |

### Library Usage

```csharp
using WikiTools;
using WikiTools.Converters;

// Read a WikidPad wiki
var wiki = new WikidpadWiki("/path/to/wikidpad");
var pages = wiki.GetAllPages();

// Convert to Obsidian
var converter = new WikidPadToObsidianConverter(
    "/path/to/wikidpad",
    "/path/to/obsidian"
);
converter.ConvertAll();
```

## Project Structure

```
WikiTools.Net/
├── src/
│   ├── WikiTools/               # Core library
│   │   ├── Pages/               # Page implementations
│   │   ├── Wikis/               # Wiki implementations
│   │   └── Converters/          # Format converters
│   └── WikiTools.CLI/           # Command-line interface
└── tests/
    └── WikiTools.Tests/         # Unit tests
```

## License

See [LICENSE](LICENSE) file for details.
