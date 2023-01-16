using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private SpriteRenderer m_chatBubbleRenderer;
    private SpriteRenderer m_iconSpriteRenderer;
    private TextMeshPro m_textMeshPro;

    private void Awake()
    {
        this.m_chatBubbleRenderer = transform.Find("ChatBubbleBackground").GetComponent<SpriteRenderer>();
        this.m_iconSpriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Change the chat bubble's sprite to be SPRITE.
    public void updateSprite(Sprite sprite)
    {
        this.m_iconSpriteRenderer.sprite = sprite;
    }
}
