using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Wave_Function_Collapse_First_Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Image sampleImage = Image.FromFile("Path of the sample.");
            Bitmap sampleBitmap = new Bitmap(sampleImage);

            Color[,] colorMap = new Color[sampleBitmap.Width, sampleBitmap.Height]; //Making a 2D array of T

            for (int y = 0; y < sampleBitmap.Height; y++)
            {
                for (int x = 0; x < sampleBitmap.Width; x++)
                {
                    colorMap[x, y] = sampleBitmap.GetPixel(x, y);
                }
            }

            MapConverter<Color> mapConverter = new MapConverter<Color>(); //Claas for converting this 2D array of T in usable int 2D array
            mapConverter.Init(colorMap);

            int[,] map = mapConverter.Convert(colorMap); //The usable 2D array

            PatternMap patternMap = new PatternMap(map, sampleBitmap.Width);
            patternMap.FindPatterns(3); //Pattern of size 3 (so here 3 pixel per pattern)
            patternMap.GetPatternsNeighbor(); //^&<- Finding pattern and their rules
            
            WfcSolver solver = new WfcSolver(patternMap, 4269101);
            int[,] output = solver.Run(25); //Generate a output of 25x25 size (25 x 25 pattern so 75 * 75 pixel)


            Color[,] result = mapConverter.Convert(patternMap.Convert(output, 25)); //Convert it back to a pattern -> int -> color array

            Bitmap resultBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int y = 0; y < result.GetLength(1); y++)
            {
                for (int x = 0; x < result.GetLength(0); x++)
                {
                    resultBitmap.SetPixel(x, y, result[x, y]);
                }
            }

            resultBitmap.Save("Path for saving result.", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
