
# MonoGame Skinned Mesh Importer

[![N|Solid](https://static.wixstatic.com/media/29dac0_84c639f416df456883d70bd8ecdae970~mv2.png/v1/fill/w_100,h_100,al_c,q_85,usm_0.66_1.00_0.01/LogoV2.webp)](https://www.lovebirb.com/)

This is a custom importer, processor, reader, writer, and framework to handle skinned meshes within MonoGame. It uses MonoGame's content system to avoid needing to package Assimp with your game, as well as fitting into your pre-existing content methodology.

# About

After looking through many projects that import animations into MonoGame, I decided that I would need to make my own. Many other projects are incredibly messy with code that is hard to read and even harder to maintain, and I have not yet found an importer that actually uses MonoGame's content pipeline at all.

This importer is written neatly using OOP principles, almost everything is commented, all important state is handled by the respective classes, and there is a simple interface in order to use its features. The system exists as a shared project including the data classes and structs (animations, bones, meshes, etc.), and classes to handle playing animations and rendering the skinned meshes.

The tool was tested with Blender 2.93.1 exporting to FBX files, more on that below.

# Installation

This tool is available on NuGet as Lovebirb.MonoGame.SkinnedMeshImporter and can be installed in Visual Studio with the NuGet package manager.

# Usage

## Adding the Content Builder Reference

Once the package has been installed to the MonoGame project of your choice, the reference to the content builder has to be added via the Content Pipeline Tool.

1. Open the content file for the project within the Content Pipeline tool.
2. Select the main "Content" object within the tool.
3. Find the "References" property in the window below and click on it, a window should open.
4. Click the "Add" button at the top right of the window.
5. Navigate to C:\Users\{USERNAME}\.nuget\packages and open the lovebirb.monogame.skinnedmeshimporter folder.
6. Open the folder of the used version (e.g. 1.0.0), then the lib folder, finally the netstandard2.0 folder, and select the SkinnedMeshImporter.dll file.
7. Click the "Ok" button on the window to close it.
8. The content builder has now been referenced and skinned meshes can now be added as content.

## Exporting

All instructions are written with Blender 2.93.1 in mind. Other tools have not been tested, however; they should work as this content builder uses Assimp, which is quite capable. If you do try this content builder with something other than Blender, it would be appreciated if you could get in contact.

1. Open your Blender file with your rigged and animated model.
2. Select the armature and mesh(es).
3. Click on File>Export>FBX(.fbx).
4. Choose a folder to export the file, obviously this is likely to be within your project's Content folder.
5. Set "Path Mode" to copy, unless your project handles textures separately.
6. Make sure "Limit to selected objects" is checked.
7. Under "Transform", "Apply Scalings" should be set to "FBX All", "Forward" to "-Z Forward", "Up" to "Y Up", "Apply Unit" should be false, and "Use Space Transform" and "Apply Transform" should be true.
8. Under "Armature", "Add Leaf Bones" should be false. This can be left true to include a bone at the end of any bone with no child, but has not been tested.
9. Click Export FBX. The model is now ready to be imported.

## Importing

1. Within the Content Pipeline Tool with the content file for your project opened, add the model as you would add any other content with the tool.
2. Click on the model within the content tool and change the "Importer" property to the "SkinnedMeshImporter - Liru3D" option, the processor should automatically change to match.
3. Save the file. The model is now ready to be used in the project, which will be covered in the next section.

## Loading

The model can now be loaded like any other content within MonoGame, using the Content.Load function. The type is SkinnedModel.

```csharp
SkinnedModel characterModel = Content.Load<SkinnedModel>("Character");
```

This SkinnedModel instance includes the meshes, bones, and animations loaded from the file, but by itself does not do anything. The data is immutable, meaning that it will forever remain as it was loaded.

## Animation

The AnimationPlayer class handles playing animations for a SkinnedModel. It is created using a SkinnedModel instance.

```csharp
AnimationPlayer animationPlayer = new AnimationPlayer(characterModel);
```

The player's current animation can be set to any animation, even those from other files. It looks for bones with the same names, so ensure the skeletons of the models are the same in layout.

```csharp
animationPlayer.Animation = characterModel.Animations[0];
```

The player must be updated.

```csharp
animationPlayer.Update(gameTime);
```

In order to make sure the system can work with custom effects and is not limited with anything built-in, the AnimationPlayer itself does not handle drawing. It does, however; work with MonoGame's SkinnedEffect.

```csharp
SkinnedEffect effect = new SkinnedEffect(GraphicsDevice);

effect.View = ...
effect.Projection = ...
effect.World = ...

foreach (SkinnedMesh mesh in characterModel.Meshes)
{
	animationPlayer.SetEffectBones(effect);
	effect.CurrentTechnique.Passes[0].Apply();
	mesh.Draw();
}
```

Playback can be started and stopped with the IsPlaying property. Playback speed and looping can be changed with the PlaybackSpeed and IsLooping properties respectively.

```csharp
animationPlayer.PlaybackSpeed = 2.0f;
animationPlayer.IsLooping = true;
animationPlayer.IsPlaying = true;
```

Currently, directly changing the time of the animation is not supported. It may very well work, but it has not been tested at all, and would require the access of the property to be changed within the source code if you wish for it to be enabled.

# Future Plans

I want to keep this project lightweight and simple to use. I'm not trying to compete with Unity or anything more capable, merely providing an option for devs to use skinned meshes without needing knowledge of asset importing and bone-space transforms.

This was mainly made as I needed animations for my project, Terror Cell. As I implement this into Terror Cell, some changes may be made and additions included. I eventually want to implement a nice UI to properly load and test animations, including a seek bar. A unique model with proper animations would also be nice at some point.

The main plan right now is to neaten and comment everything in the project and ensure it's all 100% readable and maintainable, then I will create some tests to ensure it's all bug free, then the previously mentioned UI. After that, if I have the time, I will possibly look into some more advanced features like blending and parameters.

Regardless of my future plans, the current version does what it's supposed to do.

# Credits

SkinnedMeshLoader by [Laura Jenkins (Liru)](https://www.lovebirb.com/)

[Assimp](https://www.assimp.org/) and [AssimpNet](https://www.nuget.org/packages/AssimpNet/5.0.0-beta1) for removing large amounts of pain from asset importing
[AlienScribble](https://www.youtube.com/user/AlienScribble) for the FBXLoader project which I used extensively to test against
[Dillinger](https://dillinger.io/) for the great markdown tool for this readme file
