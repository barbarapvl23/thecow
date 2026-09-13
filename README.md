# The Cow 🐮

Juego de plataformas 2D hecho en **Unity**. Controlas a una vaquita que debe avanzar por distintos niveles, saltar entre plataformas, recolectar monedas, esquivar peligros (zonas de caída) y llegar a la meta de cada nivel.

El proyecto está pensado para correr tanto en **escritorio** (teclado) como en **dispositivos móviles/touch** (botones en pantalla).

---

## 1. Tecnologías y versión de Unity

| Ítem | Valor |
|---|---|
| Motor | Unity **6000.0.53f1** (Unity 6) |
| Render pipeline | Universal Render Pipeline (URP) 17.0.4 |
| Lenguaje | C# (Assembly-CSharp) |
| UI de texto | TextMesh Pro |
| Cámara | Cinemachine 3.1.4 |
| Otros paquetes clave | Input System 1.14.0, Visual Scripting 1.9.7, Timeline 1.8.7, 2D Feature Set 2.0.1, Test Framework 1.5.1 |

> ⚠️ **Es importante usar exactamente la versión `6000.0.53f1`** (o una compatible dentro de Unity 6) para evitar que Unity intente migrar/actualizar assets, materiales de URP o el proyecto entero al abrirlo. La versión exacta está en [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).

La lista completa de dependencias está en [Packages/manifest.json](Packages/manifest.json).

---

## 2. Requisitos previos

Antes de clonar el repo, instala:

1. **Git** — https://git-scm.com/
2. **Unity Hub** — https://unity.com/download
3. Desde Unity Hub, instala el **Editor 6000.0.53f1** (pestaña "Instalaciones" → "Instalar editor" → buscar esa versión exacta o una `6000.0.x` cercana).
   - Al instalarlo, agrega los módulos según tu plataforma de prueba:
     - **Build Support: Android** y/o **iOS** (el juego tiene controles táctiles pensados para móvil).
     - **Build Support: WebGL** (opcional, si quieren compartir builds jugables desde el navegador).
     - **Microsoft Visual Studio Community** o, si prefieres, **JetBrains Rider** (el proyecto trae soporte para ambos vía `com.unity.ide.visualstudio` y `com.unity.ide.rider`).
4. Un editor de código para C# (Visual Studio, Rider o VS Code con la extensión de C#).

No hace falta instalar nada de Node/Python/etc.: es un proyecto Unity puro.

---

## 3. Primeros pasos para clonar y abrir el proyecto

```bash
git clone <URL-del-repositorio>
cd thecow
```

1. Abre **Unity Hub**.
2. Click en **"Add"** / **"Abrir"** → **"Add project from disk"** y selecciona la carpeta donde clonaste el repo (la que contiene `Assets/`, `ProjectSettings/`, `Packages/`).
3. Unity Hub debería detectar automáticamente que el proyecto requiere la versión `6000.0.53f1`. Si no la tienes instalada, te la ofrecerá instalar.
4. Abre el proyecto. **La primera vez tardará varios minutos**: Unity va a generar la carpeta `Library/` (caché de assets importados), que **no viaja en git** (está en `.gitignore`) y se reconstruye localmente en cada máquina.
5. Cuando termine de importar, en la ventana **Project**, ve a `Assets/Scenes/` y abre la escena `Menu.unity` (es la pantalla de inicio del juego). Dale Play para probar que todo cargó bien.

No necesitas tocar nada de `ProjectSettings/`, `Packages/manifest.json` ni los archivos `.sln`/`.csproj` — Unity los regenera solo.

### 3.1 Configurar las escenas en Build Settings (paso manual pendiente)

Actualmente `File > Build Settings` en este proyecto **solo tiene una escena de ejemplo desactivada** (`SampleScene`, deshabilitada). Antes de generar cualquier build necesitas agregar las escenas reales, en este orden:

1. `Assets/Scenes/Menu.unity`
2. `Assets/Scenes/Level1.unity`
3. `Assets/Scenes/Level2.unity`
4. `Assets/Scenes/Level3.unity`
5. `Assets/Scenes/Level4.unity`

Para hacerlo: `File > Build Settings...` → arrastra las 5 escenas (en ese orden) a la lista **"Scenes In Build"** → quita/desmarca `SampleScene` si aparece.

> El código en [`Assets/Scripts/Menu.cs`](Assets/Scripts/Menu.cs) hace `SceneManager.LoadScene(1)` al presionar Play en el menú, es decir, carga **la escena en el índice 1** de esa lista — por eso el orden importa (`Menu` debe quedar en el índice 0, `Level1` en el índice 1, etc.).

---

## 4. Estructura del proyecto

```
Assets/
├── Animations/     Animator Controllers (.controller) y clips (.anim) de la vaca, monedas, UI, etc.
├── Audio/          Efectos de sonido y música.
├── Characters/      Sprites de personajes/enemigos (vaca, "Mr. Circuit", etc.)
├── Fonts/          Fuentes (TTF/OTF) y sus assets SDF para TextMesh Pro.
├── Images/         Imágenes sueltas usadas en UI/menús.
├── Materials/      Materiales (principalmente para sprites/URP).
├── Objects/        Sprites y prefabs de objetos del mundo: monedas, checkpoints, banderas, botones de UI.
├── Palettes/       Paletas de color.
├── Player/         Assets específicos del personaje jugable.
├── Resources/      Assets cargados dinámicamente en runtime (Resources.Load).
├── Scenes/         Las 5 escenas del juego: Menu, Level1-4.
├── Scripts/        Todo el código C# del gameplay (ver sección 5).
├── TextMesh Pro/   Recursos default del paquete TMP (fuentes, shaders, sprites).
└── Tilesets/       Tiles y tilemaps usados para construir los niveles.
```

Otras carpetas relevantes en la raíz (no se tocan a mano):

- `ProjectSettings/` — configuración del proyecto (física, tags, capas, input, calidad, etc.). Se versiona en git.
- `Packages/` — dependencias del proyecto (`manifest.json` + `packages-lock.json`). Se versiona en git.
- `Library/`, `Temp/`, `obj/`, `Logs/`, `UserSettings/`, `.utmp/` — generados por Unity/el IDE en cada máquina. **Ignorados por git**, no deberían aparecer en ningún commit.

---

## 5. Scripts de gameplay (`Assets/Scripts`)

| Script | Responsabilidad |
|---|---|
| `PlayerController.cs` | Movimiento horizontal, doble salto, flip del sprite, detección de suelo, sonidos de salto/muerte/recolección, y maneja colisiones con `Collectable`, `Hazard` y `DeathZone`. |
| `CoinManager.cs` | Singleton (`DontDestroyOnLoad`) que lleva el conteo de monedas recolectadas vs. totales por escena, y actualiza el texto de UI. |
| `LifeController.cs` | Singleton que maneja las 3 vidas del jugador, los corazones de UI, el respawn en el checkpoint y la pantalla de Game Over con resumen de monedas. |
| `CollectableScripts.cs` | Marca un objeto como recolectable (tipo de ítem). |
| `HazardDamage.cs` / `DeathZone.cs` | Restan una vida al jugador al tocar un enemigo o al caer fuera del nivel. |
| `NextLevel.cs` | Trigger de meta: pasa al siguiente nivel o muestra la pantalla de victoria si es el último. |
| `EndGameSummary.cs` | Muestra el resumen de monedas recolectadas al final del juego. |
| `SceneSetup.cs` | Re-conecta las referencias del `LifeController` (jugador, checkpoint, corazones, pantallas) al cargar cada escena. |
| `DialogManager.cs` / `TalkingCow.cs` (clase `TalkingCowIntro`) | Diálogos con efecto de máquina de escribir y animación de la vaca hablando. `TypewriterDialog.cs` existe pero está vacío/sin usar. |
| `Menu.cs` / `BackToMenu.cs` / `ExitGame.cs` / `ButtonSound.cs` | Navegación de menús, volver al menú principal (reseteando el juego) y salir de la aplicación. |
| `MenuCowIdle.cs` / `CowFollower.cs` | Movimiento ambiental/decorativo de vacas (menú y niveles) moviéndose entre puntos aleatorios. |
| `TouchController.cs` / `MobileButton.cs` | Estado de input táctil (izquierda/derecha/salto) accionado por botones de UI en pantalla, consumido por `PlayerController`. |

### Controles

- **Teclado**: flechas o A/D para moverse, `Espacio` (o el botón mapeado a "Jump" en el Input Manager) para saltar (doble salto habilitado).
- **Táctil/móvil**: botones en pantalla (`MobileButton`) conectados a `TouchController`; si `TouchController` existe en la escena, el juego prioriza el input táctil sobre el teclado.

### Tags y capas personalizados

Definidos en [`ProjectSettings/TagManager.asset`](ProjectSettings/TagManager.asset):

- Tags: `NextLevel`, `Collectable`, `DeathZone`, `Hazard` (además de los tags built-in de Unity como `Player`).
- Capas: `Suelo`, `Water`, `Items` (además de las capas por defecto).
- Sorting Layers (orden de dibujado 2D): `Background`, `Entities`, `Foreground`, `Clouds`, `UICharacters`.

> ⚠️ El código también busca objetos por los tags `"Checkpoint"` y `"Coin"` (`GameObject.FindGameObjectWithTag(...)`), pero **esos dos tags no están declarados** en `TagManager.asset`. Si al abrir una escena ven errores de `UnityException: Tag ... is not defined`, es por esto — hay que agregar esos tags manualmente en `Edit > Project Settings > Tags and Layers` y asignarlos a los objetos correspondientes (el checkpoint de cada nivel y las monedas), o revisar si las escenas ya los traen embebidos y sólo falta declarar el tag.

---

## 6. Flujo del juego

1. `Menu.unity` — pantalla principal, botón **Play** (`Menu.cs`) carga la escena en el índice 1 de Build Settings.
2. `Level1.unity` → `Level2.unity` → `Level3.unity` → `Level4.unity` — el jugador recolecta monedas, esquiva peligros y llega al trigger de `NextLevel` para avanzar.
3. Si el jugador pierde las 3 vidas, se muestra la pantalla de **Game Over** con el resumen de monedas recolectadas.
4. Al llegar a la meta del último nivel, se muestra la pantalla de **victoria** (`panelYouWin`) con el resumen final.
5. Desde Game Over/Victoria se puede volver al menú principal, lo que resetea vidas, monedas y posición del jugador (`LifeController.ResetGame`).

---

## 7. Trabajando en equipo / buenas prácticas de Git

- **Nunca commitear** `Library/`, `Temp/`, `obj/`, `Logs/`, `UserSettings/`, `.utmp/`, ni archivos `.sln`/`.csproj` — ya están en [`.gitignore`](.gitignore) y Unity los regenera solo en cada máquina.
- Los archivos `.meta` de Unity **sí se versionan** (uno por cada asset/carpeta). No los borren ni los ignoren: son los que mantienen los GUIDs de referencias entre assets, prefabs y escenas.
- Para evitar conflictos difíciles de resolver en `.unity`/`.prefab` (son YAML pero grandes y sensibles al orden), lo ideal es:
  - Evitar que dos personas editen la **misma escena** al mismo tiempo.
  - Usar `Edit > Project Settings > Editor > Asset Serialization > Force Text` (ya debería venir así) para que los archivos sean diffeables como texto.
  - Si hay conflicto de fusión en una escena/prefab, normalmente es más rápido que una persona rehaga su cambio sobre la versión más reciente, en vez de mergear el YAML a mano.
- Antes de subir cambios, revisa `git status`: si ves que aparecen `Library/`, `.vs/`, `*.csproj` o similares como "untracked", probablemente hay un `.gitignore` roto o estás en la carpeta incorrecta — no los agregues.

---

## 8. Generar un build

1. Configura las escenas en `Build Settings` (ver sección 3.1) si aún no lo hiciste.
2. `File > Build Settings` → elige la plataforma (`PC, Mac & Linux Standalone`, `Android`, `iOS`, `WebGL`, etc.) → **Switch Platform** si no está seleccionada.
3. Revisa `Edit > Project Settings > Player`:
   - **Company Name**: `Barbarapvl`, **Product Name**: `The Cow`, **Version**: `1.0` (ajusten si corresponde).
   - **Active Input Handling**: el proyecto tiene instalado el paquete *Input System*, pero todo el código de gameplay usa la clase clásica `UnityEngine.Input` (`Input.GetAxisRaw`, `Input.GetButtonDown`). Verifiquen que esté en **"Input Manager (Old)"** o **"Both"** — si queda solo en **"Input System Package (New)"**, los controles de teclado dejarán de funcionar.
4. Click en **Build** (o **Build and Run** para probar directo en un dispositivo/emulador conectado).

---

## 9. Notas / pendientes conocidos

- `TypewriterDialog.cs` está vacío (sin lógica) — el efecto de máquina de escribir real vive en `DialogManager.cs`. Podría eliminarse o completarse según lo que necesite el equipo.
- Hay dos archivos de solución (`The Cow.sln`, `game1.sln`) sueltos en el repo generados por versiones previas del proyecto (posiblemente de cuando se llamaba "game1"); no están trackeados en git y Visual Studio/Rider los regenera automáticamente al abrir el proyecto, así que pueden ignorarse o borrarse localmente sin problema.
- Revisar `SceneSetup.cs`: asigna `LifeController.instancia.player` dos veces seguidas (una con `newPlayer` y luego con `winnerScreen`), lo que probablemente sea un error de copy-paste a corregir.

---

## 10. Créditos

Assets de arte de terceros incluidos en `Assets/Characters`, `Assets/Objects`, `Assets/Tilesets`, etc. (sprites tipo "Free Cow Sprites", "Mr. Circuit", "Ballooney", pixel art de items) — verificar licencias de cada asset pack antes de una publicación comercial o de hacer el repositorio público, en caso de que alguno no sea de uso libre.
