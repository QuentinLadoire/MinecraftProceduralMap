using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	static PlayerController instance = null;
	public static Vector3 Position { get => instance.transform.position; }
	public static Vector3 PreviousPosition { get => instance.previousPosition; }

	[Header("Parameters")]
	[SerializeField] float speed = 5.0f;
	[SerializeField] float angularSpeed = 5.0f;

	[Header("References")]
	[SerializeField] new Camera camera = null;

	//Camera rotation
	Quaternion lerpMax = Quaternion.identity;
	Quaternion lerpMin = Quaternion.identity;
	float lerpValue = 0.0f;

	Vector3 previousPosition = Vector3.zero;

	private void Awake()
	{
		instance = this;

		lerpMax = Quaternion.LookRotation(Vector3.up, Vector3.back);
		lerpMin = Quaternion.LookRotation(Vector3.down, Vector3.forward);
	}
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		previousPosition = transform.position;
	}
	private void Update()
	{
		previousPosition = transform.position;

		//Player movement
		Vector3 direction = Vector3.zero;

		if (Input.GetKey(KeyCode.Z))
			direction += transform.forward;
		if (Input.GetKey(KeyCode.S))
			direction += -transform.forward;
		if (Input.GetKey(KeyCode.Q))
			direction += -transform.right;
		if (Input.GetKey(KeyCode.D))
			direction += transform.right;
		if (Input.GetKey(KeyCode.Space))
			direction += transform.up;
		if (Input.GetKey(KeyCode.LeftShift))
			direction += -transform.up;

		transform.position += direction.normalized * speed * Time.deltaTime;

		float axisX = Input.GetAxisRaw("Mouse X");
		transform.Rotate(Vector3.up, axisX * angularSpeed * 100.0f * Time.deltaTime);

		//Camera rotation
		float axisY = -Input.GetAxisRaw("Mouse Y");
		lerpValue += axisY * angularSpeed * Time.deltaTime;
		lerpValue = Mathf.Clamp(lerpValue, 0.0f, 1.0f);
		camera.transform.rotation = transform.rotation * Quaternion.Lerp(lerpMax, lerpMin, lerpValue);
	}
}
