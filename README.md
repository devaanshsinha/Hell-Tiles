# Hell Tiles

Hell Tiles is a 2D pixel-art, tile-based bullet-hell survival game built in Unity 6.2 (6000.2.9f1). Players hop between floating tiles on a lava grid while dodging enemy projectiles and surviving as long as possible. Each tile behaves differently, and the environment becomes more chaotic over time. The goal is to survive waves, collect coins, and buy permanent upgrades to push further with each run.

## Gameplay Overview

- Grid-Based Movement: Players tap keys to jump tile-to-tile, creating a snappy, Crossy Road-style feel.
- Dynamic Tiles: Normal, Spike, Sticky, Health, Push, Wall, and Coin tiles—each adds new hazards or opportunities.
- Projectiles: Enemies attack in patterns and random bursts that grow faster and more frequent.
- Juice and Feedback: Blinking tiles, screen shake, dust/lava particles, bounce effects, afterimages, and sound design make actions feel responsive and impactful.
- Progression: Survive longer to earn coins; spend them on permanent upgrades (health, movement speed, visuals).
- Exit Tile: After surviving a level’s timer, an Exit Tile appears. Stepping on it transitions the player to the next stage.

## Team

| Role | Member |
|------|---------|
| Producer | Conner |
| Programmer | Devaansh |
| Artist | Riley |

## Technology

- Engine: Unity 6.2 (6000.2.9f1)
- Language: C#
- Version Control: Git + Git LFS (Large File Storage)
- Merge Tool: UnityYAMLMerge for scene/prefab conflict resolution
- Art Style: 16-bit pixel art, lava-themed environment
- Audio: Lava ambience, impact effects, fast-paced loops

## Project Structure

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

This structure is ready for future asset and script creation. Currently, only setup and configuration have been completed.

## Getting Started

This section explains how to properly clone and open the project so all team members can begin working without setup issues.

### 1. Install Prerequisites

- Install **Unity Hub** and Unity **6.2 (6000.2.9f1)**.
- Install **Git** and **Git LFS**:
  ```bash
  brew install git git-lfs
  git lfs install
  ```

### 2. Clone the Repository

Clone the project using Git LFS:
```bash
git clone https://github.com/<username>/hell-tiles.git
cd "Hell Tiles"
git lfs install
```

### 3. Open in Unity Hub

1. Open Unity Hub.
2. Click "Add project from disk" and select the `Hell Tiles` folder.
3. Ensure Unity 6.2 (6000.2.9f1) is selected.
4. Open the project.

### 4. Configure Unity Smart Merge (one-time setup)

Run these commands in the terminal:
```bash
git config --global merge.tool unityyamlmerge
git config --global mergetool.unityyamlmerge.cmd "/Applications/Unity/Hub/Editor/6000.2.9f1/Unity.app/Contents/Tools/UnityYAMLMerge merge -p \"$BASE\" \"$REMOTE\" \"$LOCAL\" \"$MERGED\""
git config --global mergetool.unityyamlmerge.trustExitCode true
```
To confirm it is configured correctly:
```bash
git config --global --get mergetool.unityyamlmerge.cmd
```
If you see the UnityYAMLMerge path printed, setup is complete.

### 5. Branching and Workflow

- Always create a new branch for your work:
  ```bash
  git checkout -b feature/<feature-name>
  ```
- Push your branch and open a Pull Request into `develop`.
- After review and testing, `develop` will merge into `main`.
- Never commit or push directly to `main`.
- Use small, descriptive commits.

### 6. Resolving Merge Conflicts

If a conflict occurs in a `.unity` or `.prefab` file, run:
```bash
git mergetool
```
This will automatically launch Unity’s Smart Merge (UnityYAMLMerge). Do not manually edit `.unity` or `.prefab` files to resolve conflicts.

## Development Workflow

1. Work only on assigned features or assets.
2. Test all changes in Unity before pushing.
3. Keep scene and prefab edits minimal to avoid merge conflicts.
4. Communicate with teammates about shared assets.

## Milestones

| Week | Goals |
|------|--------|
| 1 | Unity and Git setup, assign team roles |
| 2 | MVP: Player movement, tile grid, basic hazards |
| 3 | Add juice: particles, sounds, effects, enemy patterns |
| 4 | Add shop, coins, upgrades, and level transitions |
| 5 | Polish, bug fixes, and presentation |

## Instructor Feedback (as of Oct 23, 2025)

- Add visual and auditory juice (screen shake, particles, sounds).
- Add a positive feedback loop through a permanent shop upgrade system.
- Add an Exit Tile for level progression.
- Save progress after every level.
