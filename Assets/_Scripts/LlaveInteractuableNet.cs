using Unity.Netcode;
using UnityEngine;

public class LlaveInteractuableNet : NetworkBehaviour
{
    private bool yaRecogida = false;

    void OnTriggerEnter(Collider other)
    {
        // Solo el servidor decide quién agarró la llave
        if (!IsServer) return;
        if (yaRecogida) return;

        // Verificamos si lo que chocó con la llave es un jugador
        if (other.CompareTag("Player"))
        {
            yaRecogida = true;

            // Buscamos el cerebro y sumamos la llave
            DungeonManagerNet manager = FindAnyObjectByType<DungeonManagerNet>();
            if (manager != null)
            {
                manager.SumarLlave();
            }

            // La apagamos de la red sin destruirla de la memoria (false)
            if (GetComponent<NetworkObject>().IsSpawned)
            {
                GetComponent<NetworkObject>().Despawn(false);
            }

            // La ocultamos visualmente en nuestra pantalla
            gameObject.SetActive(false);
        }
    }
}