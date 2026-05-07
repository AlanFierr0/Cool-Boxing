# 🥊 Motion Matching Scoring System para VR Boxeo

## ✨ ¿Qué hace?

Sistema completo que permite al jugador **imitar movimientos del Robot Kyle** recibiendo un **score de 0 a 100** basado en qué tan fiel fue la imitación.

```
🤖 Robot Kyle ejecuta golpe
   ↓
📹 Sistema samplea la animación (posición + rotación cada 0.02s)
   ↓
👤 Jugador imita el movimiento moviendo cabeza y brazos
   ↓
⚡ Sistema compara poses en tiempo real (con tolerancia de timing flexible)
   ↓
📊 Genera score final basado en error promedio ponderado
```

---

## 📋 ¿Qué incluye?

### 5 Scripts Completos (Listos para Usar)
```
Assets/scripts/
├─ MotionFrame.cs              ← Modelo de datos
├─ RobotMotionSampler.cs       ← Samplea robot humanoid
├─ VRPlayerTracker.cs          ← Trackea VR player
├─ MotionComparer.cs           ← Compara + calcula error
└─ MotionScoreSystem.cs        ← Orquesta todo
```

### 1 Script de Ejemplo
```
├─ MotionMatchingExample.cs    ← Muestra cómo usar (copiar lógica)
```

### 5 Documentos de Referencia
```
MOTION_MATCHING_SETUP.md          ← Setup paso a paso (MÁS IMPORTANTE)
MOTION_MATCHING_QUICK_START.md    ← Resumen rápido (5 min)
UI_OPTIONAL_SETUP.md              ← Inspector visual
IMPLEMENTATION_COMPLETE.md        ← Documentación técnica
MOTION_MATCHING_CHEATSHEET.txt    ← Para imprimir
```

---

## 🚀 Cómo Empezar (3 pasos)

### 1️⃣ Setup en Unity (10 min)
Lee: **`MOTION_MATCHING_SETUP.md`** (es el documento clave)

Sigue paso a paso:
- Crea `MotionMatchingManager` GameObject
- Asigna los 4 componentes
- Rellena el Inspector con referencias de robot y jugador VR
- ✅ No debería haber errores

### 2️⃣ Integra en tu código (5 min)
```csharp
// En tu script principal:
scoreSystem.StartRobotSampling();
// Robot anima...
scoreSystem.StopRobotSampling();

scoreSystem.StartComparison();
// Jugador imita...
scoreSystem.StopComparison();

float score = scoreSystem.FinalScore;
```

### 3️⃣ Prueba

**En Editor:**
- Play Mode
- Context menu de `MotionMatchingExample`: "Simulate Quick Test"
- Deberías ver en Console: `Score: XX.X / 100`

**En VR (Meta Quest):**
- Build APK
- Deploy con SideQuest
- Prueba con headset real

---

## 🎯 Características Clave

✅ **Timing Flexible**
   - El jugador puede ir más lento o más rápido
   - El sistema compara por "progreso normalizado" (0-1)
   - Solo penaliza si la trayectoria es diferente

✅ **Scoring Ponderado**
   - 40% mano izquierda
   - 40% mano derecha  
   - 20% cabeza
   - Puedes ajustar los pesos

✅ **Local Space**
   - Comparación independiente de posición global
   - Robot y jugador pueden estar en cualquier lado del nivel

✅ **Debug Visual**
   - Gizmos muestran líneas amarillas entre poses
   - Líneas cortas = buen score ✅
   - Líneas largas = score bajo ❌

✅ **Arquitectura Limpia**
   - 5 scripts independientes
   - Fácil de extender
   - Código comentado

---

## 📚 Documentación

| Urgencia | Documento | Contenido |
|----------|-----------|----------|
| 🔴 Primero | **MOTION_MATCHING_SETUP.md** | Setup paso a paso + checklist |
| 🟡 Segundo | **MOTION_MATCHING_QUICK_START.md** | Resumen rápido (5 min) |
| 🟢 Tercero | **UI_OPTIONAL_SETUP.md** | Setup visual en Inspector |
| 🔵 Referencia | **IMPLEMENTATION_COMPLETE.md** | Documentación técnica |
| 📌 Portable | **MOTION_MATCHING_CHEATSHEET.txt** | Para imprimir |

---

## 🔧 Parámetros Ajustables

```csharp
// En MotionScoreSystem:
maxAllowedError = 0.5f;  
  → 0.2 = MUY difícil
  → 0.5 = Equilibrado ⭐
  → 1.0 = Muy fácil

// En MotionComparer:
leftHandWeight = 0.4f;   (40%)
rightHandWeight = 0.4f;  (40%)
headWeight = 0.2f;       (20%)
```

---

## ✨ Ejemplo Completo

```csharp
public class BoxerTutorial : MonoBehaviour
{
    [SerializeField] private Animator robotAnimator;
    [SerializeField] private MotionScoreSystem scoreSystem;

    public void StartPunchLesson()
    {
        StartCoroutine(LessonRoutine());
    }

    private IEnumerator LessonRoutine()
    {
        // 1. Robot demuestra
        Debug.Log("Observa cómo golpea el robot...");
        scoreSystem.StartRobotSampling();
        robotAnimator.SetTrigger("PunchLeft");
        
        yield return new WaitForSeconds(2.5f);
        scoreSystem.StopRobotSampling();

        // 2. Show instructions
        Debug.Log("¡Ahora imita el movimiento!");
        yield return new WaitForSeconds(1f);

        // 3. Jugador imita
        scoreSystem.StartComparison();
        yield return new WaitForSeconds(4f);
        scoreSystem.StopComparison();

        // 4. Mostrar resultado
        float score = scoreSystem.FinalScore;
        Debug.Log($"Score: {score:F1}/100");

        if (score >= 80f)
            Debug.Log("✅ ¡Excelente!");
        else if (score >= 60f)
            Debug.Log("👍 ¡Bien!");
        else
            Debug.Log("💪 Intenta de nuevo");
    }
}
```

---

## 🎮 Casos de Uso

- **Tutorial de Golpes**: Enseñar punches correctas
- **Mode de Práctica**: Entrenar movimientos específicos
- **Desafíos**: Score mínimo para avanzar
- **Leaderboard**: Guardar scores más altos
- **Feedback**: Mostrator dónde falló el jugador

---

## 🧪 Testing

### Test 1: Verify en Editor
```
1. Play Mode
2. MotionMatchingExample → "Simulate Quick Test"
3. Espera resultado
4. Console debe mostrar: "Score: XX.X / 100"
```

### Test 2: Verify Debug Visuals
```
1. Activar "Draw Debug Gizmos" en MotionComparer
2. Play Mode → iniciar comparación
3. Scene View (no Game View) debe mostrar líneas amarillas
```

### Test 3: Verify en VR
```
1. Build APK
2. Deploy con SideQuest
3. Usa Meta Quest real
4. Ejecuta el algoritmo
```

---

## 📊 Cómo Funciona Internamente

### Fase 1: Sampleo
```
Cada 0.02 segundos mientras robot anima:
  ├─ GetBoneTransform(Head/LeftHand/RightHand)
  ├─ InverseTransformPoint → local space
  └─ Guardar en List<MotionFrame>
Result: ~125 frames en 2.5 segundos
```

### Fase 2: Comparación (Timing Flexible)
```
Cada frame while jugador imita:
  ├─ Calcular "progreso normalizado" = jugadorTime / robotDuration
  ├─ Buscar frame del robot en ese progreso
  ├─ Calcular distancia: robot vs jugador
  └─ Acumular error
```

### Fase 3: Scoring
```
Score = (1 - avgError / maxAllowedError) * 100
  ├─ avgError = promedio de errores capturados
  ├─ maxAllowedError = configurable (0.5 por defecto)
  └─ Clamp entre 0 y 100
```

---

## 🐛 Troubleshooting

| Problema | Solución |
|----------|----------|
| **Score siempre 0** | Aumenta `maxAllowedError` a 0.7 o 1.0 |
| **"Bones not found"** | Verifica Animator sea Humanoid |
| **Lineas debug no aparecen** | Activa "Draw Debug Gizmos" en MotionComparer |
| **Comparación no compara** | Verifica que haya frames grabados |
| **VR Player no se trackea** | Asigna transforms en Inspector |

---

## 🚀 Quick Links

- 📖 **Para setup**: Lee `MOTION_MATCHING_SETUP.md` (es clave)
- 📋 **Para resumen**: Lee `MOTION_MATCHING_QUICK_START.md`
- 🔧 **Para código**: Copia lógica de `MotionMatchingExample.cs`
- 📚 **Para tecnicaturas**: Lee `IMPLEMENTATION_COMPLETE.md`
- 📌 **Para llevar**: Imprime `MOTION_MATCHING_CHEATSHEET.txt`

---

## ✅ Checklist Antes de Producción

- [ ] 5 scripts en `Assets/scripts/`
- [ ] `MotionMatchingManager` GameObject creado
- [ ] Referencias asignadas en Inspector
- [ ] "Simulate Quick Test" funciona
- [ ] Debug gizmos funcionan
- [ ] Score se calcula correctamente
- [ ] Documentos leídos
- [ ] Parámetros ajustados según juego

---

## 📞 FAQ

**P: ¿Funciona sin XR Simulator?**
R: Sí, usa hardware real (headset + controllers)

**P: ¿Funciona en Meta Quest?**
R: Sí, con build APK + SideQuest

**P: ¿Puedo cambiar scoring?**
R: Sí, edita weights y maxAllowedError

**P: ¿Puedo grabar múltiples golpes?**
R: Sí, ejecuta StartRobotSampling + StopRobotSampling varias veces

**P: ¿Cómo hago replay del movimiento?**
R: Próxima extensión: agrega MotionRecorder para grabar jugador también

---

## 🎬 Próximos Pasos

1. Lee **`MOTION_MATCHING_SETUP.md`** completamente
2. Sigue el setup paso a paso
3. Ejecuta "Simulate Quick Test"
4. Ajusta parámetros según tus necesidades
5. Integra en tu gameloop
6. ¡Juega! 🥊

---

**¡Todo listo para boxear en VR!** 🚀

Para dudas o problemas, revisa los documentos de referencia o el código comentado.


