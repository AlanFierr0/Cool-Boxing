using UnityEngine;

/// <summary>
/// Captura un frame de movimiento en un momento específico del tiempo.
/// Almacena posición y rotación en espacio local (relativo al root).
/// </summary>
public class MotionFrame
{
    /// <summary>Tiempo en segundos dentro de la captura.</summary>
    public float time;

    /// <summary>Posición local de la cabeza respecto al root.</summary>
    public Vector3 headPositionLocal;

    /// <summary>Rotación local de la cabeza.</summary>
    public Quaternion headRotationLocal;

    /// <summary>Posición local de la mano izquierda respecto al root.</summary>
    public Vector3 leftHandPositionLocal;

    /// <summary>Rotación local de la mano izquierda.</summary>
    public Quaternion leftHandRotationLocal;

    /// <summary>Posición local de la mano derecha respecto al root.</summary>
    public Vector3 rightHandPositionLocal;

    /// <summary>Rotación local de la mano derecha.</summary>
    public Quaternion rightHandRotationLocal;

    public MotionFrame()
    {
        time = 0f;
        headPositionLocal = Vector3.zero;
        headRotationLocal = Quaternion.identity;
        leftHandPositionLocal = Vector3.zero;
        leftHandRotationLocal = Quaternion.identity;
        rightHandPositionLocal = Vector3.zero;
        rightHandRotationLocal = Quaternion.identity;
    }

    /// <summary>
    /// Crea una copia profunda del frame.
    /// </summary>
    public MotionFrame Clone()
    {
        return new MotionFrame
        {
            time = this.time,
            headPositionLocal = this.headPositionLocal,
            headRotationLocal = this.headRotationLocal,
            leftHandPositionLocal = this.leftHandPositionLocal,
            leftHandRotationLocal = this.leftHandRotationLocal,
            rightHandPositionLocal = this.rightHandPositionLocal,
            rightHandRotationLocal = this.rightHandRotationLocal
        };
    }
}

