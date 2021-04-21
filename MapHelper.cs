public class MapHelper
{
    public int width
    {
        get;
        private set;
    }
    public int height
    {
        get;
        private set;
    }

    int[,] map;

    public int this[int x, int y]
    {
        get {
            return map[mod(x, width), mod(y, height)];
        }

        set
        {
            map[mod(x, width), mod(y, height)] = value;
        }
    }

    public int[,] this[int x, int y, int size]
    {
        get
        {
            int[,] data = new int[size, size];

            for (int _y = y; _y < y + size; _y++)
            {
                for (int _x = x; _x < x + size; _x++)
                {
                    data[_x - x, _y - y] = this[_x, _y];
                }
            }

            return data;
        }
    }

    public MapHelper(int[,] map, int width, int height)
    {
        this.map = map;
        this.width = width;
        this.height = height;
    }

    public void Rotate90()
    {
        int[,] tmp = new int[height, width];

        for (int y = 0; y < height; y ++)
        {
            for (int x = 0; x < width; x++)
            {
                tmp[y, x] = map[x, y];
            }
        }

        map = tmp;
        width = map.GetLength(0);
        height = map.GetLength(1);
    }

    private int mod(int x, int m) //source : https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    {
        return (x % m + m) % m;
    }
}
