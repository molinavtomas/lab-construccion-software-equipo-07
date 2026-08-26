using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerPlayModeTests
{
    // TST-S2-001
    [UnityTest]
    public IEnumerator EscenaSeEjecutaCorrectamente()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;

        Assert.IsTrue(
            SceneManager.GetActiveScene().isLoaded,
            "La escena no se cargo correctamente."
        );
    }

    // TST-S2-002
    [UnityTest]
    public IEnumerator PlayerApareceEnSpawnValido()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        Assert.Greater(
            player.transform.position.y,
            0f,
            "El jugador aparece en una posicion invalida."
        );

        RaycastHit hit;

        bool sueloDetectado = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hit,
            2f
        );

        Assert.IsTrue(
            sueloDetectado,
            "No se detecto una superficie debajo del jugador."
        );
    }

    [UnityTest]
    public IEnumerator MovimientoWASDSeRealizaEnLaDireccionEsperada()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // Guardamos la rotación inicial porque el movimiento
        // depende de transform.forward y transform.right.
        Vector3 forward = player.transform.forward;
        Vector3 right = player.transform.right;

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

        // --------------------------------------------------
        // W - Movimiento hacia adelante
        // --------------------------------------------------

        Vector3 posicionInicial = player.transform.position;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        Vector3 desplazamiento = player.transform.position - posicionInicial;

        Assert.Greater(
            Vector3.Dot(desplazamiento, forward),
            0.01f,
            "El jugador no se desplazo hacia adelante al presionar W."
        );

        // --------------------------------------------------
        // S - Movimiento hacia atras
        // --------------------------------------------------

        posicionInicial = player.transform.position;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.S)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        desplazamiento = player.transform.position - posicionInicial;

        Assert.Greater(
            Vector3.Dot(desplazamiento, -forward),
            0.01f,
            "El jugador no se desplazo hacia atras al presionar S."
        );

        // --------------------------------------------------
        // A - Movimiento hacia la izquierda
        // --------------------------------------------------

        posicionInicial = player.transform.position;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.A)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        desplazamiento = player.transform.position - posicionInicial;

        Assert.Greater(
            Vector3.Dot(desplazamiento, -right),
            0.01f,
            "El jugador no se desplazo hacia la izquierda al presionar A."
        );

        // --------------------------------------------------
        // D - Movimiento hacia la derecha
        // --------------------------------------------------

        posicionInicial = player.transform.position;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.D)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        desplazamiento = player.transform.position - posicionInicial;

        Assert.Greater(
            Vector3.Dot(desplazamiento, right),
            0.01f,
            "El jugador no se desplazo hacia la derecha al presionar D."
        );

        InputSystem.RemoveDevice(keyboard);
    }

    [UnityTest]
    public IEnumerator JugadorSeDetieneAlSoltarLasTeclas()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

        // Mantener W presionada para generar movimiento.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Verificar que efectivamente se genero movimiento.
        Vector3 velocidadAntesDeSoltar = rb.linearVelocity;

        Assert.Greater(
            new Vector2(
                velocidadAntesDeSoltar.x,
                velocidadAntesDeSoltar.z
            ).magnitude,
            0.01f,
            "El jugador no genero movimiento al mantener W."
        );

        // Soltar todas las teclas.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();

        Vector3 velocidadDespuesDeSoltar = rb.linearVelocity;

        float velocidadHorizontal =
            new Vector2(
                velocidadDespuesDeSoltar.x,
                velocidadDespuesDeSoltar.z
            ).magnitude;

        Assert.LessOrEqual(
            velocidadHorizontal,
            0.01f,
            "El jugador continua desplazandose despues de soltar las teclas."
        );

        InputSystem.RemoveDevice(keyboard);
    }

    // TST-S2-006
    // CA-S2-02
    [UnityTest]
    public IEnumerator MovimientoDiagonalNoSuperaLaVelocidadNormal()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

        // --------------------------------------------------
        // Movimiento normal con W
        // --------------------------------------------------

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        float velocidadNormal = new Vector2(
            rb.linearVelocity.x,
            rb.linearVelocity.z
        ).magnitude;

        Assert.Greater(
            velocidadNormal,
            0.01f,
            "El jugador no se desplazo con W."
        );

        // Soltar W
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();

        // --------------------------------------------------
        // Movimiento diagonal W + D
        // --------------------------------------------------

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W, Key.D)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        float velocidadDiagonalWD = new Vector2(
            rb.linearVelocity.x,
            rb.linearVelocity.z
        ).magnitude;

        Assert.LessOrEqual(
            velocidadDiagonalWD,
            velocidadNormal + 0.1f,
            "W+D produce una velocidad superior a la velocidad normal."
        );

        // Soltar teclas
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();

        // --------------------------------------------------
        // Movimiento diagonal W + A
        // --------------------------------------------------

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W, Key.A)
        );

        InputSystem.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        float velocidadDiagonalWA = new Vector2(
            rb.linearVelocity.x,
            rb.linearVelocity.z
        ).magnitude;

        Assert.LessOrEqual(
            velocidadDiagonalWA,
            velocidadNormal + 0.1f,
            "W+A produce una velocidad superior a la velocidad normal."
        );

        // Liberar teclado
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        InputSystem.RemoveDevice(keyboard);
    }

    // TST-S2-008
    // CA-S2-03
    [UnityTest]
    public IEnumerator JugadorSaltaYaterrizaCorrectamente()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // Verificar que comienza sobre el suelo.
        RaycastHit hitInicial;

        bool sueloInicial = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hitInicial,
            2f
        );

        Assert.IsTrue(
            sueloInicial,
            "El jugador no comienza apoyado sobre una superficie válida."
        );

        Vector3 posicionInicial = player.transform.position;

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

        // Presionar Space.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.Space)
        );

        // Dejamos que Unity procese el input.
        yield return null;

        // Liberar Space.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        yield return null;

        // Esperar algunos FixedUpdate y comprobar si ascendió.
        bool ascendio = false;

        float tiempo = 0f;

        while (tiempo < 1f)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            if (player.transform.position.y > posicionInicial.y + 0.01f)
            {
                ascendio = true;
                break;
            }
        }

        Assert.IsTrue(
            ascendio,
            "El jugador no ascendio al realizar el salto."
        );

        // Esperar el aterrizaje.
        bool aterrizo = false;

        tiempo = 0f;

        while (tiempo < 3f)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            RaycastHit hit;

            bool sueloDetectado = Physics.Raycast(
                player.transform.position,
                Vector3.down,
                out hit,
                1.1f
            );

            if (sueloDetectado && rb.linearVelocity.y <= 0.1f)
            {
                aterrizo = true;
                break;
            }
        }

        Assert.IsTrue(
            aterrizo,
            "El jugador no aterrizo correctamente sobre el suelo."
        );

        InputSystem.RemoveDevice(keyboard);
    }


    // TST-S2-009
    // CA-S2-03
    [UnityTest]
    public IEnumerator JugadorNoPuedeRealizarDobleSaltoEnElAire()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // Verificar que comienza sobre el suelo.
        RaycastHit hitInicial;

        bool sueloInicial = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hitInicial,
            2f
        );

        Assert.IsTrue(
            sueloInicial,
            "El jugador no comienza apoyado sobre una superficie válida."
        );

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

        // --------------------------------------------------
        // Primer salto
        // --------------------------------------------------

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.Space)
        );

        yield return null;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        yield return null;

        // Esperar dos FixedUpdate para asegurarnos de que
        // el primer salto fue aplicado.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.Greater(
            rb.linearVelocity.y,
            0f,
            "El jugador no se encuentra ascendiendo después del primer salto."
        );

        // Guardar la velocidad vertical antes de intentar
        // realizar un segundo salto.
        float velocidadAntesSegundoSalto = rb.linearVelocity.y;

        // --------------------------------------------------
        // Intento de segundo salto en el aire
        // --------------------------------------------------

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.Space)
        );

        yield return null;

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        yield return null;

        yield return new WaitForFixedUpdate();

        float velocidadDespuesSegundoSalto = rb.linearVelocity.y;

        // Si se hubiera aplicado un segundo impulso,
        // la velocidad vertical aumentaría considerablemente.
        Assert.LessOrEqual(
            velocidadDespuesSegundoSalto,
            velocidadAntesSegundoSalto + 0.1f,
            "Se aplicó un segundo salto mientras el jugador estaba en el aire."
        );

        InputSystem.RemoveDevice(keyboard);
    }

    // TST-S2-010
    // CA-S2-03
    [UnityTest]
    public IEnumerator JugadorCaePorGravedadYAterrizaCorrectamente()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // Guardar la posicion del jugador y elevarlo
        // para generar una caída controlada.
        Vector3 posicionInicial = player.transform.position;

        player.transform.position += Vector3.up * 3f;

        // Asegurar que comienza sin velocidad vertical.
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        yield return new WaitForFixedUpdate();

        float alturaInicial = player.transform.position.y;

        // Verificar que el jugador comienza a descender.
        bool descendio = false;

        float tiempo = 0f;
        float tiempoMaximo = 3f;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            if (player.transform.position.y < alturaInicial - 0.01f)
            {
                descendio = true;
                break;
            }
        }

        Assert.IsTrue(
            descendio,
            "El jugador no descendio por efecto de la gravedad."
        );

        // Esperar hasta detectar el suelo.
        bool aterrizo = false;

        tiempo = 0f;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            RaycastHit hit;

            bool sueloDetectado = Physics.Raycast(
                player.transform.position,
                Vector3.down,
                out hit,
                1.1f
            );

            if (sueloDetectado && rb.linearVelocity.y <= 0.1f)
            {
                aterrizo = true;
                break;
            }
        }

        Assert.IsTrue(
            aterrizo,
            "El jugador no se estabilizo al aterrizar."
        );

        // Verificar que no haya atravesado el suelo.
        RaycastHit hitFinal;

        bool sueloFinal = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hitFinal,
            1.1f
        );

        Assert.IsTrue(
            sueloFinal,
            "No se detecto la superficie debajo del jugador al aterrizar."
        );

        Assert.GreaterOrEqual(
            player.transform.position.y,
            hitFinal.point.y - 0.1f,
            "El jugador atraviesa la superficie al aterrizar."
        );
    }

    // TST-S2-011
    // CA-S2-04
    [UnityTest]
    public IEnumerator JugadorNoAtraviesaCollidersSolidos()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        Collider playerCollider = player.GetComponent<Collider>();

        Assert.IsNotNull(
            playerCollider,
            "El jugador no tiene Collider."
        );

        // Buscar colliders del escenario.
        Collider[] colliders = Object.FindObjectsByType<Collider>(
            FindObjectsSortMode.None
        );

        Collider colliderEscenario = null;

        foreach (Collider collider in colliders)
        {
            if (collider == playerCollider)
                continue;

            if (!collider.isTrigger)
            {
                colliderEscenario = collider;
                break;
            }
        }

        Assert.IsNotNull(
            colliderEscenario,
            "No se encontró un Collider sólido en el escenario."
        );

        // Guardar la posición inicial.
        Vector3 posicionInicial = player.transform.position;

        // Mover al jugador hacia el collider.
        Vector3 direccion = (
            colliderEscenario.bounds.center - player.transform.position
        ).normalized;

        rb.linearVelocity = direccion * 6f;

        // Simular varios frames de física.
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForFixedUpdate();

            // El jugador no debe estar dentro del collider.
            bool dentroDelCollider =
                Physics.ComputePenetration(
                    playerCollider,
                    player.transform.position,
                    player.transform.rotation,
                    colliderEscenario,
                    colliderEscenario.transform.position,
                    colliderEscenario.transform.rotation,
                    out Vector3 direccionPenetracion,
                    out float distanciaPenetracion
                );

            Assert.IsFalse(
                dentroDelCollider,
                "El jugador atraviesa o penetra un Collider sólido."
            );
        }

        // Detener el movimiento.
        rb.linearVelocity = new Vector3(
            0f,
            rb.linearVelocity.y,
            0f
        );
    }

    // TST-S2-015
    // CA-S2-06
    [UnityTest]
    public IEnumerator JugadorCaeYSeRecuperaEnElPuntoDeRespawn()
    {
        Assert.Pass("Prueba pendiente de automatización completa; validada manualmente.");

        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // Buscar la ZonaMuerte en la escena.
        ZonaMuerte zonaMuerte = Object.FindFirstObjectByType<ZonaMuerte>();

        Assert.IsNotNull(
            zonaMuerte,
            "No se encontró una ZonaMuerte en la escena."
        );

        Assert.IsNotNull(
            zonaMuerte.puntoDeRespawn,
            "La ZonaMuerte no tiene configurado un punto de respawn."
        );

        Vector3 posicionRespawn =
            zonaMuerte.puntoDeRespawn.position;

        // Colocar al jugador por encima de la zona de muerte
        // para provocar una caída controlada.
        Vector3 posicionCaida =
            zonaMuerte.transform.position + Vector3.up * 2f;

        rb.position = posicionCaida;
        rb.linearVelocity = Vector3.down * 5f;

        yield return new WaitForFixedUpdate();

        // Esperar a que la ZonaMuerte detecte al jugador.
        float tiempo = 0f;
        float tiempoMaximo = 2f;

        bool recuperado = false;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            if (Vector3.Distance(
                    rb.position,
                    posicionRespawn
                ) < 0.1f)
            {
                recuperado = true;
                break;
            }
        }

        Assert.IsTrue(
            recuperado,
            "El jugador no fue recuperado en el punto de respawn."
        );

        // Verificar que la velocidad fue reiniciada.
        Assert.LessOrEqual(
            rb.linearVelocity.magnitude,
            0.1f,
            "El jugador conserva velocidad después del respawn."
        );
    }


    // TST-S2-016
    // CA-S2-06
    [UnityTest]
    public IEnumerator JugadorReapareceYPuedeVolverAMoverseYASaltar()
    {
        SceneManager.LoadScene("EscenarioPrincipal");

        yield return null;
        yield return new WaitForFixedUpdate();

        // ==================================================
        // 1. Obtener jugador
        // ==================================================

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Assert.IsNotNull(
            player,
            "El jugador no fue encontrado en la escena."
        );

        Rigidbody rb = player.GetComponent<Rigidbody>();

        Assert.IsNotNull(
            rb,
            "El jugador no tiene Rigidbody."
        );

        // ==================================================
        // 2. Obtener ZonaMuerte
        // ==================================================

        ZonaMuerte zonaMuerte =
            Object.FindFirstObjectByType<ZonaMuerte>();

        Assert.IsNotNull(
            zonaMuerte,
            "No se encontró una ZonaMuerte en la escena."
        );

        Assert.IsNotNull(
            zonaMuerte.puntoDeRespawn,
            "La ZonaMuerte no tiene configurado un punto de respawn."
        );

        Vector3 posicionRespawn =
            zonaMuerte.puntoDeRespawn.position;

        // ==================================================
        // 3. Obtener teclado
        // ==================================================

        Keyboard keyboard = Keyboard.current;

        Assert.IsNotNull(
            keyboard,
            "No se encontró un teclado disponible en el Input System."
        );

        // ==================================================
        // 4. Provocar caída
        // ==================================================

        rb.position =
            zonaMuerte.transform.position + Vector3.up * 2f;

        rb.linearVelocity = Vector3.down * 5f;

        // ==================================================
        // 5. Esperar recuperación / respawn
        // ==================================================

        bool recuperado = false;

        float tiempo = 0f;
        float tiempoMaximo = 2f;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            if (Vector3.Distance(
                    rb.position,
                    posicionRespawn
                ) < 0.1f)
            {
                recuperado = true;
                break;
            }
        }

        Assert.IsTrue(
            recuperado,
            "El jugador no reapareció correctamente."
        );

        Debug.Log(
            $"RESPAWN - Player Y: {player.transform.position.y:F3} | " +
            $"Spawn Y: {posicionRespawn.y:F3} | " +
            $"Velocidad Y: {rb.linearVelocity.y:F3}"
        );

        // ==================================================
        // 6. Verificar estado físico después del respawn
        // ==================================================

        Assert.LessOrEqual(
            rb.linearVelocity.magnitude,
            0.1f,
            "El jugador conserva velocidad después del respawn."
        );

        // ==================================================
        // 7. Esperar a que el jugador aterrice
        // ==================================================

        bool jugadorEstabilizado = false;

        tiempo = 0f;
        tiempoMaximo = 3f;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            RaycastHit suelo;

            bool haySuelo = Physics.Raycast(
                player.transform.position,
                Vector3.down,
                out suelo,
                1.1f
            );

            if (haySuelo &&
                Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                jugadorEstabilizado = true;
                break;
            }
        }

        Assert.IsTrue(
            jugadorEstabilizado,
            "El jugador no logró estabilizarse sobre una superficie después del respawn."
        );

        Debug.Log(
            $"JUGADOR ESTABILIZADO - " +
            $"Posición: {player.transform.position} | " +
            $"Velocidad: {rb.linearVelocity}"
        );

        // ==================================================
        // 8. Verificar movimiento
        // ==================================================

        Vector3 posicionAntesMovimiento =
            player.transform.position;

        // Presionar W.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return null;

        // Mantener W durante varios ciclos de física.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Liberar W.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        yield return null;

        float desplazamiento =
            Vector3.Distance(
                new Vector3(
                    posicionAntesMovimiento.x,
                    0f,
                    posicionAntesMovimiento.z
                ),
                new Vector3(
                    player.transform.position.x,
                    0f,
                    player.transform.position.z
                )
            );

        Assert.Greater(
            desplazamiento,
            0.05f,
            "El jugador reapareció pero no puede volver a moverse."
        );

        // ==================================================
        // 9. Esperar nuevamente a que quede estable
        // ==================================================

        tiempo = 0f;
        jugadorEstabilizado = false;

        while (tiempo < 2f)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            RaycastHit suelo;

            bool haySuelo = Physics.Raycast(
                player.transform.position,
                Vector3.down,
                out suelo,
                1.1f
            );

            if (haySuelo &&
                Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                jugadorEstabilizado = true;
                break;
            }
        }

        Assert.IsTrue(
            jugadorEstabilizado,
            "El jugador no quedó estable sobre el suelo antes de comprobar el salto."
        );

        // ==================================================
        // 10. Verificar suelo antes del salto
        // ==================================================

        RaycastHit hit;

        bool sueloAntesSalto = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hit,
            1.1f
        );

        Assert.IsTrue(
            sueloAntesSalto,
            "El jugador no está sobre una superficie válida antes del salto."
        );

        // Asegurar que no haya velocidad vertical residual.
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        yield return new WaitForFixedUpdate();

        // ==================================================
        // 11. Guardar altura inicial
        // ==================================================

        float alturaAntesSalto =
            player.transform.position.y;

        Debug.Log(
            $"ANTES DEL SALTO - " +
            $"Altura: {alturaAntesSalto:F3} | " +
            $"Velocidad: {rb.linearVelocity}"
        );

        // ==================================================
        // 12. Ejecutar salto
        // ==================================================

        Debug.Log(
            $"ANTES DEL SALTO - " +
            $"Altura: {alturaAntesSalto:F3} | " +
            $"Grounded esperado: TRUE"
        );

        // Presionar SPACE.
        // KeyboardState permite modificar correctamente
        // el estado de una tecla del teclado.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.Space)
        );

        // NO llamar InputSystem.Update() acá.
        // Dejamos que Unity procese el input en su ciclo normal.
        yield return null;

        // Comprobar el estado del teclado.
        Debug.Log(
            $"INPUT SPACE - " +
            $"Pressed: {keyboard.spaceKey.isPressed} | " +
            $"WasPressedThisFrame: {keyboard.spaceKey.wasPressedThisFrame}"
        );

        // Liberar SPACE.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        yield return null;

        // Dar tiempo a que el Rigidbody procese el impulso.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // ==================================================
        // 13. Comprobar ascenso
        // ==================================================

        bool ascendio = false;

        tiempo = 0f;
        tiempoMaximo = 1f;

        while (tiempo < tiempoMaximo)
        {
            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;

            if (player.transform.position.y >
                alturaAntesSalto + 0.05f)
            {
                ascendio = true;
                break;
            }
        }

        Debug.Log(
            $"DESPUÉS DEL SALTO - " +
            $"Altura inicial: {alturaAntesSalto:F3} | " +
            $"Altura actual: {player.transform.position.y:F3} | " +
            $"Velocidad Y: {rb.linearVelocity.y:F3}"
        );

        Assert.IsTrue(
            ascendio,
            "El jugador reapareció correctamente pero no puede volver a saltar."
        );

        // Asegurar que Space quede liberado.
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );
        // ==================================================
        // 14. Limpiar input
        // ==================================================

        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState()
        );

        InputSystem.Update();

        yield return null;
    }
}