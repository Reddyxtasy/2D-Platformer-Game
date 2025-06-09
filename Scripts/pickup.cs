using UnityEngine;

public class pickup : MonoBehaviour
{
    public enum pickupType { coin, gem, health }
    public pickupType pt;

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        switch (pt)
        {
            case pickupType.coin:
                GameManager.instance?.IncrementCoinCount();
                break;

            case pickupType.gem:
                GameManager.instance?.IncrementGemCount();
                break;

            case pickupType.health:
                // You can add health logic here
                break;
        }

       

        Destroy(gameObject, 0.2f);
    }
}
