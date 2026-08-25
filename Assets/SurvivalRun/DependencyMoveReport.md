# SurvivalRun Export Layout

Date: 2026-08-25T12:20:59
Scene: `Assets/SurvivalRun/EliminationRace_Demo_City_ZOMBIE_CHASE.unity`

## Final Layout
The scene-used dependency folders now live directly under `Assets/SurvivalRun` so the folder can be exported without confusing root-level zombie/city/model folders.

Included under `Assets/SurvivalRun`:
- `GraphCaches`
- `Model`
- `PolygonApocalypse`
- `PolygonBossZombies`
- `PolygonZombies`
- `Zombie_Animations`
- `UI Text Resources`
- `Samples/Universal RP`
- `Scripts/OnTriggerEvent.cs`

Existing SurvivalRun folders remain in place:
- `Animations`
- `Demo_City_ZOMBIE_CHASE`
- `Materials`
- `Not So Scary Zombie Pack`
- `Prefabs`
- `Scary Zombie Pack`
- `Scripts`
- `Textures`

## Intentionally Excluded
A* sample/package assets are intentionally outside `Assets/SurvivalRun` because FDBarArcade already has A*.

Current intentional outside-`SurvivalRun` Asset references from the scene scan:
- 18 files under `Assets/Samples/A_ Pathfinding Project/...`

Kept inside SurvivalRun:
- `Assets/SurvivalRun/GraphCaches/AStarZombieRoads.bytes`, because this is scene-specific graph data, not the A* package/runtime itself.

## Verification
Recursive GUID scan result after the final move:
- Total scene Asset dependencies reached: 2554
- Outside `Assets/SurvivalRun`: 18, all under `Assets/Samples/A_ Pathfinding Project/...`
- No `_Dependencies` folder remains under `Assets/SurvivalRun`

Package dependencies still expected from the project/packages:
- A* Pathfinding Project
- Cinemachine
- Unity Splines
- Unity UI
- URP

## Existing Prototype Missing References
These unresolved GUIDs were already not present as Assets/package-cache `.meta` files in the source project. They were not caused by the folder move.

- `4ffd6127cf2cd0448a1a343c4174fe32`: material reference in the scene.
- `49d72b427420946b5a2c04962006e724`: material reference in the scene.
- `9d5a483824ecb4c3facadf30d2f38245`: material reference in the scene.
- `39d2aa534225f4a57858f2152dc281f1`: motion clip reference in `Animations/ZombieAnimatorController*.controller`.
- `e7595f0fdaed0f8448daa2995203d7b2`: texture reference in `PolygonApocalypse_Material_Chr_01_A/B.mat`.
- `cf75cef0c04674bf88463d85196d8876`: cubemap/texture reference in A* sample MineBot material.
