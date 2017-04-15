using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
	public bool LimitBounces = true;
	public Text scoreText;
	public Text highScoreText;
	public Text newRecordText;
	public GameObject ratePanel;
	private int offset;
	private int bounceTimes;
	private int loseCount = 0;
	public int bounceLimit = 1;
	RectTransform rect;
	public RectTransform RateUsRect;
    public GameObject rateButton;

	static int UI_HEIGHT = 1920;
	static int GAMES_TO_RATE = 5;

	void Awake ()
	{
		rect = GetComponent <RectTransform> ();
	}

	void OnDisable ()
	{
		ratePanel.SetActive(false);
	}

	void OnEnable ()
	{
        if(PlayerPrefs.GetInt("RateTimer", -100000) == -100000)
        {
            PlayerPrefs.SetInt("RateTimer", GAMES_TO_RATE);
        }
        Debug.Log(PlayerPrefs.GetInt("Rated"));
        if (PlayerPrefs.GetInt("RateTimer") <= 0 && PlayerPrefs.GetInt("Rated") != 1)
        {
            ratePanel.SetActive(true);
            PlayerPrefs.SetInt("RateTimer", GAMES_TO_RATE);
        }
        else
            PlayerPrefs.SetInt("RateTimer", PlayerPrefs.GetInt("RateTimer") - 1);
		
		RateUsRect.anchoredPosition = new Vector2 (0,RateUsRect.anchoredPosition.y); //reset the x position

		offset = UI_HEIGHT;
		bounceTimes = 0;

		scoreText.text = "YOUR SCORE\n" + GameController.Instance.GetScore();

		highScoreText.text = "RECORD\n" + PrefsGetSavedRecord();

		if (PrefsGetSavedRecord() < GameController.Instance.GetScore())
		{
			newRecordText.text = "NEW RECORD!";
			PrefsSaveRecord();
		}
		else newRecordText.text = " ";

        rect.offsetMin = new Vector2(rect.offsetMin.x, offset);
        rect.offsetMax = new Vector2(rect.offsetMin.x, offset);

		loseCount++;

        StartCoroutine(Coroutine1());
	}

	/// <summary>
	/// Функция, записывающая очки на момент вызова в память телефона
	/// </summary>
	void PrefsSaveRecord ()
	{
		if (GameController.Instance.GetScore() > PrefsGetSavedRecord()) // Добавлено для безопасности. Не нужно перезаписывать рекорд, если он меньше.
		{
		    PlayerPrefs.SetInt("Highscore", GameController.Instance.GetScore());
		}
	}

	/// <summary>
	/// Функция, возвращающая записанное в памяти значение рекорда
	/// </summary>
	int PrefsGetSavedRecord ()
	{
		return PlayerPrefs.GetInt("Highscore");
	}

	public void OpenMarket ()
	{
		Debug.Log("Opened");
		ratePanel.SetActive(false);
		PlayerPrefs.SetInt("Rated", 1);
		Application.OpenURL("market://details?id=com.gameloft.android.ANMP.GloftPOHM");
	}

	public void DontOpenMarket ()
	{
		Debug.Log("Not Opened");
		StartCoroutine(RateUsSlideOff());
	}

	public IEnumerator RateUsSlideOff ()
	{
		while (RateUsRect.anchoredPosition.x < 1000)
		{
			RateUsRect.anchoredPosition += new Vector2 (1400*Time.deltaTime,0);
			yield return new WaitForEndOfFrame();
		}
		ratePanel.SetActive(false);
		yield return null;
	}

	public IEnumerator Coroutine1 ()
	{
		int speed = 0;

		while (offset > 0)
		{
			speed += 15;
			offset -= Mathf.Min(speed, offset);
			yield return new WaitForEndOfFrame();
		}
        if (speed > 10) StartCoroutine(Coroutine2(speed / 2));
		yield return null;
	}

	public IEnumerator Coroutine2 (int speed)
	{
		do
		{
			speed -= 15;
			offset += offset + speed < 0 ? -offset : speed;
			yield return new WaitForEndOfFrame();
		}
		while (offset > 0);
		bounceTimes++;
        if ((-speed - 15 > 0) & (LimitBounces ? (bounceTimes <= bounceLimit) : true)) StartCoroutine(Coroutine2(-speed - 25));
		yield return null;
	}

	void Update ()
    {
        rect.offsetMin = new Vector2(rect.offsetMin.x, offset);
        rect.offsetMax = new Vector2(rect.offsetMin.x, offset);
        // Данный кусок кода отвечает за сдвиг UI, каждый кадр устанавливая его на значение "offset"
    }
}
