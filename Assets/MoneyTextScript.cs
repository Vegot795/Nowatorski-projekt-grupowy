using TMPro;
using UnityEngine;

public class MoneyTextScript : MonoBehaviour
{
    public Money Money;
    public TMP_Text m_Text;

    void Start()
    {
        m_Text.text = "Money: " + Money.currentMoney.ToString();
    }

    void Update()
    {
        m_Text.text = "Money: " + Money.currentMoney.ToString();
    }
}
