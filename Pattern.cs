using System;
using System.Collections.Generic;
using System.Linq;

struct Pattern : IEquatable<Pattern>
{
    public int[,] data;
    public int size;

    public List<int> top;
    public List<int> bottom;
    public List<int> right;
    public List<int> left;

    public Pattern(int size)
    {
        data = new int[size, size];
        this.size = size;

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
