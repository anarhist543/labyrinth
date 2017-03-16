using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
	public Text scoreText;
	public Text highScoreText;
	public Text newRecordText;
	private int offset;
	RectTransform rect;

	static int UI_HEIGHT = 1920;

	void Awake ()
	{
		rect = GetComponent <RectTransform> ();
	}

	void OnEnable ()
	{
		offset = UI_HEIGHT;

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

	public IEnumerator Coroutine1 ()
	{
		int speed = 0;

		while (offset > 0)
		{
			speed += 5;
			offset -= Mathf.Min(speed, offset);
			yield return new WaitForEndOfFrame();
		}
		if (speed-35 > 0) StartCoroutine(Coroutine2(speed-35));
		yield return null;
	}

	public IEnumerator Coroutine2 (int speed)
	{
		do
		{
			speed -= 5;
			offset += speed;
			yield return new WaitForEndOfFrame();
		}
		while (offset > 0);
		if (-speed - 15 > 0) StartCoroutine(Coroutine2(-speed - 15));
		yield return null;
	}

	void Update ()
    {
        rect.offsetMin = new Vector2(rect.offsetMin.x, offset);
        rect.offsetMax = new Vector2(rect.offsetMin.x, offset);
        // Данный кусок кода отвечает за сдвиг UI, каждый кадр устанавливая его на значение "offset"
    }
}
