using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionCanvas : MonoBehaviour
{
    [Header("Transition Canvas")]
    [SerializeField] private GameObject _canvas;
    private CanvasGroup _canvasGroup;

    private IEnumerator _coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeInOut(string doorScene)
    {
        _coroutine = LoadLevel(doorScene);
        StartCoroutine(_coroutine);
    }

    // Loads the _doorScene level
    private IEnumerator LoadLevel(string doorScene)
    {
        var op = SceneManager.LoadSceneAsync(doorScene);
        op.allowSceneActivation = false;

        float t = 0;
        while (op.progress < 0.9f || t < 1)
        {
            t += Time.deltaTime * 1.5f;
            t = Mathf.Clamp01(t);
            _canvasGroup.alpha = t;
            yield return null;
        }

        op.allowSceneActivation = true;

        t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime * 1.5f;
            t = Mathf.Clamp01(t);
            _canvasGroup.alpha = t;
            yield return null;
        }
    }
}
