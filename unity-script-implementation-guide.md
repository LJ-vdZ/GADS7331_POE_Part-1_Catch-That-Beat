# Unity C# Script Implementation Guide

This guide explains how to add and configure generated C# scripts in Unity for **Catch That Beat!**.

## 1) Create Script Folders
In Unity `Assets`, create:
- `Assets/Scripts/Player`
- `Assets/Scripts/AI`
- `Assets/Scripts/Gameplay`
- `Assets/Scripts/UI`
- `Assets/Scripts/Audio`

Keep scripts grouped by role for easier maintenance.

## 2) Add C# Scripts
1. In Unity, right-click a target script folder.
2. Select **Create > C# Script**.
3. Name the script exactly as required by the class name.
4. Open script in Cursor/IDE and paste generated code.
5. Save the file and return to Unity to compile.

## 3) Create Required GameObjects
Create key scene objects before attaching scripts:
- `Player` (**recommended first-person rig:** one GameObject with `CharacterController`, **`Camera`**, and `PlayerControllerFPS`—mouse look rotates this object so the camera *is* the player. Alternative: parent body + child camera—then assign **Camera Pivot** on the script.)
- `Droid` (with model, collider, and AI scripts)
- `GameManager` (round logic, timer, win/lose flow)
- `UIManager` (HUD and end-state panels)
- `AudioManager` (global audio handling if used)

## 4) Attach Scripts to GameObjects
1. Select GameObject in the Hierarchy.
2. In Inspector, click **Add Component**.
3. Search for script class name.
4. Add and assign required references in exposed fields.

Exact script attachment map:
- `PlayerControllerFPS.cs` -> Attach to `Player` (with `CharacterController`; add **`Camera` on this same object** for standard FP, or use a child camera + **Camera Pivot** field)
- `DroidAIController.cs` -> Attach to `Droid` (with `NavMeshAgent`)
- `CatchDetector.cs` -> Attach to `Droid` catch trigger child (set as `Is Trigger`)
- `AudioTrackerFeedback.cs` -> Attach to `Droid` (same object as `AudioSource`)
- `RoundManager.cs` -> Attach to `GameManager` (empty manager object)
- `GameUIController.cs` -> Attach to `UIManager` or `Canvas` controller object

Required helper objects for references:
- `PlayerSpawnPoint` and `DroidSpawnPoint` -> Empty transforms in scene
- **Hide / room anchor points** -> Empty transforms marking valid spots or rooms where the droid may **wait** (random pick at start and after each escape). The sample `DroidAIController` still names this array `roamPoints` in code; treat them as **candidate hide locations**, not “always patrolling” waypoints once you refactor AI to match the design (see `plan.md` / `refinements-changes.md`).
- `UI Text Elements` -> Timer, catches, and state labels
- `EndPanel` -> Win/lose panel object, disabled by default

### Droid behavior (design target)
**Intended loop:** random **hide** → **wait** for player → **flee** until player is **lost** → **new hide**. This is harder and more dynamic than **continuous roaming**, which can make accidental finds too common and reduce fun. Implement in `DroidAIController` (or successor) with states such as hiding, fleeing, and relocating; tune detection (e.g. line-of-sight / distance) for “lost player.”

## 5) Configure Serialized Fields
For each script, set all serialized fields in Inspector:
- References (camera, target transforms, UI text, audio source)
- Gameplay values (move speed, timer seconds, catches to win)
- Audio values (min/max distance, occlusion checks, volume curve)

Tip: Start with conservative values, then tune during playtests.

### Input System (fixes `InvalidOperationException` on player)
If Unity **Player Settings → Active Input Handling** is set to **Input System Package** (new) only, code that calls `UnityEngine.Input.GetAxis`, `GetKey`, or `GetButton` throws:

`InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package`

This project’s player scripts avoid legacy `Input` and use the **Input System** instead:
- Install the **Input System** package (**Window → Package Manager**).
- `PlayerControllerFPS.cs` uses **WASD** via `Keyboard.current.wKey` / `aKey` / `sKey` / `dKey` (same idea as `GetKey("w")` style checks).
- **Mouse** movement drives look (`Mouse.current.delta`). If **`Camera`** is on the **same** GameObject as `CharacterController` at **Play**, the script spins up an **`FPSCameraRig`** child, moves the camera there, and uses **body yaw + rig pitch** (so **Camera Local Offset** applies). Otherwise yaw is on the body and pitch on **Camera Pivot** (or Main Camera fallback).
- **`Camera Local Offset`** (`PlayerControllerFPS`): Inspector **`Vector3`** for the camera rig’s **local position**—mainly **Y** for **eye height**; tweak per character scale and capsule center.
- **Mouse Sensitivity** (`PlayerControllerFPS`): tune in the Inspector. In this project, sensitivity was **lowered** from the default in the scene/prefab because the original value made look **too sensitive** and uncomfortable during play.
- **Hold Left Shift or Right Shift** to **sprint** (faster than walk; disabled while crouching or standing still).
- **Jump** defaults to **Space**; crouch defaults to **Left Ctrl** (serialized **Key** fields).
- `CatchDetector.cs` uses `Keyboard.current[catchKey]` (default **E**), not `KeyCode` + `Input.GetKeyDown`.

You may still use **Both** (old + new) in Player Settings if you prefer; these scripts work with the new API either way.

## 6) Configure Physics and Navigation
- Add colliders to level geometry and droid/player objects.
- Bake NavMesh for droid movement if AI uses navigation.
- Confirm layers/tags are set correctly (e.g., `Player`, `Droid`, `Obstacle`).

## 7) Hook Up UI
- Create a Canvas with timer, catch count, and message text.
- Connect UI elements to the UI script via Inspector.
- Add win/lose panels and disable them by default.

### Optional: Wandering UI Buttons
If you want the three menu/buttons (e.g., “Restart”, “Quit”, etc.) to move subtly:
1. Attach `UIButtonWanderer` to each button GameObject (must have a `RectTransform`).
2. Tune movement to be slow and random:
   - Set `Min Delay Seconds` / `Max Delay Seconds` to control how often they jump.
   - Set `Move Duration Seconds` to control how quickly they glide.
   - Set `Max Offset` to control how far they move.
3. (Recommended) Leave `Boundary` empty to use the button’s parent RectTransform.

Notes:
- If your UI buttons are under a `LayoutGroup` (Grid/Vertical/Horizontal), movement may be overridden. Consider disabling layout for those buttons or moving them outside the layout.

## 8) Configure Audio Tracking
- Add `AudioSource` to droid object.
- Enable 3D sound (spatial blend to 1.0).
- Set rolloff distances and test sound direction from multiple rooms.
- If script supports occlusion, ensure obstacles are on correct layers.

**Design note (no proximity pitch):** The project intentionally **does not** raise or lower music **pitch** as the player gets closer to the droid—that was removed to avoid fatiguing or irritating players over long listens. Use **volume**, **spatial audio**, and **low-pass / occlusion** cues for locating the droid. If you use `AudioTrackerFeedback.cs`, leave pitch neutral by setting **Min Pitch** and **Max Pitch** both to **1** in the Inspector (scripts stay as generated; this is a configuration choice).

## 9) Test in Play Mode
Run the scene and validate:
- Player movement and camera look
- Droid behavior: hide → discovered → flee → relocate (once AI matches design)
- Catch detection and timer resets
- Win after 3 catches, lose on timer expiry
- Audio clues remain clear and directional

## 10) Debug and Refine
If something fails:
1. Check Unity Console for compile/runtime errors.
2. Confirm script name equals class name.
3. Confirm all Inspector references are assigned.
4. Verify required components exist on the same GameObject.
5. Re-test after each fix.

## 11) Keep Documentation Updated
Whenever scripts or gameplay behavior change:
- Update `plan.md` if timeline/scope changes
- Update `requirements.txt` if dependencies change
- Add an entry in `refinements-changes.md`
- Update this guide if setup steps changed
