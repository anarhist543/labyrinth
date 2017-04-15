using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GoogleMobileAds.Api;

public class GameController : MonoBehaviour
{
    InterstitialAd ad;
    BannerView bannerV;

	public Camera gameCamera;
	public GameObject squarePrefab;
    public GameObject endScreen;
    public GameObject menuScreen;
    public GameObject gameCanvas;
    public PauseBuilder pause;
    public Image introImage;
    public float introTime;
    public float fadeTime;
    private float currentFadeTime;
    bool introFlag, introEnd, fadeIntro;
    [HideInInspector]
    public bool paused;
    public bool started;

	public static GameController Instance;
	public Labyrinth labyrinthInstance = new Labyrinth ();

	public float startingSpeed;
	public float acceleration;
	private float currentSpeed;
	private float generateDistance;
	private float currentDistance = 0;
	private float score = 0; //Edited, was without " = 0"
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

	private AudioSource audioSource;
	public float musicSpeedupPercent = 0.05f;
	// List of all music
	public AudioClip genericLoop;
	//

	private float timer;
    
	public float SCORE_CONSTANT = 1f;

	public void PlayMusic (AudioClip clipToPlay)
	{
		audioSource.clip = clipToPlay;
		audioSource.Play();
	}

	public void ToggleMute ()
	{
		if (audioSource.mute)
			audioSource.mute = false;
		else
			audioSource.mute = true;
	}

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            if(isPlaying || isCourutineActive)
                Pause();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
            if(isPlaying || isCourutineActive)
                Pause();
    }

    public void Pause()
    {
        if (!paused)
        {
            pause.BuildPause();
            paused = true;
        }
    }

    public void Resume()
    {
        pause.Resume();
        StartCoroutine(Unpause());
    }

    IEnumerator Unpause()
    {
        yield return null;
        paused = false;
    }

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
		PlayMusic(genericLoop);

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
        isPlaying = false;

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
		audioSource = GetComponent<AudioSource>();
	}

	void Start ()
	{
        introFlag = false;
        introEnd = false;
        fadeIntro = true;
        currentFadeTime = fadeTime;
	}

    void Update()
    {
        if (!introEnd)
            IntroLoop();
        else if (isCourutineActive)
        {
            if (paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Pause();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Resume();
                }
            }
        }
        else if (isPlaying) MainPlayLoop();
        else MainMenuScrollingBackground();
	}

    void IntroLoop()
    {
        if (fadeIntro)
        {
            currentFadeTime -= Time.deltaTime;
            float a = currentFadeTime / fadeTime;
            if (currentFadeTime > 0)
            {
                if (!introFlag)
                {
                    introImage.color = new Color(1 - a, 1 - a, 1 - a, 1);
                }
                else
                {
                    introImage.color = new Color(1, 1, 1, a);
                }
            }
            else
            {
                if (!introFlag)
                {
                    introImage.color = new Color(1, 1, 1, 1);
                    Init();
                    introFlag = true;
                    fadeIntro = false;
                    introTime += currentFadeTime;
                }
                else
                {
                    introEnd = true;
                    introImage.transform.parent.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            introTime -= Time.deltaTime;
            if(introTime<=0)
            {
                fadeIntro = true;
                currentFadeTime = fadeTime;

            }
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(endScreen.activeInHierarchy)
            {
                menuScreen.SetActive(true);
                endScreen.SetActive(false);
            } else
            if(menuScreen.activeInHierarchy)
            {
                Application.Quit();
            }
        }
	}

	public void GameStart ()
	{
		if (isPlaying || isCourutineActive)
			return;
		isCourutineActive = true;
        started = false;
        labyrinthInstance.GenerateBall (passedSectorsOnStart - 3, ballPrefab);
        StartCoroutine(StartingMove(0f, passedSectorsOnStart * generateDistance - currentDistance - generateDistance / gridHeight - HeightPixelsToUnits(adsPixelSize)));
        score = 0;
		timer = 0;
        if (adTime == adTimer)
        {
            ad = new InterstitialAd("ca-app-pub-5377701829054453/4751160121");
            AdRequest request = new AdRequest.Builder().Build();
            ad.LoadAd(request);
        }
        menuScreen.GetComponent<GraphicRaycaster>().enabled = false;
        endScreen.GetComponent<GraphicRaycaster>().enabled = false;
        gameCanvas.SetActive(true);
    }

	public IEnumerator StartingMove (float a, float b) //currentDistance - generateDistance?
	{
		while (a < b)
		{
            if (!paused)
            {
                float completion = 1f - a / b;

                if (ball != null)
                    ball.GetComponent<BallController>().Move(Vector3.down * (speedOnstart * completion * Time.deltaTime + 0.5f));

                labyrinthInstance.Move(Vector3.down * (speedOnstart * completion * Time.deltaTime + 0.5f));
                currentDistance += speedOnstart * completion * Time.deltaTime + 0.5f;

                if (currentDistance >= generateDistance)
                {
                    currentDistance -= generateDistance;
                    labyrinthInstance.GenerateNextSector();
                }

                a += speedOnstart * completion * Time.deltaTime + 0.5f;

                opacityUI = completion;
            }
            yield return null;
        }
        started = true;
        StartCoroutine (Countdown (3));
	}

	IEnumerator Countdown(int c)
	{
		while (c > 0) {
            if (!paused)
            {
                score = c;
                c--;
            }
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
        if (!paused)
        {
			audioSource.pitch = Mathf.Lerp (1, currentSpeed / startingSpeed, musicSpeedupPercent);
			Debug.Log("Audio pitch is currently" + audioSource.pitch.ToString());

            timePassedGame += Time.deltaTime;

            currentSpeed += acceleration * Time.deltaTime;

            if (ball != null)
                ball.GetComponent<BallController>().Move(Vector3.down * currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime);
            labyrinthInstance.Move(Vector3.down * currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime);
            currentDistance += currentSpeed / gridWidth * screenWidthUnits * Time.deltaTime;
            score += currentSpeed * Time.deltaTime * SCORE_CONSTANT;

            if (currentDistance >= generateDistance)
            {
                currentDistance -= generateDistance;
                labyrinthInstance.GenerateNextSector();
            }
            if (ball.GetComponent<BallController>().GetSquareLastTap().y < gameCamera.ScreenToWorldPoint(Vector3.zero).y)
            {
                Lose();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Resume();
            }
        }
    }

    void Lose()
    {
		audioSource.pitch = 1;
		Debug.Log("Game loss. Audio pitch is currently " + audioSource.pitch.ToString());

        menuScreen.GetComponent<GraphicRaycaster>().enabled = true;
        endScreen.GetComponent<GraphicRaycaster>().enabled = true;
        Destroy(ball);
        if (adTime <= 0)
        {
            ad.Show();
            adTime = adTimer;
        }
        else
            adTime--;
        endScreen.SetActive(true);
        isPlaying = false;
        opacityUI = 1f;
        gameCanvas.SetActive(false);

    }

	float HeightPixelsToUnits(int pixels)
	{
		return pixels * screenHeightUnits / (float)screenHeightPixels;
	}
}