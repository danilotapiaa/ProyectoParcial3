using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class ControladorRedStarter : NetworkBehaviour
{
    private PlayerInput playerInput;
    private ThirdPersonController thirdPersonController;
    private CharacterController characterController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        characterController = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        // AHORA SOLO EL DUEÑO (sea Host o Cliente) decide dónde nace y prende sus controles
        if (IsOwner)
        {
            GameObject puntoSpawn = GameObject.Find("Punto_Spawn");

            if (puntoSpawn != null)
            {
                if (characterController != null) characterController.enabled = false;

                transform.position = puntoSpawn.transform.position;

                if (characterController != null) characterController.enabled = true;
            }

            playerInput.enabled = true;
            thirdPersonController.enabled = true;
        }
        else
        {
            // Si vemos al clon de otro jugador en nuestra pantalla, le apagamos el teclado para no controlarlo
            playerInput.enabled = false;
            thirdPersonController.enabled = false;
        }
    }
}