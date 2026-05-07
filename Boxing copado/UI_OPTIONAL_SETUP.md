# UI Opcional: Score en Pantalla (Headset)

Si querés ver el score DENTRO del headset sin necesidad de SideQuest logcat, seguí estos pasos.

## Setup en Unity (5 minutos)

### 1. Crear un Canvas en mundo

```
Hierarchy → Right+Click → UI → Canvas
├─ Nombrá: "GhostUI"
└─ Cambiar a "World Space":
   └─ Canvas → Render Mode: "World Space"
```

---

### 2. Crear dos Text elementos

**Crear Status Text:**
```
GhostUI → Right+Click → UI → Text (Legacy)
├─ Nombrá: "StatusText"
├─ Cambiar ancho/alto a gusto (ej: 500 x 200)
├─ En el componente Text:
│  ├─ Font Size: 30
│  ├─ Best Fit: Activado
│  └─ Content: "READY"
└─ En Rect Transform:
   └─ Pos Z: -2 (para que esté al frente)
```

**Crear Score Text:**
```
GhostUI → Right+Click → UI → Text (Legacy)
├─ Nombrá: "ScoreText"
├─ Pos Y: -150 (debajo del status)
├─ Font Size: 25
└─ Content: "Error: 0m"
```

---

### 3. Asignar el componente GhostPunchUI

1. En `GhostPunchSystem`, agregá:
   - Add Component → `GhostPunchUI`

2. Asigná los campos:
   - **Recorder**: tu `VRGhostRecorder`
   - **Playback**: tu `VRGhostPlayback`
   - **Scorer**: tu `GhostPunchScorer`
   - **Status Text**: `StatusText`
   - **Score Text**: `ScoreText`

---

## Cómo se ve en el juego

```
Durante GRABACIÓN:
└─ StatusText → "RECORDING..."

Cuando PRESIONAS PLAY (auto):
├─ StatusText → "PLAYBACK 1.5s"
└─ ScoreText → "Error: 0.123m"

Cuando TERMINA:
├─ StatusText → "READY"
└─ ScoreText → "SCORE: 87.3/100"
```

---

## Alternativa: Usar Canvas Screen Space (en pantalla)

Si preferís que esté en la esquina de pantalla (no en el mundo):

```
Canvas → Render Mode: "Screen Space - Camera"
├─ Render Camera: Arrastrá "Main Camera"
├─ Ui Scale Mode: Constant Pixel Size
└─ Reference Pixels Per Unit: 100
```

Luego posicioná los textos donde quieras en la pantalla.

---

## Notas

- Si no ves el Canvas, probá cambiar el Rect Transform Z a -10 (más atrás)
- Si el texto queda muy pequeño, aumentá el Font Size
- Si querés logs sin UI, mirá `SIDEQUEST_QUICK_START.md`

---

Done! Ya podés ver el score en vivo sin SideQuest logcat. 🎮

