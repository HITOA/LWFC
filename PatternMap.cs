using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

public static class PatternMapSerializer
{
    public static void Save(List<Pattern> patterns, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SerializablePattern[]));

        SerializablePattern[] serializablePatterns = new SerializablePattern[patterns.Count];

        for (int i = 0; i < serializablePatterns.Length; i++)
        {
            serializablePatterns[i] = new SerializablePattern(patterns[i]);
        }

        TextWriter writer = new StreamWriter(path);
        serializer.Serialize(writer, serializablePatterns);
        writer.Close();
    }

    public static List<Pattern> Load(string path)
    {
        TextReader reader = new StreamReader(path);
        XmlSerializer serializer = new XmlSerializer(typeof(SerializablePattern[]));

        SerializablePattern[] serializablePatterns = (SerializablePattern[])serializer.Deserialize(reader);
        List<Pattern> patterns = new List<Pattern>(serializablePatterns.Length);

        foreach(SerializablePattern sPattern in serializablePatterns)
        {
            patterns.Add(sPattern.ToPattern());
        }

        return patterns;
    }
}

public class PatternMapper
{
    public List<Pattern> patterns;
    public MapHelper map;

    bool overlap;
    bool tileableX;
    bool tileableY;

    float at;

    public PatternMapper(int[,] map, bool overlap = false, bool tileableX = false, bool tileableY = false, float at = 3)
    {
        this.patterns = new List<Pattern>();
        this.map = new MapHelper(map, map.GetLength(0), map.GetLength(1));
        this.overlap = overlap;
        this.tileableX = tileableX;
        this.tileableY = tileableY;
        this.at = at;
    }

    public int[,] Convert(int[,] patternMap)
    {
        int trueSizeX = patternMap.GetLength(0) * patterns[0].size;
        int trueSizeY = patternMap.GetLength(1) * patterns[0].size;
        int[,] result = new int[trueSizeX, trueSizeY];

        for(int y = 0; y < trueSizeY; y+= patterns[0].size)
        {
            for (int x = 0; x < trueSizeX; x += patterns[0].size)
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
        if (map.width <= size || map.height <= size)
            return;

        for (int y = 0; y < map.height - (tileableY ? 0 : size + 1); y += overlap ? 1 : size)
        {
            for (int x = 0; x < map.width - (tileableX ? 0 : size + 1); x += overlap ? 1 : size)
            {
                Pattern currentPattern = new Pattern(size, at);
                currentPattern.data = map[x, y, size];

                if (!patterns.Contains(currentPattern))
                {
                    patterns.Add(currentPattern);
                }else
                {
                    Pattern tmp = patterns[patterns.IndexOf(currentPattern)];
                    tmp.count++;
                    patterns[patterns.IndexOf(currentPattern)] = tmp;
                }
            }
        }
    }

    public void GetPatternsNeighbor()
    {
        int size = patterns[0].size;

        for (int y = 0; y < map.height - (tileableY ? 0 : size + 1); y+= overlap ? 1 : size)
        {
            for (int x = 0; x < map.width - (tileableX ? 0 : size + 1); x+= overlap ? 1 : size)
            {
                Pattern currentPattern = new Pattern(size, at);
                currentPattern.data = map[x, y, size];

                Pattern currentTopPattern = new Pattern(size, at);
                currentTopPattern.data = map[x, y + size, size];

                Pattern currentBottomPattern = new Pattern(size, at);
                currentBottomPattern.data = map[x, y - size, size];

                Pattern currentRightPattern = new Pattern(size, at);
                currentRightPattern.data = map[x + size, y, size];

                Pattern currentLeftPattern = new Pattern(size, at);
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
