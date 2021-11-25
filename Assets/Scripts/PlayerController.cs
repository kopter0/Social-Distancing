using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public GameController game;
    public float turn_rate = 2.0f;
    public float speed = 4.0f;



    private Vector3 camera_offset;
    private Rigidbody rb;
    private float mvx = 0;
    private float mvy = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera_offset = transform.position - Camera.main.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (game.gameState != GameController.MyState.InGame)
            return;
        switch (other.name)
        {
            case "Sphere":
                game.gameState = GameController.MyState.PostGame;
                game.StartCoroutine("Lose");
                break;
            case "Finish":
                game.gameState = GameController.MyState.PostGame;
                game.StartCoroutine("Win");
                break;
            default:
                break;
        }

        
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        mvx = movementVector.x;
        mvy = movementVector.y;
    }

    private void OnSpace()
    {
        game.SwitchGameState();
    }

    private void FixedUpdate()
    {

        if (game.gameState == GameController.MyState.InGame)
        {
            rb.AddForce(speed * mvx, 0, speed * mvy);
            rb.AddTorque(turn_rate * mvy, 0, -turn_rate * mvx);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position - camera_offset, speed * Time.deltaTime);
        }
    }
}
