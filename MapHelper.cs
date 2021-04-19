class MapHelper
{
    public int size
    {
        get;
        private set;
    }

    int[,] map;

    public int this[int x, int y]
    {
        get {
            return map[mod(x, size), mod(y, size)];
        }

        set
        {
            map[mod(x, size), mod(y, size)] = value;
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

    public MapHelper(int[,] map, int size)
    {
        this.map = map;
        this.size = size;
    }

    private int mod(int x, int m) //source : https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    {
        return (x % m + m) % m;
    }
}
