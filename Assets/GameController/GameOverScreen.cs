using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
	public Text scoreText;
	public Text newRecordText;
	private int offset;
	RectTransform rect;

	void Start ()
	{
		rect = GetComponent <RectTransform> ();
	}

	void OnEnable ()
	{
		offset = 1920;

		scoreText.text = "YOUR SCORE IS\n" + GameController.Instance.GetScore();

		if (PrefsGetSavedRecord() < GameController.Instance.GetScore())
		{
			newRecordText.text = "NEW RECORD!";
			PrefsSaveRecord();
		}
		else newRecordText.text = " ";

		StartCoroutine(Coroutine1());
	}
		
	void PrefsSaveRecord ()
	{
		PlayerPrefs.SetInt("Highscore", GameController.Instance.GetScore());
	}

	int PrefsGetSavedRecord ()
	{
		return PlayerPrefs.GetInt("Highscore");
	}

	public IEnumerator Coroutine1 ()
	{
		while (offset > 0)
		{
			offset -= Mathf.Min(100, offset);
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(Coroutine2());
		yield return null;
	}

	public IEnumerator Coroutine2 ()
	{
		while (offset < 1000)
		{
			offset += 50;
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(Coroutine3());
		yield return null;
	}

	public IEnumerator Coroutine3 ()
	{
		while (offset > 0)
		{
			offset -= Mathf.Min(20, offset);
			yield return new WaitForEndOfFrame();
		}
		//StartCoroutine(Coroutine2()); De-comment for hillarious effect
		yield return null;
	}

	void Update ()
	{
		//GameController.Instance.GetPixelHeight()
		rect.offsetMin = new Vector2 (rect.offsetMin.x, offset);
		rect.offsetMax = new Vector2 (rect.offsetMin.x, offset);
	}
}
