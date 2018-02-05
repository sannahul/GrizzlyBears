using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine; 
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour 
{
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    private Vector3 prevPosition;
	private Vector3 startPosition;
	private Quaternion startRotation;
	private Vector3 setPosition;
	private Quaternion setRotation;
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
	private GameObject audio;
	private GameObject pole;
	private GameObject fall;
	private GameObject parser;
	string levelName;

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
		levelName = SceneManager.GetActiveScene().name;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, GRAVITY, 0);
        // could have something interpretting GRID_SIZE from external source here
		audio = GameObject.Find ("PlayerSounds");
		pole = GameObject.Find ("Pole");
		parser = GameObject.Find ("MovementControl");
		set ();
    }

	void reset()
	{
		count = 0;
		rb.useGravity = false;
		rb.velocity = Vector3.zero;
		transform.position = startPosition;
		transform.rotation = startRotation;
		targetPosition = startPosition;
		targetRotation = startRotation.eulerAngles;
		rb.useGravity = true;
		set ();
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
		case BlockType.Set:
			set ();
			break;
		case BlockType.Teleport:
			teleport ();
			break;
		default:
			break;
		}
		return;
	}

    // Return back to previous position on collision with object on same y-level
    void OnCollisionEnter (Collision col)
    {
		if ((Mathf.Abs(transform.position.y - col.transform.position.y) - 0.5) < Y_COLL_BUFF) 
		{
            targetPosition = prevPosition;
        }
    }

    // Move target position forward if not completing move
    public void forward() 
    {
		audio.SendMessage ("move");
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

	//Set command: saves position and puts down flag
	public void set()
	{
		if(commandComplete())
		{
			setPosition = normToGrid(transform.position);
			setRotation = transform.rotation;
			float xOffset, zOffset; 
			float offset = (7f/20f)*GRID_SIZE;
			if (getDirection() == X)
			{
				xOffset = offset;
				zOffset = -offset;
			}
			else if (getDirection() == Z)
			{
				xOffset = offset;
				zOffset = offset;
			}
			else if (getDirection() == -X)
			{
				xOffset = -offset;
				zOffset = offset;
			}		
			else
			{
				xOffset = -offset;
				zOffset = -offset;
			}			
			Vector3 polePosition = new Vector3(setPosition.x + xOffset, setPosition.y - 0.5f*YGRID_SIZE, setPosition.z + zOffset);
			pole.transform.position = polePosition;
			pole.transform.rotation = transform.rotation;
		}
	}

	//Teleport command: returns to saved position
	public void teleport()
	{
		if(commandComplete())
		{
			transform.position = setPosition;
			transform.rotation = setRotation;
			targetPosition = setPosition;
			targetRotation = setRotation.eulerAngles;
		}
	}

    // Jump command: simple upwards force + forward(); if not completing a move
    public void jump()
    {
		audio.SendMessage ("jump");
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
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			audio.SendMessage ("fish");
			other.gameObject.GetComponent<Renderer>().enabled = (false);
			count = count + 1;
			SetCountText ();
			Debug.Log ("Fish : " + count.ToString ());
		}

		if (other.gameObject.CompareTag("Victory_Tile_Tag"))
		{
			audio.SendMessage ("endtile");
			saveData (count);
			OnDisable(); 
		}

		if (other.gameObject.CompareTag("UnderSea"))
		{
			parser.SendMessage ("resetPress");
		}

		if (other.gameObject.CompareTag("Water"))
		{
			audio.SendMessage ("splash");
			//OnDisable(); 
		}

		if (other.gameObject.CompareTag ("Falling Tile"))
		{
			fall = FindClosestObject ();

		}
	}

	public void saveData (int fishes) {
		Debug.Log (PlayerPrefs.GetInt (levelName, 0).ToString());
		if (PlayerPrefs.GetInt (levelName, 0) < fishes)
		{
			PlayerPrefs.SetInt (levelName, fishes);
			Debug.Log ("Saving level: " + levelName + ", fishes: " + fishes.ToString ()); 
		}
	}


	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag ("Falling Tile"))
		{


			//GameObject.FindGameObjectWithTag ("Falling_1")
			FallObject();
			//Invoke("FallObject", 0.1f); 
		} 
	}

	void FallObject(){
		fall.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None; 
	}

	GameObject FindClosestObject()
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Falling_1");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos)
		{
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	void OnDisable(){
		gameObject.SetActive(false); 	
	}

	void SetCountText()
	{
		countText.text = "Star Count: " + count.ToString();
	} 

}