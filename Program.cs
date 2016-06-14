using System;
using System.Drawing;

namespace PictureGeneticAlgorithm
{
    class Program
    {
        const int PRINT_EVERY_X = 50000;
        static Random rand;

        static void Main(string[] args)
        {
            rand = new Random();
            string numberFmt= "00";
            string baseImageLocation = @"C:\Users\Thomas\Pictures\geneticTarget.bmp";
            string saveImageLocationFormat = @"C:\Users\Thomas\Pictures\geneticResult{0}.bmp";
            Bitmap target = new Bitmap(baseImageLocation);
            Bitmap bestGuess = new Bitmap(target.Width, target.Height);
            for(int x = 0; x < bestGuess.Width; x++)
            {
                for(int y = 0; y < bestGuess.Height; y++)
                {
                    bestGuess.SetPixel(x, y, Color.Black);
                }
            }
            int bestScore = test(target, bestGuess);
            int count = 0;
            int saveCount = 0;
            while (bestScore != 0 && saveCount < 20)
            {
                //Only print every PRINT_EVERY_X iterations
                if (count % PRINT_EVERY_X == 0)
                {
                    Console.WriteLine(String.Format("The best score is: {0}", bestScore));
                    bestGuess.Save(string.Format(saveImageLocationFormat, saveCount.ToString(numberFmt)));
                    saveCount++;
                    count = 1;
                }

                var mutationResults = mutateRandomPixel(bestGuess);
                Bitmap newGuess = mutationResults.Item1;
                int newScore = pixelChangeTest(mutationResults.Item2, mutationResults.Item3, target, newGuess, bestGuess, bestScore);
                if (newScore < bestScore)
                {
                    bestGuess = newGuess;
                    bestScore = newScore;
                    count++;
                }

            }

            bestGuess.Save(string.Format(saveImageLocationFormat, "Final"));
        }

        static int pixelChangeTest(int x, int y, Bitmap target, Bitmap newGuess, Bitmap bestGuess, int bestScore)
        {
            var targetPixel = target.GetPixel(x, y);
            var newPixel = newGuess.GetPixel(x, y);
            var bestPixel = bestGuess.GetPixel(x, y);

            int newDiff = 0;
            newDiff += Math.Abs(targetPixel.R - newPixel.R);
            newDiff += Math.Abs(targetPixel.G - newPixel.G);
            newDiff += Math.Abs(targetPixel.B - newPixel.B);

            int bestDiff = 0;
            bestDiff += Math.Abs(targetPixel.R - bestPixel.R);
            bestDiff += Math.Abs(targetPixel.G - bestPixel.G);
            bestDiff += Math.Abs(targetPixel.B - bestPixel.B);

            return (bestScore - bestDiff) + newDiff;
        }

        static Tuple<Bitmap, int, int> mutateRandomPixel(Bitmap start)
        {
            Bitmap newGuess = new Bitmap(start);

            var randX = rand.Next(newGuess.Width);
            var randY = rand.Next(newGuess.Height);

            var currPixel = newGuess.GetPixel(randX, randY);
            Color newColor = mutateColor(currPixel, 100); //play around with this
            newGuess.SetPixel(randX, randY, newColor);

            return Tuple.Create(newGuess, randX, randY);
        }

        private static Color mutateColor(Color currPixel, int variance)
        {
            var r = currPixel.R + (rand.Next(variance) - (variance / 2));
            var g = currPixel.G + (rand.Next(variance) - (variance / 2));
            var b = currPixel.B + (rand.Next(variance) - (variance / 2));
            r = wrapColor(r);
            g = wrapColor(g);
            b = wrapColor(b);
            return Color.FromArgb(255, r, g, b);
        }

        static int wrapColor(int val)
        {
            if (val < 0)
                return 256 + val;
            if (val >= 256)
                return val - 256;
            return val;
        }

        static int clampColor(int val)
        {
            if (val < 0)
                return 0;
            if (val > 255)
                return 255;
            return val;
        }

        static int test(Bitmap target, Bitmap testing)
        {
            int diff = 0;
            for (int x = 0; x < target.Width; x++)
            {
                for (int y = 0; y < target.Height; y++)
                {
                    var targetPixel = target.GetPixel(x, y);
                    var testPixel = testing.GetPixel(x, y);

                    diff += Math.Abs(targetPixel.R - testPixel.R);
                    diff += Math.Abs(targetPixel.G - testPixel.G);
                    diff += Math.Abs(targetPixel.B - testPixel.B);
                }
            }
            return diff;
        }
    }
}
