using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private enum InputNames
	{
		Jump,
		LookHorizontal,
		LookLateral,
		LookVertical,
		MoveHorizontal,
		MoveLateral,
		MoveVertical,
		ToggleGravity,
	}
	
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
	
	private Vector3 proxyRotation = Vector3.zero;
	private float hRot = 0;
	private float vRot = 0;
	private bool zeroG = false;
	private bool enableMagnet = true;
	private float idleTime = 0;
	
	private CharacterController controller;
	private CapsuleCollider rigidCollider;
	private Rigidbody rigidBody;
	
	private Vector3 Movement
	{
		get
		{
			return new Vector3
			{
				x = Input.GetAxis(InputNames.MoveLateral.ToString()),
				y = Input.GetAxis(InputNames.MoveVertical.ToString()),
				z = Input.GetAxis(InputNames.MoveHorizontal.ToString())
			};
		}
	}
	
	private Vector3 Rotation
	{
		get
		{
			var vertAxis = Input.GetAxis(InputNames.LookVertical.ToString());
			var horizAxis = Input.GetAxis(InputNames.LookHorizontal.ToString());
			
			hRot += horizAxis * gTurnSpeed * 2;
			vRot = Mathf.Clamp(vRot + (vertAxis * -gTurnSpeed * 2), vertMin, vertMax);
			
			var rot = new Vector3
			{
				x = vertAxis * -turnSpeed,
				y = horizAxis * turnSpeed,
				z = Input.GetAxis(InputNames.LookLateral.ToString()) * rollSpeed
			};
			
			enableMagnet = zeroG && rot.magnitude <= 0.4f && rot.z == 0;
			return rot;
		}
	}
	
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		gravity = Physics.gravity.y;
		
		controller = GetComponent<CharacterController>();
		rigidCollider = GetComponent<CapsuleCollider>();
		rigidBody = GetComponent<Rigidbody>();
		
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
			rigidBody.AddRelativeTorque(Rotation, ForceMode.VelocityChange);
			rigidBody.AddRelativeForce(Movement * speed, ForceMode.VelocityChange);
			
			magnetToRightAngle();
		}
	}
	
	private void Update()
	{
		if(Input.GetButtonDown(InputNames.ToggleGravity.ToString()))
			toggleZeroG();
	}
	
	private void fullGravityMovement()
	{
		var moveDirection = transform.TransformDirection(
			new Vector3(
				Input.GetAxis(InputNames.MoveLateral.ToString()) * gSpeed,
				0,
				Input.GetAxis(InputNames.MoveHorizontal.ToString()) * gSpeed
			)
		);
		
		if(controller.isGrounded)
		{
			gravAccel = 0;
			
			if(Input.GetButtonDown(InputNames.Jump.ToString()))
				gravAccel += Mathf.Sqrt(jumpHeight * -gravity);
		}
		
		gravAccel += gravity * Time.deltaTime;
		
		moveDirection.y = gravAccel;
		controller.Move(moveDirection * Time.deltaTime);
	}
	
	private void fullGravityRotation()
	{
		hRot += Input.GetAxis(InputNames.LookHorizontal.ToString()) * gTurnSpeed;
		vRot = Mathf.Clamp(vRot + (Input.GetAxis(InputNames.LookVertical.ToString()) * -gTurnSpeed), vertMin, vertMax);
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(vRot, hRot, 0), Time.deltaTime * gTurnSpeed);
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
	}
}
