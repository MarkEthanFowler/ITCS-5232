using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerCharacterInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
    public bool JumpDown;
}
public class MyCharacterController : MonoBehaviour, ICharacterController//interface for the character controller methods
{
    public KinematicCharacterMotor motor;

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15;
    public float OrientationSharpness = 10;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 10f;
    public float AirAccelerationSpeed = 5f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;
    public bool AllowDoubleJump = false;
    public float JumpSpeed = 10f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Misc")]
    public bool RotationObstruction;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;

    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;

    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;
    private bool _doubleJumpConsumed = false;

    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

        // Move and look inputs
        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = cameraPlanarDirection;

        // Jumping input
        if (inputs.JumpDown)
        {
            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }
    }

    void Start()
    {
        //assign the Character controller reference in the motor object
        motor.CharacterController = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This is called before the motor does anything
    public void BeforeCharacterUpdate(float deltaTime)
    {
        
    }

    // This is called when the motor wants to know what its rotation should be right now
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
        }
    }

    // This is called when the motor wants to know what its velocity should be right now
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 targetMovementVelocity = Vector3.zero;
        if (motor.GroundingStatus.IsStableOnGround)
        {
            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
            currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
        }
        else
        {
            // Add move input
            if (_moveInputVector.sqrMagnitude > 0f)
            {
                targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                // Prevent climbing on un-stable slopes with air movement
                if (motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
            }

            // Gravity
            currentVelocity += Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }

        // Handle jumping
        _jumpedThisFrame = false;
        _timeSinceJumpRequested += deltaTime;
        if (_jumpRequested)
        {
            // Handle double jump
            if (AllowDoubleJump)
            {
                if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !motor.GroundingStatus.FoundAnyGround : !motor.GroundingStatus.IsStableOnGround))
                {
                    motor.ForceUnground(0.1f);

                    // Add to the return velocity and reset jump state
                    currentVelocity += (motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                    _jumpRequested = false;
                    _doubleJumpConsumed = true;
                    _jumpedThisFrame = true;
                }
            }

            // See if we actually are allowed to jump
            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
            {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = motor.CharacterUp;
                if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                {
                    jumpDirection = motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                motor.ForceUnground(0.1f);

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                _jumpRequested = false;
                _jumpConsumed = true;
                _jumpedThisFrame = true;
            }
        }
    }

    // This is called after the motor has finished everything in its update
    public void AfterCharacterUpdate(float deltaTime)
    {
        // Handle jump-related values
        {
            // Handle jumping pre-ground grace period
            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
            {
                _jumpRequested = false;
            }

            if (AllowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
            {
                // If we're on a ground surface, reset jumping values
                if (!_jumpedThisFrame)
                {
                    _doubleJumpConsumed = false;
                    _jumpConsumed = false;
                }
                _timeSinceLastAbleToJump = 0f;
            }
            else
            {
                // Keep track of time since we were last able to jump (for grace period)
                _timeSinceLastAbleToJump += deltaTime;
            }
        }
    }

    // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
    public bool IsColliderValidForCollisions(Collider coll)
    {
        
        return true;
    }

    // This is called when the motor's ground probing detects a ground hit
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    // This is called when the motor's movement logic detects a hit
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    public void PostGroundingUpdate(float deltaTime)
    {
        
    }

    // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        
    }


}
