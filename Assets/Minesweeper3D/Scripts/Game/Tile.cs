using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int x, y, z;
    public bool isMine = false, isRevealed = false;
    public GameObject minePrefab, textPrefab;
    public Gradient textGradient;

    [Range(0, 1)]
    public float mineChance = 0.15f;

    private Animator anim;
    private Collider col;
    private GameObject mine, text;
    private Color originalColor;
    public Color flagColor;
    public Renderer rend;
    public bool isFlagged = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();

    }
    GameObject SpawnChild(GameObject prefab)
    {
        GameObject child = Instantiate(prefab, transform);
        child.transform.localPosition = Vector3.zero;

        child.SetActive(false);
        return child;
    }
    private void Start()
    {
        originalColor = rend.material.color;
        isMine = Random.value < mineChance;

        if (isMine)
        {
            mine = SpawnChild(minePrefab);
        }
        else
        {
            text = SpawnChild(textPrefab);
        }
    }
    public void Reveal(int adjacentMines = 0)
    {
        isRevealed = true;
        anim.SetTrigger("Reveal");
        col.enabled = false;
        if (isMine)
        {
            mine.SetActive(true);
        }
        else
        {
            if(adjacentMines > 0)
            {
                text.SetActive(true);
                TextMeshPro tmp = text.GetComponent<TextMeshPro>();
                float step = adjacentMines / 9f;
                tmp.color = textGradient.Evaluate(step);

                tmp.text = adjacentMines.ToString();
            }
        }
    }
    public void Flag()
    {
        isFlagged = !isFlagged;
        rend.material.color = isFlagged ? flagColor : originalColor;
    }
}
