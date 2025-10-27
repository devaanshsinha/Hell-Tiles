# Chat Context — Hell Tiles

## Current Status

The **Hell Tiles** Unity project has been successfully set up and version-controlled. No gameplay or scripting work has been done yet — the team is currently in the setup phase. The focus so far has been establishing the project structure, Git repository, Unity configuration, and team workflow. This file is used to keep AI assistants (like ChatGPT, Copilot, or other LLMs) consistent with the project's real progress and context.

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

- Unity project created locally (version **6.2 / 6000.2.9f1**).
- Git repository initialized locally, linked to GitHub (empty repo created first).
- `.gitignore` and `.gitattributes` files added for Unity and Git LFS tracking.
- Git LFS installed and initialized for binary asset handling.
- **Unity Smart Merge (YAMLMerge)** configured globally to handle scene/prefab merge conflicts.
- Verified correct tool path:  
  `/Applications/Unity/Hub/Editor/6000.2.9f1/Unity.app/Contents/Tools/UnityYAMLMerge`
- Team roles assigned:
  - Conner → Producer
  - Devaansh → Programmer
  - Riley → Artist

No gameplay, prefabs, or scripts have been created yet — only version control and environment setup are complete.

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

This structure is ready for asset and script creation but currently empty.

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
   The AI should always assume the current state is limited to:

   - Unity 6.2 (6000.2.9f1)
   - Repository setup complete, but no gameplay or assets yet
   - Scripts, prefabs, and assets will be added later by the team

6. **Future updates:**  
   When new scripts or assets are added, this `chat.md` file will be updated with:
   - Script purpose
   - High-level function descriptions
   - Dependencies between systems

---

## Summary

The **Hell Tiles** project is currently in its foundational stage. The Unity environment, GitHub repository, Git LFS, and Unity Smart Merge have all been successfully configured. The game itself has not yet entered the development or implementation phase. This file exists solely to ensure future AI systems fully understand the context, structure, and progress so far — and adhere to consistent setup and workflow conventions.

