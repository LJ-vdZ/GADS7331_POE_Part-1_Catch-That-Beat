# Development Plan - Catch That Beat!

## Project Goal
Build a first-person Unity (C#) game that blends **hide-and-seek** with **audio-driven chase**: the player tracks and catches a mischievous social droid in a cozy apartment.

## Scope Summary (from concept document)
- **Game type:** First-person 3D chase game
- **Theme:** Light-hearted, cozy, zero-violence
- **Core mechanic:** Directional audio tracking
- **Droid loop (design target):** At round start the droid **picks a random hide spot** (room/location) and **stays put** until the player finds it; then it **runs away** until it **breaks line of sight / loses the player**, then **settles at a new random location** and waits again—**not** endless roaming (roaming makes the droid too easy to stumble onto and can flatten pacing).
- **Win condition:** Catch droid 3 times in a row
- **Lose condition:** Timer reaches zero during a round
- **Difficulty scaling:** Each round, the available time to find/catch the droid **decreases**, increasing challenge in a fun way

## Milestones

## Final Sprint Status

### Yesterday (Implementation Complete)
- Imported and implemented core gameplay scripts in Unity
- Modeled the droid character with Tripo 3D and implemented it in-scene
- Purchased and integrated a 3D environment from Unity Asset Store (Tripo 3D limitation: individual models rather than full environment packs)
- Generated and implemented UI for:
  - Main Menu
  - Gameplay scene
  - Win screen
  - Lose screen
- Completed debugging/testing pass
- Created a short demonstration video

### Today (Final Day Plan)
- Polish UI (add missing buttons such as Main Menu, Retry, Resume)
- Refine and debug scripts:
  - Droid orientation and behavior
  - Game rounds flow
  - Player jump controls
  - Cursor visibility/availability for gameplay UI buttons while using first-person mouse look
- Create final showcase video
- Write project reflection
- Write AI Ethics Statement

### Milestone 1 - Project Setup and Core Scene
- Create Unity project and folder structure
- Build apartment blockout (living room, kitchen, bedroom, bathroom, hallway, study)
- Add player first-person controller
- Set up base lighting/post-processing for warm golden-hour style
- Deliverable: Playable apartment scene with player movement

### Milestone 2 - Droid Prototype and Chase Loop
- Create droid placeholder model and movement behavior
- Implement state machine aligned with hide-and-seek + chase (e.g. **Hiding / Waiting**, **Fleeing**, **Relocating** to a new random spot—not continuous patrol roam)
- Implement catch detection and round reset logic
- Add round timer and 3-catch win tracker
- Deliverable: Full loop with win/lose conditions and dynamic “find → chase → lose → re-hide” pacing

### Milestone 3 - Audio-Driven Gameplay
- Add 3D spatial audio source to droid
- Scale volume/filter effects by distance and obstruction
- Tune sound cues to support tracking through rooms/objects
- Add escalating intensity each round (e.g. tempo, droid speed, **shorter round time each round**—**not** proximity-based music pitch; see refinements log)
- Deliverable: Reliable audio-guided droid tracking gameplay

### Milestone 4 - Interaction and Polish
- Add interactables (doors, crouch spaces, room checks)
- Add UI (timer, catches, objective prompts, retry/win screens)
- [ ] Attach `UIButtonWanderer` to each of the three menu buttons; tune slow random motion
- Improve droid feedback animations and particles (musical notes)
- Add SFX/VFX for catches, dashes, and time pressure
- Deliverable: Polished vertical slice ready for submission/demo

### Milestone 5 - QA, Balancing, and Packaging
- Playtest for fairness and readability of audio clues
- Balance droid speed, timer length, and obstacle density
- Fix gameplay and performance bugs
- Write final documentation and build instructions
- Deliverable: Stable final build + complete documentation

## AI Tools Used
- **ChatGPT:** Concept iteration, writing support, and idea refinement
- **Grok:** Primary AI support for Unity C# scripting, concept feedback, and feature brainstorming
- **Cursor:** Documentation maintenance and project organization support (no longer used for Unity scripting)
- **Producer AI:** Music track generation for gameplay atmosphere and chase pacing
- **Tripo 3D:** 3D model generation support for the droid character

## Task List

### Design Tasks
- [ ] Finalize level layout and object placement plan
- [ ] Define droid behavior rules per difficulty escalation
- [ ] Finalize visual style guide (palette, materials, UI style)

### Programming Tasks (Unity C#)
- [ ] Player movement and camera look script (**Input System**, WASD, **mouse look**, **Shift sprint**, **Space** jump; **Camera on player** rig; see refinements log)
- [ ] Droid AI: random hide location, wait until discovered, flee until player lost, settle at next spot (see `refinements-changes.md`)
- [ ] Catch detection and round manager script
- [ ] Timer and score/win tracking script
- [ ] Audio proximity and occlusion script
- [ ] UI controller script

### Art/Audio Tasks
- [ ] Source/create low-poly apartment props
- [ ] Source/create droid model and animations
- [ ] Prepare spatial music loop and SFX pack
- [ ] Create simple VFX for droid movement and catches

### Testing/Refinement Tasks
- [ ] Verify all win/lose transitions
- [ ] **Mouse look:** `PlayerControllerFPS` **Mouse Sensitivity** lowered in Inspector—defaults felt too sensitive (see `refinements-changes.md`)
- [ ] Tune audio readability through walls and doors
- [ ] Tune pacing of each round escalation
- [ ] Run final bug pass and performance checks

## Risks and Mitigation
- **Risk:** Audio clues are too subtle or too obvious  
  **Mitigation:** Add test scenes and tune attenuation/filters early
- **Risk:** Droid pathing gets stuck on clutter  
  **Mitigation:** Use NavMesh + fallback teleport/unstick behavior
- **Risk:** Scope creep from extra rooms/features  
  **Mitigation:** Keep to single apartment and chase-first gameplay

## Definition of Done
- Core loop is complete and enjoyable (**hide-and-seek discovery** + **chase** tension, not trivial constant roaming)
- Spatial audio consistently guides the player
- 3 catches to win and timer-loss logic works correctly
- Visual style and UI match intended cozy-cheeky experience
- Documentation is complete and up to date

## Reflection
Where AI excelled and where it misled me: Grok was strongest for debugging and understanding code—explaining what the code does and what caused a bug, then providing step-by-step implementation guidance. However, it sometimes added unnecessary details that I later removed to keep the design clean, and the free plan limited speed. ChatGPT helped with consistent UI and background art generation, but free-plan limits slowed output. Cursor was good for generating documentation (.md/.txt), but its scripts were sometimes misleading and didn’t work when implemented, even after prompting for corrections. Producer AI reliably generated music that matched the game’s theme and atmosphere without major misleading behavior. Tripo AI produced a 3D droid model from my concept image, but export limitations forced a lower-quality model and required extra Blender work to fix axis/pivot orientation issues.

How AI altered my process: it sped up scripting, debugging, and asset direction, helping me refine and implement my idea rather than starting from scratch, though it also slowed development in areas where outputs were unreliable.

Ethics and fair use: I used AI only as a supportive tool, maintained originality, followed each tool’s terms, disclosed AI use in my demonstration video, and credited AI contributions. I did not share personal data, and I avoided any use that could violate fair use, originality, or intellectual property.

Next time, I wouldn’t rely on AI for everything; selective use and paid plans (if affordable) would improve control and authenticity.
