<div align="center">
  <img src=".github/OSDSYS.png" alt="PS2BIOS Toolkit Logo" width="384" />
</div>

<h1 align="center">PS2BIOS.Toolkit</h1>
<p align="center">Parse and Extract PS2 BIOS Modules.</p>

## Architecture

```mermaid
flowchart TB
    %% Paleta Refinada (Igual ao Exemplo)
    %% Accent: #378FB6 | Glow: #5BDEF3 | Dark: #0A121D | Border: #40FFFFFF

    subgraph CLI ["CLI Layer (Spectre.Console)"]
        direction TB
        subgraph UI [User Interface]
            Renderer["OutputRenderer<br/>(Tables, Errors)"]
            Progress["Progress Bars<br/>(AnsiConsole)"]
        end

        Program["Program.cs<br/>(System.CommandLine)"]
        
        Program --> Renderer
        Program --> Progress
    end

    subgraph Core ["Core Library (PS2Bios.Core)"]
        direction TB
        subgraph Services [Services]
            Parser["BiosParserService<br/>(Span byte, BinaryPrimitives)"]
            Extractor["ExtractionService<br/>(File Streams)"]
        end

        Models["Models<br/>(BiosMetadata, RomModule)"]
        Helpers["Helpers<br/>(PaddingHelper)"]

        Services --> Models
        Services --> Helpers
    end

    Program -- "Calls Services" --> Services
    Renderer -. "Reads" .- Models

    %% Estilização Premium para Dark Mode
    %% CLI: Azul Profundo
    style CLI fill:#1F536B,stroke:#5BDEF3,stroke-width:2px,color:#FFFFFF
    
    %% Core: Fundo Sóbrio
    style Core fill:#0A121D,stroke:#378FB6,stroke-width:2px,color:#FFFFFF
    
    %% Camadas Internas
    style UI fill:#2B7291,stroke:#93F5FF,color:#FFFFFF
    style Services fill:#151819,stroke:#378FB6,color:#FFFFFF

    %% Links
    linkStyle default stroke:#5BDEF3,stroke-width:1px

```

## Workflow

### Parsing Process

```mermaid
flowchart LR
    File[BIOS Dump File] --> Stream[FileStream]
    Stream --> Find["Find RESET Signature<br/>(Span Search)"]
    Find --> ReadDir[Read ROMDIR]
    ReadDir --> Meta["BiosMetadata<br/>(Modules List)"]
    
    %% Estilo "Ativo/Glow"
    style File fill:#378FB6,stroke:#93F5FF,stroke-width:3px,color:#FFFFFF
    style Meta fill:#378FB6,stroke:#93F5FF,stroke-width:3px,color:#FFFFFF
    linkStyle 0,1,2,3 stroke:#5BDEF3,stroke-width:2px

```

## Technologies

* **.NET 9** - Latest stable runtime.
* **Spectre.Console 0.49** - Beautiful console output, tables, and progress bars.
* **System.CommandLine** - Robust command-line argument parsing.
* **Span<byte>** - High-performance memory manipulation for binary parsing.
* **XUnit & FluentAssertions** - Comprehensive unit testing.

## How to Use

### CLI Usage

You can use the tool directly from the command line to inspect or extract modules.

```bash
# Extract all modules from a BIOS dump
dotnet run -- project src/CLI --file "scph10000.bin" --all

# Extract a single module by index
dotnet run -- project src/CLI --file "scph10000.bin" --index 4

```

### Library API (Core)

You can use the `PS2Bios.Core` library in your own projects for direct access to BIOS data.

```csharp
using PS2Bios.Core.Services;
using PS2Bios.Core.Interfaces;

// Initialize services
IBiosParser parser = new BiosParserService();
IExtractionService extractor = new ExtractionService();

// 1. Parse the file
using var stream = File.OpenRead("bios.bin");
var metadata = await parser.ParseAsync(stream, "bios.bin");

// 2. Inspect modules
foreach (var module in metadata.Modules)
{
    Console.WriteLine($"Module: {module.Name} | Size: {module.HexSize}");
}

// 3. Extract specific module
await extractor.ExtractAsync(stream, metadata.Modules[0], "./output_folder");
```

## Benefits

1. **Efficiency**: Utilizes `ReadOnlySpan<byte>` and `BinaryPrimitives` for fast parsing without unnecessary allocations.
2. **User Experience**: Provides formatted tables and extraction progress bars via `Spectre.Console`.
3. **Accuracy**: Correctly handles 16-byte alignment and padding required by the PS2 BIOS format.
4. **Modular**: The parsing logic is completely separated from the UI, allowing reuse in GUI applications.
5. **Modern**: Built on pure .NET 9 with asynchronous file I/O operations.

## Next Steps

* [ ] Implement compressed module handling.
* [ ] Improved validation for incomplete dumps.