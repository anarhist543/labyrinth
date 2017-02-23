using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float speed;
    public float delta;
    public Ray r;
    public Camera cam;
    public RaycastHit hit;
    public List<Square> squareList;

    void Start() {
        System.Random rnd = new System.Random();
        int rndNum = 35 + rnd.Next(0, 6);

        GameObject squareStart;
        squareStart = GameObject.Find("Square(Clone) (" + rndNum + ")");
        transform.position = squareStart.transform.position;
        squareList.Add(squareStart.GetComponent<Square>());
    }

    void Update() {

        InputController();
        
        //Чтобы двигался только по прямым
        while (squareList.Count > 1 && 
                    squareList[1].transform.position.x != squareList[0].transform.position.x &&
                    squareList[1].transform.position.y != squareList[0].transform.position.y) {
                squareList.RemoveAt(1);

            //если нажал не по прямой то произойдет удаление, чтоб цикл обращался списку с индесом один без ошибки
            if (squareList.Count < 1)
                squareList[1] = squareList[0];

        }

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

        /*
        //Тест
        for (int i = 0; i < squareList.Count; i++) Debug.Log(System.String.Format("{0}", squareList[i]));
        Debug.Log(System.String.Format("******************************************"));*/
    }

    //Метод нажатия на экран
    public void InputController() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            r = cam.ScreenPointToRay(Input.GetTouch(0).position);
            
            if (Physics.Raycast(r, out hit, 1000f)) {
                Square squareOne = hit.collider.GetComponent<Square>();
                Square squareTwo = squareList[squareList.Count - 1];
                if (true) {
                    squareList.Add(squareOne);
                }
            }
        }
    }

    public Square GetSquareLastTap(List<Square> squareList) {
        return squareList[squareList.Count - 1];
    }
}