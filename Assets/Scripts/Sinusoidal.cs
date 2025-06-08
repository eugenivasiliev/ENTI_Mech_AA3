using UnityEngine;

public class Sinusoidal : MonoBehaviour
{
    struct SinusoidalWave
    {
        public float A;             //Amplitude
        public float L;             //WaveLength
        public int D;               //Direction
        public float f;             //Frequency
        public float phase;         //Phase
        public float v;             //Velocity
        public float k;             //Wave number
        public float omega;         //Angular frequency
        public float constAngle;    //For computation efficiency

        public void init(Vector2 offset)
        {
            //Set up constant values for optimised calculations
            D = D * 2 - 1;          //We correct the index in the selection UI
            v = L * f * D;
            k = 2 * Mathf.PI / L;
            omega = k * v;

            constAngle = k * offset.x + phase;
        }
    }

    [SerializeField] private SinusoidalWave wave;
    [SerializeField] private SinusoidalWave nextWave;

    GridManager GM;

    void Start()
    {
        //Default wave
        wave = new SinusoidalWave();
        wave.L = 1; //Avoid NaN
        GM = GridManager.Instance;

        wave.init(GM.CurGrid.offset);
    }



    void Update()
    {
        //Get constant values to avoid unnecessary calls to memory
        float t = SimulatorManager.Instance.time;
        float preAngle = wave.constAngle - wave.omega * t;

        float spacing = GM.CurGrid.spacing;
        Vector2 offset = GM.CurGrid.offset;
        int indexWidth = GM.CurGrid.indexWidth;
        int indexHeight = GM.CurGrid.indexHeight;

        Vector2 newPos = offset;
        for (int x = 0; x < indexWidth; x++)
        {
            newPos.x += spacing;
            newPos.y = offset.y + wave.A * Mathf.Sin(wave.k * newPos.x + preAngle);
            for (int y = 0; y < indexHeight; y++)
            {
                //Since all y offsets are the same, we only compute the added spacing offset each vertical strip
                newPos.y += spacing;
                GameObject cell = GM.cells[x, y];
                cell.GetComponent<SpriteRenderer>().color = GM.GetColor(newPos.y); //Sea color
                cell.transform.position = newPos;
            }
        }
    }

    private void Reset()
    {
        wave = nextWave;
        wave.init(GM.CurGrid.offset);
    }

    public void SetA(string A) => nextWave.A = float.Parse(A);
    public void SetL(string L) => nextWave.L = float.Parse(L);
    public void SetD(int D) => nextWave.D = D;
    public void Setf(string f) => nextWave.f = float.Parse(f);
    public void Setphase(string phase) => nextWave.phase = float.Parse(phase);
    public void SetWave()
    {
        Reset();
    }
    public void ResetWave()
    {
        //Set to default
        wave = new SinusoidalWave();
        wave.L = 1;
        wave.init(GM.CurGrid.offset);
    }
}
