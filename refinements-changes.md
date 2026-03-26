# Refinements and Changes Log

This running log is maintained in Cursor to track scope shifts, design decisions, and implementation refinements during development.

## Change Log

### 2026-03-24
- Created initial documentation set based on the concept document:
  - `plan.md`
  - `README.md`
  - `requirements.txt`
  - `unity-script-implementation-guide.md`
- Locked initial scope:
  - First-person apartment chase
  - Audio-led tracking mechanic
  - 3 catches to win, timer expiry to lose
- Confirmed AI tools in use:
  - ChatGPT
  - Grok
  - Cursor
- Added initial Unity C# gameplay script set under `Scripts/`:
  - `PlayerControllerFPS.cs`
  - `DroidAIController.cs`
  - `RoundManager.cs`
  - `CatchDetector.cs`
  - `AudioTrackerFeedback.cs`
  - `GameUIController.cs`
- Design decision:
  - Implemented a modular architecture (player, AI, game-state, UI, audio) to simplify testing and iterative balancing.
- AI usage refinement:
  - Added Producer AI as a confirmed tool for generating the game's music tracks.
- Documentation and code annotation refinement:
  - Updated `unity-script-implementation-guide.md` with exact script-to-GameObject attachment map.
  - Added short inline comments to each line in all current C# scripts for clarity.
- **Audio UX:** Removed **proximity-based pitch change** on the droid chase music (pitch shifting as the player gets close). That was dropped so players are not irritated by constant pitch warble over a full session; tracking still uses **spatial volume**, **3D positioning**, and **occlusion-style filtering**, with round escalation via other means (e.g. tempo, droid speed)—not distance-based pitch. Documented in `README.md`, `plan.md`, and `unity-script-implementation-guide.md`; for `AudioTrackerFeedback`, use neutral pitch in Inspector if needed.
- **Input / Player Settings:** `InvalidOperationException` when using **`UnityEngine.Input`** while **Active Input Handling** is **Input System Package** only (e.g. `GetAxis("Mouse X")` in `PlayerControllerFPS`). **Fix:** Refactored `PlayerControllerFPS.cs` to **Input System** (`UnityEngine.InputSystem`) with explicit **WASD** key checks, mouse **delta** look, and **Key**-based jump/crouch. Updated `CatchDetector.cs` similarly so **E** uses `Keyboard.current`. Documented in `README.md`, `requirements.txt`, and `unity-script-implementation-guide.md`.
- **Player controls / FP rig:** **Mouse** for look; **hold Shift** (Left or Right) to **sprint**; **Space** jump; crouch unchanged. **First-person setup:** `Camera` on the **same** GameObject as `CharacterController` + `PlayerControllerFPS` so the camera **is** the player view (unified yaw/pitch). Child-camera option still supported via **Camera Pivot**. Documented and implemented in `PlayerControllerFPS.cs`, `README.md`, `unity-script-implementation-guide.md`.
- **Mouse look sensitivity:** The default **Mouse Sensitivity** on `PlayerControllerFPS` felt **too fast** in playtesting, so the value was **turned down in the Unity Inspector** on the player prefab/instance (script source left as-is). See `README.md` and `unity-script-implementation-guide.md`.
- **Camera height (Inspector):** `PlayerControllerFPS` exposes **`Camera Local Offset`** (`Vector3`, default higher **Y** for eye level). It is applied each frame to the camera rig’s **local position** so developers can tune eye height without code changes. If **`Camera`** starts on the **same** GameObject as `CharacterController`, **Play** creates an **`FPSCameraRig`** child, moves the camera there, copies core camera settings, and uses that pivot for pitch—so the offset always works. Manual child-camera setups still respect **Camera Pivot** + the same offset field.

### 2026-03-25
- **Droid AI (design refinement):** Shifted away from **constant roaming**. Target behavior: at game (or round) start the droid **chooses a random hide location/room** and **stays there** until the player **finds** it; then it **runs away** until it **loses** the player, then **stops at the next random hide spot** and waits again. **Rationale:** pure roaming makes the droid easier to bump into by accident and can make rounds feel **repetitive or too easy**; **hide-and-seek + chase** keeps pacing **fun and dynamic**. Documented in `plan.md`, `README.md`, and `unity-script-implementation-guide.md`. **Note:** `DroidAIController.cs` may still reflect older roam/dash logic until refactored to match this design.
- Added `UIButtonWanderer` script for optional playful, slow, random UI button motion.
- AI workflow refinement: Stopped using Cursor for Unity script generation/debugging and switched Unity scripting support to Grok for better control and understanding of script behavior during implementation/debug.
- AI tools refinement: Added Tripo 3D as a used AI tool for droid character modeling.
- Implementation progress update: Imported and implemented scripts in Unity; modeled and implemented droid via Tripo 3D; integrated purchased Unity Asset Store 3D environment due Tripo limitation on full environment generation; implemented Main Menu, Gameplay, Win and Lose UI; completed debugging/testing pass; produced short demonstration video.

### 2026-03-26
- Final day execution plan logged: polish UI (missing Main Menu, Retry, Resume buttons), refine/debug scripts (droid orientation/behavior, game rounds, jump controls, and cursor handling for gameplay UI), then produce final showcase video, reflection, and AI Ethics Statement.
- Difficulty refinement: Each round, the time the player has to find the droid becomes **shorter**, increasing challenge in a fun way and reducing repetitive “easy finds.”
- Documentation update: Added an **AI Ethics Statement** section to `README.md` based on your provided Word document, summarising originality and fair use.
- Documentation update: Added two short reflection sections to `plan.md` and `README.md`, based on your provided `Reflection Prompts` notes (AI collaboration, originality, and fair use).
- Documentation correction: Replaced reflections in `plan.md` and `README.md` with an updated ~200-word reflection that answers all seven prompts from `Reflection Prompts` notes.

## Decision Record Template

Use this format for every meaningful refinement:

Date: YYYY-MM-DD  
Area: (Gameplay / Audio / UI / Art / Performance / Scope / Tech)  
Change: Brief description of what changed  
Reason: Why this was changed  
Impact: What this affects (scripts, scenes, timeline, difficulty, etc.)  
Action Taken: What was implemented/documented

---

Example Entry:

Date: 2026-03-25  
Area: Audio  
Change: Increased droid audio minimum volume at medium range  
Reason: Player feedback showed sound was too hard to locate in hallway corners  
Impact: Easier tracking, slightly reduced difficulty  
Action Taken: Updated audio attenuation curve and playtest notes
