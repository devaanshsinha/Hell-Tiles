# Hell Tiles

Hell Tiles is a 2D grid-based bullet-hell built in Unity 6.2 (6000.2.9f1). Players tap to hop tile-to-tile, dodge hazards, collect pickups, and survive either endless mode or level-based timers.

## Current Features
- Grid hopping with buffered input: one hop per key press; buffered taps fire as soon as a hop ends.
- Tile systems: walkable map with spike, push, cracked tiles (telegraph/arm/break), and occupancy manager to prevent overlapping spawns or spawning on the player.
- Hazards: straight fireballs, homing shots, random arrows, row sweeps, and push pads; endless mode has ramping spawn intervals.
- Pickups: hearts (health), coins (persistent wallet), angel (clears hazards/projectiles, restores tiles/health), with timed despawn/flicker.
- Audio: one-shot SFX for damage, coin, angel; start audio helper (optional loop); sounds use 2D one-shots so they stack over music.
- Shop: first skin auto-owned/equipped; other skins purchasable/equippable; selector highlight forced visible (WebGL-safe); coin label shows only the count.
- Level select: ordered nodes array; origin unlocked; levels unlock only after the previous level is completed; spacer nodes just unlock; checkmarks per level; blocking message “Complete previous level first.”
- Levels: 80s countdown (`LevelCountdownTimer`) auto-marks completion via `LevelCompletionTracker` and loads “Won.” Death loads “Lost.” Space in Won/Lost goes to LevelSelect (`SpaceToScene`).
- Endless: `EndlessProjectileDirector` ramps projectile spawn intervals; `EndlessHazardScaler` ramps hazard/pickup intervals independently (endless only).

## Scenes & Flow
- New Game → LevelSelect / Shop / Tutorial → SampleScene or level scenes → Won/Lost → LevelSelect.
- Endless scene uses the endless directors/scalers; level scenes use fixed timers.

## Setup (quick)
1. Unity Hub + Unity 6.2 (6000.2.9f1). Install Git + Git LFS (`git lfs install`).
2. Clone: `git clone <repo> && cd "Hell Tiles" && git lfs install`.
3. Open in Unity Hub (Editor 6000.2.9f1).
4. Smart Merge (once):
   ```bash
   git config --global merge.tool unityyamlmerge
   git config --global mergetool.unityyamlmerge.cmd "/Applications/Unity/Hub/Editor/6000.2.9f1/Unity.app/Contents/Tools/UnityYAMLMerge merge -p \"$BASE\" \"$REMOTE\" \"$LOCAL\" \"$MERGED\""
   git config --global mergetool.unityyamlmerge.trustExitCode true
   ```

## Controls
- Movement: WASD/Arrow keys (one hop per press; buffered if pressed during a hop).
- Interact: contextual (e.g., LevelSelect Enter, Space to advance in New Game, Won/Lost to LevelSelect).
- Push tiles: instantly shove to their target tile if walkable; otherwise no move.

## Key Scripts
- Player: `PlayerGridMover` (hops + buffering), `PlayerHealth` (hearts, damage SFX), `SkinApplier`, `PlayerOccupancyUpdater`.
- Tiles/Hazards: `TileGridController`, `TileOccupancyManager`, `Spike/Push/Cracked` spawners & hazards, `RowSweepHazard/Track`.
- Projectiles: `BasicProjectile`, `DirectionalProjectile`, `ProjectileSpawner`, `HomingProjectileTrack`, `ArrowProjectileTrack`, `ArrowTileTrack`, `RowSweepTrack`, `ProjectileDirector`, `EndlessProjectileDirector`, `ProjectileRegistry`.
- Powerups: `Heart/Coin/Angel` pickups & spawners, `EndlessHazardScaler`.
- UI/Flow: `NewGameSceneController`, `TutorialSceneController`, `GameOverSceneController`, `CountdownController`, `SurvivalTimer`, `LevelCountdownTimer`, `LevelCompletionTracker`, `SpaceToScene`, `ShopController`, `LevelSelectController`, `GameStartAudio`.
- Audio: `OneShotAudio` helper for stacked 2D SFX.

## Notes
- Assign audio clips: angel/coin/damage on their scripts; start audio via `GameStartAudio` (set delay/loop). Looping music uses the loop option.
- Assign Layout Root in ShopController for WebGL highlight; ensure selector highlights and checkmarks are wired in the scene instance.
- Occupancy: add `TileOccupancyManager` once per scene, and `PlayerOccupancyUpdater` on the player; spawners find/use it if present.
