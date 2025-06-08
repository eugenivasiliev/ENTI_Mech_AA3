using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;

public class Gerstner : MonoBehaviour
{

    struct GerstnerWave
    {
        public float A;         //Amplitude
        public float L;         //WaveLength
        public int D;           //Direction
        public float phase;     //Phase
        public float k;         //Wave number
        public float omega;     //Angular frequency

        public void init()
        {
            D = D * 2 - 1; //We correct the index in the selection UI
            k = 2 * Mathf.PI / L;
            omega = Mathf.Sqrt(k * Physics.Constants.g);
        }
    }


    [Header("Wave Parameters")]
    private List<GerstnerWave> waves = new List<GerstnerWave>();
    private GerstnerWave nextWave = new GerstnerWave();

    private GridManager GM;

    void Start()
    {
        GM = GridManager.Instance;
    }

    void Update()
    {
        //Get constant values to avoid unnecessary calls to memory
        float t = SimulatorManager.Instance.time;
        float spacing = GM.CurGrid.spacing;
        Vector2 GM_offset = GM.CurGrid.offset;
        float indexWidth = GM.CurGrid.indexWidth;
        float indexHeight = GM.CurGrid.indexHeight;

        Vector2 basePos = GM_offset;
        Vector2 newPos;
        for (int x = 0; x < indexWidth; x++)
        {
            //We compute the basePos in real time, avoiding calls to initialPositions
            basePos.x += spacing;
            basePos.y = GM_offset.y;
            newPos = NewPos(basePos, t);
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

    Vector2 NewPos(Vector2 initial, float t)
    {
        Vector2 sum = Vector2.zero;
        foreach (GerstnerWave w in waves)
        {
            //More efficient than calculating separately
            math.sincos(w.k * initial.x - w.D * w.omega * t + w.phase, out float s, out float c); 
            sum.x += w.A * s;
            sum.y -= w.A * c;
        }
        return initial + sum;
    }

    public void SetA(string A) => nextWave.A = float.Parse(A);
    public void SetL(string L) => nextWave.L = float.Parse(L);
    public void SetD(int D) => nextWave.D = D;
    public void Setphase(string phase) => nextWave.phase = float.Parse(phase);
    public void AddNextWave() {
        nextWave.init();
        waves.Add(nextWave);
    } 
    public void ResetWaves() => waves.Clear();
}
