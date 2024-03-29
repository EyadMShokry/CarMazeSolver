﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parallel_Processing
{
    public struct CarNode
    {

        public CarNode(Point Point, int direction, int path)
        {
            this.Point = Point;
            this.direction = direction;
            this.path = path;
        }
        public Point Point;
        public int direction;
        public int path;
    };
    class Maze
    {
        int[] direction_x = { 0, 1, 0, -1 };
        int[] direction_y = { -1, 0, 1, 0 };
        int MazeBoard_x;
        int MazeBoard_y;
        bool[,] boolPoint;
        Dictionary<Tuple<Point, int>, CarNode> PathReference;
        List<Tuple<Point, int>> visitedNodes;
        List<Point> BlockedPoints;
        Point startPoint;
        Point endPoint;
        CarNode node;
        Queue<CarNode> queue;
        private Stopwatch sw;
        private TimeSpan ts;
        List<Task> Tasks;
        CancellationTokenSource cantoks = new CancellationTokenSource();
        private readonly Mutex mutex_lock = new Mutex();
        bool finish;
        private List<CarNode> Resultnodes;

        public Maze(string pa)
        {
            startPoint = new Point(-1, -1);
            endPoint = new Point(-1, -1);
            BlockedPoints = new List<Point>();
            sw = new Stopwatch();
            Fill_data_from_file(pa);
        }
        private void Fill_data_from_file(string file_path)
        {
            string fileContent = File.ReadAllText(file_path);
            string[] rows = fileContent.Split('\n');
            int row_char_Length = rows[0].Length - 1;
            int rowCount = rows.Length;
            MazeBoard_x = row_char_Length;
            MazeBoard_y = rowCount;
            boolPoint = new bool[MazeBoard_x, MazeBoard_y];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < row_char_Length; j++)
                {
                    if (rows[i][j] == 'E')
                    {
                        endPoint = new Point(j, i);
                    }
                    else if (rows[i][j] == '^')
                    {
                        startPoint = new Point(j, i);
                    }
                    else if (rows[i][j] == '*')
                    {
                        boolPoint[j, i] = true;
                        BlockedPoints.Add(new Point(j, i));
                    }
                }
            }
        }
        private bool is_valid(Point Point)
        {
            if (BlockedPoints.Contains(Point) || Point.X >= MazeBoard_x || Point.Y >= MazeBoard_y || Point.X < 0 || Point.Y < 0)
                return false;
            return true;
        }

        public void Sequential_Solve()
        {
            sw.Reset();
            sw.Start();
            if (startPoint == new Point(-1, -1) || endPoint == new Point(-1, -1))
            {
                MessageBox.Show("Start Point or End Point isn't set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                queue = new Queue<CarNode>();
                node = new CarNode(startPoint, 0, 0);
                visitedNodes = new List<Tuple<Point, int>>();
                PathReference = new Dictionary<Tuple<Point, int>, CarNode>();
                queue.Enqueue(node);
                bool status = false;
                while (queue.Count > 0)
                {
                    node = queue.Dequeue();
                    if (node.Point == endPoint)
                    {
                        status = true;
                        List<CarNode> PATH = new List<CarNode>();
                        PATH.Add(node);
                        while (node.path != 0)
                        {
                            Tuple<Point, int> tuple = new Tuple<Point, int>(node.Point, node.direction);
                            node = PathReference[tuple];
                            PATH.Add(node);
                        }
                        ts = sw.Elapsed;
                        String Time = string.Format("Timer:{0,2}.{1:00}", ts.Seconds, ts.Milliseconds);
                        sw.Stop();
                        ResultForm resultForm = new ResultForm(MazeBoard_x, MazeBoard_y, startPoint, endPoint, BlockedPoints, PATH, Time);
                        resultForm.ShowDialog();
                        break;
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        int new_direction = (node.direction + i) % 4;
                        Point new_point = new Point(node.Point.X + direction_x[new_direction], node.Point.Y + direction_y[new_direction]);
                        Tuple<Point, int> new_reference = new Tuple<Point, int>(new_point, new_direction);
                        if (is_valid(new_point) && !visitedNodes.Contains(new_reference))
                        {
                            queue.Enqueue(new CarNode(new_point, new_direction, node.path + 1));
                            PathReference[new_reference] = new CarNode(node.Point, node.direction, node.path);
                            visitedNodes.Add(new_reference);
                        }
                    }
                }
                if(status == false)
                    MessageBox.Show("No Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
                          
        }

        public async void Parallel_Solve()
        {
            if (startPoint == new Point(-1, -1) || endPoint == new Point(-1, -1))
            {
                MessageBox.Show("Start Point or End Point isn't set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                bool booltask = await Parallel_Search_function();
                if (!booltask)
                {
                    ts = sw.Elapsed;
                    sw.Stop();
                    MessageBox.Show("No Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ts = sw.Elapsed;
                    string Timer = string.Format("Timer:{0,2}.{1:00}", ts.Seconds, ts.Milliseconds);
                    sw.Stop();
                    ResultForm ResultForm = new ResultForm(MazeBoard_x, MazeBoard_y, startPoint, endPoint, BlockedPoints, Resultnodes,Timer);
                    ResultForm.ShowDialog();
                }
            }
        }
        private async Task<bool> Parallel_Search_function()
        {
            try
            {
                sw.Reset();
                sw.Start();
                cantoks = new CancellationTokenSource();
                List<CarNode> Lis = new List<CarNode>();
                Tasks = new List<Task>();
                Lis.Add(new CarNode(startPoint, 0, 0));
                this.finish = false;
                Task task = Task.Run(() => NodesThread(Lis, cantoks.Token));
                Tasks.Add(task);
                await Task.WhenAny(Tasks);
                if (this.finish)
                    return true;
                else return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return false;
            }

        }

        private void NodesThread(List<CarNode> nodes, CancellationToken cantoks)
        {
            List<CarNode> nodes_lst = new List<CarNode>();
            CarNode node = nodes.Last();
            mutex_lock.WaitOne();
            if (cantoks.IsCancellationRequested)
            {
                mutex_lock.ReleaseMutex();
                return;
            }
            if (node.Point == endPoint)
            {
                this.cantoks.Cancel();
                Resultnodes = nodes;
                this.finish = true;
                mutex_lock.ReleaseMutex();
                return;
            }
            mutex_lock.ReleaseMutex();
            for (int i = 0; i < 2; ++i)
            {
                int new_direction = (node.direction + i) % 4;
                Point new_point = new Point(node.Point.X + direction_x[new_direction], node.Point.Y + direction_y[new_direction]);
                if (is_valid(new_point) && !nodes.Exists(n => n.Point == new_point && n.direction == new_direction))
                    nodes_lst.Add(new CarNode(new_point, new_direction, node.path + 1));
            }
            if (nodes_lst.Count == 2)
            {

                List<CarNode> right_node = new List<CarNode>(nodes);
                List<CarNode> left_node = new List<CarNode>(nodes);
                right_node.Add(nodes_lst.Last());
                left_node.Add(nodes_lst.First());
                Task task = Task.Run(() => NodesThread(right_node, cantoks));
                Tasks.Add(task);
                NodesThread(left_node, cantoks);
            }
            else if (nodes_lst.Count == 1)
            {
                nodes.Add(nodes_lst.Last());
                NodesThread(nodes, cantoks);
            }
            return;
        }

        
    }
}
