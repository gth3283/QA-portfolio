using UnityEngine;
using UnityEngine.UI;

public enum MapType
{
    Start,
    Battle,
    Event,
    Boss
}

public class MapNode : MonoBehaviour
{
    public MapType type;

    public Sprite startSprite;
    public Sprite battleSprite;
    public Sprite eventSprite;
    public Sprite bossSprite;

    private Button button;
    private Image image;
    private MapManager mapmanager;
    private bool cango;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Clicked);
        button.interactable = false;
    }

    public void Init(MapManager mapmanager, MapType type, bool cango)
    {
        this.mapmanager = mapmanager;
        this.type = type;
        SetSprite();
        Setgo(cango);
    }
    private void SetSprite()
    {
        if (image == null)
        {
            return;
        }

        switch (type)
        {
            case MapType.Start:
                image.sprite = startSprite;
                break;

            case MapType.Battle:
                image.sprite = battleSprite;
                break;

            case MapType.Event:
                image.sprite = eventSprite;
                break;

            case MapType.Boss:
                image.sprite = bossSprite;
                break;
        }
    }
    public void Setgo(bool value)
    {
        cango = value;
        button.interactable = value;
    }

    private bool CanIGo()
    {
        return cango;
    }

    private void Clicked()
    {
        mapmanager.SelectNode(this);
    }
}
