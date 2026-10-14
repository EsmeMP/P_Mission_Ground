using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class MinimapaFollow : MonoBehaviour
{
    [Header("Seguimiento")]
    public Transform target;      // arrastra a Cassandra

    [Header("Límites del mapa")]
    public Tilemap mapa;          // arrastra el Tilemap Piso
    public float margen = 2f;     // cuánto puede salirse la cámara del mapa

    [Header("Zoom")]
    public float zoom = 12f;      // más grande = se ve más lejos

    private Camera cam;
    private Vector2 min;
    private Vector2 max;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = zoom;

        if (mapa != null)
        {
            mapa.CompressBounds();
            Bounds b = mapa.localBounds;
            Vector3 a = mapa.transform.TransformPoint(b.min);
            Vector3 c = mapa.transform.TransformPoint(b.max);
            min = new Vector2(a.x - margen, a.y - margen);
            max = new Vector2(c.x + margen, c.y + margen);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mitadAlto = cam.orthographicSize;
        float mitadAncho = mitadAlto * cam.aspect;

        float x = target.position.x;
        float y = target.position.y;

        if (mapa != null)
        {
            if (max.x - min.x <= mitadAncho * 2f)
                x = (min.x + max.x) / 2f;
            else
                x = Mathf.Clamp(x, min.x + mitadAncho, max.x - mitadAncho);

            if (max.y - min.y <= mitadAlto * 2f)
                y = (min.y + max.y) / 2f;
            else
                y = Mathf.Clamp(y, min.y + mitadAlto, max.y - mitadAlto);
        }

        transform.position = new Vector3(x, y, -10f);
    }
}