using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public MapNode startNode;
    public List<MapNode> Firstnodes;
    public List<MapNode> Secondnodes;
    public MapNode endNode;

    public RectTransform playerIcon;

    private void Start()
    {
        if (!GameManager.HasRunStarted)
        {
            GameManager.StartNewRun();
        }
        GenerateMap();
        UnlockNode();
    }

    private void GenerateMap()
    {
        startNode.Init(this, MapType.Start,false);
        endNode.Init(this, MapType.Boss,false);

        if(!GameManager.HasGeneratedMap)
        {
            for (int i = 0; i < 3; i++)
            {
                GameManager._1Types[i] = Random.value < 0.5f
                    ? MapType.Battle
                    : MapType.Event;
            }
            for (int i = 0; i < 3; i++)
            {
                GameManager._2Types[i] = Random.value < 0.5f
                    ? MapType.Battle
                    : MapType.Event;
            }
            GameManager.HasGeneratedMap = true;
        }
        else
        {
           playerIcon.position = GameManager.playerIcon;
        }

        for (int i = 0; i < Firstnodes.Count; i++)
        {
            Firstnodes[i].Init(this, GameManager._1Types[i], false);
        }

        for (int i = 0; i < Secondnodes.Count; i++)
        {
            Secondnodes[i].Init(this, GameManager._2Types[i], false);
        }
    }

    private void UnlockNode()
    {
        int i = GameManager.MapCurrentStep;

        startNode.Setgo(false);
        endNode.Setgo(false);

        for (int n = 0; n < Firstnodes.Count; n++)
        {
            Firstnodes[n].Setgo(false);
        }

        for (int n = 0; n < Secondnodes.Count; n++)
        {
            Secondnodes[n].Setgo(false);
        }

        if (!GameManager.HasSelectedRoute)
        {
            if (Firstnodes.Count > 0)
            {
                Firstnodes[0].Setgo(true);
            }

            if (Secondnodes.Count > 0)
            {
                Secondnodes[0].Setgo(true);
            }

            return;
        }

        List<MapNode> selectedRoute = GameManager.SelectedRoute1
            ? Firstnodes
            : Secondnodes;

        if (i >= selectedRoute.Count)
        {
            endNode.Setgo(true);
            return;
        }

        selectedRoute[i].Setgo(true);
    }

    private void IconMove(MapNode node)
    {
        RectTransform target = node.GetComponent<RectTransform>();
        GameManager.playerIcon = target.position;
        playerIcon.position = target.position;
    }

    public void SelectNode(MapNode node)
    {
        IconMove(node);

        if (node.type == MapType.Battle)
        {
            EnterBattle(node);
        }
        else if (node.type == MapType.Event)
        {
            EnterEvent(node);
        }
        else if (node.type == MapType.Boss)
        {
            EnterBoss();
        }
    }

    private void EnterBattle(MapNode node)
    {
        SaveMapProgress(node);

        SceneManager.LoadScene("Battle");
    }

    private void EnterEvent(MapNode node)
    {
        SaveMapProgress(node);

        //SceneManager.LoadScene("Event"); 시간 관계상 이벤트 미구현
        SceneManager.LoadScene("Battle");
    }

    private void EnterBoss()
    {
        GameManager.boss = true;
        SceneManager.LoadScene("Battle");
    }

    private void SaveMapProgress(MapNode selectedNode)
    {
        if (!GameManager.HasSelectedRoute)
        {
            GameManager.SelectedRoute1 = Firstnodes.Contains(selectedNode);
            GameManager.HasSelectedRoute = true;
        }

        GameManager.MapCurrentStep++;
    }
}
