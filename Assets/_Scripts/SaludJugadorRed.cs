using Unity.Netcode;
using UnityEngine;

public class SaludJugadorRed : NetworkBehaviour
{
    public NetworkVariable<int> energia = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> vidas = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void OnGUI()
    {
        if (!IsOwner) return;

        GUI.Box(new Rect(10, 10, 150, 70), "=== TÚ ===");
        GUI.Label(new Rect(20, 35, 130, 20), "Energía: " + energia.Value + "%");
        GUI.Label(new Rect(20, 55, 130, 20), "Vidas: " + vidas.Value);
    }

    public void RecibirDanio(int cantidad)
    {
        if (!IsServer) return;

        // 1. Buscamos el Cerebro del juego
        DungeonManagerNet manager = FindAnyObjectByType<DungeonManagerNet>();

        // Si el juego ya se acabó (Victoria o Derrota), somos invencibles
        if (manager != null && manager.estadoJuego.Value != 0) return;

        energia.Value -= cantidad;

        if (energia.Value <= 0)
        {
            vidas.Value -= 1;

            if (vidas.Value <= 0)
            {
                // GAME OVER DEFINTIVO
                vidas.Value = 0; // Ponemos un tope para que no existan vidas negativas
                energia.Value = 0;

                if (manager != null)
                {
                    manager.ActivarDerrota(); // ¡Llamamos a la pantalla de Game Over!
                }
            }
            else
            {
                // PERDIÓ UNA VIDA PERO SIGUE JUGANDO
                energia.Value = 100;

                // Le ordenamos a ese jugador específico que vuelva al punto de inicio
                TeletransportarAlInicioClientRpc();
            }
        }
    }

    [ClientRpc]
    void TeletransportarAlInicioClientRpc()
    {
        // Solo el dueño de este cuerpo obedece esta orden de teletransporte
        if (IsOwner)
        {
            GameObject puntoSpawn = GameObject.Find("Punto_Spawn");
            if (puntoSpawn != null)
            {
                CharacterController cc = GetComponent<CharacterController>();

                // Magia para mover al jugador sin que el CharacterController lo bloquee
                if (cc != null) cc.enabled = false;
                transform.position = puntoSpawn.transform.position;
                if (cc != null) cc.enabled = true;
            }
        }
    }
}