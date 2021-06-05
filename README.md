# Bullet Hell Pattern Generator

The Bullet Hell Pattern Generator is a way to create patterns of various complexities in editor using prefabs. Patterns can be edited in engine without the use of any code and can be changed during editor runtime for rapid iteration. The system also comes with a bullet script (BH_BULLET) that must be placed on all prefabs used within the system, this script allows you to easily customise bullets to be used in your patterns. 

## How To Use
Currently this project is in active development, so it may be unstable. https://github.com/Volpanic/BulletHellPatternGenerator/releases/tag/Unstable

### BH_BulletHellPatternGenerator
*	Patterns is a list that hold the pattern scriptable objects, you can stack multiple on top of each other to create even more complex patterns.
*	Target is a transform that the patterns will aim at if selected in the pattern editor.

### BH_BULLET
*	Max Life Time is the duration the bullet stays alive for before deactivating.
*	The slider controls Orbital Velocity, making the bullets direction rotate over time.
*	Rotation offset is the initial offset of the object rotation.
*	Rotation Velocity is how much to increment the bullets rotation by.
*	Speed modifier is a multiplier for the bullets speed, initially controlled by the pattern.
*	Homing speed is the speed at which the bullet homes in on the generators target.
*	Disable when offscreen makes the object disable when offscreen.


### Pattern Editor

Patterns themselves can be created by right clicking in the asset browser and selecting ‘Bullet Hell Pattern’ these scriptable objects can be double clicked to open the pattern editor; Alternatively you can open the editor from the top toolbar of the unity editor by selecting ‘Bullet Hell/Pattern Editor’.

*	The Asset Selector can be used to change the pattern currently being edited.
*	The Layer Toolbar can be used to add more layers of complexity to your patterns, a drop down allows you to change the currently viewed layer. Add layer adds a new layer and remove layer removes the currently selected layer. The duration of the pattern controls any time dependent parts of the pattern, mainly when curves are used.
*	The Bullet dropdown controls how bullets are chosen for the pattern, The 2 current modes (which can be changed by clicking the ‘Change Bullet Mode Button’) have a list of bullets that can be expanded to as many bullets as needed, remember bullets need the BH_Bullet script creating them at runtime will cause an error.
*	The Timing drop down controls how often the bullets are spawned.
*	The Pattern drop down allows you to edit various variables for a bullet patterns, this is the main panel that will be edited. each of the I built patterns have similar variables, mess around with them and create unique patterns

### Further Customisation
The 3 pillars of the patterns (Bullets, Timing, Pattern) can all be added to, if you are inclined to check the BulletBase, TimingBase and PatternBase scripts to see how the previous parts were written and how you can create your own.
