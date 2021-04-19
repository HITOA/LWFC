using System.Collections.Generic;

class PatternMap
{
    public List<Pattern> patterns;
    public MapHelper map;

    public PatternMap(int[,] map, int size)
    {
        this.patterns = new List<Pattern>();
        this.map = new MapHelper(map, size);
    }

    public int[,] Convert(int[,] patternMap, int size)
    {
        int trueSize = size * patterns[0].size;
        int[,] result = new int[trueSize, trueSize];

        for(int y = 0; y < trueSize; y+= patterns[0].size)
        {
            for (int x = 0; x < trueSize; x += patterns[0].size)
            {
                for (int _y = 0; _y < patterns[0].size; _y++)
                {
                    for (int _x = 0; _x < patterns[0].size; _x++)
                    {
                        result[x + _x, y + _y] = patterns[patternMap[x / patterns[0].size, y / patterns[0].size]].data[_x, _y];
                    }
                }
            }
        }

        return result;
    }

    public void FindPatterns(int size)
    {
        if (map.size <= size)
            return;

        for (int y = 0; y < map.size; y++)
        {
            for (int x = 0; x < map.size; x++)
            {
                Pattern currentPattern = new Pattern(size);
                currentPattern.data = map[x, y, size];

                if (!patterns.Contains(currentPattern))
                    patterns.Add(currentPattern);
            }
        }
    }

    public void GetPatternsNeighbor()
    {
        int size = patterns[0].size;

        for (int y = 0; y < map.size; y++)
        {
            for (int x = 0; x < map.size; x++)
            {
                Pattern currentPattern = new Pattern(size);
                currentPattern.data = map[x, y, size];

                Pattern currentTopPattern = new Pattern(size);
                currentTopPattern.data = map[x, y + size, size];

                Pattern currentBottomPattern = new Pattern(size);
                currentBottomPattern.data = map[x, y - size, size];

                Pattern currentRightPattern = new Pattern(size);
                currentRightPattern.data = map[x + size, y, size];

                Pattern currentLeftPattern = new Pattern(size);
                currentLeftPattern.data = map[x - size, y, size];

                int indexOfCurrentPattern = patterns.IndexOf(currentPattern);

                int indexOfCurrentTopPattern = patterns.IndexOf(currentTopPattern);
                int indexOfCurrentBottomPattern = patterns.IndexOf(currentBottomPattern);
                int indexOfCurrentRightPattern = patterns.IndexOf(currentRightPattern);
                int indexOfCurrentLeftPattern = patterns.IndexOf(currentLeftPattern);

                if (indexOfCurrentPattern == -1 ||
                    indexOfCurrentTopPattern == -1 ||
                    indexOfCurrentBottomPattern == -1 ||
                    indexOfCurrentRightPattern == -1 ||
                    indexOfCurrentLeftPattern == -1)
                {
                    continue;
                }

                patterns[indexOfCurrentPattern].TryAddNeighbor(indexOfCurrentTopPattern, Direction.top);
                patterns[indexOfCurrentPattern].TryAddNeighbor(indexOfCurrentBottomPattern, Direction.bottom);
                patterns[indexOfCurrentPattern].TryAddNeighbor(indexOfCurrentRightPattern, Direction.right);
                patterns[indexOfCurrentPattern].TryAddNeighbor(indexOfCurrentLeftPattern, Direction.left);
            }
        }
    }
}
