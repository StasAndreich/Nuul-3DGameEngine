<br />
<p align="center">
  <a href="https://github.com/StasAndreich/Nuul-3DGameEngine/tree/main/src">
    <img src="readme/nuul-logo.png" alt="Nuul icon" width="256" height="256">
  </a>


  <h3 align="center">Nuul - 3D Game Engine</h3>
  <p align="center">
    Built with .NET and DirectX 11 wrapper (SharpDX).
    <br />
    <a href="https://github.com/StasAndreich/Null-3DGameEngine/tree/main/src"><strong>Source code »</strong></a>
  </p>
</p>

<br>

## How it should work
<br>
Example of creating game logic: <br>

``` cs
// Create Scene.
Scene scene = new Scene("Main");

// Create game objects with components.
GameObject gameObject = new GameObject("obstacle");
MeshRenderer meshRenderer = new MeshRenderer();
meshRenderer.AddMesh("model1.fbx");
gameObject.AddComponent(meshRenderer);

// Add object to scene.
scene.AddGameObject(gameObject);
```

Another example: <br>

``` cs
// Create game objects with components.
GameObject gameObject1 = new GameObject("obstacle1");
MeshRenderer meshRenderer1 = new MeshRenderer("model1.fbx");
gameObject1.AddComponent(meshRenderer1);

// Add object to collection.
GameObjectCollection collection = new GameObjectCollection();
collection.Add(gameObject1);

// And than create Scene.
Scene scene1 = new Scene("Main1", collection);
```

Getting component from GameObject instance: <br>

``` cs
RigidBody rb = gameObject.GetComponent<RigidBody>();
```
