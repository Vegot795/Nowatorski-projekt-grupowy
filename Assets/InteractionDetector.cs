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

    public void TryInteract()
    {
        interactableInRange?.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteraction interactable) && interactable.CanInteract())
        { 
            interactableInRange = interactable;
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteraction interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionText.SetActive(false);
        }
    }
}
