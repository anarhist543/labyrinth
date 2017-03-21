using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public float speed;
	public Camera cam;
    Ray r;
    RaycastHit hit;
    List<Square> squareList;

	public void Init(Square s){
		GameController.Instance.GetBall (gameObject);
		squareList = new List<Square> ();
		squareList.Add (s);
		cam = GameController.Instance.gameCamera;
	}

    void Update() {
        if(!GameController.Instance.paused)
            InputController();

        //Само движение кружка
        if ((Mathf.Abs(squareList[0].transform.position.x - transform.position.x) > speed * Time.deltaTime) ||
                ((Mathf.Abs(squareList[0].transform.position.y - transform.position.y) > speed * Time.deltaTime))) {

            if (squareList[0].transform.position.x - transform.position.x > 0)
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);

            if (squareList[0].transform.position.x - transform.position.x < 0)
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);

            if (squareList[0].transform.position.y - transform.position.y > 0)
                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);

            if (squareList[0].transform.position.y - transform.position.y < 0)
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
        } else {
            transform.position = new Vector3(squareList[0].transform.position.x, squareList[0].transform.position.y, transform.position.z);
            if (squareList.Count > 1)
                squareList.RemoveAt(0);
        }
    }

    //Метод нажатия на экран
    public void InputController() {
		if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Ended)) {
			r = cam.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(r, out hit, 1000f)) {
                Square squareOne = hit.collider.GetComponent<Square>();
                Square squareTwo = squareList[squareList.Count - 1];
				if (GameController.Instance.CanGo(squareOne, squareTwo)) {
                    squareList.Add(squareOne);
                }
            }
        }
		#if UNITY_EDITOR
		if(Input.GetMouseButton(0)) {
			r = cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(r, out hit, 1000f)) {
				Square squareOne = hit.collider.GetComponent<Square>();
				Square squareTwo = squareList[squareList.Count - 1];
				if (GameController.Instance.CanGo(squareOne, squareTwo)) {
					squareList.Add(squareOne);
				}
			}
		}
		#endif
    }

    public Square GetSquareLastTap() {
        return squareList[squareList.Count - 1];
    }
}