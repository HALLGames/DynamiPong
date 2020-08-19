using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public enum WinCondition {Freeplay, FirstTo10, FirstTo20, MostAfter5, MostAfter10}

    public bool useBot = false;
    public WinCondition winCon = WinCondition.Freeplay;
}
