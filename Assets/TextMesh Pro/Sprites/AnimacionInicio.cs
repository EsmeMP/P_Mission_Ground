using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class AnimacionInicio : MonoBehaviour
{
    [Header("Personajes en la Escena")]
    public GameObject leo;
    public GameObject cassandra;

    [Header("Puntos de la Cinemática")]
    public Transform puntoInicio;
    public Transform puntoPuerta;
    public Transform spawInterior;

    [Header("Cámara")]
    public CinemachineCamera cinemachineCam;

    [Header("Ajustes de Caminata")]
    public float velocidadCaminata = 3f;

    [Header("Script de Control Manual (Opcional)")]
    public MonoBehaviour scriptControlJugador;

    private GameObject personajeActivo;
    private Animator animatorActivo;
    private Rigidbody2D rbActivo;

    void Start()
    {
        // 1. Enlazar Cinemachine si no está asignada en el Inspector
        if (cinemachineCam == null)
        {
            cinemachineCam = Object.FindFirstObjectByType<CinemachineCamera>();
        }

        // 2. Leer personaje elegido (1 = Leo, 2 = Cassandra)
        int eleccion = PlayerPrefs.GetInt("PersonajeSeleccionado", 1);
        Debug.Log(">>> [IntroEdificio] Iniciando con personaje: " + eleccion);

        if (eleccion == 1)
        {
            personajeActivo = leo;
            if (leo != null) leo.SetActive(true);
            if (cassandra != null) cassandra.SetActive(false);
        }
        else
        {
            personajeActivo = cassandra;
            if (leo != null) leo.SetActive(false);
            if (cassandra != null) cassandra.SetActive(true);
        }

        if (personajeActivo == null)
        {
            Debug.LogError(">>> [IntroEdificio] ERROR: No hay personaje activo asignado.");
            return;
        }

        // 3. Colocar en punto inicio asegurando Z = 0
        if (puntoInicio != null)
        {
            personajeActivo.transform.position = new Vector3(puntoInicio.position.x, puntoInicio.position.y, 0f);
        }

        // 4. Enfocar cámara al personaje desde el inicio
        if (cinemachineCam != null)
        {
            cinemachineCam.Target.TrackingTarget = personajeActivo.transform;
            cinemachineCam.ForceCameraPosition(new Vector3(personajeActivo.transform.position.x, personajeActivo.transform.position.y, -10f), Quaternion.identity);
        }

        animatorActivo = personajeActivo.GetComponent<Animator>();

        // Desactivar temporalmente movimiento manual
        if (scriptControlJugador != null)
        {
            scriptControlJugador.enabled = false;
        }

        // Asegurar física cinemática para que ningún collider lo frene al caminar
        rbActivo = personajeActivo.GetComponent<Rigidbody2D>();
        if (rbActivo != null)
        {
            rbActivo.linearVelocity = Vector2.zero;
            rbActivo.bodyType = RigidbodyType2D.Kinematic;
        }

        // Iniciar la caminata
        StartCoroutine(RutinaCaminata());
    }

    IEnumerator RutinaCaminata()
    {
        if (puntoPuerta == null)
        {
            Debug.LogError(">>> [IntroEdificio] ERROR: 'puntoPuerta' no está asignado.");
            yield break;
        }

        Vector3 destinoPuerta = new Vector3(puntoPuerta.position.x, puntoPuerta.position.y, 0f);
        Debug.Log(">>> [IntroEdificio] Caminando desde " + personajeActivo.transform.position + " hacia " + destinoPuerta);

        if (Vector2.Distance(personajeActivo.transform.position, destinoPuerta) < 0.2f)
        {
            Debug.LogWarning(">>> [IntroEdificio] ADVERTENCIA: PuntoInicio y PuntoPuerta están casi en la misma posición.");
        }

        float tiempoLimite = 6f;
        float tiempo = 0f;

        // Bucle de caminata
        while (Vector2.Distance(personajeActivo.transform.position, destinoPuerta) > 0.1f && tiempo < tiempoLimite)
        {
            personajeActivo.transform.position = Vector3.MoveTowards(
                personajeActivo.transform.position,
                destinoPuerta,
                velocidadCaminata * Time.deltaTime
            );

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Fijar posición exacta en la puerta
        personajeActivo.transform.position = destinoPuerta;
        Debug.Log(">>> [IntroEdificio] Llegó a la puerta.");

        yield return new WaitForSeconds(0.3f);

        // TELETRANSPORTE AL INTERIOR Y CORTE DE CÁMARA
        if (spawInterior != null)
        {
            Vector3 destinoInterior = new Vector3(spawInterior.position.x, spawInterior.position.y, 0f);

            // 1. Mover al personaje al interior
            personajeActivo.transform.position = destinoInterior;

            // 2. Reiniciar la cámara virtual para clavar el encuadre en el interior sin retrasos
            if (cinemachineCam != null)
            {
                cinemachineCam.enabled = false;
                cinemachineCam.transform.position = new Vector3(destinoInterior.x, destinoInterior.y, -10f);
                cinemachineCam.Target.TrackingTarget = personajeActivo.transform;
                cinemachineCam.enabled = true;
            }

            Debug.Log(">>> [IntroEdificio] ¡Teletransportado al interior y cámara reanudada!");
        }

        // Restaurar física y control
        if (rbActivo != null)
        {
            rbActivo.bodyType = RigidbodyType2D.Dynamic;
        }

        if (scriptControlJugador != null)
        {
            scriptControlJugador.enabled = true;
        }

        Debug.Log(">>> [IntroEdificio] ¡Control activado para jugar!");
    }
}