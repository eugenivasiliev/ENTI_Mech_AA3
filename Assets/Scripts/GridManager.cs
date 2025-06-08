using TMPro;
using UnityEngine;
using static UnityEditor.ShaderData;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject[,] cells;
    public struct GridSettings
    {
        public float width;
        public float height;
        public float spacing;

        public int indexWidth;
        public int indexHeight;

        public Vector2 offset;

        public void init(GameObject cellPrefab)
        {
            indexWidth = Mathf.RoundToInt(width / spacing);
            indexHeight = Mathf.RoundToInt(height / spacing);
            offset = Vector2.left * width / 2 + Vector2.down * 25.0f;

            cellPrefab.transform.localScale = Vector2.one * spacing * 2;
        }
    }

    public static GridManager Instance { get; private set; }

    [Header("Visuals")]
    [SerializeField] private Color lightest;
    [SerializeField] private Color darkest;
    [SerializeField] private AnimationCurve blendCurve;

    [Header("Cell Grid")]
    private GridSettings curGrid;
    public GridSettings CurGrid { get { return curGrid; } }
    private GridSettings newGrid;

    [SerializeField] private TMP_Text resolutionLabel;
    [SerializeField] private TMP_Text widthLabel;
    [SerializeField] private TMP_Text heightLabel;

    public float seaLevel { get { return curGrid.offset.y + curGrid.height; } }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        curGrid = new GridSettings();
        curGrid.width = 30;
        curGrid.height = 25;
        curGrid.spacing = 0.25f;
        curGrid.init(cellPrefab);
        newGrid = curGrid; //Default Value
        cells = new GameObject[curGrid.indexWidth, curGrid.indexHeight];

        for (int x = 0; x < curGrid.indexWidth; x++) for (int y = 0; y < curGrid.indexHeight; y++)
            {
                Vector2 pos = curGrid.offset + new Vector2(x * curGrid.spacing, y * curGrid.spacing);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                cells[x, y] = cell;
            }
    }

    public Color GetColor(float depth)
    {
        float t = (depth - curGrid.offset.y) / curGrid.height;
        return Color.Lerp(lightest, darkest, blendCurve.Evaluate(t));
    }
    public void SetResolution(float resolution)
    {
        newGrid.spacing = resolution;
        resolutionLabel.text = "Resolution: " + resolution.ToString("0.00");
    }
    public void SetWidth(float width)
    {
        newGrid.width = width;
        widthLabel.text = "Width: " + width.ToString("0.00");
    }
    public void SetHeight(float height)
    {
        newGrid.height = height;
        heightLabel.text = "Height: " + height.ToString("0.00");
    }

    public void SetGrid()
    {
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
        curGrid = newGrid;
        curGrid.init(cellPrefab);
        cells = new GameObject[curGrid.indexWidth, curGrid.indexHeight];
        for (int x = 0; x < curGrid.indexWidth; x++) for (int y = 0; y < curGrid.indexHeight; y++)
            {
                Vector2 pos = curGrid.offset + new Vector2(x * curGrid.spacing, y * curGrid.spacing);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                cells[x, y] = cell;
            }
    }
}
