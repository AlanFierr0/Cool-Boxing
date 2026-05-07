# 🤖 Motion Matching Scoring System - Setup Guide

## ✅ Scripts Listos
```
Assets/scripts/
├─ MotionFrame.cs              ← Modelo de datos de movimiento
├─ RobotMotionSampler.cs       ← Samplea animación del robot
├─ VRPlayerTracker.cs          ← Trackea pose del jugador VR
├─ MotionComparer.cs           ← Compara poses (timing flexible)
└─ MotionScoreSystem.cs        ← Orquesta todo el flujo
```

---

## 📋 Checklist Setup en Unity

- [ ] Crear `MotionMatchingManager` GameObject
- [ ] Agregar componentes: RobotMotionSampler, VRPlayerTracker, MotionComparer, MotionScoreSystem
- [ ] Asignar Robot Kyle:
  - [ ] robotRoot (raíz del robot)
  - [ ] robotAnimator (Humanoid Animator del robot)
- [ ] Asignar VR Player:
  - [ ] playerRoot (XR Origin)
  - [ ] headTransform (Main Camera)
  - [ ] leftHandTransform (Left Controller)
  - [ ] rightHandTransform (Right Controller)
- [ ] Conectar referencias cruzadas entre componentes
- [ ] [OPCIONAL] Crear UI para mostrar score

---

## 🛠️ Paso 1: Crear el Manager

En tu escena, crea un GameObject vacío:
```
MotionMatchingManager
```

---

## 🤖 Paso 2: Configurar RobotMotionSampler

### Asignar en el Inspector:

| Campo | Qué Asignar | Notas |
|-------|------------|-------|
| **Robot Root** | La raíz del Robot Kyle | Ej: `Robot_Kyle` |
| **Robot Animator** | El Animator del robot | Debe ser **Humanoid** |
| **Sample Interval** | `0.02` | Samplea cada 0.02s (50 Hz) |

### Validar que el Animator sea Humanoid:
1. Selecciona el modelo del robot en `Assets/`
2. En Inspector → **Rig Tab** → Avatar Type: **Humanoid**
3. Aplica cambios

---

## 🎮 Paso 3: Configurar VRPlayerTracker

### Asignar en el Inspector:

| Campo | Qué Asignar | Ruta típica |
|-------|------------|-----------|
| **Player Root** | XR Origin | `XR Origin` |
| **Head Transform** | Main Camera | `XR Origin/Camera Offset/Main Camera` |
| **Left Hand Transform** | Left Controller | `XR Origin/LeftHand Controller` |
| **Right Hand Transform** | Right Controller | `XR Origin/RightHand Controller` |

---

## 🔄 Paso 4: Configurar MotionComparer

### Asignar en el Inspector:

| Campo | Qué Asignar |
|-------|------------|
| **Robot Sampler** | Tu RobotMotionSampler |
| **Player Tracker** | Tu VRPlayerTracker |
| **Left Hand Weight** | `0.4` (40%) |
| **Right Hand Weight** | `0.4` (40%) |
| **Head Weight** | `0.2` (20%) |
| **Draw Debug Gizmos** | `true` para desarrollar |
| **Gizmo Sphere Size** | `0.05` |

---

## 🎯 Paso 5: Configurar MotionScoreSystem

### Asignar en el Inspector:

| Campo | Qué Asignar |
|-------|------------|
| **Robot Sampler** | Tu RobotMotionSampler |
| **Player Tracker** | Tu VRPlayerTracker |
| **Motion Comparer** | Tu MotionComparer |
| **Max Allowed Error** | `0.5` (metros) |
| **Auto Start Sampling** | `false` (controlarás manualmente) |
| **Debug Logs** | `true` para desarrollar |

---

## 🎬 Flujo Típico en Código

```csharp
// En tu script de gameloop:
MotionScoreSystem scoreSystem; // Tu referencia

// 1. Robot inicia animación
animator.SetTrigger("PunchLeft");

// 2. Iniciar sampleo del robot
scoreSystem.StartRobotSampling();

// 3. Esperar a que termine la animación (ej: 1.5s)
yield WaitForSeconds(1.5f);

// 4. Detener sampleo
scoreSystem.StopRobotSampling();

// 5. Mostrar "Ghosts" o instrucciones al jugador
Debug.Log("Ahora imita el movimiento del robot");

// 6. Iniciar comparación
scoreSystem.StartComparison();

// 7. El jugador imita (el sistema compara en Update)

// 8. Después de X segundos, detener comparación
yield WaitForSeconds(3f);
scoreSystem.StopComparison();

// 9. Leer resultado
Debug.Log($"Score: {scoreSystem.FinalScore:F1}/100");
Debug.Log($"Error: {scoreSystem.AverageError:F4}m");
```

---

## 🗝️ Características Clave

### ✅ Timing Flexible (Normalized Progress)
```
El sistema NO compara por frame exacto.
Usa "progreso normalizado" (0 a 1 en la secuencia).

Ejemplo:
- Robot duracion: 2 segundos
- Jugador tiempo: 0.5s
- Progreso: 0.5/2 = 0.25 (25%)
- Se compara contra el frame del robot en 25% de progreso

Resultado:
- Jugador puede ir más lento → obtiene score si trayectoria es correcta
- Jugador puede ir más rápido → obtiene score si trayectoria es correcta
- Cambios de trayectoria → score baja
```

### ✅ Scoring Ponderado
```
Score = (40% left_hand + 40% right_hand + 20% head)
Score = Clamp(1 - averageError / maxAllowedError) * 100

Manos son más importantes que cabeza (es un juego de boxeo).
```

### ✅ Tolerancia de Rotación
```
Se valida tanto posición como rotación.
Posición: 80% del error
Rotación: 20% del error (en escala métrica equivalente)
```

---

## 🧪 Probar en Editor

### 1. Play Mode
1. Presiona Play
2. En el Scene view, selecciona `MotionMatchingManager`
3. Ve a Console
4. En Inspector de MotionScoreSystem:
   - Click en "Start Robot Sampling"
   - Observa cómo el robot se mueve
   - Después de ~2s, click en "Stop Robot Sampling"
   - Click en "Start Comparison"
   - Mueve manualmente a Main Camera y Controllers (con transform edit tools)
   - Observa los logs con score actual

### 2. Debug Visual
- Activa "Draw Debug Gizmos" en MotionComparer
- En Scene view verás líneas amarillas entre manos del robot y jugador
- Líneas cortas = buen score
- Líneas largas = malo

---

## 🚀 Flujo Completo: Tutorial Boxer

```csharp
public class BoxerTutorial : MonoBehaviour
{
    [SerializeField] private Animator robotAnimator;
    [SerializeField] private MotionScoreSystem scoreSystem;

    public void StartTutorialPunch(string punchName)
    {
        StartCoroutine(TutorialRoutine(punchName));
    }

    private IEnumerator TutorialRoutine(string punchName)
    {
        // 1. Samplear movimiento del robot
        scoreSystem.StartRobotSampling();
        robotAnimator.SetTrigger(punchName);

        // 2. Esperar a que termine la animación
        yield return new WaitForSeconds(2.5f); // Ajustar según duración

        // 3. Detener sampleo
        scoreSystem.StopRobotSampling();

        // 4. Mostrar "Ahora tu turno"
        Debug.Log("¡Ahora imita el movimiento!");
        yield return new WaitForSeconds(0.5f);

        // 5. Iniciar comparison
        scoreSystem.StartComparison();

        // 6. Esperar a que el jugador imite
        yield return new WaitForSeconds(4f); // Dar tiempo para imitar

        // 7. Detener y mostrar resultado
        scoreSystem.StopComparison();
        
        float finalScore = scoreSystem.FinalScore;
        Debug.Log($"¡Resultado: {finalScore:F1}/100!");

        if (finalScore >= 80f)
            Debug.Log("¡Excelente!");
        else if (finalScore >= 60f)
            Debug.Log("¡Bien!");
        else
            Debug.Log("Intenta de nuevo.");
    }
}
```

---

## 📊 UI Opcional

Si quieres mostrar score en pantalla:

```csharp
public class MotionScoreUI : MonoBehaviour
{
    [SerializeField] private MotionScoreSystem scoreSystem;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text errorText;

    private void OnEnable()
    {
        scoreSystem.ComparisonTick += UpdateUI;
        scoreSystem.ComparisonEnded += ShowFinalScore;
    }

    private void UpdateUI(float currentScore, float currentError)
    {
        scoreText.text = $"Score: {currentScore:F1}/100";
        errorText.text = $"Error: {currentError:F4}m";
    }

    private void ShowFinalScore(float finalScore, float averageError)
    {
        scoreText.text = $"Final: {finalScore:F1}/100";
        scoreText.color = finalScore >= 80f ? Color.green : Color.red;
    }
}
```

---

## 🐛 Troubleshooting

| Problema | Solución |
|----------|----------|
| Score siempre 0 | Verifica que max error > error promedio del robot |
| Bones no se sampléan | Verifica que Animator sea Humanoid; chequea robotRoot y robotAnimator |
| Lineas debug no aparecen | Activa "Draw Debug Gizmos"; verifica Scene view (no Game view) |
| Comparación no inicia | Sampleo no terminó; verifica que hay frames grabados |
| VR Player no se trackea | Verifica que playerRoot y transforms estén asignados |

---

## 📌 Parámetros Recomendados

```csharp
// RobotMotionSampler
sampleInterval = 0.02f;  // 50 Hz, resolución fina

// MotionComparer
leftHandWeight = 0.4f;   // Importante en boxeo
rightHandWeight = 0.4f;  // Importante en boxeo
headWeight = 0.2f;       // Menos crítica

// MotionScoreSystem
maxAllowedError = 0.5f;  // Metros. Ajustar según juego.
                         // 0.3 = muy strict
                         // 0.5 = moderado
                         // 0.7 = flexible
```

---

## 🎯 Próximos Pasos

- [ ] Agregar múltiples animaciones (izquierda, derecha, gancho, upper cut)
- [ ] Sistema de combo (varias animaciones seguidas)
- [ ] Feedback háptico en controllers VR
- [ ] Persistencia de scores (leaderboard)
- [ ] Replay del movimiento grabado del jugador
- [ ] Correcciones parciales (mostrarte dónde fallaste)

---

**¡Listo para boxear!** 🥊


