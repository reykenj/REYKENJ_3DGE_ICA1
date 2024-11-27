using UnityEngine;
public class AmmoItem : MonoBehaviour, Item
{
    [SerializeField] private int magazineAmount;
    [SerializeField] private AudioClip PickUpSFX;
    public void Use(FPSController playerController)
    {
        if (PickUpSFX != null)
        {
            AudioSource.PlayClipAtPoint(PickUpSFX, transform.position);
        }
        playerController.currentWeapon.magazineCount += magazineAmount;
        playerController.InvokeAmmoCountChanged();
        Destroy(gameObject);
    }
}
