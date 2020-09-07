/*
 * File name : ProgressAnimation
 * This file has logic for loading screen circle
 */

using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;

    //Adjust rotating speed here
    private float rotateSpeed = 200f;

    // Instantiate game object
    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
    }

    // Logic to rotate circle over time
    private void Update()
    {
        rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}