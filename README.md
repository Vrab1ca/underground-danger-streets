**BASSIK** is a first-person action and survival game prototype made with Unity 3D. The player explores dangerous environments, fights zombies, collects weapons and survival equipment, drives cars, pilots a combat helicopter, and manages health, armor, ammunition, fuel, flashlight batteries, and other useful items.

> This project is currently under active development. Features, controls, balance, scenes, and assets may change.

## Gameplay Features

- First-person movement and camera controls
- Zombie enemies with health, detection, attacks, and damage
- Firearms with magazines, reserve ammunition, reloads, spread, effects, and different ammunition types
- Automatic and semi-automatic weapons
- Sniper zoom with multiple zoom levels
- Fists and melee weapons such as a knife and baseball bat
- Dynamic hotbar with up to eight inventory slots
- Weapon and item pickup, selection, use, and dropping
- Normal grenades and Molotov cocktails
- Fire zones that damage nearby objects and enemies
- Health items with different healing values
- Multiple armor types and armor durability
- Placeable jump platforms
- Flashlight with battery drain and replaceable battery types
- Drivable car with speed, gears, RPM, boost, drifting, and HUD information
- Flyable helicopter with a gun, ammunition, bombs, fuel consumption, and bomb loading
- Vehicle fuel pickup, carrying, dropping, and refuelling systems
- Dynamic day and night cycle
- Sun, moon, stars, clouds, and cloud movement/LOD systems
- Intro, main menu, loading screen, tutorial, and survival gameplay scenes

## Controls

### On Foot

| Action | Control |
|---|---|
| Move | `W`, `A`, `S`, `D` |
| Look | Mouse |
| Jump | `Space` |
| Sprint | `Left Shift` |
| Attack / shoot / use selected item | `Left Mouse Button` |
| Sniper scope | `Right Mouse Button` |
| Change sniper zoom | Mouse wheel while scoped |
| Reload weapon | `R` |
| Pick up a nearby supported item | `F` |
| Select hotbar slot | `1`–`8` |
| Change hotbar slot | Mouse wheel |
| Select fists / empty hands | `Q` |
| Drop selected weapon | `G` |
| Toggle selected flashlight | `T` |
| Install a selected battery in the flashlight | `R` |
| Enter or exit a vehicle | `E` |

Health items, armor, grenades, Molotov cocktails, and jump platforms are used with the **Left Mouse Button** after selecting their hotbar slot.

### Car

| Action | Control |
|---|---|
| Enter / exit | `E` |
| Accelerate / reverse | `W` / `S` |
| Steer | `A` / `D` |
| Handbrake / drift | `Space` |
| Boost | `Left Shift` |
| Fire the selected player weapon | `Left Mouse Button` |

### Helicopter

| Action | Control |
|---|---|
| Enter / exit | `E` |
| Fly forward / backward | `W` / `S` |
| Turn left / right | `A` / `D` |
| Ascend | `Space` |
| Descend | `Left Ctrl` or `C` |
| Boost | `Left Shift` |
| Fire helicopter gun | `Left Mouse Button` |
| Reload helicopter gun | `R` |
| Drop bomb | `B` |

## Built-in Scenes

The current build configuration contains the following scenes in this order:

1. `Assets/Scenes/IntroScene.unity`
2. `Assets/Scenes/MainMenu.unity`
3. `Assets/Scenes/LoadingScreen.unity`
4. `Assets/Scenes/TutorialRoom.unity`
5. `Assets/Scenes/SurvivalLevel 1.unity`

## Requirements

- **Unity:** `6000.3.9f1`
- **Render pipeline:** Universal Render Pipeline `17.3.0`
- **Input System:** `1.18.0`
- **AI Navigation:** `2.0.12`
- Git
- Visual Studio, JetBrains Rider, or another C# editor supported by Unity

Opening the project with the exact Unity version is recommended to avoid unnecessary project upgrades or package compatibility problems.

## Installation and Setup

### 1. Clone the repository

```bash
git clone https://github.com/Vrab1ca/underground-danger-streets.git
cd underground-danger-streets
```

### 2. Open the project

1. Open **Unity Hub**.
2. Select **Add** or **Add project from disk**.
3. Choose the cloned `underground-danger-streets` folder.
4. Open it with Unity `6000.3.9f1`.
5. Allow Unity to import assets and restore packages.

### 3. Run the game

Open one of these scenes:

- `Assets/Scenes/IntroScene.unity` to begin from the intro flow
- `Assets/Scenes/MainMenu.unity` to begin from the menu
- `Assets/Scenes/TutorialRoom.unity` to test gameplay systems
- `Assets/Scenes/SurvivalLevel 1.unity` to open the survival level directly

Press the **Play** button in the Unity Editor.

## Creating a Build

1. Open **File → Build Profiles** in Unity.
2. Select the target platform.
3. Confirm that the required scenes are enabled and ordered correctly.
4. Select **Build** or **Build and Run**.
5. Choose an output folder outside the Unity project's `Assets` directory.

## Main Project Folders

```text
Assets/
├── Armor-Scrpts/          # Armor pickup, inventory, durability, and visuals
├── Car-scripts/           # Car driving, entering/exiting, HUD, and related systems
├── DayNight-scripts/      # Day/night cycle, clouds, stars, and environment lighting
├── Flashlight-scripts/    # Flashlight pickup, battery inventory, charge, and controls
├── Grenade/               # Grenades, Molotov cocktails, explosions, and fire zones
├── Helicopterscript/      # Helicopter flight, gun, bombs, loading, and HUD
├── JumpPlatform/          # Jump-platform inventory, placement, pickup, and pad behaviour
├── Melle-scrpts/          # Fist and melee combat animations and damage
├── Scenes/                # Intro, menus, loading, tutorial, and survival scenes
├── Scripts/               # Core player, health, UI, inventory, and game systems
└── Weapon/                # Weapons, ammunition, shooting, pickups, and effects
```

The folder names above match the current repository structure, including existing spelling.

## Important Systems

### Dynamic Hotbar

The hotbar can contain weapons, grenades, Molotov cocktails, jump platforms, health items, armor, the flashlight, and batteries. Fists are available separately and do not consume an inventory slot.

### Combat

Weapons support configurable damage, range, fire rate, magazine size, ammunition type, spread, shotgun pellets, reload time, automatic fire, melee mode, and optional sniper zoom.

### Vehicles

The car uses Unity wheel colliders and includes steering, acceleration, reverse movement, drifting, boost speed, gears, RPM, and an in-game HUD.

The helicopter includes physics-based movement, ascending and descending, turning, boost, rotor visuals, a mounted gun, ammunition, bombs, fuel use, and an engine-off falling state after exit.

### Survival Equipment

The player can collect and use healing items, armor, grenades, Molotov cocktails, jump platforms, a flashlight, and several battery types. Collected items use available hotbar slots.

### Environment

The environment system controls the time of day and visual transitions between morning, afternoon, evening, and night. It also supports sun and moon lighting, stars, moving clouds, and cloud level-of-detail behaviour.

## Development Status

The game is a **work in progress**. Some systems are experimental and may require references to be assigned in the Unity Inspector. The project may contain temporary models, materials, debug messages, test scenes, or unfinished balancing.

There is currently no stable release attached to this repository. To test the latest version, open the project in Unity and run one of the included scenes.

## Reporting Problems

When creating an issue, include:

- A clear description of the problem
- Steps to reproduce it
- The scene where it happens
- Any Unity Console errors or warnings
- Screenshots or a short video when useful
- Your Unity version and operating system

## Contributing

Contributions, suggestions, and bug reports are welcome. Keep pull requests focused on one feature or fix, explain what changed, and test the affected scenes before submitting.

## Author

Developed by [Vrab1ca]-(N.Kirov)(https://github.com/Vrab1ca).

## License

A license file has not been added yet. Until a license is provided, the project remains under the repository owner's default copyright rights.
