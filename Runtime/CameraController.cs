using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    public float moveSpeed;
    public float rotSpeed;

    private Vector3 _input;

    Queue<object> _debug = new Queue<object>();

    private void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var y = Input.GetAxis("Jump");

        _input.x = x;
        _input.z = z;
        _input.y = y;

        var q = Input.GetAxis("Rotation");
        transform.Rotate(Vector3.up, q * Time.deltaTime * 10 * rotSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 movement = transform.TransformDirection(_input);

        rb.velocity = movement * Time.fixedDeltaTime * 40 * moveSpeed;

    }

}
