using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string weaponName;
    public GunSO gunSO;
    public RawImage image;
    public Transform parentAfterDrag;
    public bool equiped = false;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
        if (equiped == false)
        {
            Instantiate(gameObject, transform.parent);
        }
        image.raycastTarget = false;
        parentAfterDrag = null;
        transform.SetParent(transform.root);
        equiped = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        image.raycastTarget = true;
        if (parentAfterDrag != null)
        {
            equiped = true;
            transform.SetParent(parentAfterDrag);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
