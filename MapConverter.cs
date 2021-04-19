using System.Collections.Generic;

namespace Wave_Function_Collapse_First_Test
{
    class MapConverter<T>
    {
        public List<T> correspondence;

        public MapConverter()
        {
            correspondence = new List<T>();
        }

        public void Init(T[,] data)
        {
            foreach (T element in data)
            {
                if (!correspondence.Contains(element))
                    correspondence.Add(element);
            }
        }

        public int[,] Convert(T[,] data)
        {
            int[,] result = new int[data.GetLength(0), data.GetLength(1)];

            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    result[x,y] = correspondence.IndexOf(data[x, y]);
                }
            }

            return result;
        }

        public T[,] Convert(int[,] data)
        {
            T[,] result = new T[data.GetLength(0), data.GetLength(1)];

            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    result[x, y] = correspondence[data[x, y]];
                }
            }

            return result;
        }
    }
}
