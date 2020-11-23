using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	static PlayerController instance = null;
	public static Vector3 Position { get => instance.transform.position; }

	[Header("Parameters")]
	[SerializeField] float speed = 5.0f;
	[SerializeField] float angularSpeed = 5.0f;
	[SerializeField] float jumpForce = 25.0f;

	[Header("References")]
	[SerializeField] new Camera camera = null;

	new Rigidbody rigidbody = null;

	float t = 0.0f;

	private void Awake()
	{
		instance = this;

		rigidbody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		Vector3 direction = Vector3.zero;

		if (Input.GetKey(KeyCode.Z))
			direction += transform.forward;
		if (Input.GetKey(KeyCode.S))
			direction += -transform.forward;
		if (Input.GetKey(KeyCode.Q))
			direction += -transform.right;
		if (Input.GetKey(KeyCode.D))
			direction += transform.right;
		if (Input.GetKeyDown(KeyCode.Space))
			rigidbody.AddForce(Vector3.up * jumpForce * 10.0f);

		transform.position += direction.normalized * speed * Time.deltaTime;

		float axisX = Input.GetAxisRaw("Mouse X");
		transform.Rotate(Vector3.up, axisX * angularSpeed * 100.0f * Time.deltaTime);

		float axisY = -Input.GetAxisRaw("Mouse Y");
		t += axisY * angularSpeed * Time.deltaTime;
		t = Mathf.Clamp(t, 0.0f, 1.0f);
		camera.transform.rotation = transform.rotation * Quaternion.Lerp(Quaternion.LookRotation(Vector3.up, Vector3.back), Quaternion.LookRotation(Vector3.down, Vector3.forward), t);
	}
}
