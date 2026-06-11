using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public int DataPlayerHP;
    public float[] DataPlayerPosition;

    public PlayerData(CharacterBasics characterBasics)
    {
        DataPlayerHP = characterBasics.CurrentHP;
        DataPlayerPosition = new float[3];
        DataPlayerPosition[0] = characterBasics.transform.position.x;
        DataPlayerPosition[1] = characterBasics.transform.position.y;
        DataPlayerPosition[2] = characterBasics.transform.position.z;
    }
}
