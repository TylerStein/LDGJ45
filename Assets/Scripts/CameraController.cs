using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public Transform followTransform;
    [SerializeField] public float targetZDistance = 8f;
    [SerializeField] public float followDampening = 0.1f;

    [SerializeField] public Vector3 shakeOffset = Vector3.zero;

    [SerializeField] private Vector3 _moveVelocity = Vector3.zero;

    [SerializeField] private Coroutine activeShakeRoutine = null;
    [SerializeField] private float shakeAmplitude = 0f;
    [SerializeField] private float shakeFrequency = 0f;
    [SerializeField] private float shakeDuration = 0f;

    private void FixedUpdate() {

        Vector3 targetPosition = new Vector3(followTransform.position.x, followTransform.position.y, followTransform.position.z + targetZDistance);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _moveVelocity, followDampening * Time.deltaTime) + shakeOffset;
    }

    public void Shake(float amplitude = 0.15f, float frequency = 0.05f, float duration = 0.15f) {
        if (activeShakeRoutine != null) StopCoroutine(shakeRoutine());

        shakeOffset = Vector3.zero;
        shakeAmplitude = amplitude;
        shakeFrequency = frequency;
        shakeDuration = duration;

        activeShakeRoutine = StartCoroutine(shakeRoutine());
    }

    IEnumerator shakeRoutine() {
        float progress = 0f;
        float step = shakeFrequency / shakeDuration;
        while (progress < shakeDuration) {
            progress += step;
            shakeOffset = generateShakeOffset(shakeAmplitude);
            yield return new WaitForSeconds(step);
        }
        shakeOffset = Vector3.zero;
        activeShakeRoutine = null;
    }

    Vector3 generateShakeOffset(float amplitude = 0.15f) {
        return new Vector3(Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude), 0);
    }
}
