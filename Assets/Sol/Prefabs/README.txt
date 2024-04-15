HOW TO USE THE PREFABS FOR ROOM GENERATION

- each room has to have its own prefab (which doesnt include walls, UNLESS its supposed to be a dead end*)
- each room must have one entry point which will have a wall and a door already

* rooms dont need to have any exit points, as long as it has an entry point, only walls in the direction of exit points cannot have walls. 
  for example, a medium room could have two spawners and an entry point, the one wall that doesnt have a spawner or entry point can have a wall there in the prefab, 
  but the walls where there are spawners cannot have walls.

Creating a room prefab:
1. copy the debug room prefab that u wanna use (i.e. "large debug room" if u wanna make a large room), namne it whatever u wanna
2. remove the objects under the collectables GameObject (dont remove the collectables GameObject)
3. add & arrange collectable objects / rugs, countertops, ect. to ur liking
4. make sure each collectable object has the tag "Collectable" and the script "Object" (found int Sol>Scripts>Object) with a score, make sure the layer is "Default" and it has a collider and rigidbody
5. make sure any not collectable object is static, is Untagged, and is in the layer "Ignore Raycast"
6. if there are any walls / directions that u dont want the generation to continue in (for example theres a tv on that wall so u dont want a door and a room to spawn behind it), remove the spawner object that is in that direction and add a wall (u can find debug wall in Sol>Prefabs>"debug blank wall")

please ask if uve got any questions, i hope this makes any ounce of sense :3