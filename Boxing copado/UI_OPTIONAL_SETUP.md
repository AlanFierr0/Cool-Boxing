# 🎨 Motion Matching - Setup Visual Paso a Paso

## 📐 Jerarquía de GameObjects Recomendada

```
Scene Root
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera
│   ├── LeftHand Controller
│   └── RightHand Controller
├── Robot_Kyle (tu robot con animaciones)
│   └── (bones: Head, LeftHand, RightHand)
└── MotionMatchingManager (☆☆☆ AQUÍ VA TU SISTEMA)
    ├── [Script] RobotMotionSampler
    ├── [Script] VRPlayerTracker
    ├── [Script] MotionComparer
    ├── [Script] MotionScoreSystem
    └── [Script] MotionMatchingExample (opcional)
```

---

## ✅ Setup Paso a Paso

### PASO 1: Crear el Manager GameObject

```
1. En tu escena, click derecho → Create Empty
2. Renombra a "MotionMatchingManager"
3. Reset position/rotation/scale (si no está ya)
```

**Resultado:**
```
Hierarchy:
└── MotionMatchingManager (Transform en 0,0,0)
```

---

### PASO 2: Agregar los 5 Scripts

Selecciona `MotionMatchingManager` → En Inspector:

```
1. Click "Add Component"
2. RobotMotionSampler
3. Click "Add Component"
4. VRPlayerTracker
5. Click "Add Component"
6. MotionComparer
7. Click "Add Component"
8. MotionScoreSystem
9. [OPCIONAL] Click "Add Component" → MotionMatchingExample
```

**Resultado:**
```
Inspector MotionMatchingManager:
├─ Transform
├─ RobotMotionSampler
├─ VRPlayerTracker
├─ MotionComparer
└─ MotionScoreSystem
```

---

### PASO 3: Configurar RobotMotionSampler

**En Inspector, busca el componente RobotMotionSampler:**

```
Robot Setup
├─ Robot Root:        [Arrastra "Robot_Kyle"]
├─ Robot Animator:    [Arrastra el Animator del robot]

Sampleo
├─ Sample Interval:   0.02
```

**Cómo arrastrar:**
1. En Hierarchy, encuentra `Robot_Kyle`
2. Arrastra a la casilla vacía de "Robot Root"
3. Haz lo mismo con el Animator

**Verificación:**
- ✅ Animator es Humanoid (Select Robot_Kyle → Assets → Rig → Avatar: Humanoid)
- ✅ Método `Animator.GetBoneTransform()` funcionará sin errores

---

### PASO 4: Configurar VRPlayerTracker

**En Inspector, busca el componente VRPlayerTracker:**

```
VR Player Setup
├─ Player Root:             [Arrastra "XR Origin"]
├─ Head Transform:          [Arrastra "Main Camera"]
├─ Left Hand Transform:     [Arrastra "LeftHand Controller"]
└─ Right Hand Transform:    [Arrastra "RightHand Controller"]
```

**La ruta típica es:**
```
XR Origin
├── Camera Offset
│   └── Main Camera        ← Esta
├── LeftHand Controller     ← Esta
└── RightHand Controller    ← Esta
```

**Cómo arrastrar:**
1. En Hierarchy, expande XR Origin
2. Arrastra XR Origin a "Player Root"
3. Arrastra Main Camera a "Head Transform"
4. Arrastra LeftHand Controller a "Left Hand Transform"
5. Arrastra RightHand Controller a "Right Hand Transform"

---

### PASO 5: Configurar MotionComparer

**En Inspector, busca el componente MotionComparer:**

```
Referencias
├─ Robot Sampler:          [El RobotMotionSampler del MotionMatchingManager]
└─ Player Tracker:         [El VRPlayerTracker del MotionMatchingManager]

Ponderación
├─ Left Hand Weight:       0.4
├─ Right Hand Weight:      0.4
└─ Head Weight:            0.2

Debug
├─ Draw Debug Gizmos:      true
└─ Gizmo Sphere Size:      0.05
```

**Cómo asignar las referencias:**
1. Arrastra el componente RobotMotionSampler de MotionMatchingManager
2. Arrastra el componente VRPlayerTracker de MotionMatchingManager

O por código:
```csharp
// Si no se asignan automáticamente:
RobotMotionSampler sampler = GetComponent<RobotMotionSampler>();
VRPlayerTracker tracker = GetComponent<VRPlayerTracker>();
MotionComparer comparer = GetComponent<MotionComparer>();
// Luego asignarlas manualmente
```

---

### PASO 6: Configurar MotionScoreSystem

**En Inspector, busca el componente MotionScoreSystem:**

```
Componentes
├─ Robot Sampler:         [RobotMotionSampler del MotionMatchingManager]
├─ Player Tracker:        [VRPlayerTracker del MotionMatchingManager]
└─ Motion Comparer:       [MotionComparer del MotionMatchingManager]

Scoring
├─ Max Allowed Error:      0.5
└─ Auto Start Sampling:    false

Debug
└─ Debug Logs:            true
```

**Asignación igual a PASO 5.**

---

### PASO 7 [OPCIONAL]: Configurar MotionMatchingExample

Si agregaste el script de ejemplo:

```
Motion Score System:       [Arrastra el MotionScoreSystem del MotionMatchingManager]
Robot Animator:            [Arrastra el Animator del robot]
Animation Duration:        2     (segundos de la animación)
Imitation Time Window:     4     (segundos que tiene el jugador para imitar)
```

---

## 🎬 Verificación Final

En Play Mode:

```
1. Play
2. En Hierarchy, selecciona MotionMatchingManager
3. Ve a Console (ventana de Debug)
4. En Inspector de MotionMatchingExample:
   - Click derecho en el script → "Simulate Quick Test"
   
Esperado en Console:
✅ "Simulando test rápido..."
✅ "RobotMotionSampler: grabación iniciada."
✅ "[Sampleo] Iniciado..."
✅ "[Comparación] Iniciada..."
✅ "RESULTADO FINAL"
✅ "Score: XX.X / 100"
```

Si ves errores:
```
❌ "robotAnimator no asignado"      → Asigna Robot Animator
❌ "playerRoot no asignado"         → Asigna Player Root / XR Origin
❌ "no se encontraron algunos bones" → Verifica Animator es Humanoid
```

---

## 📊 Estructura Final en Inspector

```
MotionMatchingManager (Active)
│
├─ Transform
│  └─ Position: 0, 0, 0
│
├─ RobotMotionSampler
│  ├─ Robot Root: Robot_Kyle
│  ├─ Robot Animator: [Animator del robot]
│  └─ Sample Interval: 0.02
│
├─ VRPlayerTracker
│  ├─ Player Root: XR Origin
│  ├─ Head Transform: Main Camera
│  ├─ Left Hand Transform: LeftHand Controller
│  └─ Right Hand Transform: RightHand Controller
│
├─ MotionComparer
│  ├─ Robot Sampler: [VRGhostRecorder(RobotMotionSampler)]
│  ├─ Player Tracker: [VRPlayerTracker]
│  ├─ Left Hand Weight: 0.4
│  ├─ Right Hand Weight: 0.4
│  ├─ Head Weight: 0.2
│  ├─ Draw Debug Gizmos: ON
│  └─ Gizmo Sphere Size: 0.05
│
├─ MotionScoreSystem
│  ├─ Robot Sampler: [RobotMotionSampler]
│  ├─ Player Tracker: [VRPlayerTracker]
│  ├─ Motion Comparer: [MotionComparer]
│  ├─ Max Allowed Error: 0.5
│  ├─ Auto Start Sampling: OFF
│  └─ Debug Logs: ON
│
└─ MotionMatchingExample [OPCIONAL]
   ├─ Score System: [MotionScoreSystem]
   ├─ Robot Animator: [Animator del robot]
   ├─ Animation Duration: 2
   └─ Imitation Time Window: 4
```

---

## 🧪 Flujo de Test

### Test 1: Verificar Sampleo del Robot
```
1. Play Mode
2. Inspector de RobotMotionSampler
3. Click derecho → "StartRecording" (desde MotionMatchingExample context menu)
4. Observa que robotAnimator.SetTrigger("PunchLeft") se ejecuta
5. Espera 2 segundos
6. Click en "StopRecording"
7. Console debe mostrar: "Sampleo de robot detenido. Frames: X"
```

### Test 2: Verificar Comparación
```
1. Play Mode
2. En MotionMatchingExample context menu: "Simulate Quick Test"
3. Observa logs:
   ✅ "Sampleo iniciado"
   ✅ "Grabación detenida. Frames: X"
   ✅ "Comparación iniciada"
   ✅ "RESULTADO FINAL: Score: XX.X / 100"
```

### Test 3: Debug Visual
```
1. Play Mode
2. Activa "Draw Debug Gizmos" en MotionComparer
3. Inicia comparación
4. En Scene View verás LÍNEAS AMARILLAS entre:
   - Mano del robot vs mano del jugador
5. Líneas cortas = buen score
6. Líneas largas = malo
```

---

## 🚀 Lleva a Producción

Una vez verificado:

```csharp
// En tu GameController o manager principal:

[SerializeField] private MotionScoreSystem scoreSystem;

public void ExecutePunchTutorial()
{
    GetComponent<MotionMatchingExample>().StartPunchTutorial("PunchLeft");
}
```

Luego:
```
Button (UI) → OnClick → ExecutePunchTutorial()
```

---

## 📌 Checklist Final

- [ ] MotionMatchingManager tiene 4+ componentes
- [ ] Robot Root apunta a tu robot
- [ ] RobotAnimator apunta bien y es Humanoid
- [ ] Player Root apunta a XR Origin
- [ ] Head, LeftHand, RightHand assignadas correctamente
- [ ] Sin errores rojos en Console
- [ ] Teste "Simulate Quick Test" funciona
- [ ] Score aparece en Console

**¡Listo para producción!** 🚀


