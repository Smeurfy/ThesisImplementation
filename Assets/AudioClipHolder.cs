using UnityEngine;

public class AudioClipHolder : MonoBehaviour
{
    [Header("Health Related")]
    [SerializeField] private AudioClip[] gotDamaged;
    [SerializeField] private AudioClip[] died;

    [Header("Throw And PickUp")]
    [SerializeField] private AudioClip[] throwed;
    [SerializeField] private AudioClip[] pickedUp;

    public AudioClip GetGotDamagedSound() { return gotDamaged[Random.Range(0, gotDamaged.Length)]; }

    public AudioClip GetDiedSound() { return died[Random.Range(0, died.Length)]; }

    public AudioClip GetThrowedSound() { return throwed[Random.Range(0, throwed.Length)]; }

    public AudioClip GetPickUpSound(){ return pickedUp[Random.Range(0, pickedUp.Length)]; }
}
