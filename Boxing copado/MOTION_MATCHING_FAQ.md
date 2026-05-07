# ❓ Motion Matching - FAQ & Troubleshooting

## 🔴 ERRORES COMUNES Y SOLUCIONES

### Error 1: "robotAnimator not assigned"
```
❌ Error en Console:
   "RobotMotionSampler: robotAnimator no asignado."

✅ Solución:
   1. En Inspector, busca RobotMotionSampler
   2. Campo: "Robot Animator"
   3. Arrastra el componente Animator del robot
   4. Si no ves, selecciona el robot y busca este componente
   
💡 Tips:
   - El Animator debe estar EN el robot
   - No en un padre lejano
   - Si no existe, agrega Script > Animator al robot
```

---

### Error 2: "no se encontraron algunos bones humanoid"
```
❌ Error en Console:
   "RobotMotionSampler: no se encontraron algunos bones humanoid."

✅ Solución:
   1. Selecciona el modelo del robot en Assets/
   2. En Inspector, ve a la pestaña "Rig"
   3. Verifica que "Avatar Type" = "Humanoid"
   4. Si es "Generic", cambia a "Humanoid"
   5. Click "Apply"
   6. Vuelve a Unity y refresca
   
💡 Tips:
   - Los rigging humanoid tienen bones: Head, LeftHand, RightHand
   - Si es generic, Animator.GetBoneTransform() no funciona
   - Necesita estar configurado correctamente desde Blender/Max
```

---

### Error 3: "playerRoot not assigned"
```
❌ Error en Console:
   "VRPlayerTracker: playerRoot no asignado (debería ser XR Origin)."

✅ Solución:
   1. En Inspector, busca VRPlayerTracker
   2. Campo: "Player Root"
   3. Arrastra el XR Origin de tu escena
   4. No la cámara, sino la raíz del rig
   
Estructura típica:
   XR Origin (← Esta)
   ├── Camera Offset
   │   └── Main Camera
   ├── LeftHand Controller
   └── RightHand Controller
   
💡 Tips:
   - playerRoot es la raíz de todo el rig VR
   - No es Main Camera
   - Si no tienes XR Origin, necesitas XR Rig setup
```

---

### Error 4: "Score siempre 0"
```
❌ Problema:
   Score = 0 siempre, sin importar cómo se mueva

✅ Soluciones:

Opción A: maxAllowedError muy bajo
   1. Va a MotionScoreSystem en Inspector
   2. Parámetro: "Max Allowed Error"
   3. Aumenta de 0.5 a 0.7 o 1.0
   4. Prueba de nuevo

Opción B: Robot nunca se samplea correctamente
   1. Verifica que StartRobotSampling() sea llamado
   2. Verifica que robotAnimator tenga animación
   3. Verifica duración: animación debe durar > 1 segundo
   4. En Console: busca "Frames capturados: X"
      - Si X es pequeño (< 10), la animación es muy corta

Opción C: Poses del jugador no se registran
   1. Verifica que playRoot, headTransform, etc. estén asignados
   2. En Play mode, selecciona VRPlayerTracker
   3. Abre la pestaña "Transform" y verifica posiciones cambian
```

---

### Error 5: "Lineas amarillas no aparecen en Scene view"
```
❌ Problema:
   No veo las líneas de debug que conectan manos robot/jugador

✅ Solución:
   1. En Inspector, busca MotionComparer
   2. Checkbox: "Draw Debug Gizmos" → activar ✅
   3. Asegúrate de estar en SCENE VIEW (no Game view)
   4. Inicia comparación
   5. Deberías ver líneas amarillas
   
💡 Tips:
   - Game View no muestra gizmos
   - Scene View sí, pero solo en modo Play
   - Líneas cortas = buen score
   - Líneas largas = score bajo
```

---

### Error 6: "Comparación nunca inicia"
```
❌ Problema:
   Llamo a StartComparison() pero no pasa nada

✅ Solución:
   1. ¿Llamaste StarttRobotSampling() primero?
      - Si no: hazlo antes de StartComparison()
   
   2. ¿Verificaste que recordedFrames tiene contenido?
      - En Console: busca "Frames capturados: X"
      - Si X = 0, el sampleo no funcionó
   
   3. ¿RobotMotionSampler está asignado en MotionScoreSystem?
      - Verifica en Inspector
   
   4. ¿Llamaste StopRobotSampling()?
      - Debe terminar el sampleo antes de comparar

Flujo correcto:
   StartRobotSampling() → [espera] → StopRobotSampling() 
   → StartComparison() → [espera] → StopComparison()
```

---

### Error 7: "Transform es null"
```
❌ Problema:
   Error en Console: "NullReferenceException: Object reference not set"

✅ Solución:
   1. Verifica que todos los transforms estén asignados:
      - playerRoot
      - headTransform
      - leftHandTransform
      - rightHandTransform
      - robotRoot
      - robotAnimator
   
   2. Abre el script donde falla (línea exacta)
   
   3. Asigna el transform en Inspector
   
💡 Tips:
   - Si es null, no fue asignado
   - Busca el GameObject en Hierarchy y arrastra
```

---

## ❓ PREGUNTAS FRECUENTES

### P1: ¿Funciona sin XR Simulator?
**R:** Sí, totalmente. El sistema usa transforms reales de:
- Main Camera (para cabeza)
- Left/Right Controller (para manos)
- XR Origin como raíz

No depende de XR Simulator. Funciona con hardware real o Motion Controllers.

---

### P2: ¿Puedo cambiar los pesos del scoring?
**R:** Sí, en MotionComparer Inspector:
```
leftHandWeight = 0.4  → 40%
rightHandWeight = 0.4 → 40%
headWeight = 0.2      → 20%

Total debe sumar ~1.0
```

Ejemplo para juego diferente:
```
Solo manos:  0.5 / 0.5 / 0.0
Full body:   0.33 / 0.33 / 0.34
```

---

### P3: ¿Cómo hago el scoring más fácil/difícil?
**R:** Ajusta `maxAllowedError` en MotionScoreSystem:
```
0.1 = EXTREMADAMENTE DIFÍCIL (perfección requerida)
0.3 = Difícil
0.5 = Equilibrado ⭐ (por defecto)
0.7 = Fácil
1.5 = MUY FÁCIL
```

Mayor = más permisivo = scores más altos.
Menor = más exigente = scores más bajos.

---

### P4: ¿Puedo grabar varios golpes seguidos?
**R:** Sí, llama varias veces:
```csharp
for (int i = 0; i < 3; i++)
{
    scoreSystem.StartRobotSampling();
    robotAnimator.SetTrigger("PunchCombo" + i);
    yield WaitForSeconds(2.5f);
    scoreSystem.StopRobotSampling();
}
```

Cada llamada sobrescribe la grabación anterior.

---

### P5: ¿Cómo hago replay del movimiento del jugador?
**R:** El sistema actual solo guarda el robot, no el jugador.

Para grabar al jugador, necesitarías:
```csharp
// 1. Durante comparación, grabar frames del jugador
playerFrames = new List<MotionFrame>();
while (isComparing)
{
    playerFrames.Add(playerTracker.GetCurrentFrame());
}

// 2. Después, reproducir
foreach (MotionFrame frame in playerFrames)
{
    // Animar ghost objects del jugador
}
```

Se incluye como extensión futura.

---

### P6: ¿Qué pasa si el robot está en un lado y el jugador en otro?
**R:** No importa. El sistema usa **local space** (relativo a cada root):
```
Robot: robotRoot.InverseTransformPoint()  (local)
Jugador: playerRoot.InverseTransformPoint() (local)
Comparación: sin dependencia de posición global
```

Pueden estar en diferentes lugares del mundo y el score es igual.

---

### P7: ¿Puedo usar esto en multiplayer?
**R:** El sistema actual está diseñado para single-player.

Para multiplayer necesitarías:
- Networking para sincronizar grabaciones
- Sistema de scoring dual (ambos jugadores)
- Replicación de poses en red

Se incluye como extensión futura.

---

### P8: ¿Funciona en Meta Quest sin modificaciones?
**R:** Sí, todo funciona en hardware real.

Pasos:
1. Build APK (File → Build Settings → Android)
2. Deploy con SideQuest
3. Ejecuta en Quest
4. Mismo behavior que editor

Sin cambios de código.

---

### P9: ¿Puedo agregar más joints (nuca, hombros, etc.)?
**R:** Sí, expandiendo MotionFrame:
```csharp
// En MotionFrame.cs, agrega:
public Vector3 neckPositionLocal;
public Quaternion neckRotationLocal;

// En RobotMotionSampler.cs:
frame.neckPositionLocal = ...
frame.neckRotationLocal = ...

// En MotionComparer.cs:
// Agregar a la función CalculateLimbError()
```

Requiere editar varios archivos pero es directo.

---

### P10: ¿Cuál es la mejor forma de integrar esto en mi juego?
**R:** Patrón recomendado:
```csharp
public class LevelManager : MonoBehaviour
{
    [SerializeField] MotionScoreSystem scoreSystem;
    [SerializeField] Animator robotAnimator;
    
    public event Action<float> ScoreObtained;
    
    public IEnumerator PlayPunchChallenge(string punchType)
    {
        // Robot demuestra
        scoreSystem.StartRobotSampling();
        robotAnimator.SetTrigger(punchType);
        yield WaitForSeconds(2.5f);
        scoreSystem.StopRobotSampling();
        
        // UI: "Tu turno"
        yield WaitForSeconds(1f);
        
        // Player plays
        scoreSystem.StartComparison();
        yield WaitForSeconds(4f);
        scoreSystem.StopComparison();
        
        // Resultado
        ScoreObtained?.Invoke(scoreSystem.FinalScore);
    }
}
```

Desde cualquier Button:
```csharp
Button.onClick += () => 
    levelManager.PlayPunchChallenge("PunchLeft");
```

---

## 🔧 DEBUG AVANZADO

### Ver todos los errores disponibles
```csharp
scoreSystem.ErrorSamples  // List<float> de muestras
scoreSystem.FinalScore    // Score final 0-100
scoreSystem.AverageError  // Error promedio en metros
```

### Logs con timestamp
```csharp
if (debugLogs)
{
    Debug.Log($"[{System.DateTime.Now:HH:mm:ss.fff}] " +
              $"Score: {currentScore:F1}, Error: {currentError:F4}");
}
```

### Inspector debugging
```
En Play Mode:
1. Selecciona MotionScoreSystem
2. En Inspector:
   - IsComparing: true/false
   - AverageError: valor actual
   - FinalScore: resultado final
3. Monitorea cambios en tiempo real
```

---

## ✨ TIPS PARA OPTIMIZACIÓN

### Reducir CPU (si hay lag)
```csharp
// En RobotMotionSampler:
sampleInterval = 0.05f;  // De 0.02, reduce a 50→20 Hz
                         // Menos precisión pero más rápido

// En MotionScoreSystem:
debugLogs = false;       // Deshabilitar logs
```

### Mejorar precisión
```csharp
// En RobotMotionSampler:
sampleInterval = 0.01f;  // De 0.02, aumentar a 50→100 Hz
                         // Más preciso pero más CPU

// En MotionComparer:
gizmoSphereSize = 0.02f; // Gizmos más pequeños (visual)
```

---

## 📞 RESUMEN RÁPIDO

| Problema | Verificar | Solución |
|----------|-----------|----------|
| Score 0 | maxAllowedError | Aumentar a 0.7 |
| Bones null | Animator humanoid | Cambiar Rig a Humanoid |
| No hay frames | Score llamado antes/después | Orden: Sample → Stop → Compare |
| Gizmos no aparecen | Draw Debug Gizmos | Activar checkbox |
| Transform null | Referencias asignadas | Arrastra objetos en Inspector |
| Comparación no inicia | recordedFrames.Count | Verifica sampleo completó |

---

**¿Más dudas? Revisa los documentos de referencia o el código comentado.** 🚀


