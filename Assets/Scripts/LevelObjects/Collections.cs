using UnityEngine;

public class Collections : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private string collectionName;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collection: {collectionName} (Score: {score})");
    }
}
