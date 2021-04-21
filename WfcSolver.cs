using System;
using System.Collections.Generic;
using System.Linq;

class WfcSolver
{
    //PatternMap patternMap;
    List<Pattern> patterns;
    Random rnd;

    public WfcSolver(List<Pattern> patterns, int seed)
    {
        this.patterns = patterns;
        this.rnd = new Random(seed);
    }

    public int[,] Run(int outputWidth, int outputHeight)
    {
        int[,][] result = new int[outputWidth, outputHeight][];
        InitArray(ref result, outputWidth, outputHeight);
        Compute(ref result, outputWidth, outputHeight);

        int[,] trueResult = new int[outputWidth, outputHeight];

        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                if (result[x, y].Length < 1)
                    return Run(outputWidth, outputHeight); //Can't Collapse so let's retry i guess
                trueResult[x, y] = result[x, y][0]; //Might be an error here if it can't collapse
            }
        }

        return trueResult;
    }

    void Compute(ref int[,][] output, int width, int height)
    {
        while (!IsCollapsed(output))
        {
            (int x, int y) current = GetCellByMinEntropy(output, width, height);
            CollapseCell(ref output, current);
            Propagate(ref output, current, width, height);
        }
    }

    void Propagate(ref int[,][] output, (int x, int y) pos, int width, int height)
    {
        Queue<(int x, int y)> stack = new Queue<(int x, int y)>();
        stack.Enqueue(pos);

        while (stack.Count > 0)
        {
            (int x, int y) current = stack.Dequeue();

            foreach ((int x, int y) neighborOffset in GetValidNeighborOffset(current, width, height))
            {
                if (output[current.x + neighborOffset.x, current.y + neighborOffset.y].Length < 2)
                    continue;

                Direction currentToNeighborDir = NeighborOffsetToDirection(neighborOffset);
                Direction NeighborToCurrentDir = InvertDirection(currentToNeighborDir);

                List<int> patternsIndexNeighborList = new List<int>(output[current.x + neighborOffset.x, current.y + neighborOffset.y]);
                List<Pattern> patternsNeighborList = GetPatternOf(output[current.x + neighborOffset.x, current.y + neighborOffset.y]);
                bool hasBeenChanged = false;

                for (int i = patternsNeighborList.Count - 1; i >= 0; i--)
                {
                    if (!patternsNeighborList[i].IsValid(output[current.x, current.y], NeighborToCurrentDir))
                    {
                        patternsIndexNeighborList.RemoveAt(i);
                        patternsNeighborList.RemoveAt(i);
                        hasBeenChanged = true;
                    }
                }

                if (hasBeenChanged)
                {
                    output[current.x + neighborOffset.x, current.y + neighborOffset.y] = patternsIndexNeighborList.ToArray();
                    stack.Enqueue((current.x + neighborOffset.x, current.y + neighborOffset.y));
                }
            }
        }
    }

    void CollapseCell(ref int[,][] output, (int x, int y) pos)
    {
        int index = rnd.Next(0, output[pos.x, pos.y].Length - 1);

        output[pos.x, pos.y] = new int[] { output[pos.x, pos.y][index] };
    }

    (int x, int y) GetCellByMinEntropy(int[,][] map, int width, int height)
    {
        float mn = float.MaxValue;
        float[,] entropyMap = new float[width, height];
        List<(int x, int y)> candidates = new List<(int x, int y)>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y].Length <= 1)
                {
                    entropyMap[x, y] = float.MaxValue;
                    continue;
                }

                float currentEntropy = CalculateEntropyOfCell(map, (x, y));

                entropyMap[x, y] = currentEntropy;
                if (currentEntropy < mn)
                    mn = currentEntropy;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (entropyMap[x, y] == mn)
                    candidates.Add((x, y));
            }
        }

        if (candidates.Count == 1)
            return candidates[0];

        int index = rnd.Next(0, candidates.Count - 1);

        return candidates[index];
    }



    void InitArray(ref int[,][] output, int width, int height)
    {
        int[] patternlist = new int[patterns.Count];

        for (int i = 0; i < patterns.Count; i++)
        {
            patternlist[i] = i;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                output[x, y] = patternlist;
            }
        }
    }

    bool IsCollapsed(int[,][] map)
    {
        int mx = 0;

        foreach(int[] cell in map)
            if (cell.Length > mx)
                mx = cell.Length;

        return mx < 2;
    }

    float CalculateEntropyOfCell(int[,][] map, (int x, int y) pos)
    {
        return map[pos.x, pos.y].Sum();
    }

    (int x, int y)[] GetValidNeighborOffset((int x, int y) pos, int width, int height)
    {
        List<(int x, int y)> result = new List<(int x, int y)>();

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (x != 0 && y != 0)
                    continue;

                if ((x + pos.x) >= width || (x + pos.x) < 0)
                    continue;

                if ((y + pos.y) >= height || (y + pos.y) < 0)
                    continue;

                result.Add((x, y));
            }
        }

        return result.ToArray();
    }

    Direction NeighborOffsetToDirection((int x, int y) offset)
    {
        if (offset.y == 1)
            return Direction.top;
        if (offset.y == -1)
            return Direction.bottom;
        if (offset.x == 1)
            return Direction.right;
        return Direction.left;
    }

    Direction InvertDirection(Direction dir)
    {
        switch(dir)
        {
            case Direction.top:
                return Direction.bottom;
            case Direction.bottom:
                return Direction.top;
            case Direction.right:
                return Direction.left;
            case Direction.left:
                return Direction.right;
            default:
                return Direction.top;
        }
    }

    List<Pattern> GetPatternOf(int[] patternsIndex)
    {
        List<Pattern> patterns = new List<Pattern>();

        foreach (int i in patternsIndex)
        {
            patterns.Add(this.patterns[i]);
        }

        return patterns;
    }
}
