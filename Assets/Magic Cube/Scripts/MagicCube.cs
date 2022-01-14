using System;
using UnityEngine;

public class MagicCube {

    public int rndOffset;

    public int[][][] getMagicCube()
    {
        rndOffset = UnityEngine.Random.Range(1, 2);
        int[][][] tempCube;
        var n = 3;
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
}
