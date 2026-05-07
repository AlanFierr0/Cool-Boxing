# SideQuest: Guía Rápida para Ghost Punch

## Flujo completo en 10 minutos

### 1. Builder en Unity → APK

```
File → Build Settings
├─ Check que Android esté seleccionado
├─ Check que tu escena esté en "Scenes In Build"
└─ Build APK (selecciona carpeta destino)

➜ Esperá 2-5 minutos
➜ Resultado: app.apk en la carpeta que elegiste
```

---

### 2. Preparar Meta Quest

Conectá por USB y hacé estos dos pasos EN EL HEADSET:

```
Settings → About
└─ Toca "Build Number" 7 veces
   ➜ Aparece: "You are now a developer"

Settings → Developer → USB Debugging
└─ Activá ON
```

---

### 3. Bajar e instalar SideQuest

```
1. Bajá desde: https://sidequestvr.com/setup
2. Instalá en tu PC
3. Abrí SideQuest
4. Conectá Quest por USB
5. En el Quest, elegí "Allow" cuando pida permisos
```

---

### 4. Instalar tu APK

En SideQuest:

```
Click en ícono de carpeta (arriba izquierda)
├─ Busca: app.apk (la que buildeaste)
├─ Hacé clic para seleccionarla
└─ Click en "Install"

➜ Esperá a que termine
➜ Verás: "App installed successfully"
```

---

### 5. Ejecutar

```
En SideQuest:
└─ Installed Apps → Buscá tu app → Click "Launch"

O en el Quest:
└─ Apps → Unknown Sources → Tu app
```

---

### 6. Probar Ghost Punch

```
In game:
├─ Presioná BOTÓN DEL JOYSTICK IZQUIERDO (o el que configuraste)
│  ➜ Empieza grabación
├─ Hacé movimientos de boxeo
├─ Presioná el botón de nuevo
│  ➜ Termina grabación
│  ➜ Automáticamente se reproduce el ghost
│  ➜ Se calcula el score
└─ Mirá la consola (abajo en SideQuest) para ver el score
```

---

## Ver el Score

### En SideQuest (viendo en vivo):

```
SideQuest → Click en "Open Console" (esquina inferior derecha)
└─ Buscá el texto:
   "Ghost punch score final: X.X/100"
```

### En Logs guardados:

```
PowerShell (admin):
$env:ANDROID_SDK_ROOT = "C:\Users\[TuUsuario]\AppData\Local\Android\Sdk"
& "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe" logcat | Select-String "GhostPunch"
```

---

## Troubleshooting rápido

| Problema | Solución |
|----------|----------|
| SideQuest no ve el Quest | Desconectá/reconectá USB; reiniciá SideQuest |
| APK no instala | Borrá la versión anterior en SideQuest |
| No funciona el botón | Verificá `InputActionReference` en `VRRecorderInput` |
| Ghost no aparece | Verificá que `ghostLeftHand`, `ghostRightHand` sean objetos con mesh |
| Score es 0 | Verificá `includeLeftHand` y `includeRightHand` activados |
| No veo logs | Abrari SideQuest Console o hacé logcat |

---

## Variables de control

Si querés ajustar en el Inspector ANTES de buildear:

| Componente | Variable | Default | Qué hace |
|-----------|----------|---------|----------|
| VRGhostRecorder | Sample Interval | 0.02 | Cada cuántos segundos toma un snapshot |
| VRGhostPlayback | Playback Speed | 1 | Velocidad de reproducción (1=normal) |
| GhostPunchScorer | Max Error Distance | 0.35 m | De qué distancia en adelante el score baja |

---

## Notas

- **Primera compilación**: Tarda más (~5 min). Compilaciones siguientes son más rápidas.
- **Developer Mode permanente**: Una vez activado, no se desactiva automáticamente.
- **USB Debugging**: Se desactiva si reiniciás el Quest.
- **Score**: 100 = perfecta repetición, 0 = muy lejos del ghost.

---

## Pasos finales para producción

```
✅ Probá localmente en Quest
✅ Ajustá parámetros según gustes
✅ Agregá UI para mostrar score en pantalla
✅ Considerá agregar feedback háptico
```

¡Listo! 🎮

