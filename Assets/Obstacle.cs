using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float passMargin = 1.5f;
    [HideInInspector] public bool hit = false;
    bool passed = false;
    PlaneController plane;

    void Start()
    {
        plane = FindFirstObjectByType<PlaneController>();
    }

    void Update()
    {
        if (passed || plane == null) return;
        if (plane.transform.position.z > transform.position.z + passMargin)
        {
            passed = true;
            if (!hit) plane.OnPassedObstacle();
        }
    }
}