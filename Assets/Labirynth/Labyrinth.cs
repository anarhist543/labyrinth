using UnityEngine;
using System.Collections.Generic;

public class Labyrinth {

	Square[,] squareArray;

	int width, height, sectorsNum, currentY = 0;
	float screenWidth;
	GameObject squarePrefab;
	Vector3 currentPosition;
	bool activated = false;
	bool first = true;
	private int creationSector = -1;
	private GameObject creationObject;



	public void GenerateBall(int sectors, GameObject prefab)
	{
		creationSector = sectors;
		creationObject = prefab;
	}

    public Square GetSquare(int x, int y)
    {
        return squareArray[y, x];
    }

	/// <summary>
	/// Функция задающая параметры лабиринта
	/// </summary>
	/// <param name="w">кол-во квадратов по горизонтали</param>
	/// <param name="h">кол-во квадратов по вертикали(в секторе)</param>
	/// <param name="n">количество секторов</param>
	/// <param name="screenW">размер экрана по горизонтали(в юнитах)</param>
	/// <param name="squarePref">префаб квадрата</param>
	/// <param name="startPos">позиция где должен находиться левый нижний угол лабиринта</param>
	public void SetParams (int w, int h, int n, float screenW, GameObject squarePref, Vector3 startPos){
		if (!activated) {
			width = w;
			height = h;
			sectorsNum = n;
			screenWidth = screenW;
			squarePrefab = squarePref;
			squareArray = new Square[h*n, w];
			currentPosition = startPos + new Vector3 (0.5f, 0.5f, 0) * screenW / w;
		} else {
			Debug.LogError ("У лабиринта не заданны параметры(функция SetParams)");
		}
	}

	/// <summary>
	/// Генерирует следующий сектор и удаляет тот, сектор который был создан раньше всех
	/// </summary>
	public void GenerateNextSector ()
	{
		creationSector--;
		Square[,] sector = GenerateSector ();
		for (int i = currentY; i < currentY + height; i++) {
			for (int j = 0; j < width; j++) {
				if (squareArray [i, j] != null) {
					Object.Destroy (squareArray [i, j].gameObject);
				}
				squareArray [i, j] = sector [i - currentY, j];
				squareArray [i, j].posY = i;
				squareArray [i, j].posX = j;
			}
		}
		if (creationSector == 0) {
			GameObject created;
			int r = Random.Range (0, width);
			if (currentY == 0) {
				created = (GameObject)Object.Instantiate (creationObject, squareArray [height * sectorsNum - 1, r].transform.position + Vector3.back, Quaternion.identity);
				created.GetComponent<BallController> ().Init (squareArray [height * sectorsNum - 1, r]);
			} else {
				created = (GameObject)Object.Instantiate (creationObject, squareArray [currentY - 1, r].transform.position + Vector3.back, Quaternion.identity);
				created.GetComponent<BallController> ().Init (squareArray [currentY - 1, r]);
			}
			created.transform.localScale = Vector3.one * 0.8f * screenWidth / width;
		}
		currentY = currentY + height;
		if (currentY == height * sectorsNum)
			currentY = 0;
		
	}

	Square[,] GenerateSector()
	{
		if (currentY == 0) {
			if(squareArray [currentY, 0])
				currentPosition = squareArray [height * sectorsNum - 1, 0].transform.position + Vector3.up * screenWidth / width;
		} else if (squareArray [currentY, 0]) {
			currentPosition = squareArray [currentY-1, 0].transform.position + Vector3.up * screenWidth / width;
		}
		Square[,] result = new Square[height, width];
		for(int i = 0; i < height; i++){
			for (int j = 0; j < width; j++) {
				result [i, j] = Object.Instantiate (squarePrefab).GetComponent<Square> ();
				result [i, j].Init ();
				result [i, j].transform.localScale = Vector3.one * screenWidth / width;
				result [i, j].transform.position = currentPosition;
				currentPosition += Vector3.right * screenWidth / width;
			}
			currentPosition = result[i, 0].transform.position + Vector3.up * screenWidth / width;
		}
		int n = 0;
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				if (!first) {
					if (i < height - 1) {
						if (j < width - 1) {
							if (Random.Range (0, 2) == 0) {
								result [i, j].RemoveRight ();
								result [i, j + 1].RemoveLeft ();
								n++;
							} else {
								int k = Random.Range (j - n, j + 1);
								result [i, k].RemoveDown ();
								if (i > 0) {
									result [i - 1, k].RemoveUp ();
								}
								n = 0;
							}
						} else {
							int k1 = Random.Range (j - n, j + 1);
							result [i, k1].RemoveDown ();
							if (i > 0) {
								result [i - 1, k1].RemoveUp ();
							}
							n = 0;
						}
					} else {
						if (j < width - 1) {
							result [i, j].RemoveRight ();
							result [i, j + 1].RemoveLeft ();
						}
						result [i, j].RemoveUp ();
					}
				} else {
					if (j < width - 1) {
						result [i, j].RemoveRight ();
						result [i, j + 1].RemoveLeft ();
					}
				}
			}
			first = false;
		}
		int k2 = Random.Range (0, width);
		result [height - 2, k2].RemoveUp ();
		result [height - 1, k2].RemoveDown ();
		return result;
	}

	/// <summary>
	/// Проверяет можно ли безприпятственно пройти из квадрата _from в квадрат _to
	/// </summary>
	/// <returns><c>true</c> если можно<c>false</c> если нельзя</returns>
	public bool CanGo(Square _from, Square _to)
	{
        if (_from == _to)
            return false;
        if ((_from.posX != _to.posX && _to.posY != _from.posY))
        {
            return false;
        }
		bool b = true;
		List<Square> colored = new List<Square>();
		if (_from.posX == _to.posX) {
			int y = 0, yMax = 0;
			if (_from.transform.position.y < _to.transform.position.y) {
				y = _from.posY;
				yMax = _to.posY;
			} else {
				y = _to.posY;
				yMax = _from.posY;
			}
			while (y != yMax) {
				int dy = y + 1;
				if (dy == sectorsNum * height)
					dy = 0;
				b = b && !squareArray [y, _to.posX].up && !squareArray [dy, _to.posX].down;
				colored.Add (squareArray [y, _to.posX]);
				y = dy;
			}
			colored.Add(squareArray [yMax, _to.posX]);
			colored.Remove (squareArray [_to.posY, _to.posX]);
		}
		if (_to.posY == _from.posY) {
			int x = 0, xMax = 0;
			if (_from.posX < _to.posX) {
				x = _from.posX;
				xMax = _to.posX;
			} else {
				x = _to.posX;
				xMax = _from.posX;
			}
			for (int i = x; i < xMax; i++) {
				b = b && !squareArray [_to.posY, i].right && !squareArray [_to.posY, i + 1].left;
				colored.Add (squareArray [_to.posY, i]);
			}
			colored.Add(squareArray [_to.posY, xMax]);
			colored.Remove (squareArray [_to.posY, _to.posX]);
		}
		if (b) {
			foreach (Square s in colored) {
				s.GetComponent<SpriteRenderer> ().color -= new Color (0, 0.25f, 0.25f, 0f);
			}
		}
		return b;
	}


	public void Move (Vector3 dir){
		foreach (Square s in squareArray) {
			s.transform.position += dir;
		}
	}
}
