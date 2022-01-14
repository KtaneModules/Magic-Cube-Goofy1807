using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Texture[] Layers;
    public MeshRenderer Layer;

    private int curLayer = 0;
    private int magicConstant;
    private readonly MagicCube MagicCube = new MagicCube();

    private int[][][] magicCube;


    int moduleId;
    static int moduleIdCounter = 1;
    private bool moduleSolved = false;
    private bool moveActive = false;
    private int[] ixs = new int[] { 6, 7, 8, 0, 1, 2, 2, 5, 8, 0, 3, 6 };

    void Start()
    {
        moduleId = moduleIdCounter++;
        magicCube = MagicCube.getMagicCube();
        magicConstant = (int) (3 * (Math.Pow(3, 3) + 1) / 2) + (MagicCube.rndOffset - 1) * 3;
        Debug.Log(magicConstant);
        for (int i = 0; i < 3; i++)
        {
            UpArrows[i].OnInteract += ArrowPressed(i, 0);
            DownArrows[i].OnInteract += ArrowPressed(i, 1);
            LeftArrows[i].OnInteract += ArrowPressed(i, 2);
            RightArrows[i].OnInteract += ArrowPressed(i, 3);
        }
        ViewToggle.OnInteract += ViewTogglePressed();
        UpdateModule(false, false);
        Log("Magic Cube: {0}{1}", Environment.NewLine, magicCube.Select(layer => layer.Select(face => face.Select(cell => cell.ToString()).Join(", ")).Join(Environment.NewLine)).Join(Environment.NewLine));
        StartCoroutine(ShuffleCube(25));
    }

    private KMSelectable.OnInteractHandler ViewTogglePressed()
    {
        return delegate
        {
            if (moduleSolved || moveActive)
                return false;
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            curLayer = (curLayer + 1) % 9;
            UpdateModule(false, false);
            return false;
        };
    }

    private KMSelectable.OnInteractHandler ArrowPressed(int btn, int ix)
    {
        return delegate
        {
            if (moduleSolved || moveActive)
                return false;
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            StartCoroutine(MoveValues(btn, ix, false));
            return false;
        };
    }

    private void UpdateModule(bool cube, bool solveCheck)
    {
        if (!cube)
            Layer.material.mainTexture = Layers[curLayer];
        if (curLayer < 3)
            for (int i = 0; i < InnerValues.Length; i++)
            {
                if (cube)
                    magicCube[curLayer][i / 3][i % 3] = int.Parse(InnerValues[i].text);
                else
                    InnerValues[i].text = magicCube[curLayer][i / 3][i % 3].ToString();
            }
        else if (curLayer < 6)
            for (int i = 0; i < InnerValues.Length; i++)
            {
                if (cube)
                    magicCube[2 - (i / 3)][curLayer % 3][i % 3] = int.Parse(InnerValues[i].text);
                else
                    InnerValues[i].text = magicCube[2 - (i / 3)][curLayer % 3][i % 3].ToString();
            }
        else
            for (int i = 0; i < InnerValues.Length; i++)
            {
                if (cube)
                    magicCube[i % 3][i / 3][curLayer % 3] = int.Parse(InnerValues[i].text);
                else
                    InnerValues[i].text = magicCube[i % 3][i / 3][curLayer % 3].ToString();
            }
        for (int i = 0; i < OuterValues.Length; i++)
            OuterValues[i].text = InnerValues[ixs[i]].text;

        if (solveCheck)
        {
            var solved = true;
            for (int layer = 0; layer < magicCube.Length; layer++)
            {
                for (int ix = 0; ix < magicCube[layer].Length; ix++)
                {
                    if (magicCube[layer][ix][0] + magicCube[layer][ix][1] + magicCube[layer][ix][2] != magicConstant)
                        solved = false;
                    if (magicCube[layer][0][ix] + magicCube[layer][1][ix] + magicCube[layer][2][ix] != magicConstant)
                        solved = false;
                    if (magicCube[0][layer][ix] + magicCube[1][layer][ix] + magicCube[2][layer][ix] != magicConstant)
                        solved = false;
                }
            }
            if (magicCube[0][0][0] + magicCube[1][1][1] + magicCube[2][2][2] != magicConstant)
                solved = false;
            if (magicCube[0][0][2] + magicCube[1][1][1] + magicCube[2][2][0] != magicConstant)
                solved = false;
            if (magicCube[0][2][0] + magicCube[1][1][1] + magicCube[2][0][2] != magicConstant)
                solved = false;
            if (magicCube[0][2][2] + magicCube[1][1][1] + magicCube[2][0][0] != magicConstant)
                solved = false;

            if (solved)
            {
                Module.HandlePass();
                moduleSolved = true;
            }
        }
    }

    private IEnumerator ShuffleCube(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            for (int j = 0; j < 6; j++)
                yield return StartCoroutine(MoveValues(Random.Range(0, 3), Random.Range(0, 4), true));
            for (int k = 0; k < Random.Range(1, 4); k++)
            {
                ViewToggle.OnInteract();
                yield return new WaitForSeconds(.01f);
            }
        }
        curLayer = 0;
        UpdateModule(false, false);
    }

    private IEnumerator MoveValues(int btn, int from, bool fast)
    {
        moveActive = true;

        var duration = fast ? .01f : .1f;
        var elapsed = 0f;
        var valuePositions = new List<Vector3>();

        for (int i = 0; i < 3; i++)
            valuePositions.Add(InnerValues[from < 2 ? (i * 3 + btn) : (i + btn * 3)].transform.localPosition);

        valuePositions.Add(OuterValues[((from * 3 / 3) ^ 1) * 3 + btn].transform.localPosition);
        valuePositions.Add(OuterValues[from * 3 + btn].transform.localPosition);

        switch (from)
        {
            //Up Arrow
            case 0:

                while (elapsed < duration)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                    OuterValues[3 + btn].transform.localPosition = Vector3.Lerp(valuePositions[3], valuePositions[2], elapsed / duration);
                    InnerValues[6 + btn].transform.localPosition = Vector3.Lerp(valuePositions[2], valuePositions[1], elapsed / duration);
                    InnerValues[3 + btn].transform.localPosition = Vector3.Lerp(valuePositions[1], valuePositions[0], elapsed / duration);
                    InnerValues[0 + btn].transform.localPosition = Vector3.Lerp(valuePositions[0], valuePositions[4], elapsed / duration);
                }

                InnerValues[6 + btn].text = OuterValues[3 + btn].text;
                InnerValues[0 + btn].text = InnerValues[3 + btn].text;
                InnerValues[3 + btn].text = OuterValues[0 + btn].text;

                InnerValues[0 + btn].transform.localPosition = valuePositions[0];
                InnerValues[3 + btn].transform.localPosition = valuePositions[1];
                InnerValues[6 + btn].transform.localPosition = valuePositions[2];
                OuterValues[0 + btn].transform.localPosition = valuePositions[4];
                OuterValues[3 + btn].transform.localPosition = valuePositions[3];

                break;

            //Down Arrow
            case 1:

                while (elapsed < duration)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                    OuterValues[0 + btn].transform.localPosition = Vector3.Lerp(valuePositions[3], valuePositions[0], elapsed / duration);
                    InnerValues[0 + btn].transform.localPosition = Vector3.Lerp(valuePositions[0], valuePositions[1], elapsed / duration);
                    InnerValues[3 + btn].transform.localPosition = Vector3.Lerp(valuePositions[1], valuePositions[2], elapsed / duration);
                    InnerValues[6 + btn].transform.localPosition = Vector3.Lerp(valuePositions[2], valuePositions[4], elapsed / duration);
                }

                InnerValues[6 + btn].text = InnerValues[3 + btn].text;
                InnerValues[3 + btn].text = InnerValues[0 + btn].text;
                InnerValues[0 + btn].text = OuterValues[0 + btn].text;

                InnerValues[0 + btn].transform.localPosition = valuePositions[0];
                InnerValues[3 + btn].transform.localPosition = valuePositions[1];
                InnerValues[6 + btn].transform.localPosition = valuePositions[2];
                OuterValues[0 + btn].transform.localPosition = valuePositions[3];
                OuterValues[3 + btn].transform.localPosition = valuePositions[4];

                break;

            //Left Arrow
            case 2:

                while (elapsed < duration)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                    OuterValues[9 + btn].transform.localPosition = Vector3.Lerp(valuePositions[3], valuePositions[2], elapsed / duration);
                    InnerValues[2 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[2], valuePositions[1], elapsed / duration);
                    InnerValues[1 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[1], valuePositions[0], elapsed / duration);
                    InnerValues[0 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[0], valuePositions[4], elapsed / duration);
                }

                InnerValues[2 + 3 * btn].text = InnerValues[0 + 3 * btn].text;
                InnerValues[0 + 3 * btn].text = InnerValues[1 + 3 * btn].text;
                InnerValues[1 + 3 * btn].text = OuterValues[6 + btn].text;

                InnerValues[0 + 3 * btn].transform.localPosition = valuePositions[0];
                InnerValues[1 + 3 * btn].transform.localPosition = valuePositions[1];
                InnerValues[2 + 3 * btn].transform.localPosition = valuePositions[2];
                OuterValues[9 + btn].transform.localPosition = valuePositions[3];
                OuterValues[6 + btn].transform.localPosition = valuePositions[4];

                break;

            //Right Arrow
            case 3:

                while (elapsed < duration)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                    OuterValues[6 + btn].transform.localPosition = Vector3.Lerp(valuePositions[3], valuePositions[0], elapsed / duration);
                    InnerValues[0 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[0], valuePositions[1], elapsed / duration);
                    InnerValues[1 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[1], valuePositions[2], elapsed / duration);
                    InnerValues[2 + 3 * btn].transform.localPosition = Vector3.Lerp(valuePositions[2], valuePositions[4], elapsed / duration);
                }

                InnerValues[0 + 3 * btn].text = InnerValues[2 + 3 * btn].text;
                InnerValues[2 + 3 * btn].text = InnerValues[1 + 3 * btn].text;
                InnerValues[1 + 3 * btn].text = OuterValues[9 + btn].text;

                InnerValues[0 + 3 * btn].transform.localPosition = valuePositions[0];
                InnerValues[1 + 3 * btn].transform.localPosition = valuePositions[1];
                InnerValues[2 + 3 * btn].transform.localPosition = valuePositions[2];
                OuterValues[6 + btn].transform.localPosition = valuePositions[3];
                OuterValues[9 + btn].transform.localPosition = valuePositions[4];

                break;
        }
        UpdateModule(true, true);
        moveActive = false;
    }

    void Log(string msg, params object[] fmtArgs)
    {
        Debug.LogFormat(@"[Magic Cube #{0}] {1}", moduleId, string.Format(msg, fmtArgs));
    }

}