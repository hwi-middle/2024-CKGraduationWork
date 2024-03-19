using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    private float _elapsedTime = 0f;

    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _moveAmount = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, Mathf.Sin(_elapsedTime * _moveSpeed) * _moveAmount, transform.position.z);
    }
}
