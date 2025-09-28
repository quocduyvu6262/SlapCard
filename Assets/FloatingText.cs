using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeDuration = 1.5f;
    private TextMeshProUGUI textMesh;
    private Color startColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        startColor = textMesh.color;
    }

    void Start()
    {
        // Start the animation coroutine
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Move text up
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // Fade out the text color
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Destroy the object after the animation is complete
        Destroy(gameObject);
    }
}