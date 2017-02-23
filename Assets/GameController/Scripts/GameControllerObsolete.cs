using UnityEngine;
using System.Collections;

public class GameControllerObsolete : MonoBehaviour
{
	public static GameControllerObsolete Instance;

	public Camera cam;
	private float cameraPixelW;
	private float cameraPixelH;
	private float sizeUnitsW;
	private float sizeUnitsH;

	private float ratio;

	void Awake ()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else 
		{
			Destroy (gameObject);
		}
	}
	void Start ()
	{
		cameraPixelW = cam.pixelWidth;
		cameraPixelH = cam.pixelHeight;

		Vector3 bottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
		Vector3 upperRight = cam.ScreenToWorldPoint(new Vector3(cameraPixelW, cameraPixelH, 0f));
		sizeUnitsW = upperRight.x - bottomLeft.x;
		sizeUnitsH = upperRight.y - bottomLeft.y;

		Debug.Log(cameraPixelW + "/n" + cameraPixelH + "/n" + sizeUnitsW + "/n" + sizeUnitsH);

		ratio = sizeUnitsW / 7;
		Debug.Log(ratio);

		cam.orthographicSize = cam.orthographicSize / ratio;

		cameraPixelW = cam.pixelWidth;
		cameraPixelH = cam.pixelHeight;

		Vector3 bottomLeft2 = cam.ScreenToWorldPoint(Vector3.zero);
		Vector3 upperRight2 = cam.ScreenToWorldPoint(new Vector3(cameraPixelW, cameraPixelH, 0f));
		sizeUnitsW = upperRight2.x - bottomLeft2.x;
		sizeUnitsH = upperRight2.y - bottomLeft2.y;

		Debug.Log(cameraPixelW + "/n" + cameraPixelH + "/n" + sizeUnitsW + "/n" + sizeUnitsH);
	}
	public void GameStart ()
	{
		//
	}
	public bool CheckWay (/* Insert two waypoints */)
	{
		bool hasWay = false;
		//
		return hasWay;
	}
	public string GetDebugText ()
	{
		return cameraPixelW + "\n" + cameraPixelH + "\n" + sizeUnitsW + "\n" + sizeUnitsH;
	}
}