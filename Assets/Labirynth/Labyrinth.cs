using UnityEngine;
using System.Collections;

public class Labyrinth {

	Square[,] squareArray;

	int width, height, sectorsNum, currentY = 0;
	float screenWidth;
	GameObject squarePrefab;
	Vector3 currentPosition;
	bool activated = false;
	bool first = true;

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
				result [i, j].Start ();
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
		bool res = Check (_from, _to);
		int x = 0, xMax = 0, y = 0, yMax = 0;
		if (_from.posX < _to.posX) {
			x = _from.posX;
			xMax = _to.posX;
		} else {
			xMax = _from.posX;
			x = _to.posX;
		}
		if (_from.posY < _to.posY) {
			y = _from.posY;
			yMax = _to.posY;
		} else {
			yMax = _from.posY;
			y = _to.posY;
		}
		for (int i = x; i <= xMax; i++) {
			for (int j = y; j <= yMax; j++) {
				float r = squareArray [j, i].GetComponent<SpriteRenderer> ().color.r;
				if (r <= 0.75)
					squareArray [j, i].GetComponent<SpriteRenderer> ().color = new Color (r + 0.25f, 0, 0);
			}
		}
		return res;
	}

	private bool Check(Square _from, Square _to)
	{
		if (_from == _to)
			return false;
		if (_from.posX == _to.posX) {
			bool b = true;
			if (_from.posY > _to.posY) {
				for (int i = _to.posY; i < _from.posY - 1; i++) {
					b = b && !squareArray [i, _from.posX].up && !squareArray [i+1, _from.posX].down;
				}
			} else {
				for (int i = _from.posY; i < _from.posY - 1; i++) {
					b = b && !squareArray [i, _from.posX].up && !squareArray [i+1, _from.posX].down;
				}
			}
			return b;
		}
		if (_from.posY == _to.posY) {
			bool b = true;
			if (_from.posX > _to.posX) {
				for (int i = _to.posX; i < _from.posX - 1; i++) {
					b = b && !squareArray [_from.posY, i].right  && !squareArray [_from.posY, i+1].left;
				}
			} else {
				for (int i = _from.posX; i < _from.posX - 1; i++) {
					b = b && !squareArray [_from.posY, i].right  && !squareArray [_from.posY, i+1].left;
				}
			}
			return b;
		}
		return false;
	}

	public void Move (Vector3 dir){
		foreach (Square s in squareArray) {
			s.transform.position += dir;
		}
	}
}
