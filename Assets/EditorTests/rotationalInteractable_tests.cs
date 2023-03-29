using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests.Factories;
using UnityEngine;

namespace InteractableTests
{
    public class rotationalInteractable_tests
    {
        [Test]
        [TestCase(1f, 1f, 1f, 0f, 0f, 0f, 1.73f)]
        [TestCase(1f, 1f, 1f, 5f, 5f, 5f, 6.93f)]
        [TestCase(1f, 5f, 2f, 5f, 1f, 3f, 5.74f)]
        public void check_if_calculateRadius_returns_correct_value(float p_x, float p_y, float p_z, float r_x, float r_y, float r_z, float expectedResult)
        {
            RotationalInteractableHumble rotationalInteractable = RotationalInteractableFactory.ARotationalInteractable.Build();
            Vector3 pivotPos = new Vector3(p_x, p_y, p_z);
            Vector3 rotatorPos = new Vector3(r_x, r_y, r_z);

            float result = rotationalInteractable.CalculateRadius(pivotPos, rotatorPos);

            Assert.AreEqual(expectedResult.ToString("F2"), result.ToString("F2"), "Calculated raduis and expected radius are not equal");
        }

        [Test]
        [TestCase(1f, 0f, 0f, 150f, 0f)]
        [TestCase(0f, -1f, -90f, 150f, -90f)]
        [TestCase(-1f, 0f, 180f, 150f, 180f)]
        public void check_if_calculateCurrentAngle_returns_correct_value(float h_input, float v_input, float currentAngle, float rotationSpeed, float expectedResult)
        {
            RotationalInteractableHumble rotationalInteractable = RotationalInteractableFactory.ARotationalInteractable.Build();

            float result = rotationalInteractable.CalculateCurrentAngle(h_input, v_input, currentAngle, rotationSpeed);

            Assert.AreEqual(expectedResult.ToString("F2"), result.ToString("F2"), "Calculated angle and expected angle are not equal");
        }
    }
}
