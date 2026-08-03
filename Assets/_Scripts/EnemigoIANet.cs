using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoIANet : NetworkBehaviour
{
    [Header("Configuración de IA")]
    public float radioVision = 15f;
    public float radioAtaque = 2.5f;
    public int danioAtaque = 35;
    public float tiempoEntreAtaques = 2f;
    public float radioDivagar = 10f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform jugadorObjetivo;
    private float temporizadorAtaque;

    // Sincronizamos la animación en toda la red: 0 = Idle, 1 = Caminar, 2 = Correr
    public NetworkVariable<int> estadoIA = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. EL SERVIDOR TOMA LAS DECISIONES
        if (IsServer)
        {
            BuscarJugadorMasCercano();

            if (jugadorObjetivo != null)
            {
                float distancia = Vector3.Distance(transform.position, jugadorObjetivo.position);

                if (distancia <= radioAtaque)
                {
                    // ESTADO: ATACAR
                    agent.isStopped = true;
                    estadoIA.Value = 0; // Se queda quieto para golpear

                    if (Time.time >= temporizadorAtaque)
                    {
                        AtacarJugador();
                        temporizadorAtaque = Time.time + tiempoEntreAtaques;
                    }
                }
                else if (distancia <= radioVision)
                {
                    // ESTADO: PERSEGUIR
                    agent.isStopped = false;
                    agent.speed = 6f; // Velocidad de carrera (Injured Run)
                    agent.SetDestination(jugadorObjetivo.position);
                    estadoIA.Value = 2; // Estado 2 = Correr
                }
                else
                {
                    // ESTADO: DIVAGAR (Fuera de rango)
                    DivagarPorLaArena();
                }
            }
            else
            {
                DivagarPorLaArena();
            }
        }

        // 2. TODOS LOS CLIENTES ACTUALIZAN LA ANIMACIÓN VISUAL
        ActualizarAnimaciones();
    }

    void BuscarJugadorMasCercano()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
        float distanciaMinima = Mathf.Infinity;
        jugadorObjetivo = null;

        foreach (GameObject jugador in jugadores)
        {
            float distancia = Vector3.Distance(transform.position, jugador.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                jugadorObjetivo = jugador.transform;
            }
        }
    }

    void DivagarPorLaArena()
    {
        agent.speed = 2f; // Velocidad de patrulla lenta

        // Si ya llegó a su punto aleatorio, busca otro
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 puntoAleatorio = transform.position + Random.insideUnitSphere * radioDivagar;
            NavMeshHit hit;

            // Busca un punto válido en el suelo azul (NavMesh)
            if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioDivagar, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
                estadoIA.Value = 1; // Estado 1 = Caminar
            }
            else
            {
                estadoIA.Value = 0; // Si no encuentra camino, se queda quieto
            }
        }
    }

    void AtacarJugador()
    {
        // Avisar a todos los clientes que reproduzcan la animación de golpe
        EjecutarAnimacionAtaqueClientRpc();

        // Hacer el daño matemático al jugador
        SaludJugadorRed salud = jugadorObjetivo.GetComponent<SaludJugadorRed>();
        if (salud != null)
        {
            salud.RecibirDanio(danioAtaque);
        }
    }

    [ClientRpc]
    void EjecutarAnimacionAtaqueClientRpc()
    {
        if (anim != null) anim.SetTrigger("Ataque");
    }

    void ActualizarAnimaciones()
    {
        if (anim == null) return;
        anim.SetInteger("EstadoMovimiento", estadoIA.Value);
    }
}