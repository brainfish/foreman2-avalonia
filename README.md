# Foreman 2 (Avalonia / Linux)

Foreman 2 is a Factorio production-graph planner. This tree is a port of [DanielKote/Foreman2](https://github.com/DanielKote/Foreman2) from Windows Forms to **Avalonia 11**, so the same flowchart editor runs on **Linux** (and still on Windows/macOS) with the original models, solver, presets, and graph drawing.

The WinForms designer UI and GDI+ painting are hosted through a compatibility layer in `Foreman/WinFormsShim` (Avalonia widgets + SkiaSharp). Logic in `Foreman/` is the upstream Foreman 2 codebase, adapted only where Windows-only APIs blocked Linux.

## Requirements

- .NET 10 SDK
- Linux: fontconfig and a working desktop session for the GUI (X11 or Wayland)
- Google OR-Tools native libraries come from the `Google.OrTools` NuGet package
- Optional: a Factorio 2.0 install if you want to import a custom preset

## Run

```bash
dotnet run --project Foreman/Foreman.csproj
```

The vanilla Factorio 2.0 preset is copied next to the binary. Use **Add item** / **Add recipe** to start a graph, middle-drag to pan, mouse wheel to zoom.

```bash
dotnet test ForemanTest/ForemanTest.csproj
```

Settings are stored at `~/.config/Foreman/user-settings.json` on Linux (instead of Windows user.config).

## Factorio import on Linux

Preset import looks for a Factorio install that contains `bin/x64/factorio` (Steam libraries under `~/.steam/steam` and `~/.local/share/Steam` are scanned). Version checks use `factorio --version` instead of Windows file version resources.

## License

Blue Oak Model License 1.0.0 — see `LICENSE.md`. Original Foreman 2 © Daniel Kotes / Nick Powell.
