using UnityEngine;
public class AmmoItem : MonoBehaviour, Item
{
    [SerializeField] private int magazineAmount;
    public void Use(FPSController playerController)
    {
        playerController.currentWeapon.magazineCount += magazineAmount;
        playerController.InvokeAmmoCountChanged();
    }
}
