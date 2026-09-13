using UnityEngine;
using UnityEngine.Rendering;

public class CowFollower : MonoBehaviour
{
    public float speed = 2f; // Velocidad de movimiento de la vaca
    public float wanderTime = 2f;
    public Vector3 targetPosition;
    private float timer;

    private void Start()
    {
        SetNewTarget();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f || timer <= 0f)
        {
            SetNewTarget();
        }  
    }

    void SetNewTarget()
    {
        float x = Random.Range(-6f, 6f); // Rango de movimiento en X
        float y = transform.position.y; // Rango de movimiento en Y
        targetPosition = new Vector3(x, y, transform.position.z);
        timer = wanderTime; // Reiniciar el temporizador
    }
}