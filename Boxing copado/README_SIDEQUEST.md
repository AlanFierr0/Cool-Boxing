# 🎮 Ghost Punch VR - SideQuest Setup (Resumen Final)

## ✅ Scripts Listos
```
Assets/scripts/
├─ VRGhostFrame.cs         ← Modelo de datos
├─ VRGhostRecorder.cs      ← Grabación en tiempo real
├─ VRGhostPlayback.cs      ← Reproducción con interpolación
├─ VRRecorderInput.cs      ← Input desde InputActionReference
├─ GhostPunchScorer.cs     ← Scoring por distancia
└─ GhostPunchUI.cs         ← [OPCIONAL] UI en pantalla
```

---

## 📋 Checklist Setup en Unity

- [ ] Crear `GhostPunchSystem` GameObject
- [ ] Agregar componentes: VRGhostRecorder, VRRecorderInput, VRGhostPlayback, GhostPunchScorer
- [ ] Asignar transforms: XROrigin, Main Camera, Left/Right Controller
- [ ] Crear Input Actions asset (BoxingControls con acción ToggleRecordPunch)
- [ ] Asignar InputActionReference en VRRecorderInput
- [ ] Crear 3 Ghost objects (GhostHead, GhostLeftHand, GhostRightHand)
- [ ] Asignar ghost objects en VRGhostPlayback
- [ ] Asignar real controllers en GhostPunchScorer
- [ ] [OPCIONAL] Crear UI Canvas y asignar en GhostPunchUI

---

## 🔧 Build para SideQuest

### Paso 1: Build Settings
```
File → Build Settings
├─ Selecciona Platform: Android
├─ Asegúrate que tu escena esté en "Scenes In Build"
└─ Click en "Build APK"
```

### Paso 2: Esperar
```
⏳ Esperá 2-5 minutos
✅ Resultado: app.apk en la carpeta que elegiste
```

---

## 📱 Preparar Meta Quest

### EN EL HEADSET:
```
1. Settings → About → Toca "Build Number" 7 veces
   ✅ Aparece: "You are now a developer"

2. Settings → Developer → USB Debugging → ON

3. Conectá a PC por USB
   ✅ En Quest: Allow cuando pida permisos
```

---

## 🚀 Instalar con SideQuest

### Paso 1: Bajar SideQuest
```
https://sidequestvr.com/setup
├─ Bajá e instalá
└─ Abrí SideQuest
```

### Paso 2: Instalar la APK
```
SideQuest:
├─ Click ícono de carpeta (arriba izquierda)
├─ Selecciona tu app.apk
└─ Click "Install"
   ✅ Espera a que termine
```

### Paso 3: Ejecutar
```
SideQuest:
└─ Installed Apps → Busca tu app → "Launch"

O en Quest:
└─ Apps → Unknown Sources → Tu app
```

---

## 🎯 Probar Ghost Punch

### En el juego:
```
1. Presioná BOTÓN DEL JOYSTICK IZQUIERDO
   ➜ Empieza grabación
   ➜ StatusText (si lo configuraste): "RECORDING..."

2. Hacé movimientos de boxeo

3. Presioná BOTÓN DEL JOYSTICK IZQUIERDO de nuevo
   ➜ Termina grabación
   ➜ Automáticamente se reproduce el ghost
   ➜ ScoreText (si lo configuraste): "SCORE: XX.X/100"
```

---

## 📊 Ver Resultados

### Opción A: UI en el headset
```
[MEJOR] Si configuraste GhostPunchUI:
└─ Ves el score en tiempo real mientras reproduct
```

### Opción B: SideQuest Logcat
```
SideQuest → Open Console (abajo derecha)
├─ Busca: "Ghost punch score final"
└─ Verá: "SCORE: XX.X/100 | error promedio: X.XXXm"
```

### Opción C: PowerShell
```powershell
$env:ANDROID_SDK_ROOT = "C:\Users\[TuUsuario]\AppData\Local\Android\Sdk"
& "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe" logcat | Select-String "GhostPunch"
```

---

## 🐛 Si algo no funciona

| Síntoma | Solución |
|---------|----------|
| SideQuest no ve Quest | Desconectá/reconectá USB; Reiniciá SideQuest |
| APK no instala | Borrá la versión anterior en SideQuest |
| Botón no graba | ✅ Verificá InputActionReference en VRRecorderInput |
| Ghost no aparece | ✅ Verificá que GhostHead/LeftHand/RightHand tengan Mesh Renderer |
| Score es 0 | ✅ Verificá realLeftHand/realRightHand asignados |
| No veo logs | ✅ Abrí SideQuest Console o hacé logcat |

---

## 📚 Documentos adicionales

```
GHOST_PUNCH_SETUP.md       ← Setup detallado con imágenes mentales
SIDEQUEST_QUICK_START.md   ← Guía ultra rápida (5 min)
UI_OPTIONAL_SETUP.md       ← Cómo agregar UI en pantalla
```

---

## 🎬 Pasos siguientes (después de probar)

```
✅ Probá y asegúrate de que funciona
✅ Ajustá parámetros (sampleInterval, maxErrorDistance, etc)
✅ Agregá feedback háptico en los punches
✅ Guardá grabaciones a archivos
✅ Crea modo multijugador local (1vs1)
✅ Agrega leaderboard de scores
```

---

## 🚨 Notas importantes

- **Primera compilación**: Tarda más (~5 min), después más rápido.
- **Developer Mode**: No se desactiva automáticamente.
- **Score 100** = seguimiento perfecto del ghost.
- **Score 0** = muy lejos del movimiento grabado.
- **No uses XR Simulator**: Todo es hardware real (headset + controllers).

---

**¿Preguntas? Mirá los documentos de referencia o chequeá los scripts.** 🎮

Good luck! 🚀

