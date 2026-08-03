using Unity.Netcode;
using UnityEngine;

public class DungeonManagerNet : NetworkBehaviour
{
    public NetworkVariable<int> llavesCompartidas = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> estadoJuego = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Variable para controlar si la esfera fue tocada
    public NetworkVariable<bool> esferaTocada = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public GameObject puertaCerrada;
    public GameObject puertaAbierta;

    void OnGUI()
    {
        if (estadoJuego.Value == 0)
        {
            GUI.Box(new Rect(Screen.width - 160, 10, 150, 50), "=== OBJETIVO ===");
            GUI.Label(new Rect(Screen.width - 140, 35, 130, 20), "Llaves: " + llavesCompartidas.Value + " / 5");
        }
        else if (estadoJuego.Value == 1)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "¡VICTORIA!");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "¡Han logrado escapar del laberinto!");
        }
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

    // Activa el modo huida por 20 segundos
    public void ActivarEsfera()
    {
        if (IsServer)
        {
            esferaTocada.Value = true;

            // Cancela un temporizador previo si se recoge otra esfera antes de tiempo
            CancelInvoke(nameof(DesactivarEsfera));

            // Programa la desactivación para dentro de 10 segundos
            Invoke(nameof(DesactivarEsfera), 10f);
        }
    }

    // Método que apaga el efecto al terminar el tiempo
    private void DesactivarEsfera()
    {
        if (IsServer)
        {
            esferaTocada.Value = false; // Los monstruos vuelven a atacar
        }
    }

    [ClientRpc]
    private void AbrirPuertasClientRpc()
    {
        if (puertaCerrada != null) puertaCerrada.SetActive(false);
        if (puertaAbierta != null) puertaAbierta.SetActive(true);
    }

    public void ActivarVictoria()
    {
        if (IsServer) estadoJuego.Value = 1;
    }

    public void ActivarDerrota()
    {
        if (IsServer) estadoJuego.Value = 2;
    }
}