using UnityEngine;
using TMPro;
public class WaterText : MonoBehaviour
{


    public Money Money;
    public TMP_Text m_Text;
    void Awake()
    {
        Money = FindAnyObjectByType<Money>();
    }

    void Start()
    {
        m_Text.text = "Water: " + Money.currentMoney.ToString();
    }

    void Update()
    {
        m_Text.text = "Water: " + Money.currentWater.ToString();
    }
}


