using UnityEngine;

// Hacer que la camara siga al jugador, pero sin rotar ni cambiar el zoom.
public class CameraFollow : MonoBehaviour
{
    public Transform objetivo;

    void LateUpdate()
    {
        if (objetivo == null) return;
        transform.position = new Vector3(
            objetivo.position.x,
            objetivo.position.y,
            transform.position.z);
    }
}