# Folder Colorizer

A Unity Editor tool that automatically assigns rainbow colors to folders in the Project window for better visual organization.

![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black)
![License](https://img.shields.io/badge/License-MIT-green)

## Features

- **Automatic Rainbow Colors** - Folders receive unique colors based on their path
- **Golden Ratio Distribution** - Sibling folders get maximally separated hues for easy differentiation
- **Color Inheritance** - Files inside colored folders inherit their parent's color
- **Text Outline** - Black stroke around labels for readability on any background
- **Hover Effects** - Configurable color changes on mouse hover
- **Fully Configurable** - Adjust saturation, brightness, opacity, and text colors

## Installation

### Via Git URL (Unity Package Manager)

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL**
3. Enter: `https://github.com/TheSmallKiwi/FolderColorizer.git`

### Manual Installation

1. Download or clone this repository
2. Copy the folder into your project's `Packages/` directory

## Usage

1. Open **Window > Folder Colorizer**
2. Adjust settings as desired:
   - **Background Opacity** - Transparency of folder color overlay
   - **Saturation** - Color intensity
   - **Brightness** - Color lightness
   - **Min Folder Depth** - Skip top-level folders (0 = Assets, 1 = Assets/*, etc.)
   - **Text Colors** - Customize outline and text colors for normal and hover states

## Configuration

| Setting | Default | Description |
|---------|---------|-------------|
| Enabled | true | Toggle colorization on/off |
| Background Opacity | 0.35 | Transparency of color overlay |
| Saturation | 0.7 | Color intensity |
| Brightness | 0.85 | Color lightness |
| Colorize Children | true | Files inherit parent folder color |
| Min Folder Depth | 1 | Minimum depth to start coloring |

## License

MIT License - See [LICENSE](LICENSE) for details.
