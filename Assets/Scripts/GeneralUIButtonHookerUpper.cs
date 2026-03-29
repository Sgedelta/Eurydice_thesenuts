using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GeneralUIButtonHookerUpper : MonoBehaviour
{
    [SerializeField] private Button InvButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartCoroutine(SetupAfterGM());
    }

    public IEnumerator SetupAfterGM()
    {
        while (GameManager.instance == null)
        {
            yield return null;
        }

        InvButton.onClick.RemoveAllListeners();
        InvButton.onClick.AddListener(() => {
            UIManager.instance.ToggleInventory();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
