# Behaviour Callback Order
## Inheritance
RIObjects are controlled by a group of components.
You can write your own script to control the RIObject by simply inherit your script
from the `ActorScript` class.

However, to make your script run properly, you need to override some methods. Most importantly,
make sure you have the executing order in mind.

## Callbacks and Executing Order
The following is the executing order of the callbacks.

- OnSpawn() called when the RIObject is spawned.
- OnEnable() called when the RIObject is enabled.
- OnInit() called when the RIObject is started. Usually used for initialization.

- OnUpdate() called every frame.
- OnFixedUpdate() called every fixed frame. (At present not implemented)
- OnRenderFinished() called when the render of the frame is ended.

- OnDisable() called when the RIObject is disabled.
- OnDestroy() called when the RIObject is destroyed.

By the way, internal components like `Transform` and `Camera` are also scripts. They will
update themselves in the `OnUpdate()` callback after other scripts.