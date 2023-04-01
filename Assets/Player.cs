using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
	
	[Header("Kinematic")]
	[SerializeField]
	private float kinematicSpeed = 5.0f;
	[SerializeField]
	private float kinematicTurnSpeed = 5.0f;
	
	[Header("Movement")]
	[SerializeField]
	private float speed = 1.0f;
	[SerializeField]
	private float jumpHeight = 3.0f;
	
	[Header("Rotation")]
	[SerializeField]
	private float turnSpeed = 1.0f;
	[SerializeField]
	private float rollSpeed = 0.1f;
	[SerializeField]
	private float vertMin = -90.0f;
	[SerializeField]
	private float vertMax = 90.0f;
	
	private float gravity = -9.8f;
	private bool zeroG = false;
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
	
	private Vector3 FreeMovement
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
	
	private Vector3 FreeRotation
	{
		get
		{
			var rot = new Vector3
			{
				x = Input.GetAxis(InputNames.LookVertical.ToString()) * -turnSpeed,
				y = Input.GetAxis(InputNames.LookHorizontal.ToString()) * turnSpeed,
				z = Input.GetAxis(InputNames.LookLateral.ToString()) * rollSpeed
			};
			
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
			rigidBody.AddRelativeTorque(FreeRotation, ForceMode.VelocityChange);
			rigidBody.AddRelativeForce(FreeMovement * speed, ForceMode.VelocityChange);
		}
		else
		{
			kinematicRotation();
			kinematicMovement();
		}
	}
	
	private void Update()
	{
		if(Input.GetButtonDown(InputNames.ToggleGravity.ToString()))
			toggleZeroG();
	}
	
	private void kinematicMovement()
	{
		var moveDirection = rigidBody.transform.TransformDirection(
			new Vector3(
				Input.GetAxis(InputNames.MoveLateral.ToString()) * kinematicSpeed,
				0,
				Input.GetAxis(InputNames.MoveHorizontal.ToString()) * kinematicSpeed
			)
		);
		
		if(OnWall)
		{
			//TODO: Handle wall interactions
		}
		
		if(OnGround)
		{
			gravAccel = 0;
			if(Input.GetButtonDown("Jump"))
				gravAccel += Mathf.Sqrt(jumpHeight * -gravity);
		}
		else
			gravAccel += gravity * Time.fixedDeltaTime;
		
		moveDirection.y = gravAccel;
		rigidBody.MovePosition(rigidBody.position + moveDirection * Time.fixedDeltaTime);
	}
	
	private void kinematicRotation()
	{
		var angles = rigidBody.transform.eulerAngles;
		var desired = new Vector3
		{
			x = angles.x - Input.GetAxis(InputNames.LookVertical.ToString()) * kinematicTurnSpeed,
			y = angles.y + Input.GetAxis(InputNames.LookHorizontal.ToString()) * kinematicTurnSpeed,
			z = 0
		};
		
		rigidBody.MoveRotation(Quaternion.Euler(desired));
	}
	
	private void toggleZeroG(bool? force = null)
	{
		zeroG = force != null ? (bool)force : !zeroG;
		
		rigidBody.isKinematic = !zeroG;
	}
}
