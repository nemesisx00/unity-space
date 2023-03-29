using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private float gravity = -9.8f;
	private float gravAccel = 0;
	private float speed = 1.0f;
	private float jumpHeight = 1.0f;
	
	private float turnSpeed = 1.0f;
	private float rollSpeed = 0.1f;
	
	private bool zeroG = true;
	private bool enableMagnet = true;
	private float idleTime = 0;
	
	private Rigidbody rigidBody;
	
	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		toggleZeroG();
	}
	
	private void FixedUpdate()
	{
		if(Rotation != Vector3.zero)
			rigidBody.AddRelativeTorque(Rotation, ForceMode.VelocityChange);
		if(Movement != Vector3.zero)
			rigidBody.AddRelativeForce(Movement * speed, ForceMode.VelocityChange);
		
		if(!zeroG)
		{
			var leveled = transform.eulerAngles;
			leveled.z = Mathf.Round(leveled.z / 90) * 90;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(leveled), Time.deltaTime);
		}
	}
	
	private void Update()
	{
		if(Input.GetButtonDown("ToggleGravity"))
			toggleZeroG();
		
		if(zeroG)
		{
			magnetToRightAngle();
		}
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
				z = zeroG ? Input.GetAxis("LookLateral") * rollSpeed : 0
			};
			
			enableMagnet = zeroG && rotation.magnitude <= 0.4f && rotation.z == 0;
			return rotation;
		}
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
	
	private void toggleZeroG()
	{
		zeroG = !zeroG;
		
		if(zeroG)
		{
			rigidBody.constraints = RigidbodyConstraints.None;
			rigidBody.useGravity = false;
		}
		else
		{
			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rigidBody.useGravity = true;
		}
	}
	
	//CharacterController version
	/*
	private CharacterController controller;
	
	private void Start()
	{
		controller = GetComponent<CharacterController>();
		gravity = Physics.gravity.y;
	}
	
	private void Update()
	{
		if(Input.GetButtonDown("ToggleGravity"))
			zeroG = !zeroG;
		
		if(zeroG)
		{
			zeroGravityRotation();
			zeroGravityMovement();
		}
		else
		{
			fullGravityRotation();
			fullGravityMovement();
		}
	}
	
	private void fullGravityMovement()
	{
		var moveDirection = controller.transform.TransformDirection(
			new Vector3(
				Input.GetAxis("MoveLateral") * speed,
				0,
				Input.GetAxis("MoveHorizontal") * speed
			)
		);
		
		if(controller.isGrounded)
			gravAccel = 0;
		if(Input.GetButtonDown("Jump") && controller.isGrounded)
			gravAccel += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
		gravAccel += gravity * Time.deltaTime;
		
		moveDirection.y = gravAccel;
		if(moveDirection != Vector3.zero)
			controller.Move(moveDirection * Time.deltaTime);
	}
	
	private void fullGravityRotation()
	{
		float horizontalRotation = Input.GetAxis("LookHorizontal") * turnSpeed;
		horizontalRotation += controller.transform.eulerAngles.y;
		verticalRotation += Input.GetAxis("LookVertical") * turnSpeed;
		verticalRotation = Mathf.Clamp(verticalRotation, verticalMinimum, verticalMaximum);
		
		controller.transform.eulerAngles = new Vector3(-verticalRotation, horizontalRotation, 0);
	}
	
	private void zeroGravityMovement()
	{
		var moveDirection = controller.transform.TransformDirection(
			new Vector3(
				Input.GetAxis("MoveLateral"),
				Input.GetAxis("MoveVertical"),
				Input.GetAxis("MoveHorizontal")
			)
		);
		
		if(moveDirection != Vector3.zero)
			controller.Move(moveDirection * Time.deltaTime * speed);
	}
	
	private void zeroGravityRotation()
	{
		lateralRotation += Input.GetAxis("LookLateral") * rollSpeed;
		float horizontalRotation = Input.GetAxis("LookHorizontal") * turnSpeed;
		horizontalRotation += controller.transform.eulerAngles.y;
		verticalRotation += Input.GetAxis("LookVertical") * turnSpeed;
		controller.transform.eulerAngles = new Vector3(-verticalRotation, horizontalRotation, lateralRotation);
	}
	*/
}
