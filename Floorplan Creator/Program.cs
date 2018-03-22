using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Floorplan_Creator
{
    class Program
    {
        // removes repeats that occur straight after one another.
        static public int[,] RemoveRepeats(int[,] coords)
        {
            int[,] returnCoords;
            int X1, X2, Y1, Y2;
            int arraySize = (coords.GetLength(0)-1);
            Queue coordX = new Queue();
            Queue coordY = new Queue();

            for (int i = 0; i < arraySize; i++)
            {
                X1 = coords[i, 0];
                Y1 = coords[i, 1];
                X2 = coords[i+1, 0];
                Y2 = coords[i+1, 1];
                if (X1 != X2 && Y1 != Y2)
                {
                    coordX.Enqueue(X1);
                    coordY.Enqueue(Y1);
                }
                if (i < arraySize+1)
                {
                    coordX.Enqueue(X2);
                    coordY.Enqueue(Y2);
                }
            }

            int queueSize = coordX.Count;
            returnCoords = new int[queueSize, 2];
            for (int i = 0; i < queueSize; i++)
            {
                returnCoords[i, 0] = (int)coordX.Dequeue();
                returnCoords[i, 1] = (int)coordY.Dequeue();
            }

            return returnCoords;
        }

            //rounds coordinates together if they are close enough.
        static public int[,] RoundCoords(int[,] coords)
        {
            int arraySize = (coords.GetLength(0));
            int X1, X2, Y1, Y2;
            int roundTo = 100;
            for (int i = 0; i < arraySize; i++)
            {
                X1 = coords[i, 0];
                Y1 = coords[i, 1];
                for (int j = 0; j < arraySize; j++)
                {
                    X2 = coords[j, 0];
                    Y2 = coords[j, 1];
                    if ((X1- roundTo <= X2) && (X1+ roundTo >= X2) && (Y1 - roundTo <= Y2) && (Y1 + roundTo >= Y2) )
                    {
                        coords[j, 0] = X1;
                        coords[j, 1] = Y1;
                    }
                }
            }
                return coords;
        }

        // trys to round the path of the coords.
        // finds the mid point of (x1,y1) and (x3,y3) which is (xm,ym), then 25% of (xm,ym) and 75% of (x2,y2).
        // then writes the new coord to (x2,y2) in the array. It goes through the entire array doing this.
        static public int[,] SmoothPath (int[,] coords)
        {
            int arraySize = (coords.GetLength(0))-2;
            double X1, X2, X3, Xm, Xm2, Xm3, Y1, Y2, Y3, Ym, Ym2, Ym3;
            for (int i = 0; i < arraySize; i++)
            {
                X1 = coords[i, 0];
                Y1 = coords[i, 1];
                X2 = coords[i + 1, 0];
                Y2 = coords[i + 1, 1];
                X3 = coords[i + 2, 0];
                Y3 = coords[i + 2, 1];
                Xm = (X1 + X3) / 2;
                Ym = (Y1 + Y3) / 2;
                Xm2 = (X2 + Xm) / 2;
                Ym2 = (Y2 + Ym) / 2;
                Xm3 = (X2 + Xm2) / 2;
                Ym3 = (Y2 + Ym2) / 2;
                coords[i + 1, 0] = (int)Xm3;
                coords[i + 1, 1] = (int)Ym3;
            }

            return coords;
        }

        //read the file containing coordinates and put them into an array.
        static public int[,] ReadCoords(String fileRead)
        {
            int[,] walkCoords;
            string[] coords;
            int counter = 0;
            string line;
            // read the file to find amount of lines so array can be sized correctly.
            System.IO.StreamReader file = new System.IO.StreamReader(fileRead);
            while ((line = file.ReadLine()) != null)
            {
                counter++;
            }
            file.Close();

            walkCoords = new int[counter, 2];
            coords = new string[counter];
            counter = 0;

            //reads the file each line input into the array. (Xcoord, Ycoord)
            file = new System.IO.StreamReader(fileRead);
            while ((line = file.ReadLine()) != null)
            {
                
                //Debug.WriteLine(line);
                coords[counter] = line;
                counter++;
            }
            file.Close();

            //takes the string array then splits and converts to interger.
            int arraySize = coords.Length;
            for (int i = 0; i < arraySize; i++)
            {
                string pos = coords[i];

                var res = pos.Split(',').ToArray();
                walkCoords[i, 0] = Int32.Parse(res[0]);
                walkCoords[i, 1] = Int32.Parse(res[1]);
            }

                return walkCoords;
        }

        //creates a array for testing
        static public int[,] PopulateArray(int lenght)
        {
            int[,] walkCoords = new int[lenght, 2];
            int arraySize = walkCoords.GetLength(0);

            for (int i = 0; i < arraySize; i++)
            {
                walkCoords[i, 0] = i+1;
                walkCoords[i, 1] = i+1;
            }

            return walkCoords;
        }

        static void Main()
        {
            string file = "C:/Users/Ben/Documents/Visual Studio 2015/Projects/Floorplan Creator/Floorplan Creator/bin/Debug/CoordData.txt";
            int[,] walkCoords = RemoveRepeats(RoundCoords(SmoothPath(ReadCoords(file))));
            //Debug.WriteLine(walkCoords[1,0]);
            Bitmap floorplan = FloorDraw.CreateFloor(walkCoords);
            //saves the bitmap to file.jpeg
            floorplan.Save("file.jpeg", ImageFormat.Png);
        }
    }
}

