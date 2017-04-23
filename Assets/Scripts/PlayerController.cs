using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Rigidbody2D playerRb;
	RaycastHit2D raycastHit;

	bool facingRight = true;

	float maxMovementSpeed = 80f;
	float maxFallingDistance = 2.5f;
	float maxHitDistance = 0.21f;

	float currentHeight;
	float newHeight;


	// Use this for initialization
	void Start () {
		playerRb = this.GetComponent<Rigidbody2D> ();
		playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.F))
		{
			Vector2 direction = new Vector2();

			if (facingRight)
				direction = Vector2.right;
			else
				direction = Vector2.left;

			RaycastHit2D hit = Physics2D.Raycast(playerRb.position, direction, maxHitDistance);

			if (hit && hit.collider.gameObject.tag == "Wall")
			{
				Destroy(hit.collider.gameObject);
			}
		}
	}

	public void setStartingHeight(float height)
	{
		currentHeight = height;
		newHeight = height;
	}

	// Update is called once per frame
	void FixedUpdate () {
		raycastHit = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 0.15f);


		float move = Input.GetAxis("Horizontal");
		playerRb.velocity = new Vector2(move * maxMovementSpeed * Time.deltaTime, playerRb.velocity.y);

		if (move > 0 && !facingRight)
			Flip();
		else if (move < 0 && facingRight)
			Flip();

		if (Input.GetKey(KeyCode.Space) && raycastHit.collider != null && this.GetComponent<Rigidbody2D>().velocity.y == 0)
		{
			playerRb.AddForce(Vector3.up * 4.75f, ForceMode2D.Impulse);
		}
	}


	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		
		if (coll.gameObject.tag == "Hazard") 
		{
			OnDeath ();
			Debug.Log ("Death by hazard");
		}
		if ((coll.gameObject.tag == "BorderBlock" || coll.gameObject.tag == "Wall") && raycastHit.collider != null)
		{
			newHeight = gameObject.transform.position.y;
		//	Debug.Log ("Distance: " + (currentHeight - newHeight));
			if (currentHeight - newHeight > maxFallingDistance)
			{
				Debug.Log ("Death by falling. Distance: " + (currentHeight - newHeight));
				OnDeath ();
			}
			currentHeight = gameObject.transform.position.y;
		}
		if (coll.gameObject.tag == "Exit") 
		{
			OnDeath ();
		}
	}

	void OnTriggerEnter2D(Collider2D coll) 
	{
		if (coll.gameObject.tag == "Exit") 
		{
			Debug.Log ("Level completed");
			gameObject.SetActive (false);
		}
		if (coll.gameObject.tag == "PickUp" || coll.gameObject.tag == "Key") 
		{
			coll.gameObject.SetActive (false);
		}
	}

	void OnDeath()
	{
		gameObject.SetActive (false);
	}
}

