using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoRecolector : MonoBehaviour
{
    public float velocidad = 8f;
    public float limiteIzquierdo = -9f;
    public float limiteDerecho = 9f;

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;

        float direccion = 0f;

        if (k.aKey.isPressed || k.leftArrowKey.isPressed)
            direccion = -1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed)
            direccion = 1f;

        Vector3 pos = transform.position;
        pos.x += direccion * velocidad * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, limiteIzquierdo, limiteDerecho);
        transform.position = pos;
    }
}