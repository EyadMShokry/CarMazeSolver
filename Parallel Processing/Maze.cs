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
    public struct Node
    {

        public Node(Point Point, int dir, int path)
        {
            this.Point = Point;
            this.dir = dir;
            this.path = path;
        }
        public Point Point;
        public int dir;
        public int path;
    };
    class Maze
    {
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        int SIZEX;
        int SIZEY;
        bool[,] boolBox;
        Dictionary<Tuple<Point, int>, Node> par;
        List<Tuple<Point, int>> visited;
        List<Point> Blocked;
        Point startBox;
        Point endBox;
        Node node;
        Queue<Node> queue;
        private Stopwatch sw;
        private TimeSpan ts;
        List<Task> Tasks;
        CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Mutex m_lock = new Mutex();
        bool finish;
        private List<Node> Resultnodes;

        public Maze()
        {
            startBox = new Point(-1, -1);
            endBox = new Point(-1, -1);
            Blocked = new List<Point>();
            sw = new Stopwatch();

        }
        public void Fill_data_from_file(string file_path)
        {
            string fileContent = File.ReadAllText(file_path);
            string[] rows = fileContent.Split('\n');
            int row_char_Length = rows[0].Length - 1;
            int rowCount = rows.Length;
            SIZEX = row_char_Length;
            SIZEY = rowCount;
            boolBox = new bool[SIZEX, SIZEY];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < row_char_Length; j++)
                {
                    if (rows[i][j] == 'E')
                    {
                        endBox = new Point(j, i);
                    }
                    else if (rows[i][j] == '^')
                    {
                        startBox = new Point(j, i);
                    }
                    else if (rows[i][j] == '*')
                    {
                        boolBox[j, i] = true;
                        Blocked.Add(new Point(j, i));
                    }
                }
            }
        }
        public void Sequential_Solve()
        {
            sw.Reset();
            sw.Start();
            if (startBox == new Point(-1, -1) || endBox == new Point(-1, -1))
            {
                MessageBox.Show("Start Point and End Point Required", "Hey !!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (BFS(startBox))
            {
                List<Node> PATH = new List<Node>();
                PATH.Add(node);
                while (node.path != 0)
                {
                    Tuple<Point, int> tuple = new Tuple<Point, int>(node.Point, node.dir);
                    node = par[tuple];
                    PATH.Add(node);
                }
                ts = sw.Elapsed;
                Console.WriteLine(ts.ToString());
                String Time = string.Format("Timer:{0,2}.{1,2}", ts.Seconds, ts.Milliseconds);
                sw.Stop();
                ResultForm ResultForm = new ResultForm(SIZEX, SIZEY, startBox, endBox, Blocked, PATH, Time);
                ResultForm.ShowDialog();
            }
            else
                MessageBox.Show("No Path Found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public bool valid(Point Point)  //check valid position and valid cell
        {
            if (Blocked.Contains(Point) || Point.X >= SIZEX || Point.Y >= SIZEY || Point.X < 0 || Point.Y < 0)
                return false;
            return true;
        }

        private bool BFS(Point point) //O(m*n)
        {
            queue = new Queue<Node>();
            node = new Node(point, 0, 0);
            par = new Dictionary<Tuple<Point, int>, Node>();
            visited = new List<Tuple<Point, int>>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                node = queue.Dequeue();
                if (node.Point == endBox)
                    return true;
                for (int i = 0; i < 2; ++i)
                {
                    int new_dir = (node.dir + i) % 4;
                    Point new_point = new Point(node.Point.X + dx[new_dir], node.Point.Y + dy[new_dir]);
                    Tuple<Point, int> tuple = new Tuple<Point, int>(new_point, new_dir);
                    if (valid(new_point) && !visited.Contains(tuple))
                    {
                        queue.Enqueue(new Node(new_point, new_dir, node.path + 1));
                        par[tuple] = new Node(node.Point, node.dir, node.path);
                        visited.Add(tuple);
                    }
                }
            }
            return false;
        }
        public async void Parallel_Solve()
        {
            if (startBox == new Point(-1, -1) || endBox == new Point(-1, -1))
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
                    ResultForm ResultForm = new ResultForm(SIZEX, SIZEY, startBox, endBox, Blocked, Resultnodes,Timer);
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
                List<Node> Lis = new List<Node>();
                Tasks = new List<Task>();
                Lis.Add(new Node(startBox, 0, 0));
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

        private void MyThread(List<Node> nodes, CancellationToken cts)
        {
            List<Node> ls = new List<Node>();
            Node node = nodes.Last();
            m_lock.WaitOne();
            if (cts.IsCancellationRequested)
            {
                m_lock.ReleaseMutex();
                return;
            }
            if (node.Point == endBox)
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
                int new_dir = (node.dir + i) % 4;
                Point new_point = new Point(node.Point.X + dx[new_dir], node.Point.Y + dy[new_dir]);
                if (valid(new_point) && !nodes.Exists(n => n.Point == new_point && n.dir == new_dir))
                    ls.Add(new Node(new_point, new_dir, node.path + 1));
            }
            if (ls.Count == 2)
            {

                List<Node> rnode = new List<Node>(nodes);
                List<Node> lnode = new List<Node>(nodes);
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
