using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Floorplan_Creator
{
    class FloorDraw
    {
        static private Bitmap floor;
        static private Pen pointPen = new Pen(Brushes.Blue);
        static private Pen cyclePen = new Pen(Brushes.Orange);
        static private Pen linePen = new Pen(Brushes.Green);

        // creates an empty bitmap to the max size of the coordinates given.
        static private void DrawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graph.FillRectangle(Brushes.FloralWhite, ImageSize);               
            }
            floor = bmp;
        }

        // creates a area where a possible large room could be.
        static private void CycleDraw(int[,] nodePos)
        {

            int arraySize = (nodePos.GetLength(0));
            int X1, X2, Y1, Y2, lineX1, lineY1, lineX2, lineY2;
            Queue cycleX = new Queue();
            Queue cycleY = new Queue();
            for (int i = 0; i < arraySize; i++)
            {
                X1 = nodePos[i, 0];
                Y1 = nodePos[i, 1];
                for (int j = i; j < arraySize; j++)
                {
                    X2 = nodePos[j, 0];
                    Y2 = nodePos[j, 1];

                    cycleX.Enqueue(X2);
                    cycleY.Enqueue(Y2);

                    if (X1 == X2 && Y1 == Y2)
                    {
                        // draw shape with queue of previous coords and empty queue.
                        int queueSize = cycleX.Count - 1;
                        for (int k = 0; k < queueSize; k++)
                        {
                            lineX1 = (int)cycleX.Dequeue();
                            lineY1 = (int)cycleY.Dequeue();
                            lineX2 = (int)cycleX.Peek();
                            lineY2 = (int)cycleY.Peek();
                            LineDraw(lineX1, lineY1, lineX2, lineY2, cyclePen);
                        }
                    }
                }
                cycleX.Clear();
                cycleY.Clear();
            }
        }

        //creates coloured pixels at the location specificied in the array.
        static private void MarkMap(int[,] walkPos)
        {
            int xCoord;
            int yCoord;
            int arraySize = walkPos.GetLength(0);

            for (int i = 0; i < arraySize; i++)
            {
                xCoord = walkPos[i, 0];
                yCoord = walkPos[i, 1];

                LineDraw(xCoord-1, yCoord-1, xCoord+1, yCoord+1, pointPen);
            }

        }

        //draws a line on the bitmap.
        static private void LineDraw(int x1, int y1, int x2, int y2, Pen pen)
        {       
            using (Graphics graph = Graphics.FromImage(floor))
            {
                graph.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        //called to initiate the creation and drawing of the bitmap.
        static public Bitmap CreateFloor(int[,] nodePos)
        {
            int arraySize = nodePos.GetLength(0);
            linePen.Width = 1.0f;
            cyclePen.Width = 4.0f;
            pointPen.Width = 4.0f;

            //finds the max x and y coordinates so the size of the bitmap can be determined.
            int maxX = 1;
            int maxY = 1;
            for (int i = 0; i < arraySize; i++)
            {
                if (nodePos[i,0] > maxX)
                {
                    maxX = nodePos[i, 0];
                }
                if (nodePos[i, 1] > maxY)
                {
                    maxY = nodePos[i, 1];
                }
            }

            DrawFilledRectangle(maxX+50, maxY+50);

            //draws highlights for the cycles
            CycleDraw(nodePos);

            //inputs the coordinates for creating the lines.
            for (int i = 0; i <= arraySize-2; i++)
            {
                int x1 = nodePos[i, 0];
                int y1 = nodePos[i, 1];
                int x2 = nodePos[i + 1, 0];
                int y2 = nodePos[i + 1, 1];
                LineDraw(x1, y1, x2, y2, linePen);
            }

            MarkMap(nodePos);

            linePen.Dispose();
            return floor;          
        }

    }
}
