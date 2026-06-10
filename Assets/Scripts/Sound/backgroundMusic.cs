using UnityEngine;

public class backgroundMusic : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


}

