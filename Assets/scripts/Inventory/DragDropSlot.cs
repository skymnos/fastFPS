using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropSlot : MonoBehaviour, IDropHandler
{
    public int emplacmentNb;
    private DragDropItem inventoryItem;
    public GunSO gunSO;
    [SerializeField]
    private PlayerGun playerGun;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Droped");
        if (transform.childCount == 0)
        {
            inventoryItem = eventData.pointerDrag.GetComponent<DragDropItem>();
            inventoryItem.parentAfterDrag = transform;
            gunSO = inventoryItem.gunSO;
        }
        else if (transform.childCount == 1)
        {
            inventoryItem = eventData.pointerDrag.GetComponent<DragDropItem>(); // on recup�re l'objet selectionn�
            Destroy(transform.GetChild(0).gameObject); // on supprime l'objet qui �tait la avant
            inventoryItem.parentAfterDrag = transform; // on met le parent de l'objet selectionn� au slot sur lequel il est
            gunSO = inventoryItem.gunSO;
            playerGun.SelectGun();
        }
    }
}
