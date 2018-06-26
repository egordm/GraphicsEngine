# UU Graphics Assignment 2: Game Engine
Erwin Glazenburg 6224474, Egor Dmitriev 6100120, Cody Bloemhard	6231888

# Contents
* Solution FruckEngine with two projects (FruckEngine, FruckEngineDemo)
* All the shaders are in "FruckEngine\FruckEngine\Assets\shaders"
* Extra assets **(required to run the project)** https://drive.google.com/open?id=1f6xvEnguDLdFDg9bd95pih2-g3o7DE4p

# Instructions
1. Download the project and extract.
2. Download the assets here: https://drive.google.com/open?id=1f6xvEnguDLdFDg9bd95pih2-g3o7DE4p
3. Extract assets in FruckEngine\FruckEngineDemo\Assets\
4. Make sure FruckEngine\FruckEngineDemo\Assets\ contains following folders (cubemaps, lut, models, textures)
5. Run in visual studio. Dont forget to pull nuget dependencies

## Features
* Physically Based Render **(PBR)**
* **GGX Microfacets**
* Image Based Lighting **(IBL)** (Also fake environment reflections)
* **Deferred shading** (point light, spotlight and directional light)
* Screen Space Ambient Occlusion **(SSAO)**
* Depth Of Field **(DOF)** Bokeh
* Vignette
* Bloom
* God rays
* Color cube / Color grading
* HDRI Texture and environment map support
* **Hair** and hair movement
* Obj and fbx models
* And some more less spectacular things

## Controls
* Switch Scenes Keys: 1, 2, 3, 4, 5, 6, 7, 8
* Movement - Keys **W,A,S,D** and for up and down E and Q
* Camera rotation - **Mouse movement**
* Toggle ui for screenshots: **M**
* Toggle SSAO: **O**
* Toggle DOF: **J**
* Toggle Vignette: **L**
* Toggle Bloom: **I**
* Toggle God rays: **G**
* Toggle Color Grade: **N**

## Important Datastructures
* Mesh (FruckEngine.Graphics.Mesh) - Holds vertices, faces and material
* Object (FruckEngine.Objects.Object) - Holds meshes and children. Is also (scene graph node)
* Camera (FruckEngine)
* World (FruckEngine.Objects.World) - Holds Root object and environment
* Scene (FruckEngine.Game.SceneManager) - Loads models into a world.
* Game (FruckEngine.Game.Game and FruckEngine.Game.DeferredShadingGame) - Holds the world and game specific features like movement and rendering.
* Texture (FruckEngine.Graphics.Texture) - Abstraction for opengl textures
* CoordSystem (FruckEngine.Structs.CoordSystem) - Wraps matrices needed to trasnform the world objects.

Basically all the OpenGL abstractions are in FruckEngine.Graphics
All the shaders are in FruckEngine/FruckEngine/Assets/shaders


## Sources
https://bartwronski.com/2014/04/07/bokeh-depth-of-field-going-insane-part-1/
http://www.codinglabs.net/article_physically_based_rendering.aspx
http://john-chapman-graphics.blogspot.com/2013/01/ssao-tutorial.html
http://ogldev.atspace.co.uk/www/tutorial35/tutorial35.html
http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/


https://hdrihaven.com/
https://sketchfab.com/models/cbcf188a01f54d63a10f10c227c5a6ff#
https://sketchfab.com/models/9ada9c6edc1c4509bb413b903c0824f4
https://sketchfab.com/models/ab8efe91d839475e816421ba775029ed
https://sketchfab.com/models/d08c461f8217491892ad5dd29b436c90
https://www.models-resource.com/wii_u/mariokart8/
