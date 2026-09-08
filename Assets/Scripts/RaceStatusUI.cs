using TMPro;
using UnityEngine;

public class RaceStatusUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text statusText;

    private bool subscribed;

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void Update()
    {
        if (!subscribed)
            TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (subscribed)
            return;

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return;

        gameManager.estadoCarrera.OnValueChanged += OnRaceStateChanged;
        gameManager.tiempoActual.OnValueChanged += OnElapsedTimeChanged;
        subscribed = true;
        Refresh();
    }

    private void Unsubscribe()
    {
        if (!subscribed || gameManager == null)
            return;

        gameManager.estadoCarrera.OnValueChanged -= OnRaceStateChanged;
        gameManager.tiempoActual.OnValueChanged -= OnElapsedTimeChanged;
        subscribed = false;
    }

    private void OnRaceStateChanged(
        EstadoCarrera previousState,
        EstadoCarrera currentState)
    {
        Refresh();
    }

    private void OnElapsedTimeChanged(float previousTime, float currentTime)
    {
        if (gameManager != null && gameManager.CarreraActiva)
            Refresh();
    }

    private void Refresh()
    {
        if (statusText == null)
            return;

        if (gameManager == null)
        {
            statusText.text = "ESPERANDO JUGADORES...";
            return;
        }

        switch (gameManager.estadoCarrera.Value)
        {
            case EstadoCarrera.Activa:
                statusText.text =
                    $"CARRERA ACTIVA\nTIEMPO: " +
                    $"{gameManager.TiempoRestante:0.0} s";
                break;

            case EstadoCarrera.Finalizada:
                statusText.text = "CARRERA FINALIZADA";
                break;

            default:
                statusText.text = "ESPERANDO JUGADORES...";
                break;
        }
    }
}
