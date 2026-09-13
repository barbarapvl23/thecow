using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado

public class EndGameSummary : MonoBehaviour
{
    public TMP_Text coinSummaryText; // Asigna este campo en el Inspector de Unity

    private void OnEnable()
    {
        if (CoinManager.instance != null && coinSummaryText != null)
        {
            int totalCoins = CoinManager.instance.GetTotalCoins(); // Obtiene el total de monedas recogidas
            int collectedCoins = CoinManager.instance.GetCollectedCoins(); // Obtiene las monedas recogidas en la escena actual
            
            coinSummaryText.text = "Collected Coins: " + collectedCoins + " / " + totalCoins;
        }
        else
        {
            Debug.LogWarning("CoinManager instance or coinSummaryText is not assigned.");
        }
    }
}
