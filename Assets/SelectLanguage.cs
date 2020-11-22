using UnityEngine;

public class SelectLanguage : MonoBehaviour
{
    [SerializeField] private GameSettings _settings;

    public void SetLanguage(int lang)
    {
        _settings.lang = (Lang) lang;
    }
}

public enum Lang
{
    de = 0,
    fr = 1,
    it = 2,
    en = 3,
    es = 4
}