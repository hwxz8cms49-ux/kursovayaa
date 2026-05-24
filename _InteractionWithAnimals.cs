using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems;

public class _InteractionWithAnimals : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int x;
    public int y;
    public bool bonus = false;

    private Outline useoutline;

    void Awake()
    {
        useoutline = GetComponent<Outline>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject BoardObject = GameObject.Find("_Board");
        if (BoardObject != null)
        {
            _Board BoardScript = BoardObject.GetComponent<_Board>();
            if (BoardScript != null)
            {
                BoardScript.SelectAnimal(gameObject);
            }
        }
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f); 
    }
    public void OnOutline()
    {
        if (useoutline != null)
        {
            useoutline.enabled = true;
        }
    }
    public void OffOutline()
    {
        if (useoutline != null)
        {
            useoutline.enabled = false;
        }
    }
}
