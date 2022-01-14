using System;

public class MagicCube
{
    public int rndOffset;

    public int[][][] getMagicCube()
    {
        rndOffset = UnityEngine.Random.Range(1, 10);
        int[][][] tempCube;
        var n = 3;
        tempCube = new int[n][][];
        for (int i = 0; i < n; i++)
        {
            tempCube[i] = new int[n][];
            for (int j = 0; j < n; j++)
                tempCube[i][j] = new int[n];
        }

        int l = 0, r = n / 2, c = n / 2;
        int last = n * n * n;
        for (int i = 0; i < last; i++)
        {
            tempCube[l][r][c] = i + rndOffset;
            l = (l + n - 1) % n;
            c = (c + n - 1) % n;
            if (tempCube[l][r][c] != 0)
            {
                r = (r + n - 1) % n;
                c = (c + 1) % n;
                if (tempCube[l][r][c] != 0)
                {
                    r = (r + 1) % n;
                    l = (l + 2) % n;
                }
            }
        }
        return tempCube;
    }
}
