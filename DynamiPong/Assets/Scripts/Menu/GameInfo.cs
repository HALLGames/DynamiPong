using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public enum WinCondition {FirstTo10, FirstTo20, MostAfter5, MostAfter10, Freeplay}

    public bool useBot = false;
    public WinCondition winCon = WinCondition.FirstTo10;
}
