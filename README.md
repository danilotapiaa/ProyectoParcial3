# ProyectoParcial2 - Laberinto Multijugador

Juego 3D multijugador en primera/tercera persona hecho en Unity con Netcode for GameObjects. Los jugadores deben recorrer un laberinto (dungeon), recolectar 5 llaves repartidas por el nivel y esquivar/enfrentar enemigos controlados por IA (NavMesh) para abrir la puerta final y escapar. Si un jugador pierde toda su energía tres veces (0 vidas), la partida termina en derrota; si el equipo consigue las 5 llaves y cruza la zona de meta, gana.

El proyecto usa el personaje base de Unity **Starter Assets - Third Person Controller**, sincronizado en red: el dueño de cada jugador controla su propio movimiento y cámara (Cinemachine), mientras que el servidor autoritativo gestiona el daño, las llaves, el estado de la IA enemiga y el estado global de la partida (jugando / victoria / derrota).

## Controles

- **WASD**: moverse
- **Mouse**: mirar alrededor / mover cámara
- **Espacio**: saltar
- **Shift**: correr (sprint)
- Al iniciar el juego aparece un menú simple (GUI de Unity) con dos botones: **"Iniciar Host"** (para alojar la partida) e **"Iniciar Cliente"** (para unirse a una partida existente).

## Stack

- **Unity**: 6000.4.9f1 (Unity 6)
- **Networking**: Unity Netcode for GameObjects
- **Cámara**: Unity Cinemachine
- **Input**: Unity Input System (con Starter Assets Third Person Controller)
- **UI en pantalla**: `OnGUI` (IMGUI), sin Canvas/UI Toolkit todavía

## Estructura de `Assets/`

- `_Scripts/`: scripts propios del gameplay en red (IA enemiga, salud del jugador, manager del dungeon, llaves, zona de victoria, spawn de cámara y del jugador).
- `_Prefabs/`: prefabs propios (jugador multijugador, llave interactuable, enemigo, suelo).
- `Dungeon/`: assets de terceros del laberinto (modelos, texturas, animaciones y escenas de ejemplo del asset pack usado).
- `Modelos_Enemigos/`: modelos 3D usados para los enemigos.
- `Scenes/`: escenas propias del proyecto, principalmente `Nivel_Laberinto.unity` (nivel jugable) y `SampleScene.unity`.
- `StarterAssets/`: paquete oficial de Unity (Third Person Controller) usado como base de movimiento del jugador.
- `Audio/`, `Materials/`, `Settings/`: recursos de soporte (sonidos, materiales, perfiles de render/build).
- `TextMesh Pro/`, `TutorialInfo/`: paquetes de terceros / plantilla base de Unity, sin contenido propio relevante.

## Estado actual

El proyecto es funcional como prototipo/parcial académico: hay host/cliente, movimiento sincronizado, recolección de llaves, un enemigo con IA básica (persigue, ataca y divaga) y pantallas de victoria/derrota. Sigue siendo experimental: la UI está hecha con `OnGUI` (placeholder, no una UI final), no hay menús de configuración de red (IP, puertos, etc.) más allá de host/cliente local, y existe una carpeta `Assets/_Recovery` con una escena (`0.unity`) que parece un archivo de recuperación/backup de Unity, no contenido definitivo del juego.
