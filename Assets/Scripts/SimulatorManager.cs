using TMPro;
using UnityEngine;

public class SimulatorManager : MonoBehaviour
{
    public enum Simulation : int
    {
        SINUSOIDAL = 0,
        GERSTNER = 1
    }
    public static SimulatorManager Instance { get; private set; }

    [Header("Time")]
    [SerializeField] private float dt = 0.01f;
    public float Dt { get { return dt; } }
    public float time {  get; private set; }
    [SerializeField] private TMP_Text fpsLabel;
    [SerializeField] private TMP_Text dtLabel;

    [Header("Simulations")]
    [SerializeField] private Sinusoidal sinusoidal;
    [SerializeField] private Gerstner gerstner;
    [SerializeField] private GameObject SinusoidalUI;
    [SerializeField] private GameObject GerstnerUI;
    public Simulation curSimulation { get; private set; }

    void Start()
    {
        Instance = this;

        time = 0.0f;

        sinusoidal = GetComponent<Sinusoidal>();
        gerstner = GetComponent<Gerstner>();

        sinusoidal.enabled = true;
        gerstner.enabled = false;

        SinusoidalUI.SetActive(true);
        GerstnerUI.SetActive(false);

        curSimulation = Simulation.SINUSOIDAL;
    }

    // Update is called once per frame
    void Update()
    {
        time += dt;
        SetFPS();
    }

    public void SetSimulation(int simulation)
    {
        if(simulation == (int)Simulation.SINUSOIDAL)
        {
            sinusoidal.enabled = true;
            gerstner.enabled = false;
            SinusoidalUI.SetActive(true);
            GerstnerUI.SetActive(false);
            curSimulation = Simulation.SINUSOIDAL;
        }
        else if (simulation == (int)Simulation.GERSTNER)
        {
            sinusoidal.enabled = false;
            gerstner.enabled = true;
            SinusoidalUI.SetActive(false);
            GerstnerUI.SetActive(true);
            curSimulation = Simulation.GERSTNER;
        }
    }

    public void SetSpeed(float speed)
    {
        dt = speed * 0.01f;
        dtLabel.text = "dt = " + dt.ToString("0.000");
    }

    public void SetFPS()
    {
        fpsLabel.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime);
    }
}
