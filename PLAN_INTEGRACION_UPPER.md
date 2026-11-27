# Seguimiento de integración de Planta Alta

Este documento sirve como punto de reanudación si se interrumpe el trabajo o cambia el agente. Actualizar el estado y las decisiones aquí antes de cerrar cada sesión.

## Estado al 26 de noviembre de 2026

- Proyecto: `P_Mission_Ground`, Unity 6000.3.24f1 según el análisis de integración compartido.
- Rama activa: `integracion-upper`, siguiendo `origin/integracion-upper`.
- La rama tiene el commit `da4a58f` (`feat: actualizando cambios v1 upper`) por encima de `main`.
- Árbol de trabajo limpio al revisar.
- La integración existente añade Planta Alta, sus scripts, arte, recursos, videos, minijuegos y escenas. El cambio respecto a `main` es amplio (492 archivos); la rama no es únicamente el conjunto de 16 scripts citado en el resumen anterior.
- Este plan registra el seguimiento; todavía no se han modificado escenas ni scripts como parte de la sesión actual.

## Decisiones confirmadas por el usuario

1. Habrá 8 medallas en total: 4 en Planta Baja y 4 en Planta Alta.
2. Primero se consiguen las 4 medallas de Planta Baja. Al llegar al punto de acceso acordado, se muestra `Intro` (“¡Nueva zona desbloqueada! Planta Alta · Edificio P”) y luego se carga Planta Alta. Si faltan medallas, se informa al jugador.
3. El HUD muestra `0/4` en Planta Baja y `0/4` en Planta Alta; cada piso mantiene su contador de cuatro.
4. Se mantiene la elección de Cassandra o Leo durante toda la sesión y entre escenas.
5. Se copia la escena de salones del otro equipo junto con sus scripts y recursos necesarios, organizados en las carpetas del proyecto.
6. El nombre final del juego sigue sin definirse.
7. La integración se trabaja en una rama aparte. La rama `integracion-upper` ya existe y está activa.
8. El storyboard final citado corresponde al contenido del otro equipo. La regla final de cuándo mostrarlo debe quedar alineada con el total de medallas antes de implementarla; el resumen de integración propone mostrarlo al completar las 8.

## Hallazgos verificados que requieren atención

- `Intro` carga `PlantaAlta` tras el video y no reinicia las medallas de Planta Alta al hacerlo.
- El Build Settings incluye `Intro`, `PlantaAlta` y los cuatro minijuegos de Planta Alta.
- En el repositorio existen las escenas `Salones`, `Biblioteca`, `Intro`, `PlantaAlta` y cuatro escenas de minijuegos de Planta Alta: Cables, Laberinto, Oracle y Rompecabezas.
- Hay sistemas de medallas en `Assets/Scripts/Core` (Planta Baja) y `Assets/Scripts/PlantaAlta`. Falta comprobar cómo se conectan y persisten para cumplir el flujo 4 + 4.
- Existe selección de personaje en `Assets/Scripts/Core/SeleccionPersonaje.cs`; hay que comprobar que el jugador correcto y su estado sobrevivan las cargas de escena de ambos pisos.
- El análisis pegado describe riesgos potenciales (capas de minimapa, audio, escena SampleScene y sistemas duplicados). Confirmarlos en la rama actual antes de actuar; no asumir que el análisis previo sigue vigente en todos sus detalles.

## Plan de trabajo propuesto

### Fase 1 — Auditoría del flujo actual (sin cambios)

- Leer `AGENTS.md` si existe y revisar scripts de selección, medallas, puertas, cambio de escena e intro.
- Inspeccionar en Unity YAML los objetos y referencias de `Salones`, `Biblioteca`, `Intro` y `PlantaAlta`.
- Trazar el flujo actual de escenas y localizar el punto de acceso a Planta Alta según la decisión del equipo (el lugar exacto aún no está confirmado en este registro).
- Confirmar nombres de escenas usados por cada minijuego para regresar al mapa correcto.
- Revisar referencias rotas, GUID/metas, capas del minimapa y posibles choques entre scripts de ambos pisos.

### Fase 2 — Especificación de integración

- Definir una fuente de verdad para progreso de medallas: cuatro identificadores de Planta Baja y cuatro de Planta Alta, sin perder progreso al cargar escenas.
- Definir el comportamiento del acceso: requisito 4/4 de Planta Baja, aviso si faltan medallas, transición por `Intro` y carga de `PlantaAlta`.
- Definir el HUD por piso (`x/4`) y cuándo se muestra el storyboard final (propuesta pendiente de confirmar: 8/8).
- Definir cómo se conserva Cassandra/Leo, incluyendo animaciones y controlador usado en Planta Alta.
- Definir regreso entre pisos y persistencia tras minijuegos.

### Fase 3 — Implementación por partes pequeñas

1. Corregir y registrar escenas/destinos de build; evitar renombrar escenas hasta actualizar todas las referencias.
2. Implementar y revisar persistencia de personaje y estado entre escenas.
3. Integrar progreso de 4 + 4 medallas y HUD por piso.
4. Conectar el acceso Planta Baja → `Intro` → `PlantaAlta`, con bloqueo y mensaje cuando falten medallas.
5. Conectar el regreso y validar destinos de los cuatro minijuegos de Planta Alta.
6. Revisar storyboard final, audio, pausa, diálogos y capas del minimapa para evitar interferencias entre pisos.
7. Abrir el proyecto en Unity y probar el recorrido completo con ambos personajes.

## Protocolo para reanudar

Al comenzar una sesión nueva:

1. Leer este archivo y `git status --short --branch`.
2. Confirmar la rama actual; no crear otra automáticamente si `integracion-upper` ya existe.
3. Revisar los cambios sin commit antes de editar.
4. Retomar la primera casilla pendiente y actualizar este documento con hallazgos, archivos tocados y siguiente paso.
5. No dar por validado el juego hasta abrirlo/probarlo en Unity; anotar qué verificaciones sí se realizaron.

## Registro de sesiones

### 2026-11-26 — Reconocimiento inicial

- Se leyó el análisis de unificación proporcionado y se inspeccionó la estructura, las ramas, la lista de escenas de build y los destinos de Intro.
- Confirmado: rama `integracion-upper` activa, limpia y sincronizada con su remoto.
- Sin cambios en scripts o escenas. Se añade este documento de seguimiento.
- Próximo paso: Fase 1, auditoría del flujo actual.

### 2026-11-26 — Acceso de Planta Alta

- Se confirmó que el tránsito `Salones` → `Biblioteca` se conserva y no se necesita una puerta para Planta Alta.
- Al obtener la cuarta medalla de Planta Baja, `ContadorMedallas` carga automáticamente `Intro`, tanto si se completa en Salones como en Biblioteca.
- `Intro` ahora carga `PlantaAlta`. Se quitó el reinicio de medallas que ejecutaba al terminar el video, pues borraba el progreso de la sesión.
- Se registraron las escenas de Planta Alta y sus minijuegos en Build Settings.
- Se configuraron los controladores y sprites alternos para que Leo aparezca en Planta Baja y Cassandra en Planta Alta según la selección de sesión.
- Se actualizaron los cuatro destinos serializados de regreso de los minijuegos para que vuelvan a `PlantaAlta`.
- Se corrigió la Render Texture del minimapa de Planta Alta: su formato Depth Stencil estaba en `None`, lo que generaba un error de Render Graph en cada fotograma.
- Los videos de transición se convirtieron de MP4 H.264/AAC a WebM VP8/Vorbis, formato compatible con el editor Linux para importarlos como `VideoClip`.
- El diálogo de victoria de Planta Alta ahora puede avanzar con `E` o `Enter`. El storyboard final espera a que dicho diálogo termine antes de mostrarse.
- Los controles de diálogo, pausa y storyboard de Planta Alta usan el mismo clic global que Salones. Las medallas de Salones y Biblioteca reproducen el mismo sonido de recolección de Planta Alta al iniciar su vuelo hacia el contador.
- Pendiente inmediato: abrir Unity y comprobar visualmente ambos personajes, además del recorrido 4/4 → Intro → PlantaAlta.
