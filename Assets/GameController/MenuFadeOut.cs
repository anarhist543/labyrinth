using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFadeOut : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		foreach (Transform child in transform)
		{
			if (child.GetComponent<CanvasRenderer>() != null) child.GetComponent<CanvasRenderer>().SetAlpha(GameController.Instance.GetUIOpacity());
			foreach (Transform child2 in child)
			{
				if (child2.GetComponent<CanvasRenderer>() != null) child2.GetComponent<CanvasRenderer>().SetAlpha(GameController.Instance.GetUIOpacity());
			}
		}


		
	}
}
