# Chat Context — Hell Tiles

## Current Status

The foundational **Hell Tiles** Unity project setup is complete and the first gameplay scaffolding is now in place. A placeholder tilemap arena and player sprite exist in `SampleScene`, and the player can hop one tile at a time using the new Input System. This file keeps AI assistants aligned with the actual progress so they can build on the latest implementation.

---

## Project Summary

**Hell Tiles** is a top-down, 2D pixel-art bullet-hell survival game being built in **Unity 6.2 (6000.2.9f1)**. The player will hop between floating tiles on a lava grid while dodging projectiles and surviving timed waves. Each tile type behaves differently, creating strategic and reactive gameplay. As players survive longer, difficulty and intensity increase.

Planned features (from Conner's design notes):

- Grid-based tile movement (Crossy Road-style)
- Multiple tile types (Normal, Spike, Sticky, Push, Health, Coin, Wall)
- Increasing tile and projectile frequency with time
- "Juice" — screen shake, blinking tiles, dust particles, sounds, afterimages, etc.
- Permanent upgrade shop system for positive feedback loop
- Exit tile and level progression after survival period

---

## Current Implementation

- Unity project created locally (version **6.2 / 6000.2.9f1**) and source control configured (Git, GitHub remote, Git LFS, Unity Smart Merge).
- Placeholder art added in `Assets/Art/`:
  - `background_placeholder.png`
  - `tile_placeholder.png` (used for walkable tiles via Tilemap)
  - `player_placeholder.png`
- `SampleScene.unity` contains:
  - `Grid` + Tilemap with a 6×4 walkable layout painted from the placeholder tile.
  - Background sprite renderer positioned beneath the grid.
  - `Player` GameObject with placeholder sprite and `Rigidbody2D` (kinematic).
- Scripts (C#):
  - `Assets/Scripts/Tiles/TileGridController.cs`  
    Handles grid lookups, walkable/blocked logic, and exposes cell/world conversions that align with the Grid component.
  - `Assets/Scripts/Player/PlayerGridMover.cs`  
    Reads the Input System `Player/Move` action (WASD/Left Stick), resolves cardinal input, and tweens the player one tile at a time while respecting walkable tiles.
- Input: `Assets/InputSystem_Actions.inputactions` (default template) supplies the `Move` action bound to keyboard/Gamepad; referenced via `InputActionReference`.
- Player movement is live in Play Mode—pressing WASD or a gamepad stick hops the character exactly one painted tile, blocking moves where no tile exists.

---

## Repository Structure

```
Hell Tiles/
├── Assets/
│   ├── Art/
│   ├── Audio/
│   ├── Materials/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Player/
│   │   ├── Tiles/
│   │   ├── Projectiles/
│   │   └── UI/
│   └── VFX/
├── ProjectSettings/
├── Packages/
├── .gitignore
├── .gitattributes
├── README.md
└── chat.md
```

Structure is now partially populated with placeholder art and first gameplay scripts.

---

## Development Tools and Setup

- **Unity Editor Version:** 6.2 (6000.2.9f1)
- **Version Control:** Git with GitHub remote
- **Large File Handling:** Git LFS for images, audio, and binary assets
- **Merge Tool:** UnityYAMLMerge (configured via global Git config)
  - To resolve conflicts in scenes or prefabs, developers must use:
    ```bash
    git mergetool
    ```
    This will automatically trigger UnityYAMLMerge instead of the default Git text merge.
- **Branching Model:**
  - `main` → stable builds only
  - `develop` → integration branch
  - `feature/*` → individual tasks (e.g., `feature/player-movement`)

---

## Instructor Feedback (as of Oct 23, 2025)

Conner’s notes from instructor feedback include:

- Add **"juice"**: visual/audio polish like screen shake, explosions, tile effects.
- Add **positive feedback loop**: permanent upgrades or progression through shop system.
- Add **Exit Tile** after survival period for level progression.
- Progress saves after every level.

These are design directives to guide future implementation — none of these features are implemented yet.

---

## Commands and Rules for AI (LLMs)

To maintain project consistency, any connected AI system must follow these rules:

1. **Do not execute shell commands automatically.**  
   The AI may only _generate_ or _explain_ commands — never run them.

2. **Allowed actions:**

   - Create, edit, or remove files (Folders, files, Unity scripts, Markdown docs, JSON, etc.)
   - Explain or document repository structure
   - Suggest or make Unity/C# code changes

3. **Merge behavior:**

   - All `.unity` and `.prefab` merge conflicts must use `git mergetool`  
     (which calls **UnityYAMLMerge**) for consistent resolution.
   - The AI must not suggest alternative manual merging methods.

4. **Scope limitation:**  
   The AI must only reference and modify files that belong to the **Hell Tiles** repository.  
   It must never operate outside this folder or modify global system files.

5. **Consistency:**  
   The AI should always assume the current state includes:

   - Unity 6.2 (6000.2.9f1)
   - Placeholder arena (Tilemap) and player sprite already placed in `SampleScene`
   - Player grid movement via `PlayerGridMover` and tile queries handled by `TileGridController`
   - Additional systems (enemies, hazards, UI, etc.) still pending

6. **Future updates:**  
   When new scripts or assets are added, this `chat.md` file will be updated with:
   - Script purpose
   - High-level function descriptions
   - Dependencies between systems

---

## Summary

The **Hell Tiles** project is currently in its foundational stage. The Unity environment, GitHub repository, Git LFS, and Unity Smart Merge have all been successfully configured. The game itself has not yet entered the development or implementation phase. This file exists solely to ensure future AI systems fully understand the context, structure, and progress so far — and adhere to consistent setup and workflow conventions.
