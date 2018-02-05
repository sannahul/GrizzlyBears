using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine; 

public class PlayerMovement : MonoBehaviour 
{
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    private Vector3 prevPosition;
	private Vector3 startPosition;
	private Quaternion startRotation;
    private float moveSpeed = 8;
    private float rotateSpeed = 10;
    public static readonly float GRID_SIZE = 2;
	public static readonly float YGRID_SIZE = 1;
    public static readonly float delta = 0.01f;
    public static readonly float Y_COLL_BUFF = 0.2f;
    public static readonly float JUMP_FORCE = 20f;
    public static readonly float GRAVITY = -80f;
    public static readonly Vector3 X = new Vector3 (1, 0, 0);
    public static readonly Vector3 Y = new Vector3 (0, 1, 0);
    public static readonly Vector3 Z = new Vector3 (0, 0, 1);
    private bool move = false;
    private bool right = false;
    private bool left = false;
	private bool jumping = false;
	private int count;	
    public Rigidbody rb;
	public Text countText;
	public Text winText; 

    // Set fields
    void Start() 
    {
		count = 0;
		SetCountText(); 
		targetPosition = normToGrid(transform.position);
		prevPosition = normToGrid(transform.position);
		startPosition = normToGrid(transform.position);
		startRotation = transform.rotation;
        targetRotation = transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, GRAVITY, 0);
        // could have something interpretting GRID_SIZE from external source here
    }

	void reset()
	{
		rb.useGravity = false;
		rb.velocity=Vector3.zero;
		transform.position = startPosition;
		transform.rotation = startRotation;
		targetPosition = startPosition;
		targetRotation = startRotation.eulerAngles;
		rb.useGravity = true;
	}
	
    // Update Loop: slightly roundabout but necessary for keeping the moves discrete from each other.
    void Update() 
    {
        if (move) 
        {
            movement();
        }
        if (right || left) 
        {
            rotation();
        }
		if (jumping)
		{
			if (transform.position.y > prevPosition.y+0.5)
			{
				forward();
				jumping = false;
			}
		}
        updateCommandComplete();
        if (commandComplete()) 
        {
            targetPosition = normToGrid(transform.position);
			correctRotation();
        }
    }

	public void makeMove(BlockType move)
	{
		switch (move) {
		case BlockType.Forward:
			forward();
			break;
		case BlockType.Left:
			turnLeft();
			break;
		case BlockType.Right:
			turnRight();
			break;
		case BlockType.Jump:
			jump();
			break;
		default:
			break;
		}
		return;
	}

    // Return back to previous position on collision with object on same y-level
    void OnCollisionEnter (Collision col)
    {
        if (Mathf.Abs(transform.position.y - col.transform.position.y) < Y_COLL_BUFF) 
		{
            targetPosition = prevPosition;
        }
    }

    // Move target position forward if not completing move
    public void forward() 
    {
        if (commandComplete() || (jumping && !(move))) 
		{
            prevPosition = normToGrid(transform.position);
            move = true;
            forwardTargetPosition();
        }
    }

    // Move target rotation right by 90 if not completing a move
    public void turnRight()
    {
        if (commandComplete()) 
		{
            right = true;
            rightTargetRotation();
        }
    }

    // Move target rotation left by 90 if not completing a move
    public void turnLeft()
    {
        if (commandComplete()) 
		{
            left = true;
            leftTargetRotation();
        }
    }

    // Jump command: simple upwards force + forward(); if not completing a move
    public void jump()
    {
		bool canJump = false;
		if (commandComplete()) 
		{
            prevPosition = normToGrid(transform.position);
			Vector3 forwardPosition = targetPosition + (2 * getDirection());
			Debug.Log("forwardPosition = "+forwardPosition + "   targetPosition = "+targetPosition);
			Collider[] hitColliders = Physics.OverlapSphere(forwardPosition,  min(GRID_SIZE, YGRID_SIZE)/4);
			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i].tag != "Pick Up") canJump = true;
			}
			if (canJump == true)
			{
				jumping = true;
				rb.AddForce (new Vector3 (0, JUMP_FORCE, 0), ForceMode.Impulse);
			}
			else
			{
				Debug.Log("NO JUMP");
				jumping = false;
				rb.AddForce (new Vector3 (0, JUMP_FORCE, 0), ForceMode.Impulse);
			}
        }
    }

	// Shifts the target position forward in correct direction (don't call directly)
    private void forwardTargetPosition() 
    {
		targetPosition = normToGrid(targetPosition + GRID_SIZE*getDirection());
    }
	
	// Shifts target rotation right by 90 degrees (don't call directly)
    private void rightTargetRotation()
    {
        float y = 90*Mathf.Round(transform.eulerAngles.y/90);
        y = mod((y + 90), 360);
        targetRotation = new Vector3(0, y, 0);
    }
	
	// Shifts target rotation left by 90 degrees (don't call directly)
    private void leftTargetRotation()
    {
        float y = 90*Mathf.Round(transform.eulerAngles.y/90);
        y = mod((y - 90), 360);
        targetRotation = new Vector3(0, y, 0);
    }

    // Check if player has finished last move
    public bool commandComplete() 
    {
        return (move == false && right == false && left == false && jumping == false);
    }

    // Set commands to complete if target positions are met
    public void updateCommandComplete()
    {
        if (Mathf.Abs(transform.position.x - targetPosition.x) < delta && Mathf.Abs(transform.position.z - targetPosition.z) < delta) 
		{
            move = false;
        }
        if (Mathf.Abs(Vector3.Distance(transform.eulerAngles, targetRotation)) < delta) 
		{
            right = false;
            left = false;
        }
    }
	
	// movement loop
    private void movement() 
    {
        float step = moveSpeed * Time.deltaTime;
        //if (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
        if ((Mathf.Abs(transform.position.x - targetPosition.x) > 0.01f) || (Mathf.Abs(transform.position.z - targetPosition.z) > 0.01f)) 
		{
            transform.position = xzLerp(transform.position, targetPosition, step);//Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else
        {
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            move = false;
			jumping = false;
        }
    }
	
	// rotation loop
    private void rotation() 
    {
        float degree = rotateSpeed * Time.deltaTime;
        if (Vector3.Distance (transform.eulerAngles, targetRotation) > 3f) 
		{
            transform.eulerAngles = AngleLerp(transform.rotation.eulerAngles, targetRotation, degree);
        } 
		else 
		{
            if (targetRotation.y >= 360) 
			{
                targetRotation = new Vector3(0, 0, 0);
            }
            transform.eulerAngles = targetRotation;
            right = false;
            left = false;
        }
    }

    // Corrects rotation outside of movement loop (kinda hacky but compensates 
    //  for unity physics when non rotation moves affect rotation)
    private void correctRotation()
    {
        if (Mathf.Abs(Vector3.Distance(transform.eulerAngles, targetRotation)) >= delta) 
		{
            rotation();
        }
    }

    // Custom Lerp functions
    public Vector3 AngleLerp(Vector3 startAngle, Vector3 finishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle (startAngle.x, finishAngle.x, t);
        float yLerp = Mathf.LerpAngle (startAngle.y, finishAngle.y, t);
        float zLerp = Mathf.LerpAngle (startAngle.z, finishAngle.z, t);
        Vector3 Lerped = new Vector3 (xLerp, yLerp, zLerp);
        return Lerped;
    }

    public Vector3 xzLerp(Vector3 startPos, Vector3 finishPos, float t)
    {
        float xLerp = Mathf.Lerp (startPos.x, finishPos.x, t);
        //float yLerp = Mathf.Lerp (startPos.y, finishPos.y, t);
        float zLerp = Mathf.Lerp (startPos.z, finishPos.z, t);
        Vector3 Lerped = new Vector3 (xLerp, transform.position.y, zLerp);
        return Lerped;
    }

    // Get players orientation as a unit vector
    public Vector3 getDirection()
    {
        float angle = mod(targetRotation.y, 360);
		if (angle > 45 && angle <= 135) 
        {
            return X;
        } 
        else if (angle > 135 && angle <= 225) 
        {
            return -Z;
        } 
        else if (angle > 225 && angle < 315) 
        {
            return -X;
        } 
        else 
        {
            return Z;
        }
    }

    // Custom modulus function
    public float mod(float a, float b)
    {
        return a - b * Mathf.Floor (a / b);
    }

    // Turn a vector in the opposite direction
    public Vector3 negVector(Vector3 arg)
    {
        Vector3 ret = arg;
        ret.x = -arg.x;
        ret.y = -arg.y;
        ret.z = -arg.z;
        return ret;
    }
	
	// Norms a position vector to the grid
	public Vector3 normToGrid(Vector3 input)
	{
        float x = GRID_SIZE*Mathf.Round(input.x/GRID_SIZE);
        float y = YGRID_SIZE*Mathf.Round(input.y/YGRID_SIZE);
        float z = GRID_SIZE*Mathf.Round(input.z/GRID_SIZE);
		return new Vector3(x, y, z);
	}
	
	// return minimum of two floats
	public float min(float a, float b)
	{
		if (a < b) return a;
		else return b;
	}
	
	// return maximum of two floats 
	public float max(float a, float b)
	{
		if (a > b) return a;
		else return b;
	}
	
	void OnTriggerEnter(Collider other) 
	{	
		if (other.gameObject.CompareTag ( "Pick Up"))
		{
			other.gameObject.GetComponent<Renderer>().enabled = (false);
			count = count + 1;
			SetCountText ();
		}

		if (other.gameObject.CompareTag("Victory_Tile_Tag"))
		{
			winText.text = "YOU WIN";
			OnDisable(); 
		}
	}

	void OnDisable(){
		gameObject.SetActive(false); 	
	}

	void SetCountText()
	{
		countText.text = "Star Count: " + count.ToString();
	} 

}