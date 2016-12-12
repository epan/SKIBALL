using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RespawnBall : MonoBehaviour {
	private Vector3 startPosition;
	public Basket basket; //End section
	public static int currentScene = 0; //End section

	public int ballsThrown = 0;
	public GameObject badThrowObject;
	public Transform badThrowSpawn;


	void Start () {

		//Set ball respawn point at initial ball position
		startPosition = this.transform.position; 
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Ground" || other.gameObject.tag == "Hoop")
		{

			//Add a point (and speed up the hoop) if your ball hits the hoop
			if (other.gameObject.tag == "Hoop")
			{
				//basket.AddPoint();
				other.gameObject.GetComponentInParent<Basket>().AddPoint();          
			}
			if(other.gameObject.tag == "Ground")
			{
				Instantiate(badThrowObject, badThrowSpawn);
			}

			//Move the ball back to the start position if your ball hits 'Hoop' or 'Ground' tagged objects
			transform.position = startPosition;
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

			//Increase the variable keeping track of # balls thrown, load next level after enough balls thrown
			ballsThrown++; //End section
			if(ballsThrown > 4)
			{
				LoadNextLevel();
			}

		}
	}

	void LoadNextLevel()
	{
		currentScene++;
		SceneManager.LoadScene(currentScene); //make sure you have "using UnityEngine.SceneManagement;" at the top
	}   
}
