using UnityEngine;

public static class TransformExtension
{
    public static void DestroyChildren(this Transform target)
    {
        foreach (Transform child in target)
        {
            MonoBehaviour.Destroy(child.gameObject);
        }
    }
}
