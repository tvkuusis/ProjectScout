using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShipVariables", order = 1)]
public class ShipVariables : ScriptableObject
{
    public string prefabName;
    public int numberOfPrefabsToCreate;
    public float startHealth;
   
    
}