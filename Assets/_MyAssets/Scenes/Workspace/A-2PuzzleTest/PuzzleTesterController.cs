using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class PuzzleFloor
{
    public Sprite[] sprites;
    public int currentIndex;
    public int answerIndex;
    public Image image;
    public TMP_Text displayText;
}

public class PuzzleTesterController : MonoBehaviour
{
    public GameObject displayText;
    public PuzzleFloor[] floors;
    private readonly char[] _alphabet = "ABCD".ToCharArray();

    private void Awake()
    {
        OnClickShuffleButton();
    }

    public void OnClickShuffleButton()
    {
        for (int floorNum = 1; floorNum <= floors.Length; floorNum++)
        {
            PuzzleFloor floor = floors[floorNum - 1];
            int idx = Random.Range(0, floor.sprites.Length);
            floor.image.sprite = floor.sprites[idx];
            floor.currentIndex = idx;
            UpdateDisplayText(floorNum);
        }
    }

    public void OnClickNextButton(int floorNum)
    {
        PuzzleFloor floor = floors[floorNum - 1];
        floor.currentIndex = (floor.currentIndex + 1) % floor.sprites.Length;
        floor.image.sprite = floor.sprites[floor.currentIndex];
        UpdateDisplayText(floorNum);
    }

    public void OnClickPrevButton(int floorNum)
    {
        PuzzleFloor floor = floors[floorNum - 1];
        floor.currentIndex = (floor.currentIndex - 1 + floor.sprites.Length) % floor.sprites.Length;
        floor.image.sprite = floor.sprites[floor.currentIndex];
        UpdateDisplayText(floorNum);
    }

    public void OnClickToggleDisplayButton()
    {
        displayText.SetActive(!displayText.activeSelf);
    }

    private void UpdateDisplayText(int floorNum)
    {
        PuzzleFloor floor = floors[floorNum - 1];
        floor.displayText.text = $"{floorNum}{_alphabet[floor.currentIndex]}";
    }
}
