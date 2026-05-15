# Checklist de Configuración: Detección de Colisiones de Guantes en Kyle

Use este checklist para verificar que todo está bien configurado. Marque cada item a medida que lo verifica.

## PASO 1: Tags y Layers
- [ ] **Tag "Hand" existe**
  - Ve a Project Settings > Tags and Layers
  - Verifica que existe un tag llamado exactamente `Hand`
  - Si no existe, créalo

- [ ] **Los guantes tienen tag "Hand"**
  - Selecciona cada guante/hand proxy en la jerarquía (ej. glove1, glove2, LeftHand, RightHand)
  - En el Inspector, en el dropdown de **Tag** (arriba a la derecha), selecciona `Hand`
  - Repite para ambas manos

- [ ] **Las capas están configuradas para colisionar**
  - Ve a Project Settings > Physics
  - Busca la sección "Layer Collision Matrix"
  - Si los guantes están en layer "Default" y Kyle en layer "8" (u otra), asegúrate que hay un checkmark en la intersección
  - Si no hay checkmark = no colisionan, actívalo

## PASO 2: Guantes / Manos del Jugador
- [ ] **Cada guante tiene un Collider**
  - Selecciona cada guante
  - En el Inspector debe haber un componente Collider (Box, Sphere, Capsule, Mesh, etc.)
  - Si no hay, añade uno: Component > Physics > [Box/Sphere/Capsule]Collider

- [ ] **El Collider tiene "Is Trigger" = ON**
  - Selecciona cada guante
  - En el Collider component, marca la casilla **"Is Trigger"**
  - Debe estar CHEQUEADA

- [ ] **Cada guante tiene un Rigidbody**
  - Selecciona cada guante
  - En el Inspector debe haber un componente Rigidbody
  - Si no hay, añade uno: Component > Physics > Rigidbody

- [ ] **El Rigidbody está configurado correctamente**
  - **Body Type** = Dynamic (o Kinematic si usas manual transform update)
  - **Gravity** = OFF (desactiva Use Gravity)
  - **Is Kinematic** = ON (porque XR tracking controla la posición, no la física)
  - **Collision Detection** = Continuous Speculative (recomendado para objetos rápidos)

- [ ] **Cada guante tiene el tag "Hand"**
  - Ya verificado en PASO 1, pero confirma de nuevo

## PASO 3: RobotKyle - Configuración Principal
- [ ] **RobotKyle (o su raíz) tiene un Animator**
  - Selecciona el objeto raíz de RobotKyle en la jerarquía
  - En el Inspector debe haber un componente Animator
  - Si no hay, añade uno: Component > Miscellaneous > Animator

- [ ] **El Animator tiene un Controller asignado**
  - En el componente Animator, en el campo **Controller**, debe haber un Animator Controller (archivo .controller)
  - Si está vacío, arrastra tu controller desde Project

- [ ] **RobotKyle (o su raíz) tiene un Rigidbody**
  - Selecciona el objeto raíz de RobotKyle
  - En el Inspector debe haber un componente Rigidbody
  - Si no hay, añade uno: Component > Physics > Rigidbody
  - Configuración recomendada:
    - **Body Type** = Dynamic
    - **Gravity** = OFF (si Kyle no se mueve por física)
    - **Is Kinematic** = ON (si Kyle es un NPC estático)
    - **Collision Detection** = Discrete

## PASO 4: RobotKyle - Hitboxes
- [ ] **Kyle tiene objetos "hitbox" (partes del cuerpo)**
  - En la jerarquía bajo RobotKyle, busca GameObjects que representen partes del cuerpo
  - Ejemplos: Head, Chest, LeftArm, RightArm, LeftLeg, RightLeg, Torso, Body, etc.
  - **Nota:** Si Kyle es un modelo importado, estos pueden estar dentro del modelo (bajo armadura/skeleton)

- [ ] **Cada hitbox tiene un Collider**
  - Selecciona cada parte del cuerpo (Head, Chest, etc.)
  - En el Inspector debe haber un Collider (Box, Sphere, Capsule, Mesh)
  - Si no hay, añade uno: Component > Physics > [Tipo]Collider

- [ ] **Cada Collider de hitbox tiene "Is Trigger" = ON**
  - En cada Collider, marca la casilla **"Is Trigger"**
  - Debe estar CHEQUEADA para todas las partes del cuerpo

- [ ] **Cada hitbox tiene el script "collision.cs"**
  - Selecciona cada parte del cuerpo (Head, Chest, etc.)
  - En el Inspector, busca el componente **Collision**
  - Si no lo ves, el script no está attached. Añádelo: Component > Scripts > Collision
  - Repite para todas las partes que quieras que reaccionen

- [ ] **El campo "Player Tag" en collision.cs dice "Hand"**
  - Selecciona un hitbox con el script Collision
  - En el Inspector, encuentra el componente Collision
  - Busca el campo **Player Tag** *(o "playerTag" si aparece en camelCase)*
  - Debe tener el valor `Hand`
  - Si dice otra cosa, edítalo para que diga `Hand`

## PASO 5: RobotKyle - Receptor de Golpes
- [ ] **RobotKyle (o su raíz) tiene el componente "RobotHitReceiver"**
  - Selecciona el objeto raíz de RobotKyle
  - Busca en el Inspector el componente **RobotHitReceiver**
  - Si no lo ves, añádelo: Component > Scripts > RobotHitReceiver

- [ ] **RobotHitReceiver tiene un Animator asignado**
  - En el componente RobotHitReceiver, en el campo **Animator**, arrastra el Animator de Kyle
  - O selecciona el Animator que ya está en RobotKyle

- [ ] **Los nombres de parámetros son correctos**
  - **Hit Trigger** = `Hit` (o el nombre del trigger en tu Animator Controller)
  - **Intensity Param** = `HitPower` (o el nombre del float param)
  - **Area Param** = `HitArea` (o el nombre del int param)

- [ ] **Hitbox Area Mapping está configurada**
  - En RobotHitReceiver, busca la sección **Hitbox Area Mapping**
  - Debe haber una lista con al menos estas entradas (ajusta según tus names):
    ```
    Element 0:
      Hitbox Name: Head
      Area Index: 0
    
    Element 1:
      Hitbox Name: Chest (o Torso)
      Area Index: 1
    
    Element 2:
      Hitbox Name: LeftArm
      Area Index: 2
    
    Element 3:
      Hitbox Name: RightArm
      Area Index: 3
    ```
  - **Los nombres deben coincidir EXACTAMENTE con los nombres de los GameObjects de los hitboxes**
  - Si tienes otras partes, añade más elementos

## PASO 6: Animator Controller
- [ ] **El Animator Controller tiene un parámetro Trigger "Hit"**
  - Haz doble-clic en el archivo .controller de Kyle (en Project) para abrirlo
  - En la pestaña **Parameters** (arriba), busca un trigger llamado `Hit`
  - Si no existe, créalo: click en "+" > Trigger > nombrearlo `Hit`

- [ ] **El Animator Controller tiene un parámetro Float "HitPower"**
  - En Parameters, busca un float llamado `HitPower`
  - Si no existe, créalo: "+" > Float > nombrearlo `HitPower`

- [ ] **El Animator Controller tiene un parámetro Int "HitArea"**
  - En Parameters, busca un int llamado `HitArea`
  - Si no existe, créalo: "+" > Int > nombrearlo `HitArea`

- [ ] **Hay una transición que usa el trigger "Hit"**
  - En la vista del gráfico de estados, busca transiciones que digan `Hit` como condición
  - Podría ser desde "Any State" o desde el estado de idle/locomotion
  - Si no hay, crea una: 
    - Desde Any State (o un estado específico) → hacia un estado de reacción de golpe
    - Añade condición: `Hit (trigger)` es verdadero

---

## PASO 7: Prueba Rápida en Play
- [ ] **Play mode activado, consola abierta**
  - Presiona Play en Unity
  - Abre la consola (Window > General > Console)

- [ ] **Mueve los guantes cerca de Kyle**
  - Usa el VR Rig o simula movimiento de manos
  - Acerca los guantes a las partes del cuerpo de Kyle

- [ ] **Verifica los logs en la consola**
  - Si funciona, deberías ver mensajes como:
    ```
    Hit received from glove1 on Head (area: 0, power: 0.45)
    ```
  - Si ves esto = ¡todo está bien!
  - Si NO ves nada = salta al "DEBUGGING" abajo

- [ ] **Kyle reacciona (anima)**
  - Si todo está bien, Kyle debería reproducir la animación de golpe
  - Si no anima, revisa que el Animator Controller esté bien configurado

---

## DEBUGGING: Si No Funciona

### Síntoma: No hay logs en la consola (colisión no se detecta)
1. **Verifica que OnTriggerEnter se está llamando:**
   - En `collision.cs` añade un `Debug.Log` al principio de `OnTriggerEnter`
   - Si no aparece, la colisión NO se detecta. Revisa:
     - ¿Guante tiene tag `Hand`?
     - ¿Hitbox tiene Collider con Is Trigger ON?
     - ¿Guante tiene Collider con Is Trigger ON?
     - ¿Ambos tienen Rigidbody?
     - ¿Los layers están permitidos en Physics?

2. **Verifica que el script collision.cs está en el hitbox:**
   - En la jerarquía, selecciona el hitbox (ej. Head)
   - En el Inspector, ¿ves el componente Collision?
   - Si no, añádelo manualmente

### Síntoma: Logs dicen "Hit received... area: -1"
- El nombre del hitbox NO coincide con Hitbox Area Mapping
- Abre el Hitbox Area Mapping en RobotHitReceiver
- Compara el nombre en la lista vs. el nombre exacto del GameObject en la jerarquía
- Edita para que coincidan exactamente

### Síntoma: Logs aparecen pero Kyle NO anima
- El Animator no recibe el parámetro
- Verifica:
  - ¿RobotHitReceiver tiene un Animator asignado?
  - ¿El Animator tiene parámetro `Hit` (Trigger)?
  - ¿Hay una transición en el Animator que use el trigger `Hit`?
  - ¿La animación de reacción existe en el Controller?

### Síntoma: Muchos logs repetidos (spam)
- Es normal al estar dentro de un trigger en cada frame
- `collision.cs` usa deduplicación (HashSet)
- El log debería aparecer solo una vez por contacto
- Si aparece cada frame, revisa la lógica de `OnTriggerEnter` vs `OnTriggerStay`

---

## Resumen Visual de la Cadena
```
Guante (tag: Hand, Collider + Is Trigger ON, Rigidbody isKinematic ON)
    ↓ (colisiona con)
Hitbox de Kyle (Collider + Is Trigger ON, script: collision.cs)
    ↓ (envía hit a)
RobotHitReceiver (en Kyle, con Animator asignado)
    ↓ (escribe parámetros en)
Animator de Kyle (con trigger "Hit", parámetros "HitPower", "HitArea")
    ↓ (reproduce)
Animación de Reacción (Hit_Head, Hit_Torso, etc.)
```

---

Verifica todos los items de arriba y avísame cuál falla. Probablemente será uno de estos tres:
1. Guantes sin tag "Hand" o sin Rigidbody/Collider bien configurado
2. Hitbox de Kyle sin script collision.cs o sin Collider + Is Trigger
3. RobotHitReceiver sin Animator asignado o sin parámetros en el Controller

