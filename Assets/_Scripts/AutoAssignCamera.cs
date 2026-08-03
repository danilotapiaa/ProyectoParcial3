using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class AutoAssignCamera : MonoBehaviour
{
    private CinemachineCamera camara;

    void Start()
    {
        camara = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (camara != null && camara.Target.TrackingTarget == null)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
            {
                var jugadorLocal = NetworkManager.Singleton.LocalClient.PlayerObject;

                if (jugadorLocal != null)
                {
                    Transform[] huesos = jugadorLocal.GetComponentsInChildren<Transform>();
                    foreach (Transform hueso in huesos)
                    {
                        if (hueso.name == "PlayerCameraRoot")
                        {
                            camara.Target.TrackingTarget = hueso;

                            // ¡LA MAGIA!: Obliga a la cámara a teletransportarse sin volar a través de las paredes al inicio
                            camara.PreviousStateIsValid = false;

                            Debug.Log("¡Cámara teletransportada al cuello correctamente!");
                            break;
                        }
                    }
                }
            }
        }
    }
}