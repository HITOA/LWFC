using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

public struct SerializablePattern
{
    public int[] data;
    public int size;
    public int[] top;
    public int[] bottom;
    public int[] right;
    public int[] left;

    public SerializablePattern(Pattern pattern)
    {
        data = new int[pattern.size * pattern.size];
        size = pattern.size;
        top = pattern.top.ToArray();
        bottom = pattern.bottom.ToArray();
        right = pattern.right.ToArray();
        left = pattern.left.ToArray();

        int index = 0;
        foreach(int d in pattern.data)
        {
            data[index] = d;
            index++;
        }
    }

    public Pattern ToPattern()
    {
        Pattern ret = new Pattern();

        ret.data = new int[size, size];
        ret.size = size;
        ret.top = new List<int>(top);
        ret.bottom = new List<int>(bottom);
        ret.right = new List<int>(right);
        ret.left = new List<int>(left);

        int index = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                ret.data[x, y] = data[index];
                index++;
            }
        }

        return ret;
    }
}

public struct Pattern : IEquatable<Pattern>
{
    public int[,] data;
    public int size;
    public int count;
    public float at;
    public float weight
    {
        get
        {
            return (float)Math.Log10((Math.Pow((double)count / at, (double)at)) + 1);
        }
    }

    public List<int> top;
    public List<int> bottom;
    public List<int> right;
    public List<int> left;

    public Pattern(int size, float at)
    {
        data = new int[size, size];
        this.size = size;
        count = 1;
        this.at = at;

        top = new List<int>();
        bottom = new List<int>();
        right = new List<int>();
        left = new List<int>();
    }

    public bool IsValid(int[] a, Direction dir)
    {
        List<int> valid = GetNeighborListByDir(dir);

        foreach (int i in a)
        {
            if (valid.Contains(i))
                return true;
        }

        return false;
    }

    public List<int> GetNeighborListByDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.top:
                return top;
            case Direction.bottom:
                return bottom;
            case Direction.right:
                return right;
            case Direction.left:
                return left;
            default:
                return new List<int>();
        }
    }

    public void TryAddNeighbor(int id, Direction dir)
    {
        switch(dir)
        {
            case Direction.top:
                if (!top.Contains(id))
                    top.Add(id);
                break;
            case Direction.bottom:
                if (!bottom.Contains(id))
                    bottom.Add(id);
                break;
            case Direction.right:
                if (!right.Contains(id))
                    right.Add(id);
                break;
            case Direction.left:
                if (!left.Contains(id))
                    left.Add(id);
                break;
            default:
                break;
        }
    }

    public bool Equals(Pattern pattern)
    {
        return data.Cast<int>().SequenceEqual(pattern.data.Cast<int>());
    }

    public static bool operator ==(Pattern a, Pattern b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Pattern a, Pattern b)
    {
        return !a.Equals(b);
    }
}
