using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {


	public bool up, right, down, left;

	private Transform upLine, downLine, rightLine, leftLine;

	/// <summary>
	/// Позиция y квадрата в лабиринте.
	/// </summary>
	public int posY;

	/// <summary>
	/// Позиция x квадрата в лабиринте.
	/// </summary>
	public int posX;

	public void Start(){
		up = right = down = left = true;
		upLine = transform.Find ("upLine");
		downLine = transform.Find ("downLine");
		rightLine = transform.Find ("rightLine");
		leftLine = transform.Find ("leftLine");
		upLine.transform.position = transform.position + Vector3.up * transform.localScale.x * 0.5f - Vector3.forward*0.1f;
		downLine.transform.position = transform.position + Vector3.down * transform.localScale.x * 0.5f - Vector3.forward*0.1f;
		rightLine.transform.position = transform.position + Vector3.right * transform.localScale.x * 0.5f - Vector3.forward*0.1f;
		leftLine.transform.position = transform.position + Vector3.left * transform.localScale.x * 0.5f - Vector3.forward*0.1f;
	}

	/// <summary>
	/// Устанавливает размер стороны квадрата в юнитах
	/// </summary>
	/// <param name="s">количество юнитов</param>
	public void SetSize(float s){
		transform.localScale = new Vector3 (s, s, 1);
		if (upLine) {
			upLine.transform.position = transform.position + Vector3.up * s * 0.5f;
		}
		if (downLine) {
			downLine.transform.position = transform.position + Vector3.down * s * 0.5f;
		}
		if (rightLine) {
			rightLine.transform.position = transform.position + Vector3.right * s * 0.5f;
		}
		if (leftLine) {
			leftLine.transform.position = transform.position + Vector3.left * s * 0.5f;
		}
	}


	/// <summary>
	/// Удаляет правую сторону квадрата
	/// </summary>
	public void RemoveRight()
	{
		if (right) {
			right = false;
			Destroy (rightLine.gameObject);
		}
	}

	/// <summary>
	/// Удаляет левую сторону квадрата
	/// </summary>
	public void RemoveLeft()
	{
		if (left) {
			left = false;
			Destroy (leftLine.gameObject);
		}
	}

	/// <summary>
	/// Удаляет верхнюю сторону квадрата
	/// </summary>
	public void RemoveUp()
	{
		if (up) {
			up = false;
			Destroy (upLine.gameObject);
		}
	}

	/// <summary>
	/// Удаляет нижнюю сторону квадрата
	/// </summary>
	public void RemoveDown()
	{
		if (down) {
			down = false;
			Destroy (downLine.gameObject);
		}
	}

}
