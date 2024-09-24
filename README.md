## Why?
This is a demo project for a challenge, in it I had to choose between some fairly famous games, implement it and give it my spin. "Matchventures" is a Match-3 game where you have to match the tiles to the enemies to defeat them, and try to survive as long as possible.

## Code Architecture
This project is based on the Scriptable Object (S.O) architecture presented on the [Unite Austin 2017](https://www.youtube.com/watch?v=raQ3iHhE_Kk) . It mainly focuses as using Scriptable Objects as variables, events, and sometimes systems to enable dependency injection via the engine's editor.

### Why?
By using this architecture you get a lot of benefits:
* **Modular**: With S.Os as variables, your components get to be easily modular, since the data is accessed through the editor. Data dependencies are not a problem when it is always accessible from the editor.
* **Editable**: With this architecture, the game gets to be much more data driven, enabling the designer to come up with new events, creating new variables and mix and matching rules all through the editor.
* **Debuggable**: As S.Os are always in memory and editable from the editor, the game get's a powerful set of debug capabilities from the start. You can easily follow variables values, edit them, and raise events in runtime, and all that without developing a single tool.

#### In Project Examples:
Here are some examples that illustrate these qualities
#### Modularity
The Match-3 game is completely separated from any other logic from the game, making it easy to reuse for other projects. There's some room for improvement, since in the interest of time I have not implemented Typed Events, but I'll discuss this in another section (PUT SECTION HERE).
#### Editability
The Match-3 is really flexible, it lets me create new Match patterns (The only ones I used were from the example game) and types of tiles by simply creating new scriptable objects and referencing them through the editor. All without coding.
#### Debugging
As the game interactions are done through Game Events (Scriptable Objects that are used as events), they can be raised at any time, making it easy to debug if something is happening correctly directly from the editor.

## Unit Tests
One part of the game I thought was error prone and would be hard to debug was recognizing which tile pattern was formed after a move, so I created unit tests and used test driven development to code this feature. The tests helped immensely both while developing the feature and in the future changes, and I think it was a highlight of this challenge.

## The Game
The objective of the game is to match the tiles that are above the enemy head. When you achieve the quantity they're asking, they are vanquished and a new monster appears.![[Pasted image 20240924160133.png]]

There are 15 enemies across 2 biomes, all can be configured through SOs.

### ## Future Improvements

Given the limited time for this project, there's a lot of room for improvement:

- **Expanded Tile Interactions**: Currently, only heart tiles have a secondary effect (healing the player), but each tile could have unique effects.
- **Better Balancing**: Fine-tuning the difficulty curve and progression would enhance the overall experience.
- **Core Loop Enhancements**: Items, power-ups, or even a leveling system could deepen the game's core loop and improve player retention.
- **Add Side Loops**: Integrating additional game loops, such as quests or challenges, could add depth and variety to the gameplay.
- **Use Object Pool Pattern For Tiles**: As the most numerous and reused objects in the game, they could be pooled to have only the necessary tiles to fit the board, plus a few.