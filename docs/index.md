
Lunar Engine
============

Lunar Engine is a free, open source 2D online RPG engine in C#

Project Detials
---------------

Lunar Engine is provided as a software package, the binaries of which consist of a client, server, and game editor. There are a number of important core components which make up this software package which are described in detail below.

  ### Lunar.Client
  Lunar.Client contains the game-client portion of the software package. This consists of the systems handling the game rendering, client-side networking, asset management, game-world caching, and user-interface. 
  
  The dependencies for this portion of the project include MonoGame (OpenGL), Lidgren, Penumbra, and QuakeConsole.
  
  ### Lunar.Server
  Lunar.Server contains the game-server portion of the software package. This consists of the systems responsible for handling the underlying gameplay, world updating, state saving and loading, and client-to-client communication.
  
  The dependencies for this portion of the project include Lidgren.
  
  ### Lunar.Editor
  Lunar.Editor contains the game-editor portion of the software package. This consists of the systems responsible for visualized content creation (maps, items, npcs, animations, etc.) as well as for editing existing data.
  
  The dependencies for this portion of the project include MonoGame and DarkUI.
  
  ### Lunar.Core
  Lunar.Core contains code which is shared throughout the rest of the project. This includes game data definition classes, loading and saving managers, utility functions, and more. Anything which might otherwise be duplicated throughout the Lunar engine package if independently implemented in each portion should exist here.
  
  ### Lunar.Graphics
  Lunar.Graphics contains shared graphics and graphics processing related code, mainly shared between the game-editor and game-client.


License
-------

Licensed under the Apache License, Version 2.0 (the "License").

Website & Community
-------
https://www.indieorigin.com

Multimedia
-------
![](https://i.imgur.com/9K62FUP.png)
![](https://i.imgur.com/xDiIT1Y.png)
![](https://i.imgur.com/7rXeYcc.png)
![](https://i.imgur.com/PMhsVF5.png)
![](https://i.imgur.com/UIFJNjJ.png)
