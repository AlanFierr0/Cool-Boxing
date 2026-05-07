# 📘 Motion Matching - Documentación Técnica Completa

## 🏗️ Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────────┐
│            MotionScoreSystem (Orquestador Principal)        │
└─────────────────────────────────────────────────────────────┘
              ↓                    ↓                    ↓
    ┌─────────────────┐ ┌──────────────────┐ ┌──────────────────┐
    │ RobotMotion     │ │ VRPlayerTracker  │ │ MotionComparer   │
    │ Sampler         │ │                  │ │                  │
    │                 │ │ • Head           │ │ • Calcula error  │
    │ • Samplea       │ │ • LeftHand       │ │   ponderado      │
    │   animación     │ │ • RightHand      │ │ • Normalized     │
    │   del robot     │ │                  │ │   progress       │
    │                 │ │ (Espacio local)  │ │                  │
    │ (Humanoid bones)│ └──────────────────┘ └──────────────────┘
    └─────────────────┘
           ↓
    ┌─────────────────┐
    │  MotionFrame    │
    │  (Data Model)   │
    │                 │
    │ • time          │
    │ • head pose     │
    │ • hand poses    │
    │ • rotaciones    │
    │ (Local space)   │
    └─────────────────┘
```

---

## 🔄 Flujo de Datos

```
┌──────────────────────────────────────────────────────────────┐
│ 1. SAMPLEO: RobotMotionSampler                               │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  MientrasGraba():                                            │
│    ├─ Cada 0.02s (50 Hz)                                    │
│    ├─ Animator.GetBoneTransform(Head/LeftHand/RightHand)    │
│    ├─ InverseTransformPoint(worldPos) → localPos            │
│    ├─ Quaternion.Inverse() → localRot                       │
│    └─ Guardar en List<MotionFrame>                          │
│                                                              │
│  Resultado: [MotionFrame, MotionFrame, ..., MotionFrame]    │
│             ↑ t=0.02s        ↑ t=0.04s      ↑ t=1.5s      │
│                                                              │
└──────────────────────────────────────────────────────────────┘
              ↓
┌──────────────────────────────────────────────────────────────┐
│ 2. COMPARACIÓN: MotionComparer + VRPlayerTracker             │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  CadaFrameJugador():                                         │
│    ├─ GetCurrentFrame() → pose VR player en LOCAL space    │
│    ├─ Calcular "normalized progress" (0 a 1)              │
│    │   progress = playerTime / robotDuration               │
│    ├─ Buscar frame del robot EN ese progreso              │
│    │   robotTime = robotDuration * progress                │
│    ├─ Comparar:                                            │
│    │   ├─ DistanciaLeftHand = ||robot - jugador||         │
│    │   ├─ DistanciaDerecha = ||robot - jugador||          │
│    │   └─ DistanciaHead = ||robot - jugador||             │
│    └─ Score ponderado = 40% + 40% + 20%                   │
│                                                              │
│  Resultado: [Error1, Error2, ..., ErrorN]                    │
│             (una muestra por frame de jugador)               │
│                                                              │
└──────────────────────────────────────────────────────────────┘
              ↓
┌──────────────────────────────────────────────────────────────┐
│ 3. SCORING: MotionScoreSystem                                │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  FinalScore():                                               │
│    ├─ Promedio de errores capturados                        │
│    │   avgError = SUM(errors) / COUNT(errors)              │
│    └─ Convertir a score 0-100                              │
│        score = Clamp(1 - avgError / maxError) * 100         │
│                                                              │
│  Resultado: Score = 75.5 / 100                              │
│             Error = 0.145 m                                 │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## 🎯 Algoritmo de Normalized Progress (Timing Flexible)

### El Problema Original
```
❌ Si comparas frame por frame exacto:

Robot:    Frame 0  Frame 1  Frame 2  Frame 3  (2 segundos)
          │        │        │        │
Jugador:  Frame 0  Frame 1  (1 segundo, más lento)
          │        │

Resultado: Cuando el robot está en Frame 2, 
el jugador apenas está en Frame 1.
Comparación desalineada → Score bajo sin razón.
```

### La Solución: Normalized Progress
```
✅ Comparar por "progreso normalizado" (0 a 1):

Robot duration:    2 segundos
Jugador tiempo:    1 segundo
Progreso:          1 / 2 = 0.5 (50% del camino)

Se busca el frame del robot EN 0.5 de su secuencia:
  robotTimeAtProgress = 2 * 0.5 = 1 segundo
  → Se compara contra pose del robot EN 1 segundo

Resultado: Aunque el jugador sea más lento,
se compara contra la pose correcta del robot.
Score depende de TRAYECTORIA, no de timing.
```

### Ejemplo Paso a Paso
```
SETUP:
  - Robot duration: 2s
  - Robot frames: [t=0, t=0.02, t=0.04, ..., t=2.0]
  - Jugador comienza a imitar en t=0

EJECUCIÓN:
  En t=0.5s del jugador:
    ├─ Progreso normalizado = 0.5 / 2 = 0.25
    ├─ Robot time target = 2 * 0.25 = 0.5s
    ├─ Se busca frame del robot en 0.5s
    ├─ Se compara pose jugador vs Robot@0.5s
    └─ Se calcula error
    
  En t=1.0s del jugador:
    ├─ Progreso normalizado = 1.0 / 2 = 0.5
    ├─ Robot time target = 2 * 0.5 = 1.0s
    ├─ Se compara contra Robot@1.0s
    └─ Error registrado
    
  En t=2.0s del jugador (más rápido):
    ├─ Progreso normalizado = 2.0 / 2 = 1.0
    ├─ Robot time target = 2 * 1.0 = 2.0s
    ├─ Se compara contra Robot@2.0s (final)
    └─ Error registrado

RESULTADO: 
  Jugador más lento/rápido, pero trayectoria correcta = SCORE ALTO ✅
  Jugador con trayectoria diferente = SCORE BAJO ❌
```

---

## 📐 Fórmula de Score Ponderada

### Cálculo de Error por Extremidad
```csharp
error_extremidad = (distancia_posicion * 0.8) 
                 + (distancia_rotacion * 0.2)

Donde:
  distancia_posicion = ||posRobot - posJugador||  (metros)
  distancia_rotacion = (angleRobot - angleJugador) / 180 * 0.5
                      (normaliza rotación a escala métrica)

Interpretación:
  - 80% posición (más importante)
  - 20% rotación (penaliza si está rotado mal)
```

### Cálculo de Score Final
```csharp
error_ponderado = (error_left_hand * 0.4) 
                + (error_right_hand * 0.4) 
                + (error_head * 0.2)

score = Clamp(1 - (error_ponderado / maxAllowedError)) * 100

Donde:
  maxAllowedError = 0.5 (configurable)
  Clamp(x) = min(1, max(0, x))

Ejemplos:
  error_ponderado = 0.0    → score = 100 (perfecto)
  error_ponderado = 0.25   → score = 50
  error_ponderado = 0.5    → score = 0 (muy mal)
  error_ponderado = 0.75   → score = 0 (muy mal)
```

---

## 🧠 Local Space vs World Space

### ¿Por qué Local Space?

```
✅ Usa LOCAL SPACE (relativo al robot/jugador root):

Ventajas:
  1. Robot puede estar en cualquier posición del mundo
  2. Jugador puede estar en cualquier posición del mundo
  3. Comparación es INVARIANTE a posición global
  4. El score depende solo de LA FORMA del movimiento

❌ NO uses World Space:

Desventajas:
  1. Si robot está en (0,0,0) y jugador en (5,0,0)
     → Distancias enormes aunque muevan igual
  2. Mover de un lado a otro del nivel = diferentes scores
  3. Score depende de posición externa, no de calidad
```

### Conversión

```csharp
// De World a Local
Vector3 posLocal = root.InverseTransformPoint(posWorld);
Quaternion rotLocal = Quaternion.Inverse(root.rotation) * rotWorld;

// De Local a World (para debug)
Vector3 posWorld = root.TransformPoint(posLocal);
Quaternion rotWorld = root.rotation * rotLocal;
```

---

## 🔍 Ponderación Recomendada para Boxeo

```
Caso de uso: Juego de Boxeo VR

Recomendación:
  leftHandWeight = 0.4   (Muy importante, la mano golpea)
  rightHandWeight = 0.4  (Muy importante, la mano golpea)
  headWeight = 0.2       (Menos crítica, pero útil para evaluación)

Razonamiento:
  - En boxeo, la exactitud de las manos es crítica
  - La cabeza se mueve menos esencialmente
  - 40/40/20 refleja esto

Alternativas según juego:
  Juego "Full Body":     0.35 / 0.35 / 0.30 (más cabeza)
  Juego "Manos Solo":    0.50 / 0.50 / 0.00 (sin cabeza)
  Juego "Coreografía":   0.33 / 0.33 / 0.34 (igual todo)
```

---

## 📊 Parámetros Recomendados

```csharp
// RobotMotionSampler
sampleInterval = 0.02f;  // 50 Hz, suficiente resolución
                         // 0.05 = 20 Hz (más bajo)
                         // 0.01 = 100 Hz (más alto, más preciso)

// MotionComparer
leftHandWeight = 0.4f;
rightHandWeight = 0.4f;
headWeight = 0.2f;

// MotionScoreSystem
maxAllowedError = 0.5f;  // Ajustar según dificultad
                         // 0.2 = MUY difícil
                         // 0.5 = Equilibrado
                         // 1.0 = Muy fácil

// MotionMatchingExample
animationDuration = 2.5f;   // Duración de la anima robot
imitationTimeWindow = 4.0f; // Tiempo para que juegue imite
```

---

## 🐛 Debug y Troubleshooting

### Líneas Debug Amarillas en Scene View
```
Si está activado "Draw Debug Gizmos":
  - Líneas CORTAS = manos cerca (buen score)
  - Líneas LARGAS = manos lejos (score bajo)
  - Sin líneas = jugador fuera del rango
```

### Logs Importantes
```
Console:
  "[Sampleo] Iniciado..."
  "RobotMotionSampler: grabación iniciada."
  "RobotMotionSampler: grabación detenida. Frames: 125"
  
  "[Comparación] Iniciada..."
  (Cada frame: silent, a menos que error)
  
  "RESULTADO FINAL"
  "Score: 75.5 / 100"
  "Error prom: 0.1234 m"
```

### Problemas Comunes

```
❌ Score siempre 0
  └─ maxAllowedError < promedio error del robot
  └─ Solución: Aumenta maxAllowedError a 0.7 o 1.0

❌ Bones no se sampléan
  └─ Animator no es Humanoid
  └─ Solución: Rig → Avatar: Humanoid

❌ VR Pose no se registra
  └─ Transforms no asignados correctamente
  └─ Solución: Verifica playerRoot, headTransform, etc.

❌ Comparación no produce error
  └─ Frames grabados vacío
  └─ Solución: Verifica duración de animación robot
```

---

## 🎮 Caso de Uso: Punching Bag Demo

```csharp
// Pseudocódigo
public class PunchingBagGame : MonoBehaviour
{
    void Start()
    {
        scoreSystem = GetComponent<MotionScoreSystem>();
    }

    void OnPunchRequested(string punchType)
    {
        // 1. Robot demuestra
        scoreSystem.StartRobotSampling();
        robotAnimator.SetTrigger(punchType);
        yield WaitForSeconds(2.5f);
        scoreSystem.StopRobotSampling();

        // 2. Mostrar countdown
        yield WaitForSeconds(1);

        // 3. Jugador imita
        scoreSystem.StartComparison();
        yield WaitForSeconds(4);
        scoreSystem.StopComparison();

        // 4. Mostrar resultado
        float score = scoreSystem.FinalScore;
        audioManager.PlaySFX(score > 80 ? "success" : "failure");
        uiManager.DisplayScore(score);
    }
}
```

---

## 🔮 Extensiones Futuras

```
1. RECORDING & REPLAY
   - Guardar movimiento del jugador
   - Reproducir como "ghost"
   - Comparar lado a lado

2. FEEDBACK EN TIEMPO REAL
   - HUD que muestre score provisional
   - Vibración háptica en manos
   - Instrucciones ("Baja más la mano")

3. MÚLTIPLES MOVIMIENTOS
   - Combo: 3 golpes seguidos
   - Evaluar transiciones
   - Score por combo completo

4. ANÁLISIS AVANZADO
   - Mostrar dónde falló
   - Velocidad de ejecución
   - Consistencia frame a frame

5. MULTIJUGADOR
   - 1vs1: quién hace mejor score
   - Ligas y rankings
   - Desafíos específicos
```

---

## 📌 Sumario de Éxito

✅ **Arquitectura limpia**: 5 scripts independientes  
✅ **Timing flexible**: Normalized progress permite variación  
✅ **Local space**: Score independiente de posición global  
✅ **Scoring ponderado**: Extremidades según importancia  
✅ **Debug visual**: Gizmos para ver qué está pasando  
✅ **Extensible**: Fácil agregar nuevas evaluaciones  

**¡Listo para producción!** 🚀


