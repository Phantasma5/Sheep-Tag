using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    #region References
    [HideInInspector] private Rigidbody2D myRigidbody2D;
    #endregion
    #region Variables
    [HideInInspector] public float speed = 5;
    /*[HideInInspector]*/ public bool captured = false;
    [HideInInspector] private float captureCooldown = 0;
    #endregion
    private void Awake()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            References.localPlayer = this.gameObject;
        }
    }
    private void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        captureCooldown = Time.time;
    }

    private void PlayerMovement()
    {
        if(captured)
        {
            return;
        }
        myRigidbody2D.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed, 0);

        if (gameObject.name == "Player_Sheep" &&
            transform.position.x < 2 &&
            transform.position.x > -2 &&
            transform.position.y < 2 &&
            transform.position.y > -2)
        {
            if(captureCooldown<Time.time)
            {
                captureCooldown = Time.time + 2;
                GetComponent<PlayerRPC>().Rescue();
            }
        }
    }
    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            PlayerMovement();
        }
    }
}
