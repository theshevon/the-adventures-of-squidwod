# The Adventures of Squidwod

A Unity game that involves collecting Eggs from a giant fire-breathing, laser-eyed, grenade-tossing Seagull's nest while dodging its attacks. 

Made for Project B of COMP30019: Graphics & Interaction.

## Tech Stack

* C#
* Unity 3D
* Blender

## Features

### The Game
The Adventures of Squidwod follows  the narrative of Squidwod being tasked to set out and collect Eggs for Mr Krads as it’s the final ingredient he needs to get the ball rolling on the Krusty Krad’s brand new breakfast menu. Enticed with the promise of a raise, Squidwod sets off on his venture, oblvious to any risks or adversaries he might have to encounter in getting the Eggs.

In keeping true to the narrative, the player, who takes the role of Squidwod, is tasked with collecting as many Eggs as they can, while avoiding the both Seagull and Crab attacks. Whenever certain thresholds are crossed, the Seagull lands and attempts to deal heavier, close-ranged damage to the player, forcing them to use the Eggs collected to hold off the Seagull. As the overall objective of the game is to collect Eggs, the player needs to be aim well and try to get as many critical hits as possible so that they may end the game with a high number of Eggs to take back to Mr Krads.

### How to Play

The game takes directional movement from the WASD or arrow keys on the keyboard. From a 3rd person viewpoint, you can orbit the camera around the player and change your forward direction using the mouse. This configuration is very similar to many MMO and third person shooter games today, and our aim is that players who have slight experience with these particular games or just games in general can naturally jump in and use this familiar sense of movement and control. Clicking down on the spacebar allows the player to jump, an action that is crucial to both avoiding crabs and dodging one of the seagull’s attacks(when in a boss fight). We’ve also implemented an aim-and-shoot system that allows the player to move crosshairs on the screen using the mouse and fire an Egg in its direction by clicking down the left mouse button.

### Objects & Entities

#### Models

The key major interactive objects within the game include the player, Squidwod, as well as the Seagull, Crabs and Eggs. The model for Squidwod was an asset from the 2005 game ‘SpongeBob SquarePants: Lights, Camera, Pants!’ and the Crab is from ‘Pac-Man World Rally’, released in 2006. These model assets included a mesh and texture, but lacked any animation or rigging. Using Blender, these models were rigged had animations created to suit their purpose in the game. The seagull was a package downloaded from Sketchfab.com and we worked with the general bird-motion animations provided to suit the seagull’s actions and purpose within our game. In order for these different types of models to mesh well in the game, we incorporated and created a custom cel shader which we believe achieved that goal very well. The Egg was an ellipse created in Unity with a custom texture. The terrain was created using Unity’s built-in Terrain Engine.

#### Behaviour

##### Squidwod:	
As mentioned above, Squidwod’s directional motion is based off input from the keyboard and mouse. The entity has furthermore been implemented to take damage when struck by a Seagull or Crab attack along with different reaction audio cues. Squidwod also makes use of its animation controller when hurling eggs at the Seagull.
As a bonus feature, Squidwod has also been implemented to Dab as a player celebration.

##### Seagull: 
The Seagull has two distinct modes: Flight Mode and Boss Mode. 

When in Flight Mode, it flies in orbit over the terrain. In this mode, it has 3 attacks:
Laser Shots: The Seagull fires a laser shot at the player. On contact, it creates an explosion which subsequently sets the player on fire.
Fireballs: The Seagull releases fireballs towards the player. On contact, it sets the player on fire.
Laser beam: The Seagull shoots a laser beam in front of the player. The laser beam moves in a random direction, setting fire to whatever it makes contact with.

When in Boss Mode, the Seagull lands at the center of the terrain and faces the player. In this mode, it has 4 attacks:
- Gust: The Seagull  jumps up and upon touchdown, causes a red ring that expands over the terrain. It damages the player on contact.
- Flamethrower: The Seagull breathes fire onto the player while simultaneously rotating to face the player, following them in whatever direction they are moving in.
- Grenade Throw: The Seagull reaches under its wing, grabs a grenade and hurls it at the player.
- Laser Eyes: The Seagull shoots two laser beams from its eyes towards the player. The beams set fire to the area they make contact with.

In both modes, the range of attacks used depends on how much progress the player has made.

##### Krabs:
The Krabs surface from burrows spawning at random locations on the terrain and immediately move towards the player until they make contact.

##### Eggs:			
The Eggs appear at random locations on the terrain, floating and spinning about  their y-axes. When the player interacts with one, a new one spawns in a different location.

##### Krabby Patties: 	
The Krabby Patties were representative of health tokens. Much like the Eggs, they appear at random locations on the terrain, floating and spinning about their y-axes. When the player interacts with one, a new one spawns in a different location. However, unlike the Eggs, the Krabby Patties despawn after 20s when the game difficulty is set to Hard. When in Easy Mode, they are eternally available to the player and appear more frequently as well. 

Note:-
The level of damage done by Squidwod’s attacks on the Seagull and the Seagull’s/ Krabs’ attacks on Squidwod are dependent on the difficulty selected. The amount of health restored via interacting with a health token varied in a similar manner.

#### Graphics & Camera

In terms of handling the graphics pipeline, we ensured that all our static and dynamic shading was done via the use of vertex and fragment shaders, thereby exploiting the GPU. This optimised the performance of the game as using the CPU to dynamically shade our game objects and entities would have resulted in heavy lag throughout gameplay. We also ensured that all particle systems used were destroyed as soon as their purpose was fulfilled as having too many active at  any given point in time would severely impact the overall performance of the game.

The camera has two different stages, in Egg collection mode it orbits around the player in a 3rd person perspective and can be controlled using the mouse, the position of the camera also changes the input direction of the player and can be used for more precise movements such as strafing when used in conjunction with the keyboard. In Boss Fight Mode when the seagull lands, the camera locks onto the Seagull, and mouse movement is then used for aiming. The camera stays a set distance from the player while maintaining look rotation on the Seagull. When a player aims to throw an egg, the position of the camera interpolates to a custom set position to assist with more accurate aim.
In regards to collision with other objects and terrain, this is handled using a raycast from the cameras transform to the camera target transform. If anything is obstructing the view of the camera, the raycast will detect this hit and move the camera in front of the obstructing object using it’s xyz normals to calculate a new position offset from the obstruction.

#### Shaders
There are two main shaders used in the game, the first of which is the cel surface shader. This ‘toon’ style effect is achieved by calculating the dot product between the surface normal vector and the light direction, this value is then compared against set thresholds and is determined to be either lit or dark. Finally these values are combined with the surface albedo colour, a custom tint colour and an attenuation factor to reduce overexposure. There is also a second shader pass which creates an outline around the model, this vertex shader takes the vertex normal and multiplies it by the outline width which is then coloured in the fragment stage. The outline itself works due to the pass Cull Front which doesn’t render the polygons facing towards the viewer, which leaves just the polygons behind the model, creating the outline.

The second shader is the camera tint shader, which we use to desaturate and tint the screen within the game on the death and victory scenes. In the vertex stage the shader transforms each vertices object space point to the cameras clip space and returns it’s relative coordinates for use in the fragment stage. Where each of the colours are put through a lerp function between the original and a grayscale colour, the desaturation value acts as the interpolation point between the two values and finally the tint itself which is added at the end, both of which we can change dynamically in game to change the shaders effects and intensity.

## Build Status - *Complete* 

## Additional Usage Notes:

- When opening the project for the first time, please open the scene "Menu" from the 'Assets/Scenes' folder!

