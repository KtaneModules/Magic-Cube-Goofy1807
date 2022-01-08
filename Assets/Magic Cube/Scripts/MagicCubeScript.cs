using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KModkit;
using UnityEngine;

public class MagicCubeScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable[] UpArrows;
    public KMSelectable[] DownArrows;
    public KMSelectable[] LeftArrows;
    public KMSelectable[] RightArrows;
    public TextMesh[] InnerValues;
    public TextMesh[] OuterValues;
    public KMSelectable ViewToggle;
    public Texture[] Images;
    public MeshRenderer ToggleImage;

    private int[][][] magicCube;


    int moduleId;
    static int moduleIdCounter = 1;
    private bool moduleSolved = false;

    void Start()
    {
        moduleId = moduleIdCounter++;
        magicCube = getMagicCube();

        for (int i = 0; i < 3; i++)
        {
            UpArrows[i].OnInteract += UpArrowPressed(i);
            DownArrows[i].OnInteract += DownArrowPressed(i);
            LeftArrows[i].OnInteract += LeftArrowPressed(i);
            RightArrows[i].OnInteract += RightArrowPressed(i);
        }
        ViewToggle.OnInteract += ViewTogglePressed();
    }

    private KMSelectable.OnInteractHandler ViewTogglePressed()
    {
        return delegate
        {
            if (moduleSolved)
                return false;

            return false;
        };
    }

    private KMSelectable.OnInteractHandler UpArrowPressed(int btn)
    {
        return delegate
        {
            if (moduleSolved)
                return false;

            return false;
        };
    }
    private KMSelectable.OnInteractHandler DownArrowPressed(int btn)
    {
        return delegate
        {
            if (moduleSolved)
                return false;

            return false;
        };
    }
    private KMSelectable.OnInteractHandler LeftArrowPressed(int btn)
    {
        return delegate
        {
            if (moduleSolved)
                return false;

            return false;
        };
    }
    private KMSelectable.OnInteractHandler RightArrowPressed(int btn)
    {
        return delegate
        {
            if (moduleSolved)
                return false;

            return false;
        };
    }

    private int[][][] getMagicCube()
    {
        int[][][] tempCube;
        var n = 3;
        var rndOffset = UnityEngine.Random.Range(0, 10);
        tempCube = new int[n][][];
        for (int i = 0; i < n; i++)
        {
            tempCube[i] = new int[n][];
            for (int j = 0; j < n; j++)
                tempCube[i][j] = new int[n];
        }

        int l, r, c;
        l = 0;
        r = n / 2;
        c = n / 2;

        int last = (int) Math.Pow(n, 3);
        for (int i = 0; i < last; i++)
        {
            tempCube[l][r][c] = i + rndOffset;
            l--;
            l = normalize(n, l);
            c--;
            c = normalize(n, c);
            if (tempCube[l][r][c] != 0)
            {
                r--;
                r = normalize(n, r);
                c++;
                c = normalize(n, c);
                if (tempCube[l][r][c] != 0)
                {
                    r++;
                    r = normalize(n, r);
                    l += 2;
                    l = normalize(n, l);
                }
            }
        }
        return tempCube;
    }


    private int normalize(int n, int index)
    {
        while (index < 0)
        {
            index = index + n;
        }
        while (index > n - 1)
        {
            index = index - n;
        }
        return index;
    }


    void Log(string msg, params object[] fmtArgs)
    {
        Debug.LogFormat(@"[Magic Cube #{0}] {1}", moduleId, string.Format(msg, fmtArgs));
    }


    //#pragma warning disable 414
    //    private readonly string TwitchHelpMessage = @"!{0} [] | !{0} []";
    //#pragma warning restore 414
    //    IEnumerator ProcessTwitchCommand(string command)
    //    {
    //        Match m;
    //        if (moduleSolved)
    //        {
    //            yield return "sendtochaterror The module is already solved.";
    //            yield break;
    //        }
    //        else if ((m = Regex.Match(command, @"^\s*()\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
    //        {
    //            yield return null;
    //            // Code goes here
    //            yield break;
    //        }
    //        else if (Regex.IsMatch(command, @"^\s*()\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
    //        {
    //            yield return null;
    //            // Code goes here
    //            yield break;
    //        }
    //        else
    //        {
    //            yield return "sendtochaterror Invalid Command";
    //            yield break;
    //        }
    //    }
    //
    //    IEnumerator TwitchHandleForcedSolve()
    //    {
    //        Log("Module was force solved by TP", moduleId);
    //        //Code goes here
    //        yield break;
    //    }
}