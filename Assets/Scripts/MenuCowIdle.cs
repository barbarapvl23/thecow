
using System.IO;
using UnityEngine;

public class MenuCowIdle : MonoBehaviour
{
    public float speed = 2f;
    public float waitDuration = 1f;
    public Vector2 moveBoundsX = new Vector2(-7f, 7f); 
    public Vector2 moveBoundsY = new Vector2(-3f, 3f);

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float timer = 0f;

    void Start()
    {
        ChooseNewTarget();    
    }

    void Update()
    {
        if (isMoving)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                timer = waitDuration;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ChooseNewTarget();
            }
        }
    }

    void ChooseNewTarget()
    {
        float newX = Random.Range(moveBoundsX.x, moveBoundsX.y);
        float newY = Random.Range(moveBoundsY.x, moveBoundsY.y);
        targetPosition = new Vector3(newX, newY, transform.position.z);
        isMoving = true;
    }
}
