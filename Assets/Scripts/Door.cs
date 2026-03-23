using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Door : MonoBehaviour, IPointerClickHandler
{
    
    [Header("Door Type")]
    [SerializeField] private RoomType _doorType;

    [Tooltip("The next room (scene) this door leads to")]
    [Header("Door Load Scene")]
    [SerializeField] private string _doorScene;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvas;
    private CanvasGroup _canvasGroup; 
    
    private float _fadeDuration;
    private IEnumerator _coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;
        _fadeDuration = 1.0f;

        // On new room load, play the fade out effect
        FadeOut();
    }

    // Do the Fade Effect - used with coroutine
    private IEnumerator DoFade(float startAlpha, float endAlpha)
    {
        float timer = 0;

        while(timer < _fadeDuration)
        {
            timer += Time.deltaTime;

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);

            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }

    // Fade In Effect when exiting the current room
    private IEnumerator FadeIn()
    {
        _coroutine = DoFade(_canvasGroup.alpha, 1); 
        StartCoroutine(_coroutine);
        yield return new WaitForSeconds(2.0f);
    }

    // Fade Out Effect when entering a new room
    private void FadeOut()
    {
        _coroutine = DoFade(_canvasGroup.alpha, 0); 
        StartCoroutine(_coroutine);
    }

    // Loads the _doorScene level
    private void LoadLevel()
    {
        // If entering a combat room, create a new entry in data tracker dictionary
        CheckMonsterRoom();

        // If entering the end room, save data to json file
        CheckEndRoom();

        SceneManager.LoadScene(_doorScene);
    }

    // On click event
    public void OnPointerClick(PointerEventData eventData)
    {
        FadeIn();
        LoadLevel();
    }

    private void CheckMonsterRoom()
    {
        if (_doorType == RoomType.Monster)
        {
            GameManager.instance.DataTracker.Add(_doorScene, 0);
        }
    }

    private void CheckEndRoom()
    {
        if (_doorType == RoomType.Ending)
        {
            GameManager.instance.SaveData();
        }
    }
}
