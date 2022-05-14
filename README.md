# Supernova Video

This is a video-processing system meant to read pixels on a screen and use that value to toggle certain objects in a VR world.
An example would be a pixels grayscale intensity being read, with each five percent increase toggling one of a set of twenty objects.
With this, all instances of a VR world rendering the same video stream can have a synced toggle experience with a crafted video stream.
This is essentially a subset of [Lox's Shader Motion](https://gitlab.com/lox9973/ShaderMotion).
A large inspiration of the pixel reading came from [Llealloo's VRC Udon Texture Sampler](https://github.com/llealloo/vrc-udon-texture-sampler).

It is meant to only read a column one pixel wide on the screen for performance reasons.
Reading a large rectangle of pixels is easily the biggest performance hit on this script, so keep your column as small as possible.
Pixel grayscale intensity is read from a scale of zero to one.
If the intesity range for each child parameter on the TextureReader script is set to N, up to (100 / N) children can be toggled from that pixel.

For example:
With an intensity range of 5, 20 children could be toggled.
A value of .02 would mean nothing is active.
A value of .05 would mean the 1st child is activated.
A value of .97 would mean the 20th child is activated.

## 1. Installation

A convenient Unity install package can be found on the [releases page](https://git.mobiusk.com/mobi/supernova-video/releases).
This is a stripped down package without all necessary dependencies installed.

The following packages are required:
- [VRC SDK](https://vrchat.com/home/download) compatible with v2022.04.20.16.26
- [UdonSharp](https://github.com/MerlinVR/UdonSharp/releases/tag/v0.20.3) because the new ClientSim did not work for me
- [USharpVideo](https://github.com/MerlinVR/USharpVideo) v1
- [ArchiTechAnon ProTV](https://booth.pm/en/items/2536209) v2.2

The following packages are recommended:
- [CyanEmu](https://github.com/CyanLaser/CyanEmu/releases/tag/v0.3.10)

## 2. Performance

This script is going to be a small hit to your performance as copying pixels from a video render texture is pretty heavy.
I'm sure there are better ways to do it but I don't want to prematurely optimize if it doesn't end up affecting client performance.
Please keep the pixel column height being sampled as small as possible.
Furthermore the TextureSampler script is hard-coded to not run more than once per 10 frames and this can be adjusted as necessary.

## 3. Support

I can be reached on Discord at Mobi#0001 or through email at [mail@mobiusk.com](mailto:mail@mobiusk.com).

## 4. Scenes

There are three example scenes in order of ascending difficulty and application to the problem being addressed.

### 4.1. Camera Example

This was just a quick iteration scene to improve scripts as I worked on the other two scenes.
I would not recommend using this to learn how the script works at first.
It is best suited for testing out rapid changes to the pixel reading algorithm.

### 4.2. USharpVideo Example

This was the first scene I attempted to finish scripts for, using Merlins' USharpVideo.
The client desires to use ArchiTechAnon's ProTV, but I had never used that before so decided to try something "safe" first.
Luckily, this was very simple, because USharpVideo gives us a script that takes in a prefab video player and outputs a direct render texture.
That render texture is set to a camera components' target texture which triggers an OnPostRender event in our TextureSampler script.
The USharpVideo script can be found [here](https://github.com/MerlinVR/USharpVideo/blob/master/Assets/USharpVideo/Scripts/Utility/RenderTextureOutput.cs).

This scene can be run in-editor with CyanEmu and loads a [static calibrate screen test](https://www.youtube.com/watch?v=l4bDVq-nP-0).
Warning: This contains a 1KHz drone audio and will hurt your ears at loud volume. 
Because the colors are static/predictable, this is a great scene to test your pixel placement assumptions in the TextureSampler and TextureReader scripts.

For example:
Have TextureSampler read from position 200x200 with a column height of 1 pixel.
Have a TextureReader use pixel index 0, the only pixel available with a height of 1.
This will read back a pixel from the yellow bar section.

### 4.3. ProTV Example

I have to preface this section with how helpful ArchiTechAnon has been with this part.
They were very prompt in a support request and sat with me in a call trying to figure out a good portion of the script.
Please support them by rating and purchasing their great video asset on [Booth](https://booth.pm/en/items/2536209).

Warning: This scene currently only works with AVPro livestreams like Twitch, VRCDN, or YouTube streams.
It will not work with small Youtube videos that have a definite end.
This means it will not work with CyanEmu and is not a good scene to rapidly test changes in.
It's possible to get it to work with Unity streams eventually but that isn't necessary for the client right now.

The biggest learning curve with this was the difference between AVPro and Unity video players.
USharpVideo's prefab transparently handles the difference and has a utility method to grab the video render texture regardless of which player is active.
When I swapped to ProTV, I spent a lot of time on failed attempts trying to use Unity techniques on an AVPro screen and vice-versa.

Eventually I rewrote part of USharpVideo's RenderTextureOutput script to read a ProTV AVPro renderer component.
For whatever reason, the video render texture it writes to still has to have a USharpVideo material to trigger a PostRender event loop.
My knowledge fails at that point right now, just happy to have everything work.
Once the video render texture is written a camera component picks things up just like in the USharpVideo scene.
