using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalInteractableHumble : UpgradableHumble
{
    public RotationalInteractableHumble(bool isAIOnlyInteractable) : base(isAIOnlyInteractable)
    {

    }

    public float CalculateRadius(Vector3 currentPivotPos, Vector3 rotatorPos)
    {
        float radius = Vector3.Distance(currentPivotPos, rotatorPos);

        return radius;
    }

    public float CalculateCurrentAngle(float h_input, float v_input, float currentAngle, float rotationSpeed)
    {
        if (Mathf.Abs(h_input) > 0f || Mathf.Abs(v_input) > 0f)
        {
            float targetAngle = Mathf.Atan2(v_input, h_input) * Mathf.Rad2Deg;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        }

        return currentAngle;
    }

    public void CalculateRotationWASD(Vector3 currentPivotPos, Vector3 rotatorPos, float radius, float currentAngle, out Vector3 finalPos, out Quaternion finalRotation)
    {
        float x = currentPivotPos.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = currentPivotPos.y + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        float z = currentPivotPos.z;

        finalPos = new Vector3(x, y, z);
        finalRotation = Quaternion.LookRotation(Vector3.forward, currentPivotPos - rotatorPos);
    }
}
