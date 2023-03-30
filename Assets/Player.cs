using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private float gravity = -9.8f;
	private float gravAccel = 0;
	private float speed = 1.0f;
	private float gSpeed = 5.0f;
	private float jumpHeight = 3.0f;
	
	private float turnSpeed = 1.0f;
	private float gTurnSpeed = 5.0f;
	private float rollSpeed = 0.1f;
	private float vertMin = -90.0f;
	private float vertMax = 90.0f;
	
	private float horizontalRotation = 0;
	private float verticalRotation = 0;
	private bool zeroG = false;
	private bool enableMagnet = true;
	private float idleTime = 0;
	
	private CharacterController controller;
	private CapsuleCollider rigidCollider;
	private Rigidbody rigidBody;
	private Vector3 rotationProxy;
	
	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		rigidCollider = GetComponent<CapsuleCollider>();
		rigidBody = GetComponent<Rigidbody>();
		gravity = Physics.gravity.y;
		
		toggleZeroG(false);
	}
	
	private void FixedUpdate()
	{
		if(!zeroG)
		{
			fullGravityRotation();
			fullGravityMovement();
		}
		else
		{
			var rotation = Rotation;
			var movement = Movement;
			
			if(rotation != Vector3.zero)
				rigidBody.AddRelativeTorque(rotation, ForceMode.VelocityChange);
			if(Movement != Vector3.zero)
				rigidBody.AddRelativeForce(movement * speed, ForceMode.VelocityChange);
		}
	}
	
	private void Update()
	{
		if(Input.GetButtonDown("ToggleGravity"))
			toggleZeroG();
		
		if(zeroG)
			magnetToRightAngle();
	}
	
	private Vector3 Movement
	{
		get
		{
			return new Vector3
			{
				x = Input.GetAxis("MoveLateral"),
				y = zeroG ? Input.GetAxis("MoveVertical") : 0,
				z = Input.GetAxis("MoveHorizontal")
			};
		}
	}
	
	private Vector3 Rotation
	{
		get
		{
			var rotation = new Vector3
			{
				x = Input.GetAxis("LookVertical") * -turnSpeed,
				y = Input.GetAxis("LookHorizontal") * turnSpeed,
				z = Input.GetAxis("LookLateral") * rollSpeed
			};
			
			enableMagnet = zeroG && rotation.magnitude <= 0.4f && rotation.z == 0;
			return rotation;
		}
	}
	
	private void fullGravityMovement()
	{
		var moveDirection = controller.transform.TransformDirection(
			new Vector3(
				Input.GetAxis("MoveLateral") * gSpeed,
				0,
				Input.GetAxis("MoveHorizontal") * gSpeed
			)
		);
		
		if(controller.isGrounded)
			gravAccel = 0;
		if(Input.GetButtonDown("Jump") && controller.isGrounded)
			gravAccel += Mathf.Sqrt(jumpHeight * -gravity);
		gravAccel += gravity * Time.deltaTime;
		
		moveDirection.y = gravAccel;
		if(moveDirection != Vector3.zero)
			controller.Move(moveDirection * Time.deltaTime);
	}
	
	private void fullGravityRotation()
	{
		horizontalRotation += Input.GetAxis("LookHorizontal") * gTurnSpeed;
		verticalRotation += Input.GetAxis("LookVertical") * gTurnSpeed;
		verticalRotation = Mathf.Clamp(verticalRotation, vertMin, vertMax);
		
		controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, Quaternion.Euler(-verticalRotation, horizontalRotation, 0), Time.deltaTime * gSpeed);
	}
	
	private void magnetToRightAngle()
	{
		if(enableMagnet)
		{
			if(idleTime > 1)
			{
				var roundedRotation = transform.eulerAngles;
				roundedRotation.z = Mathf.Round(roundedRotation.z / 90) * 90;
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(roundedRotation), Time.deltaTime);
			}
			else
				idleTime += Time.deltaTime;
		}
		else
			idleTime = 0;
	}
	
	private void toggleZeroG(bool? force = null)
	{
		zeroG = force != null ? (bool)force : !zeroG;
		
		controller.enabled = !zeroG;
		rigidCollider.enabled = zeroG;
	}
}
