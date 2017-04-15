using UnityEngine;

public class MenuFadeOut : MonoBehaviour {

    bool b;
    // Update is called once per frame
    void Update() {
        if (GameController.Instance.GetUIOpacity() <= 0.01f)
        {
            gameObject.SetActive(false);
        }
		foreach (Transform child in transform)
		{
			if (child.GetComponent<CanvasRenderer>() != null) child.GetComponent<CanvasRenderer>().SetAlpha(GameController.Instance.GetUIOpacity());
			foreach (Transform child2 in child)
			{
				if (child2.GetComponent<CanvasRenderer>() != null) child2.GetComponent<CanvasRenderer>().SetAlpha(GameController.Instance.GetUIOpacity());
                foreach (Transform child3 in child2)
                {
                    if (child3.GetComponent<CanvasRenderer>() != null) child3.GetComponent<CanvasRenderer>().SetAlpha(GameController.Instance.GetUIOpacity());
                }
            }
		}

		
	}
}
