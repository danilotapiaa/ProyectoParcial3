using Unity.Netcode;
using UnityEngine;

public class ZonaVictoriaNet : NetworkBehaviour
{
    private bool yaActivada = false;

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (yaActivada) return;

        // Si un jugador cruza la meta
        if (other.CompareTag("Player"))
        {
            yaActivada = true;

            DungeonManagerNet manager = FindAnyObjectByType<DungeonManagerNet>();
            if (manager != null)
            {
                manager.ActivarVictoria();
            }
        }
    }
}