using Unity.Netcode;
using UnityEngine;

public class NetworkUIManager : MonoBehaviour
{
    void OnGUI()
    {
        // SEGURO DE VIDA: Si el NetworkManager aún no despierta o no existe, no hacemos nada.
        if (NetworkManager.Singleton == null) return;

        // Si ya estamos conectados (como Host o Cliente), ocultamos los botones automáticamente
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            return;

        // Dibujamos un cuadro de fondo en el centro de la pantalla
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 60, 200, 120), "MENÚ MULTIJUGADOR");

        // Botón para iniciar como HOST (Tú eres el servidor y juegas)
        if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 - 20, 160, 30), "Iniciar Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        // Botón para iniciar como CLIENTE (Te unes a una partida)
        if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 20, 160, 30), "Iniciar Cliente"))
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}