using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static CharacterController character;

    public float moveSpeed = 10f;
    private const float gravity = 9.8f;

    public Transform groundChecker;
    public const float groundSphereRad = 1.5f;
    public LayerMask groundMask;

    private float gravitySpeed = 0.0f;
    public static bool isOnGround;

    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 15f;
        }
        else
        {
            moveSpeed = 10f;
        }

        isOnGround = Physics.CheckSphere(groundChecker.position, groundSphereRad, groundMask);

        if(isOnGround && gravitySpeed < 0.0f)
        {
            gravitySpeed = -0.1f;
        }

        gravitySpeed += -gravity * Time.deltaTime * Time.deltaTime;

        Vector3 moveVec = (transform.right * inputX + transform.forward * inputZ) * moveSpeed * Time.deltaTime;
        moveVec.y = gravitySpeed;

        character.Move(moveVec);
    }
}
