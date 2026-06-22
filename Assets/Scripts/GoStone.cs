using UnityEngine;

public enum GoType
{
    White,
    Black
}
public class GoStone
{
    public GoType GoType;

    public GoStone(GoType goType)
    {
        this.GoType = goType;
    }
}
