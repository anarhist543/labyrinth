using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public float speed;
	public Camera cam;
    Ray r;
    RaycastHit hit;
    List<Vector3> squareList;
    Square squareOne, squareTwo;
    Vector3 lastTap;

	public void Init(Square s){
		GameController.Instance.GetBall (gameObject);
		squareList = new List<Vector3> ();
		squareList.Add (s.transform.position);
		cam = GameController.Instance.gameCamera;
        squareTwo = s;
	}

    public void Move(Vector3 dir)
    {
        for(int i = 0; i<squareList.Count; i++)
        {
            squareList[i] += dir;
        }
        transform.position += dir;
    }

    void Update() {
        if(!GameController.Instance.paused && GameController.Instance.started)
            InputController();

        //Само движение кружка
        if (!GameController.Instance.paused)
        {
            if ((Mathf.Abs(squareList[0].x - transform.position.x) > speed * Time.deltaTime) ||
                    ((Mathf.Abs(squareList[0].y - transform.position.y) > speed * Time.deltaTime)))
            {

                if (squareList[0].x - transform.position.x > 0)
                    transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);

                if (squareList[0].x - transform.position.x < 0)
                    transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);

                if (squareList[0].y - transform.position.y > 0)
                    transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);

                if (squareList[0].y - transform.position.y < 0)
                    transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(squareList[0].x, squareList[0].y, transform.position.z);
                if (squareList.Count > 1)
                    squareList.RemoveAt(0);
            }
        }
    }

    //Метод нажатия на экран
    public void InputController()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            r = cam.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(r, out hit, 1000f))
            {
                squareOne = hit.collider.GetComponent<Square>();
                //squareTwo - текущая позиция шарика
                if ((Mathf.Abs(squareOne.posX - squareTwo.posX) == 1 || Mathf.Abs(squareOne.posX - squareTwo.posX) == GameController.Instance.startingGenSectors * GameController.Instance.gridHeight-1) && (Mathf.Abs(squareOne.posY - squareTwo.posY) == 1 || Mathf.Abs(squareOne.posY - squareTwo.posY) == GameController.Instance.startingGenSectors * GameController.Instance.gridHeight - 1))
                {
                    if (GameController.Instance.CanGo(GameController.Instance.labyrinthInstance.GetSquare(squareOne.posX, squareTwo.posY), squareTwo) && GameController.Instance.CanGo(squareOne, GameController.Instance.labyrinthInstance.GetSquare(squareOne.posX, squareTwo.posY)))
                    {
                        squareList.Add(GameController.Instance.labyrinthInstance.GetSquare(squareOne.posX, squareTwo.posY).transform.position);
                        squareList.Add(squareOne.transform.position);
                        squareTwo = squareOne;
                    }
                    else if (GameController.Instance.CanGo(GameController.Instance.labyrinthInstance.GetSquare(squareTwo.posX, squareOne.posY), squareTwo) && GameController.Instance.CanGo(squareOne, GameController.Instance.labyrinthInstance.GetSquare(squareTwo.posX, squareOne.posY)))
                    {
                        squareList.Add(GameController.Instance.labyrinthInstance.GetSquare(squareTwo.posX, squareOne.posY).transform.position);
                        squareList.Add(squareOne.transform.position);
                        squareTwo = squareOne;
                    }

                }
                else
                {
                    if (GameController.Instance.CanGo(squareOne, squareTwo))
                    {
                        squareList.Add(squareOne.transform.position);
                        squareTwo = squareOne;
                    }
                }
            }
        }
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                r = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out hit, 1000f))
                {
                    squareOne = hit.collider.GetComponent<Square>();
                    if (GameController.Instance.CanGo(squareOne, squareTwo))
                    {
                        squareList.Add(squareOne.transform.position);
                        squareTwo = squareOne;
                    }
                }
            }
#endif
        if (squareTwo != null)
        {
            lastTap = squareTwo.transform.position;
        }
    }

    public Vector3 GetSquareLastTap() {
        return lastTap;
    }
}