using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour, IInteraction
{
    public bool IsOpen { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab; //usunac potem
    public Sprite openSprite;
    void Start()
    {
        //ChestID =
    }
    public bool CanInteract()
    {
        return !IsOpen;
    }
    public void Interact()
    {

    }

}
