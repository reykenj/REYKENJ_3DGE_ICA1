using UnityEngine;
public class AmmoItem : MonoBehaviour, Item
{
    [SerializeField] private int ammoAmount;
    public void Use(FPSController playerController)
    {
        playerController.currentWeapon.ammoCount += ammoAmount;
        playerController.InvokeAmmoCountChanged();
    }
}
