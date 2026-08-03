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
    public float velocidadHuida = 6f;

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
            DungeonManagerNet manager = FindAnyObjectByType<DungeonManagerNet>();

            // LA ESFERA FUE TOCADA -> MODO HUIDA ACTIVADO (ESTILO PAC-MAN)
            bool esferaTocada = (manager != null && manager.esferaTocada.Value);

            if (jugadorObjetivo != null)
            {
                float distancia = Vector3.Distance(transform.position, jugadorObjetivo.position);

                if (distancia <= radioVision)
                {
                    // SI SE COGIÓ LA ESFERA: HUYEN Y NUNCA ATACAN
                    if (esferaTocada)
                    {
                        HuirDelJugador();
                    }
                    else
                    {
                        // MODO NORMAL: PERSEGUIR Y ATACAR
                        if (distancia <= radioAtaque)
                        {
                            agent.isStopped = true;
                            estadoIA.Value = 0; // Se queda quieto para golpear

                            if (Time.time >= temporizadorAtaque)
                            {
                                AtacarJugador();
                                temporizadorAtaque = Time.time + tiempoEntreAtaques;
                            }
                        }
                        else
                        {
                            agent.isStopped = false;
                            agent.speed = 6f; // Velocidad de carrera
                            agent.SetDestination(jugadorObjetivo.position);
                            estadoIA.Value = 2; // Estado 2 = Correr
                        }
                    }
                }
                else
                {
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

    void HuirDelJugador()
    {
        agent.isStopped = false;
        agent.speed = velocidadHuida;

        // Calculamos la dirección totalmente opuesta al jugador
        Vector3 direccionOpuesta = (transform.position - jugadorObjetivo.position).normalized;
        Vector3 puntoHuida = transform.position + direccionOpuesta * 10f;

        NavMeshHit hit;
        // 1. Buscamos un punto en la dirección opuesta dentro del NavMesh
        if (NavMesh.SamplePosition(puntoHuida, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            estadoIA.Value = 2; // Animación de carrera
        }
        else
        {
            // 2. Si hay pared detrás, busca un punto aleatorio alejado para no atascarse
            Vector3 puntoAleatorio = transform.position + Random.insideUnitSphere * 8f;
            if (NavMesh.SamplePosition(puntoAleatorio, out hit, 8f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                estadoIA.Value = 2;
            }
        }
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

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 puntoAleatorio = transform.position + Random.insideUnitSphere * radioDivagar;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioDivagar, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
                estadoIA.Value = 1; // Estado 1 = Caminar
            }
            else
            {
                estadoIA.Value = 0;
            }
        }
    }

    void AtacarJugador()
    {
        EjecutarAnimacionAtaqueClientRpc();

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