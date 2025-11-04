using UnityEngine;

[CreateAssetMenu(fileName = "NewPhoneContact", menuName = "Phone/Contact")]
public class Scr_PhoneContact : ScriptableObject
{
    public string contactName;         // Name of the contact
    public string phoneNumber;         // Their number
    [TextArea(2, 6)] public string[] dialogueLines; // Dialogue lines

    // Optional: do something special when this contact answers
    public virtual void OnCallAnswered(Scr_GameManager gameManager)
    {
        Debug.Log($"{contactName} answered the phone.");
        // Custom behavior can be added by inheriting

        gameManager.currentlyCalling = contactName;

    }
}