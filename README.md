# ⚔️ Finn's Island Adventure (Proyecto Plataformas 2D)

![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f3%2B-blue.svg)
![Status](https://img.shields.io/badge/Status-En%20Desarrollo-orange.svg)

¡Bienvenido a **Finn's Island Adventure**! Un juego de acción y plataformas 2D desarrollado en Unity con URP. Controla a Finn en una isla llena de peligros, derrota enemigos para obtener botín y prepárate para la batalla final contra el Capitán Clown Nose.

## 📖 Descripción del Juego

En este proyecto, el jugador debe explorar un mapa abierto lleno de plataformas y enemigos. La clave del éxito no es solo llegar al final, sino **hacerse más fuerte** por el camino.

El núcleo del juego ("Core Loop") se basa en:
1.  **Exploración y Combate:** Recorrer el mapa derrotando enemigos con distintas dificultades.
2.  **Loteo y Mejora:** Los enemigos y zonas del mapa sueltan **Power-Ups generados aleatoriamente**.
3.  **Batalla Final:** Acumular suficientes mejoras para enfrentarse con garantías al jefe final.

## 🎮 Controles

El juego utiliza un esquema de control clásico de teclado para una respuesta rápida:

| Acción | Tecla | Descripción |
| :--- | :---: | :--- |
| **Mover Izquierda** | `A` | Desplazamiento lateral básico. |
| **Mover Derecha** | `D` | Desplazamiento lateral básico. |
| **Saltar** | `Space` | Permite subir plataformas y esquivar ataques. |
| **Sprint** | `Shift` | Aumenta la velocidad de movimiento momentáneamente. |
| **Atacar** | `F` | Realiza un golpe con la espada para dañar enemigos. |
| **Ataque Potenciado** | `Shift + F` | Realiza un golpe x2.5 con la espada para dañar enemigos. |

## 🧪 Sistema de Power-Ups

Para derrotar al jefe, es vital recolectar los potenciadores que aparecen aleatoriamente al eliminar enemigos o en puntos del mapa:

* 🍎 **Curación:** Aumenta la regeneración de vida por segundo.
* ⚡ **Velocidad:** Incremente la velocidad de movimiento permanentemente.
* ⚔️ **Ataque:** Incrementa el daño que haces con la espada permanentemente.
* 🧪 **Mana:** Aumenta la regeneración de mana por segundo.

## 🦀 Enemigos y Jefes

El mundo está habitado por criaturas con diferentes comportamientos (IA):

* **Zombie Toast (Fácil):** Enemigo que persige y ataca cuerpo a cuerpo.
* **Crabby (Medio):** Patrulla plataformas y ataca si te acercas demasiado.
* **Pink Star (Difícil):** Mayor daño y agresividad.
* **🤡 BOSS: Captain Clown Nose:** * Cuenta con una barra de vida extensa.
    * Patrones de ataque con espada y habilidades especiales.
    * Requiere que el jugador haya recolectado *power-ups* para ser derrotado.

## 🛠️ Tecnologías y Estructura

Desarrollado con **Unity** utilizando las mejores prácticas actuales:

* **Renderizado:** Universal Render Pipeline (URP) para iluminación 2D avanzada.
* **Input System:** Gestión de entradas de teclado optimizada.
* **Arquitectura:**
    * `Assets/Scripts/Player`: Lógica de movimiento, ataque y sprint.
    * `Assets/Scripts/Enemys`: Máquinas de estados para la IA de los enemigos y el Jefe.
    * `Assets/Scripts/Powerups`: Sistema de generación aleatoria (*Spawner*) y efectos.
    * `Assets/Scripts/Menu`: Gestión de UI, escenas de Victoria/Derrota y Pausa.

## 🚀 Instalación

1.  Clona el repositorio:
    ```bash
    git clone [https://github.com/mariaelenamiranda/proyecto-plataformas.git](https://github.com/mariaelenamiranda/proyecto-plataformas.git)
    ```
2.  Abre el proyecto en **Unity Hub** (versión recomendada 2022.3 o superior).
3.  Navega a `Assets/Scenes/MenuScene` y abre la escena `MainMenu`.
4.  Dale al **Play** ▶️ y ¡disfruta!

## 🎨 Créditos y Recursos

* **Programación y Diseño:** María Elena Miranda, Juan José Restrepo, James Ovalle.
* **Arte (Sprites):** [Pixel Frog](https://pixelfrog-assets.itch.io/) (Treasure Hunters Pack) y otros.
* **Personaje:** Basado en Finn (Adventure Time).
