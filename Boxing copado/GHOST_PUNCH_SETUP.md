# Ghost Punch Recording - Guía de Configuración e Instalación

## Resumen rápido
Este sistema permite grabar y reproducir movimientos de golpes VR, guardando posiciones relativas al XR Origin y calculando un score de precisión.

---

## 1. Configuración en Unity (Editor)

### Paso 1.1: Crear el GameObject Manager

1. Creá un objeto vacío en tu escena:
   - Click derecho en Hierarchy → Create Empty
   - Nombrá: `GhostPunchSystem`

2. Agregá estos componentes (Add Component):
   - `VRGhostRecorder`
   - `VRRecorderInput`
   - `VRGhostPlayback`
   - `GhostPunchScorer`

---

### Paso 1.2: Configurar VRGhostRecorder

En el Inspector del `GhostPunchSystem`, busca el componente **VRGhostRecorder**:

| Campo | Valor |
|-------|-------|
| **Player Root** | Arrastrá tu `XR Origin` |
| **Head Transform** | Arrastrá `XR Origin > Camera Offset > Main Camera` |
| **Left Hand Transform** | Arrastrá el controller izquierdo (ej: `XR Origin > LeftHand Controller`) |
| **Right Hand Transform** | Arrastrá el controller derecho (ej: `XR Origin > RightHand Controller`) |
| **Sample Interval** | `0.02` |
| **Record On Enable** | Desactivado |

**Importante**: El `Player Root` debe ser la raíz del rig XR, no la cámara.

---

### Paso 1.3: Configurar Input System

1. Creá un `Input Actions` asset si no lo tenés:
   - Click derecho en Assets → Input System → Input Actions
   - Nombrá: `BoxingControls`

2. Agregá una Action Map:
   - Click en Add Action Map
   - Nombre: `Recording`

3. Agregá una Action:
   - Nombre: `ToggleRecordPunch`
   - Action Type: `Button`

4. Agregá un Binding:
   - Click en Add Binding under ToggleRecordPunch
   - Seleccioná: `Left Hand / Thumbstick / Click` (o el botón que prefieras)

5. Guardá el asset (Ctrl+S)

6. En el `GhostPunchSystem`, en **VRRecorderInput**:
   - **Recorder**: Arrastrá el `VRGhostRecorder`
   - **Toggle Recording Action**: Arrastrá el asset `BoxingControls` y seleccioná la acción `Recording/ToggleRecordPunch`

---

### Paso 1.4: Crear Ghost Objects

1. Creá tres objetos simples para representar el ghost:

   **Ghost Head:**
   - Create Empty → Nombrá `GhostHead`
   - Add Component → Mesh Renderer
   - Material: material transparente o con alpha bajo

   **Ghost Left Hand:**
   - Create Empty → Nombrá `GhostLeftHand`
   - Add un modelo simple (ej: Cube con escala pequeña)
   - Material transparente

   **Ghost Right Hand:**
   - Create Empty → Nombrá `GhostRightHand`
   - Add un modelo simple (ej: Cube con escala pequeña)
   - Material transparente

2. En **VRGhostPlayback** asigná:
   - **Ghost Head**: Arrastrá `GhostHead`
   - **Ghost Left Hand**: Arrastrá `GhostLeftHand`
   - **Ghost Right Hand**: Arrastrá `GhostRightHand`
   - **Player Root**: Arrastrá `XR Origin`
   - **Recorder**: Arrastrá el `VRGhostRecorder`
   - **Auto Play On Recording Stopped**: Activado

---

### Paso 1.5: Configurar GhostPunchScorer

En **GhostPunchScorer**:

| Campo | Valor |
|-------|-------|
| **Playback** | Arrastrá `VRGhostPlayback` |
| **Real Left Hand** | Arrastrá controller izquierdo real |
| **Real Right Hand** | Arrastrá controller derecho real |
| **Ghost Left Hand** | Arrastrá `GhostLeftHand` |
| **Ghost Right Hand** | Arrastrá `GhostRightHand` |
| **Max Error Distance** | `0.35` (metros) |
| **Include Left Hand** | Activado |
| **Include Right Hand** | Activado |
| **Auto Score On Playback** | Activado |

---

## 2. Build para Meta Quest con SideQuest

### Paso 2.1: Preparar el proyecto

1. En Unity, ve a **File → Build Settings**

2. Asegúrate de que:
   - **Scenes In Build**: Tu escena esté incluida
   - **Platform**: Android (si no está, click en `Switch Platform`)

3. En **Player Settings** (botón "Player Settings"):

   **Resolution and Presentation:**
   - Orientation: Landscape Left

   **XR Plug-in Management:**
   - Asegúrate de que **OpenXR** esté habilitado
   - En OpenXR Feature Groups, activa:
     - Meta Quest Support

4. Buildea una APK:
   - **File → Build Settings → Build APK**
   - Seleccioná una carpeta de destino
   - Esperá a que termine

```powershell
# Si querés buildear desde terminal (opcional):
unity.exe -projectPath "D:\alanm\facultad\gamedev\Cool-Boxing\Boxing copado" `
  -executeMethod UnityEditor.BuildPlayerWindow.ShowBuildPlayerWindow `
  -quit
```

---

### Paso 2.2: Instalar SideQuest

1. Bajá SideQuest desde: https://sidequestvr.com/
2. Instalá el cliente en tu PC
3. Abrí SideQuest

---

### Paso 2.3: Preparar Meta Quest

1. En el Quest:
   - **Settings → About**
   - Toca **Build Number** 7 veces para activar Developer Mode

2. Activa **USB Debugging**:
   - **Settings → Developer → USB Debugging** → Activado

3. Conecta el Quest a la PC por USB

4. En el Quest, selecciona **Allow** cuando pida permisos USB

---

### Paso 2.4: Instalar con SideQuest

1. En SideQuest:
   - Click en el ícono de carpeta (arriba a la izquierda)
   - Seleccioná la APK que buildeaste

2. SideQuest mostrará:
   - Nombre de la app
   - Tamaño
   - Dependencias

3. Click en **Install** y esperá

```
💡 Tip: Si SideQuest no detecta el Quest, probá:
- Desconectá y reconectá el USB
- Reiniciá SideQuest
- Asegúrate de que el Quest esté en modo Developer
```

---

## 3. Probar en Meta Quest

### Paso 3.1: Ejecutar la app

1. En SideQuest, ve a **Installed Apps**
2. Buscá tu app y hacer click en **Launch**
3. O en el Quest: **Apps → Unknown Sources** → Tu app

### Paso 3.2: Usar el sistema de Ghost Punch

1. Ponete el headset
2. Presioná el **botón del joystick** asignado (izquierdo por defecto):
   - **Presión 1**: Empieza a grabar
   - Hacé puñetazos / movimientos
   - **Presión 2**: Termina la grabación

3. Automáticamente:
   - Los objetos ghost aparecen
   - Se reproduce el movimiento que grabaste
   - Se muestra el score en la consola

4. Para ver el score:
   - Conectá el Quest por USB y abrí:
     - **adb logcat | grep GhostPunchScorer**
   - O chequeá el archivo de logs en:
     - `/sdcard/Android/data/com.UnityTechnologies.YourAppName/files/`

---

## 4. Debug / Consola en Quest

### Opción A: Logcat en tiempo real

```powershell
# Abrí PowerShell en la carpeta de Android SDK
$env:ANDROID_SDK_ROOT = "C:\Users\[YourUser]\AppData\Local\Android\Sdk"

# Mostrá logs en tiempo real
& "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe" logcat | Select-String "GhostPunch"
```

### Opción B: Capturá logs a archivo

```powershell
& "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe" logcat > logs.txt
```

Luego buscá "GhostPunchScorer" en el archivo.

---

## 5. Troubleshooting

### El grab no funciona
- ✅ Verificá que el `InputActionReference` esté asignado
- ✅ Asegúrate de que los transforms reales (`headTransform`, `leftHandTransform`, etc.) no sean nulos
- ✅ Checkeá que el `playerRoot` sea el `XR Origin`

### El ghost no aparece
- ✅ Asegúrate de que `ghostLeftHand`, `ghostRightHand`, `ghostHead` tengan materiales visibles
- ✅ Verificá que `VRGhostPlayback.playerRoot` sea el `XR Origin`
- ✅ Hacé que los objetos ghost sean niños de algo visible (ej: world root)

### El score es 0
- ✅ Verificá que `realLeftHand` y `realRightHand` sean los transforms de los controladores físicos
- ✅ Asegúrate de que `includeLeftHand` y `includeRightHand` estén activados
- ✅ Aumentá `maxErrorDistance` a `0.5` si el score sigue siendo 0

### No veo logs
- ✅ Conectá el Quest por USB
- ✅ En Settings → Developer → Activa USB Debugging
- ✅ En SideQuest, hace clic en **Open Console** (abajo a la derecha)
- ✅ Buscá "GhostPunchScorer" en la consola

---

## 6. Próximos pasos

Una vez que pruebes y funcione:

1. **Agregar UI visual** del score en pantalla
2. **Guardar grabaciones** a archivo (para comparar sesiones)
3. **Agregar feedback háptico** cuando golpeas el ghost
4. **Multiplayer local**: dos jugadores grabando y comparando golpes

---

## Notas finales

- **Sin XR Simulator**: Todo lo que grabes es movimiento real del headset y controladores.
- **Poses relativas**: El fantasma es estable aunque te muevas en el espacio.
- **Score 0-100**: Mide qué tan bien seguiste el movimiento grabado (100 = perfecto).

¿Alguna pregunta o problema en el proceso? 🎮

