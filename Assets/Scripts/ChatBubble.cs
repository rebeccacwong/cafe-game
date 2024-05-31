using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private SpriteRenderer m_chatBubbleRenderer;
    private SpriteRenderer m_iconSpriteRenderer;
    private TextMeshPro m_textMeshPro;

    private static float flashColorTimeInSec = 30f;
    private static float delayInSecondsToStartColorChange = 60f;

    private Color finalColor = new Color(0.85f, 0.25f, 0.25f);

    private void Awake()
    {
        this.m_chatBubbleRenderer = transform.Find("ChatBubbleBackground").GetComponent<SpriteRenderer>();
        this.m_iconSpriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Changes color when waitTime is almost up
    public void StartVisualCountDown(float waitTimeInSeconds)
    {
        StartCoroutine(VisualCountDownCoroutine(waitTimeInSeconds));
    }

    // Starts changing color when there is only secondsRemainingToStartColorChange seconds
    private IEnumerator VisualCountDownCoroutine(float waitTimeInSeconds)
    {
        Debug.Assert(m_chatBubbleRenderer != null, "All ChatBubble objects should have sprite renderers for the bubble.");
        
        float colorFadeTime = waitTimeInSeconds - flashColorTimeInSec - delayInSecondsToStartColorChange;
        Debug.LogWarningFormat("[ChatBubble {0:X}] Starting coroutine. Will wait for {1} seconds, then start to " +
            "change color for {2} seconds, then flash in the last {3} flashColorTimeInSec time",
            gameObject.GetInstanceID(), delayInSecondsToStartColorChange, colorFadeTime, flashColorTimeInSec);

        // Wait for the delay
        yield return new WaitForSeconds(delayInSecondsToStartColorChange);

        // Start to change color from white to finalColor, updating frame every 1.5s.
        Debug.LogWarningFormat("[ChatBubble {0:X}] Beginning color fade.", gameObject.GetInstanceID());
        for (float time = 0f; time < colorFadeTime; time += 1.5f)
        {
            m_chatBubbleRenderer.color = Color.Lerp(Color.white, finalColor, time / colorFadeTime);
            yield return new WaitForSeconds(1.5f);
        }

        Debug.LogWarningFormat("[ChatBubble {0:X}] Beginning to flash", gameObject.GetInstanceID());
        for (float time = 0f; time < flashColorTimeInSec; time += 1.5f)
        {
            if (m_chatBubbleRenderer.color == Color.red)
            {
                m_chatBubbleRenderer.color = finalColor;
            }
            else
            {
                m_chatBubbleRenderer.color = Color.red;
            }
            yield return new WaitForSeconds(1.5f);
        }
        Debug.LogWarningFormat("[ChatBubble {0:X}] Time is up on visual countdown.", gameObject.GetInstanceID());
    }

    // Change the chat bubble's sprite to be SPRITE.
    public void updateSprite(Sprite sprite)
    {
        this.m_iconSpriteRenderer.sprite = sprite;
    }
}
