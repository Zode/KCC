# KCC Debugger

The debugger can be opened from Flax's toolbar at the top: `Window` -> `KCC Debugger`.

![ Image showing out where the menu entry is ](/KCC/images/debuggermenu.png)

## Window contents

![ Image showing an example view of the KCC Debugger ](/KCC/images/debuggerwindow.png)

Toolbar contents are from left to right:

* Start/Stop capturing events toggle button
* Clear event data button
* Skip to first frame button
* Go back one frame button
* Go forward one frame button
* Skip to last frame button
* A label showing the current frame and amount of frames
* A slider to easily scrub through the frames

To the left is the hierarchy of events, selecting an event there will show the contents of that event (and its subevents if allowed to.).
KCC Debugger will sync selection with the scene hierarchy whenever possible, it will also try to do the vice versa. During frame scrubbing all selections will "bubble up" to the main event for the given actor(s).

To the right is the control panel, from where one may quickly adjust the behavior.

## Using the debugger

First start capturing events in KCC Debugger, it will only capture events when the game is running. It will automatically clear previous events if you start the play mode.

After you have captured your necessary frames, pause or stop the game and switch to the editor view. KCC Debugger will not preview any data while the game is running and not paused to preserve performance as much as possible.

Select a KCC actor or one of the KCC Debugger events to start visualizing the data.

## User events in the debugger

> [!WARNING]
> To add events or contents to events you MUST wrap the calls with `FLAX_EDITOR` define guards.

Adding custom events to the KCC Debugger is as easy as using Flax's own profiler.
You may start a custom event with `KCCDebugger.BeginEvent(string name)`, and end it with `KCCDebugger.EndEvent()`.

Normally you don't need to start "root events", but if you do you may do so with `KCCDebugger.BeginEvent(Actor actor, string name)`, and end it with `KCCDebugger.EndEvent()`.
Root events differ by carrying a reference to the actor, and you may NOT being a root event inside another. They are utilized for selection bubbling and other important things.

**All events mut be ended**, KCCDebugger will attempt to watch out for these and cry into the debug output if it notices you aren't ending your events properly.

Between any `BeginEvent` and `EndEvent` (including inside KCC's own events) you can call any of the draw commands to add visualization data inside the event.
The following are currently available:
* `KCCDebugger.DrawLine(Vector3 start, Vector3 end, Color color, bool depthTest)`
* `KCCDebugger.DrawBox(OrientedBoundingBox obb, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawCapsule(Vector3 position, Quaternion orientation, float radius, float height, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawSphere(Vector3 position, float radius, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawArrow(Vector3 position, Quaternion orientation, float scale, float capScale, Color color, bool depthTest)`
* `KCCDebugger.DrawQuad(Vector3 position, Quaternion orientation, float scale, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawMesh(Float3[] vertices, int[] indices, Vector3 position, Quaternion orientation, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawCollider(PhysicsColliderActor collider, Color fillColor, Color outlineColor, bool depthTest)`
* `KCCDebugger.DrawText(Vector3 position, string text, int size, float scale, Color color, bool depthTest)`
* `KCCDebugger.DrawText(Vector3 position, string text, bool depthTest)` (Uses KCC Debugger settings for size/scale/color)

You may access the KCC Debugger settings for the colors from: `KCCDebugger.Options`, see [KCCDebuggerOptions](/api/KCC.KCCDebuggerOptions.html) for properties.

## Settings

You may find customization settings in Flax's editor settings under the tab "KCC Debugger".

![ Image showing a view of Flax settings showcasing the KCC Debugger settings ](/KCC/images/debuggersettings.png)
