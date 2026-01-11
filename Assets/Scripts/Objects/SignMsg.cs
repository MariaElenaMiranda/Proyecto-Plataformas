using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class SignMsg : MonoBehaviour
{
    public GameObject textObject;
    public string message; // = "";
    public float visibleTime = 3f;

    private TextMeshProUGUI textMesh;

    public void Start()
    {
        textMesh = textObject.GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("textObject no se esta asignando al componente TextMeshProUGUI");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ShowMsg());
        }
    }

    IEnumerator ShowMsg()
    {
        textMesh.text = message;
        textObject.SetActive(true);
        yield return new WaitForSeconds(visibleTime);
        textObject.SetActive(false);
        textMesh.text = "";
    }
}
