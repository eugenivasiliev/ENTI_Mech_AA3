using UnityEngine;

public class Buoy : MonoBehaviour
{
    //To use multiplication instead of division, faster
    private const float oneSixth = 1.0f / 6.0f; 

    [SerializeField] private float width;   //m
    [SerializeField] private float height;  //m
    [SerializeField] private float depth;   //m
    private float baseArea;

    [SerializeField] private float density; //kg/m^3
    public float Density { get { return density; } set { density = value; } }
    [SerializeField] private float mass; //kg
    private float invMass;

    [Header("Physics attributes")]
    protected Vector2 position;
    protected Vector2 velocity;
    protected Vector2 acceleration;

    private Vector2 weight;

    void Start()
    {

        width = transform.localScale.x;
        height = transform.localScale.y;
        depth = transform.localScale.z;

        //We precompute some values to optimise the calculations later
        baseArea = width * depth;
        mass = density * height * baseArea;
        invMass = 1.0f / mass; //Avoid division, use multiplication

        //Importantly, weight is always constant, so we can just compute it once and always add it
        weight = mass * Physics.Constants.g * Vector2.down;

        //Set up base physics values
        position = transform.position;
        velocity = Vector2.zero;
        acceleration = Acceleration(position, velocity);
    }

    void Update()
    {
        (position, velocity) = RungeKutta4(position, velocity);
        transform.position = position;
    }

    private (Vector2, Vector2) differentialFunction(Vector2 _position, Vector2 _velocity) =>
        (_velocity, Acceleration(_position, _velocity));

    private (Vector2, Vector2) RungeKutta4(Vector2 _position, Vector2 _velocity)
    {
        //To avoid accessing SimulatorManager in memory and avoid repeat products
        float dt = SimulatorManager.Instance.Dt;
        float dt_2 = dt * 0.5f;

        Vector2 K1position, K1velocity, K2position, K2velocity, K3position, K3velocity, K4position, K4velocity;

        (K1position, K1velocity) = differentialFunction(_position, _velocity);
        (K2position, K2velocity) = differentialFunction(_position + dt_2 * K1position, _velocity + dt_2 * K1velocity);
        (K3position, K3velocity) = differentialFunction(_position + dt_2 * K2position, _velocity + dt_2 * K2velocity);
        (K4position, K4velocity) = differentialFunction(_position + dt * K3position, _velocity + dt * K3velocity);

        Vector2 newPosition = _position + dt * oneSixth * (K1position + 2 * K2position + 2 * K3position + K4position);
        Vector2 newVelocity = _velocity + dt * oneSixth * (K1velocity + 2 * K2velocity + 2 * K3velocity + K4velocity);

        return (newPosition, newVelocity);

    }

    protected virtual Vector2 Acceleration(Vector2 _position, Vector2 _velocity)
    {
        float sunkenHeight = GridManager.Instance.seaLevel - (_position.y - height / 2.0f);
        sunkenHeight = Mathf.Clamp(sunkenHeight, 0.0f, height);
        float sunkenVolume = sunkenHeight * baseArea;

        Vector2 force = weight + Physics.Constants.waterDensity * Physics.Constants.g * sunkenVolume * Vector2.up;
        return force * invMass;

    }
}
