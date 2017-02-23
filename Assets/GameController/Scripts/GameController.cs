using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public Camera gameCamera;
	public GameObject squarePrefab;

	public static GameController Instance;
	public Labyrinth labyrinthInstance = new Labyrinth ();

	public float startingSpeed;
	private float desiredSpeed;
	private float currentSpeed;
	private float generateDistance;
	private float currentDistance = 0;
	private float score;
	private float timePassedGame;
	private float lerp;
	private bool isPlaying;
	private bool isCourutineActive = false;
	public int gridWidth;
	public int gridHeight;
	public int startingGenSectors;

	public int adsPixelSize;
	public bool upperAdsSpace;
	public bool lowerAdsSpace;

	//This will be used for displaying main menu
	private float opacityUI = 1f;
	public float fadeOutTime;

	private float timer;

	static float SCORE_CONSTANT = 1f;

	void OnDrawGizmos ()
	{
		Vector3 bottomLeftCorner = gameCamera.ScreenToWorldPoint(new Vector3(0f, lowerAdsSpace ? adsPixelSize : 0f, 10f));
		Vector3 upperRightCorner = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, upperAdsSpace ? gameCamera.pixelHeight - adsPixelSize : gameCamera.pixelHeight, 10f));

		Gizmos.color = Color.red;
		Gizmos.DrawSphere (bottomLeftCorner, 3f);
		Gizmos.DrawSphere (upperRightCorner, 3f);
	}

	void Init ()
	{
		Vector3 bottomLeftCorner = gameCamera.ScreenToWorldPoint(new Vector3(0f, lowerAdsSpace ? adsPixelSize : 0f, 10f));
		Vector3 upperRightCorner = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, upperAdsSpace ? gameCamera.pixelHeight - adsPixelSize : gameCamera.pixelHeight, 10f));

		float screenWidthUnits = -bottomLeftCorner.x + upperRightCorner.x;
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
		StartCoroutine(coroutine(0f, 4*generateDistance - (generateDistance - currentDistance)));

		score = 0;
		desiredSpeed = startingSpeed;
		timer = 0;

	}

	public IEnumerator coroutine (float a, float b) //currentDistance - generateDistance?
	{
		
		while (a < b)
		{
			isCourutineActive = true;
			Debug.Log("active");

			float completion = 1f - a / b;

			labyrinthInstance.Move (Vector3.down * 400f * completion * Time.deltaTime);
			currentDistance += 400f * completion * Time.deltaTime;

			if (currentDistance >= generateDistance)
			{
				currentDistance -= generateDistance;
				labyrinthInstance.GenerateNextSector ();
			}

			a += 100f * Time.deltaTime;

			opacityUI = completion;

			yield return null;
		}
		StartCoroutine(coroutine2());
	}
	public IEnumerator coroutine2 ()
	{
		Debug.Log("Started");
		isCourutineActive = false;
		isPlaying = true;
		yield return null;
	}

	void MainPlayLoop ()
	{
		timePassedGame += Time.deltaTime;
		/*

		*/
		if (currentSpeed != desiredSpeed)
		{
			currentSpeed = Mathf.Lerp (currentSpeed, desiredSpeed, lerp);
			lerp += 0.5f * Time.deltaTime;
		}
		else lerp = 0f;

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
}