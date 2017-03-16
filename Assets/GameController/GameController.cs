using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class GameController : MonoBehaviour
{
    InterstitialAd ad;
    BannerView bannerV;

	public Camera gameCamera;
	public GameObject squarePrefab;
    public GameObject endScreen;

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
    public int adTimer;
    private int adTime;

	//This will be used for displaying main menu
	private float opacityUI = 1f;
	public float fadeOutTime;

	private float timer;
    
	public float SCORE_CONSTANT = 1f;

	public void GetBall (GameObject _ball)
	{
		ball = _ball;
	}

    void OnDrawGizmos()
    {
        Vector3 bottomLeftCorner = gameCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
        Vector3 upperRightCorner = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth, gameCamera.pixelHeight, 10f));
    }

	void Init ()
	{
        adTime = adTimer;
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
        bannerV = new BannerView("ca-app-pub-5377701829054453/7844227328", AdSize.Banner, AdPosition.Bottom);
        AdRequest request1 = new AdRequest.Builder().Build();
        bannerV.LoadAd(request1);
        bannerV.Show();

    }

	public int GetScore ()
	{
		return Mathf.FloorToInt(score);
	}

	public float GetUIOpacity ()
	{
		return opacityUI;
	}

	public int GetPixelHeight ()
	{
		return (int)screenHeightPixels;
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
		labyrinthInstance.Move (Vector3.down * currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime);
		currentDistance += currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime;

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
        StartCoroutine(StartingMove(0f, passedSectorsOnStart * generateDistance - currentDistance - generateDistance / gridHeight - HeightPixelsToUnits(adsPixelSize)));
        score = 0;
		timer = 0;
        if (adTime <= 0)
        {
            ad = new InterstitialAd("ca-app-pub-5377701829054453/4751160121");
            AdRequest request = new AdRequest.Builder().Build();
            ad.LoadAd(request);
        }
    }

	public IEnumerator StartingMove (float a, float b) //currentDistance - generateDistance?
	{
		while (a < b)
		{
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
			ball.transform.position += Vector3.down * currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime;
		labyrinthInstance.Move (Vector3.down * currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime);
		currentDistance += currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime;
		score += currentSpeed * Time.deltaTime * SCORE_CONSTANT;

		if (currentDistance >= generateDistance)
		{
			currentDistance -= generateDistance;
			labyrinthInstance.GenerateNextSector ();
        }
        if (ball.transform.position.y < gameCamera.ScreenToWorldPoint(Vector3.zero).y)
        {
            Destroy(ball);
            if (adTime <= 0)
            {
                while (!ad.IsLoaded())
                {

                }
                ad.Show();
                adTime = adTimer;
            }
            else
                adTime--;
            endScreen.SetActive(true);
            isPlaying = false;
            opacityUI = 1f;
        }
    }

	float HeightPixelsToUnits(int pixels)
	{
		return pixels * screenHeightUnits / (float)screenHeightPixels;
	}
}