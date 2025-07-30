using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // Array to hold references to your 3D object prefabs
    public GameObject[] objectPrefabs;

    // // Spawn object by index
    // public void SpawnObjectAtIndex(int index)
    // {
    //     if (objectPrefabs == null || objectPrefabs.Length == 0)
    //     {
    //         Debug.LogError("No objects in the objectPrefabs array!");
    //         return;
    //     }

    //     if (index < 0 || index >= objectPrefabs.Length)
    //     {
    //         Debug.LogError("Index out of range!");
    //         return;
    //     }

    //     Instantiate(objectPrefabs[index], Vector3.zero, Quaternion.identity);
    //     Debug.Log("Spawned " + objectPrefabs[index].name + " at position (0, 0, 0)");
    // }

    // Spawn object by name
    public void SpawnObjectWithName(string objectName)
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogError("No objects in the objectPrefabs array!");
            return;
        }

        // Search through all objects for a matching name
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i] != null && objectPrefabs[i].name == objectName)
            {
                // Instantiate(objectPrefabs[i], Vector3.zero, Quaternion.identity);
                GameObject newObject = Instantiate(objectPrefabs[i]); 

                Debug.Log("Spawned " + objectPrefabs[i].name);
                return;
            }
        }

        Debug.LogWarning("No object found with name: " + objectName);
    }

    // Returns a string with all object names separated by commas
    public string GetAllObjectNames()
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogWarning("No objects in the objectPrefabs array!");
            return "[]";
        }

        // Create a string builder for efficient string concatenation
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("[");

        // Add all names separated by commas
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i] != null)
            {
                sb.Append(objectPrefabs[i].name);
                if (i < objectPrefabs.Length - 1)
                {
                    sb.Append(", ");
                }
            }
        }

        sb.Append("]");
        return sb.ToString();
    }
}