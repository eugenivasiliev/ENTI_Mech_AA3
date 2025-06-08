using TMPro;
using UnityEngine;

public class BuoyManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private TMP_Text scaleLabel;
    [SerializeField] private TMP_Text densityLabel;

    private bool placingBuoy;
    public bool PlacingBuoy {  get { return placingBuoy; } set { placingBuoy = value; } }
    [Header("Prefabs")]
    [SerializeField] private GameObject placeholderBuoy;
    [SerializeField] private GameObject physicsBuoy;

    private GameObject placeholderBuoyInstance;
    private GridManager GM;
    (float, float) bounds;

    void Start()
    {

        placeholderBuoyInstance = Instantiate(placeholderBuoy);
        GM = GridManager.Instance;



        SetScale(5);
        SetDensity(1000);
        SeeBuoys(false);
    }

    // Update is called once per frame
    void Update()
    {
        bounds = (GM.CurGrid.offset.x, -GM.CurGrid.offset.x);

        placeholderBuoyInstance.SetActive(placingBuoy);
        if (!placingBuoy) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (pos.x < bounds.Item1 + placeholderBuoyInstance.transform.localScale.x * 0.5f || 
            pos.x > bounds.Item2 - placeholderBuoyInstance.transform.localScale.x * 0.5f)
        {
            pos.x = Mathf.Clamp(pos.x, 
                bounds.Item1 + placeholderBuoyInstance.transform.localScale.x * 0.5f, 
                bounds.Item2 - placeholderBuoyInstance.transform.localScale.x * 0.5f);
            placeholderBuoyInstance.transform.position = pos;
            return;
        }

        placeholderBuoyInstance.transform.position = pos;

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(physicsBuoy, pos, Quaternion.identity);
            placingBuoy = false;
        }
    }

    public void SetScale(float scale)
    {
        placeholderBuoyInstance.transform.localScale = scale * Vector2.one;
        physicsBuoy.transform.localScale = scale * Vector3.one;
        scaleLabel.text = "Scale: " + scale.ToString("0.00");
    }

    public void SetDensity(float density)
    {
        physicsBuoy.GetComponent<Buoy>().Density = density;
        densityLabel.text = "Density: " + density.ToString("0.0");
    }

    public void RemoveBuoys()
    {
        GameObject[] buoys = GameObject.FindGameObjectsWithTag("Buoy");
        foreach (GameObject buoy in buoys)
        {
            Destroy(buoy);
        }
    }
    public void SeeBuoys(bool value)
    {
        physicsBuoy.GetComponent<SpriteRenderer>().sortingOrder = value ? 3 : 0;
        placeholderBuoyInstance.GetComponent<SpriteRenderer>().sortingOrder = value ? 2 : -1;
        GameObject[] buoys = GameObject.FindGameObjectsWithTag("Buoy");
        foreach (GameObject buoy in buoys)
            buoy.GetComponent<SpriteRenderer>().sortingOrder = value ? 3 : 0;
    }
}
