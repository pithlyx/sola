using UnityEngine;

public class Assets : MonoBehaviour
{
    // Reference to the ResourceDatabase asset in the Unity Editor
    public ResourceDatabase resourceDatabase;

    private void Awake()
    {
        // Set the Instance property of ResourceDatabase
        ResourceDatabase.Instance = resourceDatabase;

        // Initialize the ResourceDatabase
        InitResourceDatabase();
    }

    private void InitResourceDatabase()
    {
        // Populate the data of the ResourceDatabase
        ResourceDatabase.Instance.PopulateData();
    }
}
