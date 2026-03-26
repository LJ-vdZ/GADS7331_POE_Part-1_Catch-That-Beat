# Catch That Beat! - Unity Project

## Overview
**Catch That Beat!** is a first-person 3D game in a cozy modern apartment that mixes **hide-and-seek** with an **audio-driven chase**.  
A mischievous social droid steals the player's AirPods. **Design intent:** the droid **starts at a random room/location** and **waits** (leaking spatial audio) until you find it; when spotted it **runs away** until it **loses you**, then **hides again** at a **new** random spot—so you get tension from **searching**, then **pursuit**, not a droid that **constantly roams** (which can make finds too frequent and pacing flat).

Core gameplay goals:
- Use directional 3D sound to locate the droid while it is hiding or fleeing
- Navigate obstacles and room layouts quickly during chases
- Catch the droid 3 times in a row to win

**Audio design:** Proximity-based **pitch** changes on the chase music are **not** used—continuous pitch shifting was dropped to keep listening comfortable; **volume, spatial position, and filtering** still support tracking. Details are in `refinements-changes.md`.

## Installation / Setup

### Requirements
- Unity Hub (latest stable)
- Unity Editor (LTS recommended)
- Windows 10/11

### Project Setup Steps
1. Open Unity Hub.
2. Click **Open** and select this project folder.
3. Install any missing Unity Editor version requested by the project.
4. Open the main scene (for example: a scene under `Assets/Scenes` once created).
5. Press **Play** to run in the Editor.

## Run Instructions
1. Ensure audio output (headphones preferred) is enabled for directional sound testing.
2. Install the **Input System** package if prompted, and use **Active Input Handling**: Input System Package (or Both)—see `unity-script-implementation-guide.md` for the error this avoids.
3. Start the game scene from Unity Editor.
4. **Controls:** **W A S D** move, **mouse** look, **hold Shift** sprint (Left or Right), **Space** jump, **Left Ctrl** crouch (`PlayerControllerFPS`), **E** catch when in range (`CatchDetector`). For **first person**, put the **Camera** on the **same** GameObject as `CharacterController` + `PlayerControllerFPS` (or use a child camera). **Play** may create **`FPSCameraRig`** and move the camera so **Camera Local Offset** (eye height) works; tune **Y** in the Inspector. **Mouse look** used to feel **too sensitive** at the script default, so **Mouse Sensitivity** on the player was **reduced in the Inspector** (see `refinements-changes.md`).
5. Catch the droid three times before the round timer reaches zero (each catch may reset tension: find → chase → re-hide). Each round the available time becomes **shorter**, increasing the challenge.

## Documentation Files
- `plan.md` - development milestones, AI tools, and task list
- `requirements.txt` - dependencies/libraries/plugins list
- `refinements-changes.md` - running log for design/scope changes
- `unity-script-implementation-guide.md` - script integration steps in Unity

## AI Tools Used
This project has used the following AI tools:
- ChatGPT
- Grok
- Cursor
- Producer AI (music track generation)
- Tripo 3D (droid character modeling)

AI assistance included:
- Concept ideation and wording refinement
- Planning and documentation support
- Unity C# scripting and implementation guidance via Grok
- Documentation maintenance support via Cursor (no Unity scripting)

## Credits
- Project concept and implementation: Student developer
- AI support tools: ChatGPT, Grok, Cursor (documentation only), Producer AI, Tripo 3D
- Engine: Unity (C#)

## AI Ethics Statement
The purpose of this statement is to highlight compliance with originality and fair use of AI tools used in this project.

Originality:
- The game’s core concept, gameplay mechanics, story, characters, and overall vision are entirely my own work.
- The main game scripts/logic were developed by me; any AI-generated scripts/assets were treated as supportive starting points and were reviewed and adjusted to fit the Unity setup and the project vision.
- The game environment was purchased from the Unity Asset Store, and the associated terms/usage conditions were followed.

Compliance & Fair Use:
- AI tools were accessed only through legitimate means and in accordance with their terms of service.
- I did not share personal data with AI systems.
- AI was used to support creation (scripts, 3D assets, sound/music, UI, and background art) and debugging, while ensuring that final creative decisions and implementations were made by me.
- For transparency, my use of AI tools was disclosed in the project’s video demonstration.

Overall, AI served as a helpful, productive tool to assist learning and creation rather than replacing my own creative work.

## Notes
- Keep all documentation files updated whenever features, scope, or technical decisions change.
- Maintain `refinements-changes.md` as a running decision log throughout development.

## Latest Development Update
- **Yesterday:** Imported/implemented scripts, implemented Tripo 3D droid model, integrated purchased Unity Asset Store environment (due Tripo environment limitations), built and implemented Main Menu + Gameplay + Win/Lose UI, completed debugging/testing, and created a short demo video.
- **Today (final day):** Final UI polish (including missing Main Menu, Retry, Resume buttons), script refinement/debugging (droid orientation/behavior, rounds flow, jump handling, cursor for gameplay UI buttons), final showcase video, reflection, and AI Ethics Statement.

## Reflection
Where AI excelled and where it misled me: Grok was strongest for debugging and understanding code—explaining what the code does and what caused a bug, then providing step-by-step implementation guidance. However, it sometimes added unnecessary details that I later removed to keep the design clean, and the free plan limited speed. ChatGPT helped with consistent UI and background art generation, but free-plan limits slowed output. Cursor was good for generating documentation (.md/.txt), but its scripts were sometimes misleading and didn’t work when implemented, even after prompting for corrections. Producer AI reliably generated music that matched the game’s theme and atmosphere without major misleading behavior. Tripo AI produced a 3D droid model from my concept image, but export limitations forced a lower-quality model and required extra Blender work to fix axis/pivot orientation issues.

How AI altered my process: it sped up scripting, debugging, and asset direction, helping me refine and implement my idea rather than starting from scratch, though it also slowed development in areas where outputs were unreliable.

Ethics and fair use: I used AI only as a supportive tool, maintained originality, followed each tool’s terms, disclosed AI use in my demonstration video, and credited AI contributions. I did not share personal data, and I avoided any use that could violate fair use, originality, or intellectual property.

Next time, I wouldn’t rely on AI for everything; selective use and paid plans (if affordable) would improve control and authenticity.
