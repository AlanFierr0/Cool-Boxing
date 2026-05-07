# ⚡ Motion Matching - Quick Start (5 min)

## 🚀 Resumen Ultra Rápido

1. **Crea** `MotionMatchingManager` GameObject
2. **Asigna componentes**: RobotMotionSampler, VRPlayerTracker, MotionComparer, MotionScoreSystem
3. **Rellena Inspector**:
   - Robot Kyle (root + Animator humanoid)
   - VR Player (XR Origin + camera + controllers)
4. **Llama en código**:
   ```csharp
   scoreSystem.StartRobotSampling();
   // Robot anima...
   scoreSystem.StopRobotSampling();
   scoreSystem.StartComparison();
   // Jugador imita
   scoreSystem.StopComparison();
   float score = scoreSystem.FinalScore;
   ```

---

## 🎯 Cómo Funciona (La Idea Clave)

```
Sistema NO compara frame exacto.
Usa "progreso normalizado" (0-1 en la secuencia).

📊 Ejemplo:
Robot:    |---X------|  (2 segundos, X = keyframe a 1s)
Jugador:  |--X--| (1.5 segundos, X = pose a 0.75s)

Progreso = tiempo_jugador / tiempo_robot = 0.75 / 2 = 0.375
Se compara contra frame del robot EN 0.375 de la secuencia.

Resultado:
✅ Jugador más lento = OK (trayectoria importa, timing flexible)
✅ Jugador más rápido = OK (trayectoria importa, timing flexible)
❌ Trayectoria diferente = score bajo
```

---

## 📋 Verificación Rápida

```
✅ ¿Animator del robot es HUMANOID?
   Animation → Model → Rig → Avatar: Humanoid

✅ ¿Robot tiene bones: Head, LeftHand, RightHand?
   Animator → GetBoneTransform() lo verifica

✅ ¿VR Player tiene XR Origin?
   Con Main Camera y Controllers como hijos

✅ ¿Todos los GameObjects asignados en Inspector?
   Sin errores en console
```

---

## 🧪 Test en Editor

```csharp
// En tu script de test
public void TestMotionMatching()
{
    // 1. Samplear
    scoreSystem.StartRobotSampling();
    yield return new WaitForSeconds(2f);
    scoreSystem.StopRobotSampling();

    // 2. Comparar (simular manualmente)
    scoreSystem.StartComparison();
    yield return new WaitForSeconds(1f);
    scoreSystem.StopComparison();

    // 3. Ver resultado
    Debug.Log($"Score: {scoreSystem.FinalScore}/100");
    Debug.Log($"Error: {scoreSystem.AverageError}m");
}
```

---

## 📚 Documentación

- **MOTION_MATCHING_SETUP.md** → Setup detallado paso a paso
- **Scripts** → Código comentado listo para pegar

---

## 🔧 Parámetros Clave (Ajustar Según Juego)

```csharp
// En MotionScoreSystem Inspector:
maxAllowedError = 0.5f;  // Tolerancia (metros)
                         // 0.3 = muy exigente
                         // 0.5 = equilibrado
                         // 0.7 = relajado

// En MotionComparer Inspector:
leftHandWeight = 0.4f;   // Mano izquierda
rightHandWeight = 0.4f;  // Mano derecha
headWeight = 0.2f;       // Cabeza (menos importancia)
```

---

## ✨ Resultado Final

```
🤖 Robot ejecuta golpe → Se samplea movimiento
👤 Jugador imita golpe → Se compara con tolerancia de timing
📊 Sistema calcula score 0-100
💯 Jugador recibe feedback
```

**Ya estás listo.** 🥊


