using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    public Transform puntoDeRespawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Situación inválida: El jugador cayó al vacío (Modo Rigidbody).");

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Lo movemos al inicio al instante
                rb.position = puntoDeRespawn.position;

                // Le matamos la inercia para que no siga cayendo como un meteorito al reaparecer
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
}