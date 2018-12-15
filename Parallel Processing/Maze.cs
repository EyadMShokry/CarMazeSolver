using System;
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

        public CarNode(Point Point, int direction, int currentpath)
        {
            this.Point = Point;
            this.direction = direction;
            this.currentpath = currentpath;
        }
        public Point Point;
        public int direction;
        public int currentpath;
    };
    class Maze
    {
        int[] direction_x = { 0, 1, 0, -1 };
        int[] direction_y = { -1, 0, 1, 0 };
        int MazeBoard_x;
        int MazeBoard_y;
        bool[,] boolPoint;
        Dictionary<Tuple<Point, int>, CarNode> par;
        List<Tuple<Point, int>> visitedNodes;
        List<Point> BlockedNodes;
        Point startPoint;
        Point endPoint;
        CarNode node;
        Queue<CarNode> queue;
        private Stopwatch sw;
        private TimeSpan ts;
        List<Task> Tasks;
        CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Mutex m_lock = new Mutex();
        bool finish;
        private List<CarNode> Resultnodes;

        public Maze()
        {
            startPoint = new Point(-1, -1);
            endPoint = new Point(-1, -1);
            BlockedNodes = new List<Point>();
            sw = new Stopwatch();

        }
        public void Fill_data_from_file(string file_path)
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
                        BlockedNodes.Add(new Point(j, i));
                    }
                }
            }
        }
        public void Sequential_Solve()
        {
            sw.Reset();
            sw.Start();
            if (startPoint == new Point(-1, -1) || endPoint == new Point(-1, -1))
            {
                MessageBox.Show("Start Point and End Point Required", "Hey !!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (BFS(startPoint))
            {
                List<CarNode> PATH = new List<CarNode>();
                PATH.Add(node);
                while (node.path != 0)
                {
                    Tuple<Point, int> tuple = new Tuple<Point, int>(node.Point, node.direction);
                    node = par[tuple];
                    PATH.Add(node);
                }
                ts = sw.Elapsed;
                Console.WriteLine(ts.ToString());
                String Time = string.Format("Timer:{0,2}.{1,2}", ts.Seconds, ts.Milliseconds);
                sw.Stop();
                ResultForm ResultForm = new ResultForm(MazeBoard_x, MazeBoard_y, startPoint, endPoint, BlockedNodes, PATH, Time);
                ResultForm.ShowDialog();
            }
            else
                MessageBox.Show("No Path Found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public bool valid(Point Point)  //check valid position and valid cell
        {
            if (BlockedNodes.Contains(Point) || Point.X >= MazeBoard_x || Point.Y >= MazeBoard_y || Point.X < 0 || Point.Y < 0)
                return false;
            return true;
        }

        private bool BFS(Point point)
        {
            queue = new Queue<CarNode>();
            node = new CarNode(point, 0, 0);
            par = new Dictionary<Tuple<Point, int>, CarNode>();
            visitedNodes = new List<Tuple<Point, int>>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                node = queue.Dequeue();
                if (node.Point == endPoint)
                    return true;
                for (int i = 0; i < 2; ++i)
                {
                    int new_direction = (node.direction + i) % 4;
                    Point new_point = new Point(node.Point.X + direction_x[new_direction], node.Point.Y + direction_y[new_direction]);
                    Tuple<Point, int> tuple = new Tuple<Point, int>(new_point, new_direction);
                    if (valid(new_point) && !visitedNodes.Contains(tuple))
                    {
                        queue.Enqueue(new CarNode(new_point, new_direction, node.path + 1));
                        par[tuple] = new CarNode(node.Point, node.direction, node.path);
                        visitedNodes.Add(tuple);
                    }
                }
            }
            return false;
        }
        public async void Parallel_Solve()
        {
            if (startPoint == new Point(-1, -1) || endPoint == new Point(-1, -1))
            {
                MessageBox.Show("Start Point and End Point Required", "Hey !!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                bool tsk = await Operation();
                if (!tsk)
                {
                    ts = sw.Elapsed;
                    sw.Stop();
                    MessageBox.Show("No Path", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ts = sw.Elapsed;
                    string Timer = string.Format("Timer:{0,2}.{1,2}", ts.Seconds, ts.Milliseconds);
                    sw.Stop();
                    ResultForm ResultForm = new ResultForm(MazeBoard_x, MazeBoard_y, startPoint, endPoint, BlockedNodes, Resultnodes,Timer);
                    ResultForm.ShowDialog();
                }
            }
        }
        private async Task<bool> Operation()
        {
            try
            {
                sw.Reset();
                sw.Start();
                cts = new CancellationTokenSource();
                List<CarNode> Lis = new List<CarNode>();
                Tasks = new List<Task>();
                Lis.Add(new CarNode(startPoint, 0, 0));
                this.finish = false;
                Task task = Task.Run(() => MyThread(Lis, cts.Token));
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

        private void MyThread(List<CarNode> nodes, CancellationToken cts)
        {
            List<CarNode> ls = new List<CarNode>();
            CarNode node = nodes.Last();
            m_lock.WaitOne();
            if (cts.IsCancellationRequested)
            {
                m_lock.ReleaseMutex();
                return;
            }
            if (node.Point == endPoint)
            {
                this.cts.Cancel();
                Resultnodes = nodes;
                this.finish = true;
                m_lock.ReleaseMutex();
                return;
            }
            m_lock.ReleaseMutex();
            for (int i = 0; i < 2; ++i)
            {
                int new_direction = (node.direction + i) % 4;
                Point new_point = new Point(node.Point.X + direction_x[new_direction], node.Point.Y + direction_y[new_direction]);
                if (valid(new_point) && !nodes.Exists(n => n.Point == new_point && n.direction == new_direction))
                    ls.Add(new CarNode(new_point, new_direction, node.path + 1));
            }
            if (ls.Count == 2)
            {

                List<CarNode> rnode = new List<CarNode>(nodes);
                List<CarNode> lnode = new List<CarNode>(nodes);
                rnode.Add(ls.Last());
                lnode.Add(ls.First());
                Task task = Task.Run(() => MyThread(rnode, cts));
                Tasks.Add(task);
                MyThread(lnode, cts);
            }
            else if (ls.Count == 1)
            {
                nodes.Add(ls.Last());
                MyThread(nodes, cts);
            }
            return;
        }

        
    }
}
