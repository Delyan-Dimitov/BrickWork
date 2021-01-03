namespace BrickWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    class Program
    {
        static void Main(string[] args)
        {
            var dimensions = Console.ReadLine()
                .Split(" ")
                .Select(int.Parse)
                .ToArray();

            var height = dimensions[0];
            var width = dimensions[1];

            var firstLayer = ReadFirstLayer(height, width);

            if (!isInputValid(height, width, firstLayer))
            {
                Console.WriteLine("Invalid Input!");
            }
            else
            {
                var validPositionsNeeded = (height * width) / 2;
                var validPositions = FindValidPositions(firstLayer, height, width);
                if (validPositions.Count >= validPositionsNeeded)
                {
                    var secondLayer = AssignSecondLayer(height, width, validPositions, validPositionsNeeded);
                    PrintSecondLayer(secondLayer, height, width);

                }
                else
                {
                    Console.WriteLine(-1);
                }
            }


        }
        public static bool isInputValid(int height, int width, int[,] firstLayer) // Validation for the input
        {
            if ((height % 2 != 0 || width % 2 != 0) && (height > 100 || width > 100)) // I am checking if the the dimesions are even and under 100
            {
                return false;
            }

            var dict = new Dictionary<int, int>(); // I am checking that no brick spans more than 2 blocks
            foreach (var item in firstLayer)
            {
                if (!dict.ContainsKey(item))
                {
                    dict.Add(item, 0);
                    continue;
                }
                else if (dict[item] > 2)
                {
                    return false;
                }
                else
                {
                    dict[item] = dict[item]++;
                }
            }
            return true;
        }
        public static int[,] ReadFirstLayer(int height, int width) // Read the first layer from the console
        {
            var firstLayer = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                var row = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();
                for (int j = 0; j < width; j++)
                {
                    firstLayer[i, j] = row[j];
                }
            }
            return firstLayer;
        }
        public static List<int[]> FindValidPositions(int[,] firstLayer, int height, int width) // Iterate through the first layer and find the possible brick positions
        {
            var validPositions = new List<int[]>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j - 1 >= 0)
                    {
                        if (firstLayer[i, j] != firstLayer[i, j - 1])
                        {
                            var position = new int[] { i, j, i, j - 1 };

                            validPositions = AddToValidPositions(validPositions, position);

                        }
                    }
                    if (j + 1 < width)
                    {

                        if (firstLayer[i, j] != firstLayer[i, j + 1])
                        {
                            var postion = new int[] { i, j, i, j + 1 };
                            validPositions = AddToValidPositions(validPositions, postion);
                        }
                    }
                    if (i - 1 >= 0)
                    {
                        if (firstLayer[i, j] != firstLayer[i - 1, j])
                        {
                            var position = new int[] { i, j, i - 1, j };
                            validPositions = AddToValidPositions(validPositions, position);

                        }
                    }

                    if (i + 1 < height)
                    {
                        if (firstLayer[i, j] != firstLayer[i + 1, j])
                        {
                            var position = new int[] { i, j, i + 1, j };
                            validPositions = AddToValidPositions(validPositions, position);
                        }
                    }
                }
            }

            return validPositions;
        }

        public static List<int[]> AddToValidPositions(List<int[]> validPositions, int[] newPosition) // Check weather a postions isn't already added or is in conflict with another
        {
            // I check if any of the indexes of the new positions are already taken since i can only have one brick that contains the index [0,0] for example
            var conflictAtFirstElement = validPositions.Any(x =>
            (x[0] == newPosition[0] && x[1] == newPosition[1]) ||
            x[0] == newPosition[2] && x[1] == newPosition[3]);

            var conflictAtSecondElement = validPositions.Any(x =>
           (x[2] == newPosition[0] && x[3] == newPosition[1]) ||
           x[2] == newPosition[2] && x[3] == newPosition[3]);


            /* I make sure that the new position isn't already contained in the list and also check that its mirrored version is also not containted since
             * [1,2][3,2] is the same brick as  [3,2][1,2]
            */
            if (!conflictAtFirstElement && !conflictAtSecondElement)
            {
                var newPositionMirrored = new int[] { newPosition[2], newPosition[3], newPosition[0], newPosition[1] };
                if (!validPositions.Contains(newPosition) && !validPositions.Contains(newPositionMirrored))
                {
                    validPositions.Add(newPosition);
                }

            }
            return validPositions;
        }

        public static int[,] AssignSecondLayer(int height, int width, List<int[]> validPositions, int validPositionsNeeded)
        // Assign bricks to the second layer based ot the valid placements i've found
        {
            var secondLayer = new int[height, width];
            for (int i = 1; i <= validPositionsNeeded; i++)
            {
                var position = validPositions[i - 1];
                secondLayer[position[0], position[1]] = i;
                secondLayer[position[2], position[3]] = i;
            }
            return secondLayer;
        }

        public static void PrintSecondLayer(int[,] secondLayer, int height, int width) // Print the output based on the given requirements
        {
            var mainSb = new StringBuilder();

            for (int i = 0; i < height; i++)
            {
                var rowSb = new StringBuilder();
                for (int j = 0; j < width; j++)
                {

                    if (j + 1 < width)
                    {
                        if (secondLayer[i, j] == secondLayer[i, j + 1])
                        {
                            rowSb.Append($"{secondLayer[i, j]}  ");
                        }
                        else
                        {
                            rowSb.Append($"{secondLayer[i, j]} * ");
                        }
                    }
                    else
                    {
                        rowSb.Append(secondLayer[i, j]);
                    }

                }
                mainSb.AppendLine(rowSb.ToString().TrimEnd());
                var columnSb = new StringBuilder();
                for (int k = 0; k < width; k++)
                {
                    if (i + 1 < height)
                    {

                        if (secondLayer[i, k] == secondLayer[i + 1, k])
                        {
                            columnSb.Append(" ");
                        }
                        else
                        {
                            if (secondLayer[i, k] >= 10)
                            {
                                columnSb.Append("* * *");
                            }
                            else
                            {
                                columnSb.Append("* *");
                            }

                        }
                        columnSb.Append(" ");
                    }
                }
                mainSb.AppendLine(columnSb.ToString().TrimEnd());
            }
            Console.WriteLine(mainSb.ToString().TrimEnd());
        }
    }
}

