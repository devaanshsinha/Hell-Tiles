# Chat Context — Hell Tiles

## Current Status

The foundational **Hell Tiles** Unity project setup is complete and a playable prototype is live. We now have a four-scene loop (`New Game → Tutorial → SampleScene → Game Over → New Game`). `SampleScene` contains a placeholder tilemap arena, a controllable player that hops between tiles, projectile spawners (standard and optional homing), a three-heart health system with blinking feedback, heart pickups, and a HUD score counter that increments every second. This file keeps AI assistants aligned with real project progress so future work builds on the latest implementation.

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
- Placeholder art added in `Assets/Art/` for background, player, and tile sprites.
- `New Game` scene: minimal UI (TextMeshPro “Press Space” text) with `NewGameSceneController` that loads `Tutorial`.
- `Tutorial` scene: instructions UI plus `TutorialSceneController` (press space to proceed into gameplay).
- `SampleScene.unity` currently includes:
  - `Grid` + Tilemap with a 6×4 walkable layout.
  - Background sprite renderer positioned beneath the board.
  - `Player` GameObject (sprite + `Rigidbody2D`) with grid-based movement (`PlayerGridMover`) and health (`PlayerHealth`, three-heart UI, invulnerability bounce).
  - Projectile director setup:
    - `ProjectileDirector` orchestrates all projectile tracks and enforces a global cap (50 active shots by default).
    - `ProjectileSpawner` track handles the original straight fireballs aimed at the player.
    - `HomingProjectileTrack` spawns the “continuous tracking” variant every ~15 seconds.
    - `ArrowProjectileTrack` launches random arrows between walkable tiles to keep the board busy.
  - `HeartSpawner` manager that periodically instantiates `HeartPickup` prefabs on random walkable tiles; pickups can be collected even at max health (wastes the pickup) and self-despawn after 4s with a flicker warning.
  - `CoinSpawner` for animated coin pickups; coins auto-flicker before despawning and increment a persistent coin wallet.
  - HUD overlay with:
    - Score counter (`SurvivalTimer`) that accrues points per second (stored in `GameSessionData`, PlayerPrefs-backed).
    - Heart icons bound to `PlayerHealth`.
    - Coin counter bound to `CoinWallet` (PlayerPrefs-backed).
    - `CountdownController` overlay that fades out after a 3-2-1-GO countdown, then enables gameplay scripts (player movement, projectile director, pickups, etc.).
- `Game Over` scene: displays last and best survival times via `GameOverSceneController`, waits for spacebar to return to `New Game`.
- UI elements (SampleScene):
  - Canvas with TextMeshPro score counter (points per second) driven by `SurvivalTimer`.
  - Hearts UI panel linked to `PlayerHealth`.
  - Coin counter (`CoinCounter`) showing the persistent value from `CoinWallet`.
  - Fullscreen countdown panel (`CountdownController`) that locks gameplay until the 3-2-1-Go animation completes and then re-enables the projectile director, player mover, and pickup spawners.
- Scene flow:
  - `New Game` scene listens for space to jump into the tutorial (`NewGameSceneController`).
  - `Tutorial` scene listens for space to enter the main gameplay (`TutorialSceneController`).
  - `SampleScene` runs the core loop and loads `Game Over` on death (`PlayerHealth`).
  - `Game Over` scene displays last/best scores and listens for space to return to `New Game` (`GameOverSceneController`).
- Scripts (C#):
  - `Assets/Scripts/Tiles/TileGridController.cs` – grid queries, walkable checks, tile bounce transform animation.
  - `Assets/Scripts/Player/PlayerGridMover.cs` – input-driven tile hopping with landing bounce.
  - `Assets/Scripts/Player/PlayerHealth.cs` – heart tracking, hit blink, scene transition to `Game Over`.
  - `Assets/Scripts/Projectiles/BasicProjectile.cs` – straight-line or homing projectiles (auto-register with director).
  - `Assets/Scripts/Projectiles/ProjectileSpawner.cs`, `HomingProjectileTrack.cs`, `ArrowProjectileTrack.cs` – individual projectile tracks (straight, homing, random arrows).
  - `Assets/Scripts/Projectiles/ProjectileDirector.cs`, `ProjectileRegistry.cs`, `IProjectileTrack.cs` – central coordinator that schedules all projectile tracks and enforces a max bullet count.
  - `Assets/Scripts/Powerups/HeartPickup.cs`, `HeartSpawner.cs`, `CoinPickup.cs`, `CoinSpawner.cs` – timed pickups and spawn management for hearts/coins.
  - `Assets/Scripts/UI/NewGameSceneController.cs`, `TutorialSceneController.cs`, `GameOverSceneController.cs`, `CountdownController.cs`, `CoinCounter.cs`, `CoinWallet.cs`, `GameSessionData.cs`, `SurvivalTimer.cs` – scene flow, countdown, persistent coins/score HUD (score and coin totals are stored via PlayerPrefs).
- Input: `Assets/InputSystem_Actions.inputactions` (default template) supplies the `Move` action bound to keyboard/gamepad.
- Layer collisions configured so projectiles only hit the player.

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

Structure is now populated with placeholder art, prototype scripts, and initial UI scenes.

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

Conner’s updated notes include previously listed directives plus the latest to-do items:

- Implement tile system with positive/neutral/negative tiles (heart, angel, coin pickups; neutral tile; spike, push, wall, cracked tiles).
- Randomly replace tiles with “juice” effects; add bounce/juice when stepping on tiles and during player movement.
- Add instructions/tutorial level and a main menu.
- Add hi-score tracking based on survival time; show hi-score/time on Game Over.
- Add score multiplier for hearts kept and bonus points for near misses.
- Add projectile difficulty scaling and “juice” for projectile spawning, near misses, and player impacts.
- Make hitbox more lenient, overall pace faster, and delay projectile spawning with an opening countdown.
- Add level progression with unique layouts that introduce mechanics gradually.
- Ensure positive feedback is immediate and noticeable.

Most of these directives are pending implementation; focus is on incrementally adding systems while preserving current prototype functionality.

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
