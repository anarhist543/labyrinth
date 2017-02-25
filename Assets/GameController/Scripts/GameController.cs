using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public Camera gameCamera;
	public GameObject squarePrefab;

	public static GameController Instance;
	public Labyrinth labyrinthInstance = new Labyrinth ();

	public float startingSpeed;
	public float acceleration;
	private float currentSpeed;
	private float generateDistance;
	private float currentDistance = 0;
	private float score;
	private float timePassedGame;
	private bool isPlaying;
	private bool isCourutineActive = false;
	private GameObject ball;
	public GameObject ballPrefab;
	public int gridWidth;
	public int gridHeight;
	public int startingGenSectors;

	public int adsPixelSize;
	private float screenHeightUnits, screenHeightPixels;
	private float screenWidthUnits, screenWidthPixels;
	public int passedSectorsOnStart;
	public float speedOnstart;

	//This will be used for displaying main menu
	private float opacityUI = 1f;
	public float fadeOutTime;

	private float timer;

	public float SCORE_CONSTANT = 1f;

	public void GetBall (GameObject _ball)
	{
		ball = _ball;
	}

	void OnDrawGizmos ()
	{
		Vector3 bottomLeftCorner = gameCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
		Vector3 upperRightCorner = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, gameCamera.pixelHeight, 10f));

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (bottomLeftCorner, 3f);
		Gizmos.DrawSphere (upperRightCorner, 3f);
	}

	void Init ()
	{
		Vector3 bottomLeftCorner = gameCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
		Vector3 upperRightCorner = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, gameCamera.pixelHeight, 10f));

		screenWidthUnits = -bottomLeftCorner.x + upperRightCorner.x;
		screenHeightUnits = upperRightCorner.y - bottomLeftCorner.y;
		screenWidthPixels = gameCamera.pixelWidth;
		screenHeightPixels = gameCamera.pixelHeight;
		generateDistance = screenWidthUnits / gridWidth * gridHeight;
		labyrinthInstance.SetParams (gridWidth, gridHeight, startingGenSectors, screenWidthUnits, squarePrefab, bottomLeftCorner); 
		for (int i = 0; i < startingGenSectors; i++) {
			labyrinthInstance.GenerateNextSector ();
		}
	}

	public int GetScore ()
	{
		return Mathf.FloorToInt(score);
	}

	public float GetUIOpacity ()
	{
		return opacityUI;
	}

	void Awake ()
	{
		if (Instance != null)
		{
			Destroy (gameObject);
		}
		else 
		{
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	void Start ()
	{
		Init();
		isPlaying = false;
	}

	void Update ()
	{
		if (isCourutineActive) 
		{
			//do nothing (?)
		}
		else if (isPlaying) MainPlayLoop ();
		else MainMenuScrollingBackground ();
	}

	void MainMenuScrollingBackground ()
	{
		currentSpeed = startingSpeed;
		labyrinthInstance.Move (Vector3.down * currentSpeed * Time.deltaTime);
		currentDistance += currentSpeed * Time.deltaTime;

		if (currentDistance >= generateDistance)
		{
			currentDistance -= generateDistance;
			labyrinthInstance.GenerateNextSector ();
		}
	}

	public void GameStart ()
	{
		if (isPlaying || isCourutineActive)
			return;
		isCourutineActive = true;
		labyrinthInstance.GenerateBall (passedSectorsOnStart - 3, ballPrefab);
		StartCoroutine(StartingMove(0f, passedSectorsOnStart*generateDistance - currentDistance - generateDistance /gridHeight - HeightPixelsToUnits(50)));
		score = 0;
		timer = 0;

	}

	public IEnumerator StartingMove (float a, float b) //currentDistance - generateDistance?
	{
		while (a < b)
		{
			Debug.Log ("active");

			float completion = 1f - a / b;

			if (ball != null)
				ball.transform.position += Vector3.down * (speedOnstart * completion * Time.deltaTime + 0.5f);

			labyrinthInstance.Move (Vector3.down * (speedOnstart * completion * Time.deltaTime + 0.5f));
			currentDistance += speedOnstart * completion * Time.deltaTime + 0.5f;

			if (currentDistance >= generateDistance)
			{
				currentDistance -= generateDistance;
				labyrinthInstance.GenerateNextSector ();
			}

			a += speedOnstart * completion * Time.deltaTime + 0.5f;

			opacityUI = completion;

			yield return null;
		}
		StartCoroutine (Countdown (3));
	}

	IEnumerator Countdown(int c)
	{
		while (c > 0) {
			score = c;
			c--;
			yield return new WaitForSeconds (1f);
		}
		Debug.Log ("Started");
		isCourutineActive = false;
		isPlaying = true;
	}

	public bool CanGo(Square _from, Square _to)
	{
		return labyrinthInstance.CanGo (_from, _to);
	}

	void MainPlayLoop ()
	{
		timePassedGame += Time.deltaTime;

		currentSpeed += acceleration * Time.deltaTime;

		if (ball != null)
			ball.transform.position += Vector3.down * currentSpeed * Time.deltaTime;
		labyrinthInstance.Move (Vector3.down * currentSpeed * Time.deltaTime);
		currentDistance += currentSpeed * Time.deltaTime;
		score += currentSpeed * Time.deltaTime * SCORE_CONSTANT;

		Debug.Log(score);

		if (currentDistance >= generateDistance)
		{
			currentDistance -= generateDistance;
			labyrinthInstance.GenerateNextSector ();
		}
	}

	float HeightPixelsToUnits(int pixels)
	{
		return pixels * screenHeightUnits / (float)screenHeightPixels;
	}
}