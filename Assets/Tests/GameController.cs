using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public Camera cam;
	public float time;
	private float currentDistance = 0;
	public int w;
	public int h;
	public int n;
	public GameObject prefab;
	public Labyrinth labyrinth;
	public float speed;
	private float generateDistance;

	void Start ()
	{
		labyrinth = new Labyrinth ();
		int pixW = cam.pixelWidth;
		int pixH = cam.pixelHeight;
		float unitW = -cam.ScreenToWorldPoint(Vector3.zero).x + cam.ScreenToWorldPoint(new Vector3(pixW, pixH, 0f)).x;
		generateDistance = unitW / w * h;
		labyrinth.SetParams (w, h, n, unitW, prefab, cam.ScreenToWorldPoint(Vector3.zero) + Vector3.forward * 100); 
		for (int i = 0; i < n; i++) {
			labyrinth.GenerateNextSector ();
		}
	}

	void Update ()
	{
		labyrinth.Move (Vector3.down * speed * Time.deltaTime);
		currentDistance += speed * Time.deltaTime;
		if (currentDistance >= generateDistance) {
			currentDistance -= generateDistance;
			labyrinth.GenerateNextSector ();
		}
	}
}