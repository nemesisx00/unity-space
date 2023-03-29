using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	float Gravity = -9.8f;
	float Speed { get; set; } = 5.0f;
	
	private float turnSpeed = 4.0f;
	private float verticalMinimum = -90.0f;
	private float verticalMaximum = 90.0f;
	
	private float verticalRotation = 0;
	
	private void Start()
	{
		
	}
	
	private void Update()
	{
		float horizontalRotation = Input.GetAxis("Mouse X") * turnSpeed;
		horizontalRotation += transform.eulerAngles.y;
		verticalRotation += Input.GetAxis("Mouse Y") * turnSpeed;
		verticalRotation = Mathf.Clamp(verticalRotation, verticalMinimum, verticalMaximum);
		transform.eulerAngles = new Vector3(-verticalRotation, horizontalRotation, 0);
		
		var direction = new Vector3(0, 0, 0);
		direction.x = Input.GetAxis("Horizontal");
		direction.z = Input.GetAxis("Vertical");
		transform.Translate(direction * Speed * Time.deltaTime);
	}
}
