using Unity.Netcode;
using UnityEngine;

public class EsferaPoderNet : NetworkBehaviour
{
    private bool yaTocada = false;

    void OnTriggerEnter(Collider other)
    {
        // Solo el servidor procesa la interacción
        if (!IsServer || yaTocada) return;

        if (other.CompareTag("Player"))
        {
            yaTocada = true;

            // Avisamos al DungeonManager que se tocó la esfera
            DungeonManagerNet manager = FindAnyObjectByType<DungeonManagerNet>();
            if (manager != null)
            {
                manager.ActivarEsfera();
            }

            // Desactivamos la esfera de la red
            if (GetComponent<NetworkObject>().IsSpawned)
            {
                GetComponent<NetworkObject>().Despawn(false);
            }

            gameObject.SetActive(false);
        }
    }
}