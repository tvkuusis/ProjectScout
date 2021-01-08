using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShipVariables", order = 1)]
public class ShipVariables : ScriptableObject
{
    public string prefabName;
    public float startHealth;
    public float engineForce;
    public float engineDashForce;
    public float dashMaxTime;
    public float maxVelocity;
    public float maxDashVelocity;
    public float sideThrusterToMainThrusterRatio;
    public float maxAngularVelocity;


}