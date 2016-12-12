using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Basket : MonoBehaviour {
	public Text scoreText;
	public int playerScore = 0;

	public float speed = 0f;
	public int maxLeft = -5;
	public int maxRight = 5;
	public int direction = 1; //1 is right, -1 is left

	public ParticleSystem particles;

	void Start()
	{
		particles = GetComponentInChildren<ParticleSystem>();
	}

	// Update is called once per frame
	void Update() {

		scoreText.text = playerScore.ToString();

		//Move the basket
		transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

		if (transform.position.x < maxLeft)
		{
			direction = 1; //this is set every frame, which could be made more efficient, but it's not too bad
		}
		if (transform.position.x > maxRight)
		{
			direction = -1;
		}

	}

	public void AddPoint()
	{
		playerScore++;
		speed += 1.5f; //This is the same as: speed = speed + 1.5f; 
		particles.Play();
	}
}
