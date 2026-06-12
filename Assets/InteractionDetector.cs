using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteraction interactableInRange = null;
    public GameObject interactionText; 
    void Start()
    {
        interactionText.SetActive(false);
    }

    public void OnInteraction(InputValue value)
    {
        if (value.isPressed)
        {
            interactableInRange?.Interact();
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactionText.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactionText.SetActive(false);
    }
}
