using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DZ_Less6
{
    internal class Program
    {
        static Random random;

        static void Main(string[] args)
        {
            random = new Random(DateTime.Now.Millisecond);
            Stopwatch stopwatch = new Stopwatch();
            // Паралельное выполнение выигрывает начинает выигрывать в матрицах размером от 1000х1000
            int rows = 1000; 
            int columns = 1000;
            int[][] matrixA = null;
            int[][] matrixB = null;
            int[][] matrixC = null;
            Task[] task = new Task[2];
            task[0] = Task.Run(() => matrixA = GenerateMatrix(rows, columns));
            task[1] = Task.Run(() => matrixB = GenerateMatrix(rows, columns));
            Task.WaitAll(task);

            //matrixA = GenerateMatrixAsync(5, 5).Result;
            //matrixB = GenerateMatrixAsync(5, 5).Result;

            //PrintMatrix(matrixA);
            //Console.WriteLine();
            //PrintMatrix(matrixB);
            //Console.WriteLine();
            stopwatch.Start();
            matrixC = MultiplyMatrixAsync(matrixA, matrixB).Result;
            stopwatch.Stop();
            Console.WriteLine($"Paralel {stopwatch.Elapsed}");
            stopwatch.Reset();
            //PrintMatrix(matrixC);
            matrixC = null;
            GC.Collect();
            stopwatch.Start();
            matrixC = MultiplyMatrixNoParalel(matrixA, matrixB);
            stopwatch.Stop();
            Console.WriteLine($"NoParalel {stopwatch.Elapsed}");
            //PrintMatrix(matrixC);
            Console.ReadLine();
        }

        static async Task<int[][]> GenerateMatrixAsync(int row, int col)
        {
            int[][] result = await Task.Run(() => GenerateMatrix(row, col));
            return result;
        }

        static int[][] GenerateMatrix(int row, int col)
        {
            Task[] task = new Task[row];
            int[][] result = new int[row][];
            for (int i = 0; i < row; i++)
            {
                int c = i;
                task[i] = Task.Run(()=> result[c] = GenerateRow(col));
            }
            Task.WaitAll(task);
            return result;
        }

        static int[] GenerateRow(int col)
        {
            int[] result = new int[col];
            for (int i = 0; i < col; i++)
            {
                result[i] = random.Next(10);
            }
            return result;
        }

        static void PrintMatrix(int [][] matrix)
        {
            if (matrix == null)
                return;
            int col = matrix[0].Length;
            int row = matrix.Length;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                    Console.Write($"{matrix[i][j],5}");
                Console.WriteLine();
            }
        }

        static async Task<int[][]> MultiplyMatrixAsync(int[][] mA, int[][] mB)
        {
            return await Task.Run(() => MultiplyMatrix(mA, mB));
        }

        static int[][] MultiplyMatrix(int[][] mA, int[][] mB)
        {
            if (mA[0].Length != mB.Length)
                return null;
            int[][] result = new int[mA.Length][];
            for (int i = 0; i < mA.Length; i++)
            {
                result[i] = new int[mB[0].Length];
            }
            Task[] tasks = new Task[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[0].Length; j++)
                {
                    int i1 = i;
                    int j1 = j;
                    tasks[j1] = Task.Run(() => result[i1][j1] = MultiplyElement(i1, j1, mA, mB));
                }
                Task.WaitAll(tasks);
            }
            
            return result;
        }

        static int[][] MultiplyMatrixNoParalel(int[][] mA, int[][] mB)
        {
            if (mA[0].Length != mB.Length)
                return null;
            int[][] result = new int[mA.Length][];
            for (int i = 0; i < mA.Length; i++)
            {
                result[i] = new int[mB[0].Length];
            }
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[0].Length; j++)
                {
                    int i1 = i;
                    int j1 = j;
                    result[i1][j1] = MultiplyElement(i1, j1, mA, mB);
                }
            }
            return result;
        }

        static int MultiplyElement(int r, int c, int[][] mA, int[][] mB)
        {
            int result = 0;
            for (int i = 0;i < mA[0].Length; i++ )
            {
                result += mA[r][i] * mB[i][c];
            }
            return result;
        }
    }

}
