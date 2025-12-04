# Chat Context — Hell Tiles

## Current Status

Playable prototype is live. Scene flow now spans **New Game → Tutorial → SampleScene → Game Over → New Game**, plus optional **Shop** and **LevelSelect** scenes. Gameplay features include grid-based hopping on a tilemap, multiple projectile patterns, hearts and coin pickups with persistence, countdown gating, and a points-per-second score system. Shop purchases and equipped skin selection are stored locally.

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
- **SampleScene**: Core gameplay (see Systems below). On death, `PlayerHealth` loads `Game Over`.
- **Game Over** (`GameOverSceneController`): Shows last score and best score; `Space` returns to `New Game`.
- **Shop** (`ShopController`): Five horizontal tiles. `A/D` to change selection, `Enter` to buy (deducts from `CoinWallet`), `E` to equip owned skin, `Escape` returns to `New Game`. Badges: “Owned” and “Equipped” highlights per tile.

---

## Systems in SampleScene

- **Grid & Movement**
  - `TileGridController`: walkable checks, snap positions, tile bounce on land/step.
  - `PlayerGridMover`: WASD/grid input to hop tile-to-tile with hop curve and bounce.
  - `PlayerHealth`: three-heart system, blink/invulnerability, death → Game Over.

- **Projectiles**
  - `ProjectileDirector`: central scheduler with global cap (default 50) and multiple tracks.
  - Tracks:
    - `ProjectileSpawner`: straight fireballs toward the player.
    - `HomingProjectileTrack`: homing variant (uses `BasicProjectile` with `continuousTracking`).
    - `ArrowProjectileTrack`: random-direction arrows between tiles.
    - `ArrowTileTrack`: fires arrows from off-board through a random tile in the [-2..2] grid range and keeps flying straight (uses `DirectionalProjectile` with sprite-forward offset support).
    - `RowSweepTrack`: spawns a row sweep hazard that telegraphs, enables its hitbox, then despawns; pick rows/columns explicitly. Columns < 0 flip the sprite (fires right), columns ≥ 0 keep default orientation. Cleared by Angel pickup.
  - `BasicProjectile`: moves forward or homes, self-limits via `ProjectileRegistry`.

- **Hazards**
  - `SpikeSpawner` + `SpikeHazard`: blinks twice harmlessly, then stays visible/dangerous for its active window; damages on contact but does not despawn on hit.
  - `CrackedTileSpawner` + `CrackedTile`: blinks three times to arm, lets the player stand once, then breaks when the player leaves that tile. The tile is removed for a set delay (default ~10s) and restored after the delay, blocking that path while missing. If never stepped on, it auto-restores after a passive lifetime.
  - `PushTileSpawner` + `PushTileHazard`: blinks in, then immediately pushes the player one tile in its arrow direction on contact. Direction set per instance (Up/Down/Left/Right) with matching sprites; pads despawn after their lifetime even if untouched.
  - `AngelSpawner` + `AngelPickup`: spawns every 15–25s, despawns after 5s if untouched. On pickup: clears all projectiles, resets walkable tiles to their original layout, clears spike/push/cracked hazards, resets player health to full, and plays a short ring VFX at the pickup spot.

- **Pickups**
  - `HeartSpawner` + `HeartPickup`: spawns on random walkable tiles; collectable even at max hearts (wasted). Lives 2.5s, flickers 1.5s, then despawns.
  - `CoinSpawner` + `CoinPickup`: animated coin, flickers before despawn. Increments `CoinWallet` (persistent).
  - Spawn positions use `TileGridController` walkable queries.

- **Score & HUD**
  - `SurvivalTimer`: accrues points per second (not minutes/seconds). Best score stored in `GameSessionData` (PlayerPrefs).
  - HUD: score TextMeshPro, hearts bound to `PlayerHealth`, coin counter bound to `CoinWallet`.
  - `CountdownController`: 3-2-1-Go overlay; fades a CanvasGroup, then enables gameplay scripts (player mover, projectile director, pickup spawners, etc.).

---

## Persistence

- `GameSessionData`: wraps PlayerPrefs for best score and coins.
- `CoinWallet`: holds current coins, saves to PlayerPrefs, exposes change events; used by HUD and Shop.
- `ShopController`: saves ownership/equipped skin in PlayerPrefs.

---

## Key Scripts (by folder)

- **Tiles:** `TileGridController` (walkable queries, bounce).
- **Player:** `PlayerGridMover` (hops), `PlayerHealth` (hearts, blink, death).
- **Projectiles:** `BasicProjectile`, `ProjectileSpawner`, `HomingProjectileTrack`, `ArrowProjectileTrack`, `RowSweepTrack`, `ProjectileDirector`, `ProjectileRegistry`, `IProjectileTrack`, `RowSweepHazard`.
- **Hazards:** `SpikeHazard`, `SpikeSpawner`, `CrackedTile`, `CrackedTileSpawner`.
- **Powerups:** `HeartPickup`, `HeartSpawner`, `CoinPickup`, `CoinSpawner`.
- **UI / Flow:** `NewGameSceneController`, `TutorialSceneController`, `GameOverSceneController`, `CountdownController`, `SurvivalTimer`, `CoinCounter`, `CoinWallet`, `GameSessionData`, `ShopController`, `LevelSelectController`, `LevelCompletionTracker` (marks level complete; can return to LevelSelect), `LevelCountdownTimer` (auto-marks completion via tracker and loads Win scene on timeout).

---

## Implementation Notes

- Layer collision matrix limits projectiles to hitting only the Player layer.
- All projectiles and pickups respect the walkable tilemap from `TileGridController`.
- Countdown must finish before `ProjectileDirector`, spawners, and player movement are enabled.
- Shop tiles show sprite + price; “Owned” and “Equipped” badges start disabled and are toggled by `ShopController`.
- LevelSelect requires a serialized list of base nodes (ordered path). Optional elevated child transforms mark level nodes; only those can be “entered”.
- Shop scene should include a coin readout (TMP text) driven by `CoinCounter`/`CoinWallet` to show available currency.

---

## Outstanding Feedback / TODO (latest)

- Add richer tile set (heart/angel/coin pickups, neutral, spike, push, wall, cracked).
- More “juice”: tile bounce, movement effects, projectile spawn/impact effects, near-miss feedback.
- Difficulty scaling on projectiles; delay initial spawns with countdown (partially done).
- Score multiplier for hearts kept; bonus for near-miss; faster overall feel and lenient hitbox.
- Add main menu, tutorial polish, level progression with unique layouts.
