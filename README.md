# Sprout Tracker for Dalamud

A Dalamud plugin for Final Fantasy XIV that helps identify new players (sprouts) and returning players in your current party and nearby area.

## Features

* Display an unobtrusive UI window showing sprouts and returners in your current party
* Option to show nearby sprouts/returners in the current zone
* Shows basic information about each player (name, job/class, world)
* Fully customizable display (colors, size, position)
* Auto-hide during combat (optional)
* Job icons for easy identification

## Installation

This plugin is designed for the experimental tab in Dalamud. To install:

1. Add the following URL to your list of custom plugin repositories in Dalamud Settings > Experimental:
   ```
   https://github.com/JungleSanctuary/z/raw/main/repo.json
   ```

2. Search for "Sprout Tracker" in the plugin installer and install it

## Commands

* `/sprouttracker` - Toggles the Sprout Tracker window
* `/stracker` - Shorter alias for toggling the window

## Configuration

The plugin offers several configuration options:

### Display Settings
* Toggle display of party members and/or nearby players
* Toggle display of sprouts and/or returners
* Adjust detection radius for nearby players
* Set maximum number of nearby players to display
* Toggle job icons

### Window Settings
* Lock window position
* Auto-hide in combat
* Adjust window size

### Color Settings
* Customize colors for sprouts, returners, and headers

## License

This plugin is licensed under the AGPL-3.0-or-later license.


The intention is less that any of this is used directly in other projects, and more to show how similar things can be done.

## How To Use

### Getting Started

To begin, [clone this template repository][new-repo] to your own GitHub account. This will automatically bring in everything you need to get a jumpstart on development. You do not need to fork this repository unless you intend to contribute modifications to it.

Be sure to also check out the [Dalamud Developer Docs][dalamud-docs] for helpful information about building your own plugin. The Developer Docs includes helpful information about all sorts of things, including [how to submit][submit] your newly-created plugin to the official repository. Assuming you use this template repository, the provided project build configuration and license are already chosen to make everything a breeze.

[new-repo]: https://github.com/new?template_name=SamplePlugin&template_owner=goatcorp
[dalamud-docs]: https://dalamud.dev
[submit]: https://dalamud.dev/plugin-development/plugin-submission

### Prerequisites

SamplePlugin assumes all the following prerequisites are met:

* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.
  * If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
* A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

### Building

1. Open up `SamplePlugin.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplePlugin/bin/x64/Debug/SamplePlugin.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `SamplePlugin` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

### Reconfiguring for your own uses

Basically, just replace all references to `SamplePlugin` in all of the files and filenames with your desired name, then start building the plugin of your dreams. You'll figure it out 😁

Dalamud will load the JSON file (by default, `SamplePlugin/SamplePlugin.json`) next to your DLL and use it for metadata, including the description for your plugin in the Plugin Installer. Make sure to update this with information relevant to _your_ plugin!
