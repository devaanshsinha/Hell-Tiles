# Chat Context — Hell Tiles

## Current Status

Playable prototype is live. Scene flow now spans **New Game → Tutorial → SampleScene → Game Over → New Game**, plus optional **Shop** and **LevelSelect** scenes, level-based timers, and endless mode. Gameplay features include grid-based hopping with buffered taps, multiple projectile patterns, tile hazards, pickups with persistence, countdown gating, and shop skins/equips stored locally.

---

## Project Summary

**Hell Tiles** is a 2D grid-based bullet-hell built in **Unity 6.2 (6000.2.9f1)**. The player hops between lava-floating tiles, dodges varied projectiles, and collects pickups. Difficulty and feedback will ramp up over time via new tiles, hazards, and “juice”.

---

## Scenes and Flow

- **New Game** (`NewGameSceneController`):  
  - `Space` → `Tutorial` scene.  
  - `Enter` → `LevelSelect` scene.  
  - `S` → `Shop` scene.
- **Tutorial** (`TutorialSceneController`): `Space` starts `SampleScene`.
- **LevelSelect** (`LevelSelectController`): Mario-style path. `A/D` move left/right along bases, `W/S` climb to level nodes, `Enter` loads the selected level, `Escape` returns to New Game. Requires a cursor object, player visual, and an ordered list of base nodes (with optional elevated child nodes for levels).
  - Unlocks: node 0 (origin) always unlocked; level nodes unlock when the previous node is completed; spacer nodes (no level) become unlocked when the previous node is completed/unlocked. Completion flags stored in `hellTiles_level_{index}_completed` (PlayerPrefs).
  - Checkmarks: assign per-level `Completed Mark` GameObject; only real level nodes need them.
  - Message: assign TMP text to `messageLabel`, keep it inactive; shows “Complete previous level first” when blocked.
- **SampleScene**: Core gameplay (see Systems below). On death, `PlayerHealth` loads `Game Over`.
- **Level scenes (5)**: 80s `LevelCountdownTimer` → win goes to “Won” (auto-marks completion), death → “Lost”. `SpaceToScene` on Won/Lost takes you to LevelSelect.
- **Endless**: uses ramping projectile director and hazard scaler for difficulty over time.
- **Game Over** (`GameOverSceneController`): Shows last score and best score; `Space` returns to `New Game`.
- **Shop** (`ShopController`): Five horizontal tiles. `A/D` to change selection, `Enter` to buy (deducts from `CoinWallet`), `E` to equip owned skin, `Escape` returns to `New Game`. Badges: “Owned” and “Equipped” highlights per tile.

---

## Systems in SampleScene

- **Grid, Movement, Input**
  - `TileGridController`: walkable checks, snap positions, bounce effect; exposes random walkable cell queries for spawners.
  - `PlayerGridMover`: WASD taps; uses `WasPressedThisFrame` with a buffered direction while hopping so rapid spam still queues a move. Input only executes when a hop finishes (no long-press multi-step). Provides forced moves for hazards (push).
  - `TileOccupancyManager` + `PlayerOccupancyUpdater`: track which cell the player sits on; hazards/pickups avoid the occupied cell and can reserve a cell while active.
  - `PlayerHealth`: three-heart system, blink/invulnerability, death → Game Over; plays damage SFX.

- **Projectiles**
  - `ProjectileDirector`: central scheduler with global cap (default 50) and multiple tracks.
  - Tracks:
    - `ProjectileSpawner`: straight fireballs toward the player.
    - `HomingProjectileTrack`: homing variant (`BasicProjectile` with `continuousTracking`).
    - `ArrowProjectileTrack`: random-direction arrows between tiles.
    - `ArrowTileTrack`: fires arrows from off-board through a random tile in the [-2..2] grid range and keeps flying straight (uses `DirectionalProjectile` with sprite-forward offset support). Skips occupied/blocked cells.
    - `RowSweepTrack`: spawns a row sweep hazard that telegraphs, enables its hitbox, then despawns; pick rows/columns explicitly. Columns < 0 flip the sprite (fires right), columns ≥ 0 keep default orientation. Cleared by Angel pickup.
  - `BasicProjectile`: moves forward or homes, self-limits via `ProjectileRegistry`.

- **Hazards**
  - `SpikeSpawner` + `SpikeHazard`: blinks twice harmlessly, then stays visible/dangerous for its active window; damages on contact but does not despawn on hit. Avoids occupied cells.
  - `CrackedTileSpawner` + `CrackedTile`: blinks three times to arm, lets the player stand once, then breaks when the player leaves that tile. The tile is removed for a set delay (default ~10s) and restored after the delay, blocking that path while missing. If never stepped on, it auto-restores after a passive lifetime. Avoids occupied cells.
  - `PushTileSpawner` + `PushTileHazard`: blinks in, then immediately pushes the player one tile in its arrow direction on contact using a forced move; ignores pushes that would leave the grid or hit a blocked tile. Pads despawn after their lifetime even if untouched. Avoids occupied cells.
  - `AngelSpawner` + `AngelPickup`: spawns every 15–25s, despawns after 5s if untouched. On pickup: clears all projectiles, resets walkable tiles to original, clears spike/push/cracked hazards, resets player health to full, and plays a short ring VFX + SFX at the pickup spot. Respects occupancy.

- **Pickups**
  - `HeartSpawner` + `HeartPickup`: spawns on random walkable tiles; collectable even at max hearts (wasted). Lives 2.5s, flickers 1.5s, then despawns. Respects occupancy.
  - `CoinSpawner` + `CoinPickup`: animated coin, flickers before despawn. Increments `CoinWallet` (persistent). Respects occupancy.

- **Score & HUD**
  - `SurvivalTimer`: accrues points per second (not minutes/seconds). Best score stored in `GameSessionData` (PlayerPrefs).
  - HUD: score TextMeshPro, hearts bound to `PlayerHealth`, coin counter bound to `CoinWallet`.
  - `CountdownController`: 3-2-1-Go overlay; fades a CanvasGroup, then enables gameplay scripts (player mover, projectile director, pickup spawners, etc.).

- **Difficulty / Scaling**
  - `EndlessProjectileDirector`: ramps each projectile track’s spawn interval over time (Start/End interval multipliers over Ramp Duration seconds).
  - `EndlessHazardScaler`: ramps spawn intervals for spike, push, cracked, heart, and coin spawners over time using the same multiplier curve. Spawners expose `SpawnInterval` so they can be scaled.

- **Audio**
  - `OneShotAudio`: helper to play 2D SFX concurrently (used for angel, coin, damage, etc.).
  - `GameStartAudio`: optional music loop; can delay start until countdown “Go”. Set clip, volume, loop, and start delay; marked `DontDestroyOnLoad`.

---

## Persistence

- `GameSessionData`: wraps PlayerPrefs for best score and coins.
- `CoinWallet`: holds current coins, saves to PlayerPrefs, exposes change events; used by HUD and Shop.
- `ShopController`: saves ownership/equipped skin in PlayerPrefs and auto-owns/equips the default skin.
- `SkinApplier`: applies the equipped skin sprite to the player visuals in gameplay scenes.

---

## Key Scripts (by folder)

- **Tiles:** `TileGridController`, `TileOccupancyManager`.
- **Player:** `PlayerGridMover`, `PlayerHealth`, `PlayerOccupancyUpdater`.
- **Projectiles:** `BasicProjectile`, `ProjectileSpawner`, `HomingProjectileTrack`, `ArrowProjectileTrack`, `ArrowTileTrack`, `RowSweepTrack`, `ProjectileDirector`, `EndlessProjectileDirector`, `ProjectileRegistry`, `IProjectileTrack`, `RowSweepHazard`.
- **Hazards:** `SpikeHazard`, `SpikeSpawner`, `CrackedTile`, `CrackedTileSpawner`, `PushTileHazard`, `PushTileSpawner`.
- **Powerups:** `HeartPickup`, `HeartSpawner`, `CoinPickup`, `CoinSpawner`, `AngelPickup`, `AngelSpawner`.
- **Spawners/Scaling:** `EndlessHazardScaler`.
- **Audio:** `OneShotAudio`, `GameStartAudio`.
- **UI / Flow:** `NewGameSceneController`, `TutorialSceneController`, `GameOverSceneController`, `CountdownController`, `SurvivalTimer`, `CoinCounter`, `CoinWallet`, `GameSessionData`, `ShopController`, `LevelSelectController`, `LevelCompletionTracker`, `LevelCountdownTimer`, `SpaceToScene`, `SkinApplier`.

---

## Implementation Notes

- Layer collision matrix limits projectiles to hitting only the Player layer.
- Spawners query `TileGridController` and `TileOccupancyManager` to avoid blocked/occupied cells.
- Countdown must finish before `ProjectileDirector`, spawners, and player movement are enabled; `GameStartAudio` can be gated to start after “Go”.
- Shop tiles show sprite + price; “Owned” and “Equipped” badges start disabled and are toggled by `ShopController`. Coin label shows just the numeric total.
- LevelSelect requires a serialized list of base nodes (ordered path). Optional elevated child transforms mark level nodes; only those can be “entered”.
- Shop scene should include a coin readout (TMP text) driven by `CoinCounter`/`CoinWallet` to show available currency.

---

## Outstanding Feedback / TODO (latest)

- Add richer tile set (heart/angel/coin pickups, neutral, spike, push, wall, cracked).
- More “juice”: tile bounce, movement effects, projectile spawn/impact effects, near-miss feedback.
- Difficulty scaling on projectiles; delay initial spawns with countdown (partially done).
- Score multiplier for hearts kept; bonus for near-miss; faster overall feel and lenient hitbox.
- Add main menu, tutorial polish, level progression with unique layouts.
