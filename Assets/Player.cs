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
	private float speed = 1.0f;
	private float jumpHeight = 3.0f;
	
	private float turnSpeed = 1.0f;
	private float rollSpeed = 0.1f;
	private float vertMin = -90.0f;
	private float vertMax = 90.0f;
	
	private bool zeroG = false;
	private bool enableMagnet = true;
	private float idleTime = 0;
	private float gravAccel = 0;
	
	private CapsuleCollider capsuleCollider;
	private Rigidbody rigidBody;
	
	private bool OnGround
	{
		get
		{
			//TODO: Use capsuleCollider to determine if we're on the ground.
			return true;
		}
	}
	
	private bool OnWall
	{
		get
		{
			//TODO: Use capsuleCollider to determine if we're in contact with a wall.
			return true;
		}
	}
	
	private Vector3 Movement
	{
		get
		{
			var up = Input.GetAxis(InputNames.MoveVertical.ToString());
			if(!zeroG)
			{
				up = 0;
				if(Input.GetButtonDown(InputNames.Jump.ToString()))
					up += Mathf.Sqrt(jumpHeight * -gravity);
			}
			
			return new Vector3
			{
				x = Input.GetAxis(InputNames.MoveLateral.ToString()),
				y = up,
				z = Input.GetAxis(InputNames.MoveHorizontal.ToString())
			};
		}
	}
	
	private Vector3 Rotation
	{
		get
		{
			var rot = new Vector3
			{
				x = Input.GetAxis(InputNames.LookVertical.ToString()) * -turnSpeed,
				y = Input.GetAxis(InputNames.LookHorizontal.ToString()) * turnSpeed,
				z = zeroG ? Input.GetAxis(InputNames.LookLateral.ToString()) * rollSpeed : 0
			};
			
			enableMagnet = rot.magnitude <= 0.4f && rot.z == 0;
			return rot;
		}
	}
	
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		gravity = Physics.gravity.y;
		
		capsuleCollider = GetComponent<CapsuleCollider>();
		rigidBody = GetComponent<Rigidbody>();
		
		toggleZeroG(false);
	}
	
	private void FixedUpdate()
	{
		
		
		if(zeroG)
		{
			rigidBody.AddRelativeTorque(Rotation, ForceMode.VelocityChange);
			rigidBody.AddRelativeForce(Movement * speed, ForceMode.VelocityChange);
			magnetToRightAngle();
		}
		else
		{
			kinematicRotation();
			kinematicMovement();
			forceUpright();
		}
	}
	
	private void Update()
	{
		if(Input.GetButtonDown(InputNames.ToggleGravity.ToString()))
			toggleZeroG();
	}
	
	private void forceUpright()
	{
		var rot = transform.eulerAngles;
		
		if(!Mathf.Approximately(rot.z, 0.0f))
		{
			rot.z = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * turnSpeed);
		}
	}
	
	private void kinematicMovement()
	{
		var moveDirection = transform.TransformDirection(
			new Vector3(
				Input.GetAxis(InputNames.MoveLateral.ToString()) * speed,
				0,
				Input.GetAxis(InputNames.MoveHorizontal.ToString()) * speed
			)
		);
		
		if(OnWall)
		{
			//TODO: Handle wall interactions
		}
		
		if(OnGround)
		{
			//gravAccel = 0;
			if(Input.GetButtonDown("Jump"))
				gravAccel += Mathf.Sqrt(jumpHeight * -gravity);
		}
		gravAccel += gravity * Time.deltaTime;
		
		moveDirection.y = gravAccel;
		transform.position = Vector3.Slerp(transform.position, moveDirection, Time.deltaTime * speed);
	}
	
	private void kinematicRotation()
	{
		var hRot = Input.GetAxis("LookHorizontal") * turnSpeed;
		var vRot = Input.GetAxis("LookVertical") * -turnSpeed;
		
		transform.rotation = Quaternion.Slerp(
			transform.rotation,
			Quaternion.Euler(
				transform.rotation.x + vRot,
				Mathf.Clamp(transform.rotation.y + hRot, vertMin, vertMax),
				0
			),
			Time.deltaTime * turnSpeed
		);
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
		
		rigidBody.isKinematic = !zeroG;
		/*
		if(zeroG)
			rigidBody.constraints = RigidbodyConstraints.None;
		else
			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		*/
	}
}
