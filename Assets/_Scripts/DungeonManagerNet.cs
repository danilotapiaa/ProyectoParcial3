using Unity.Netcode;
using UnityEngine;

public class DungeonManagerNet : NetworkBehaviour
{
    public NetworkVariable<int> llavesCompartidas = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // 0 = Jugando, 1 = Victoria, 2 = Derrota
    public NetworkVariable<int> estadoJuego = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public GameObject puertaCerrada;
    public GameObject puertaAbierta;

    void OnGUI()
    {
        // Si estamos jugando, mostramos el contador de llaves normal
        if (estadoJuego.Value == 0)
        {
            GUI.Box(new Rect(Screen.width - 160, 10, 150, 50), "=== OBJETIVO ===");
            GUI.Label(new Rect(Screen.width - 140, 35, 130, 20), "Llaves: " + llavesCompartidas.Value + " / 5");
        }
        // Si ganaron, mostramos la pantalla de victoria gigante en el centro
        else if (estadoJuego.Value == 1)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "¡VICTORIA!");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "¡Han logrado escapar del laberinto!");
        }
        // Si perdieron, mostramos la pantalla de derrota
        else if (estadoJuego.Value == 2)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "¡GAME OVER!");
            GUI.Label(new Rect(Screen.width / 2 - 120, Screen.height / 2, 250, 30), "Un jugador ha perdido todas sus vidas.");
        }
    }

    public void SumarLlave()
    {
        if (IsServer && estadoJuego.Value == 0)
        {
            llavesCompartidas.Value++;
            if (llavesCompartidas.Value >= 5)
            {
                AbrirPuertasClientRpc();
            }
        }
    }

    [ClientRpc]
    private void AbrirPuertasClientRpc()
    {
        if (puertaCerrada != null) puertaCerrada.SetActive(false);
        if (puertaAbierta != null) puertaAbierta.SetActive(true);
    }

    // Métodos para cambiar el estado (Solo el servidor los llama)
    public void ActivarVictoria()
    {
        if (IsServer) estadoJuego.Value = 1;
    }

    public void ActivarDerrota()
    {
        if (IsServer) estadoJuego.Value = 2;
    }
}