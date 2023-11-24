using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class RigidBodyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public GameObject play;
    [SerializeField]
    private Animator anim;

    private Vector2 input;
    private PlayerInput playerInput;
    public float rotationSpeed = 7f;
    [SerializeField]
    private float upForce = 290f;
    [SerializeField]
    private float playerSpeed = 4f;

    public bool IsMovementAllowed
    {
        get
        {
            return GameManager.gameManager.CurrentGameStatus == GameStatus.Playing;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        //upForce = 290f;
        play.GetComponent<isGrounded>().OnFloorCollisionChanged.AddListener(setJumpingAnimation);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsMovementAllowed)
        {
            play.GetComponent<isGrounded>().CheckGround();
            anim.SetBool("IsRunning", false);
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;
            input = playerInput.actions["Movement"].ReadValue<Vector2>();
            Vector3 inputDir = orientation.forward * input.y + orientation.right * input.x;
            if (inputDir != Vector3.zero)
            {
                anim.SetBool("IsRunning", true);
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                rb.MovePosition(player.position + inputDir * Time.deltaTime * playerSpeed);
            }
        }
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {

        if (callbackContext.performed)
        {
            if (play.GetComponent<isGrounded>().grounded)
            {
                rb.AddForce(Vector3.up * upForce);
            }

        }

    }

    private void setJumpingAnimation(bool isOnFloor)
    {
        anim.SetBool("Jumping", !isOnFloor);
    }

    public void Pause()
    {
        GameManager.gameManager.Pause();
    }


}
