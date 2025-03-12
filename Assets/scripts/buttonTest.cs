using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log("Mouse is over the button!");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        Debug.Log("Mouse left the button!");
    }


}
