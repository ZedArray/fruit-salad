using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static Transform cameraTransform;
    private static Vector3 _orignalPosOfCam;
    public float shakeFrequency;
    public bool onMainCamera;
    private static float timer;
    private static float dur;
    public static bool shaking;

    public static CameraShake main;

    private void Awake()
    {
        if (onMainCamera) {
            main = this;
        }
    }
    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    public static void shake(float duration)
    {
        if (shaking)
        {
            dur += duration / 2;
        }
        else
        {
            dur = duration;
            setpos();
            shaking = true;

        }
    }
    private void Update()
    {
        if (shaking)
        {
            timer += Time.deltaTime;
            ShakeM();
            if (timer >= dur)
            {
                StopShake();
                timer = 0;

            }
        }


    }
    private static void setpos()
    {
        cameraTransform = Camera.main.transform;
        _orignalPosOfCam = cameraTransform.position;
    }
    private void ShakeM()
    {

        cameraTransform.position = _orignalPosOfCam + Random.insideUnitSphere * shakeFrequency;
    }

    private void StopShake()
    {

        cameraTransform.position = _orignalPosOfCam;
        shaking = false;
    }
}