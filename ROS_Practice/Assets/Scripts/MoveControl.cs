using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed;

    private Vector3 forward, right;
    // Start is called before the first frame update
    void Start()
    {
        forward = transform.forward;
        right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += forward; 
        if (Input.GetKey(KeyCode.A)) direction += -right; 
        if (Input.GetKey(KeyCode.S)) direction += -forward; 
        if (Input.GetKey(KeyCode.D)) direction += right; 

        direction.Normalize();
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
