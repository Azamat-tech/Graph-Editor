using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

class Hello
{
    [STAThread]
    static void Main()
    {
        Form form = new MyForm();
        form.Text = "Untitled";
        Application.Run(form);
    }
}

class Vector 
{
    public double X, Y;
    public Vector(double X, double Y)
    {
        this.X = X;
        this.Y = Y;
    }
}

class MyForm : Form
{
    int newVertex, selected;
    const int Object_Width = 30, Object_Height = 30, Radius  = 15;
    bool check2, checkingMouseDown;
    String path;
    Graph graph = new Graph();
    
    SortedDictionary<int, Point> pos = new SortedDictionary<int, Point>();
    public MyForm()
    {
        graph.changed += Invalidate;
        ClientSize = new Size(600, 600);
        ToolStripMenuItem[] fileItems = {
            new ToolStripMenuItem("New", null, onNew),
            new ToolStripMenuItem("Open", null, onOpen),
            new ToolStripMenuItem("Save", null, onSave),
            new ToolStripMenuItem("Quit", null, onQuit)
        };
        
        ToolStripMenuItem[] topItems = {
            new ToolStripMenuItem("File", null, fileItems)
        };

        MenuStrip strip = new MenuStrip();
        foreach (var item in topItems)
            strip.Items.Add(item);

        Controls.Add(strip);   
    }
    public void onNew(object sender, EventArgs e)
    {
        pos.Clear();
        graph.emptyDictionary();
        selected = 0;
        Invalidate();
    }

    public void onOpen(object sender, EventArgs e)
    {
        pos.Clear();
        graph.emptyDictionary();
        string inFile = string.Empty;
        using (OpenFileDialog openFile = new OpenFileDialog())
        {
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                path = openFile.FileName;
                try 
                {
                    using(StreamReader reader = new StreamReader(openFile.OpenFile()))
                    {
                        bool open = false;
                        while(reader.ReadLine() is string s)
                        {
                            if(s == "END")
                                break;
                            else if(s == "Connection")
                                open = true;
                            else
                            {
                                string[] numbers = s.Trim().Split();
                                if(!open)
                                {
                                    pos.Add(int.Parse(numbers[0]), new Point(int.Parse(numbers[1]),int.Parse(numbers[2])));
                                    graph.addVertex(int.Parse(numbers[0]));
                                }else
                                {
                                    for(int i = 1; i < numbers.Length; i++)
                                    {
                                        graph.connect(int.Parse(numbers[0]), int.Parse(numbers[i]));
                                    }
                                }
                            }
                        }
                    }
                    string se = path.Substring(path.LastIndexOf('\\') + 1);
                    Text = se.Substring(0, se.LastIndexOf('.'));
                    Invalidate();
                }catch(FileNotFoundException c)
                {
                    Console.WriteLine(c.FileName);
                    return;
                }
            }
        }
    }

    public void onSave(object sender, EventArgs e)
    {   
        if(Text != "Untitled")
        {
            using(StreamWriter write = new StreamWriter(File.Create(path)))
                {
                    String p = "";
        
                    foreach(KeyValuePair<int, Point> w in pos)
                    {
                        p += w.Key + " " + w.Value.X  + " " + w.Value.Y; 
                        write.WriteLine(p);
                        p = "";
                    }
                    write.WriteLine("Connection");
                    String s = "";
                    foreach(int v in graph.vertices())
                    {
                        s += v + " "; 
                        foreach(int i in graph.neighbors(v))
                        {
                            s += i + " ";
                        }
                        write.WriteLine(s);
                        s = "";
                    }
                    write.WriteLine("END");
                }
        }
        else
        {
            using(SaveFileDialog saving = new SaveFileDialog())
            {
                if(saving.ShowDialog() == DialogResult.OK)
                {
                    path = saving.FileName;
                    using(StreamWriter write = new StreamWriter(File.Create(path)))
                    {
                        String p = "";
                        foreach(KeyValuePair<int, Point> w in pos)
                        {
                            p += w.Key + " " + w.Value.X  + " " + w.Value.Y; 
                            write.WriteLine(p);
                            p = "";
                        }
                        write.WriteLine("Connection");
                        String s = "";
                        foreach(int v in graph.vertices())
                        {
                            s += v + " "; 
                            foreach(int i in graph.neighbors(v))
                            {
                                s += i + " ";
                            }
                            write.WriteLine(s);
                            s = "";
                        }
                        write.WriteLine("END");
                    }
                    string se = path.Substring(path.LastIndexOf('\\') + 1);
                    Text = se.Substring(0, se.LastIndexOf('.'));
                }
            }
        }
    }
    public void onQuit(object sender, EventArgs e)
    {
        Application.Exit();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        List<int> toRemove = new List<int>();
        if(Keys.Delete == e.KeyCode)
        {
            foreach(int v in graph.vertices())
            {
                foreach(int i in graph.neighbors(v))
                {
                    if(i == selected)
                    {
                        toRemove.Add(v);
                    }
                }
            }
            pos.Remove(selected);
            graph.deleteVertex(selected);
            foreach(int i in toRemove)
            {
                graph.disconnect(i, selected);
            }
            selected = 0;
            Invalidate();
        }
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {   
        int xc, yc, endpoint;
        if(Keys.Shift == Control.ModifierKeys) 
        {
            newVertex = graph.addVertex(0);
            pos.Add(newVertex, new Point(e.X - Radius, e.Y - Radius));
            check2 = true;
            selected = newVertex;
            
        }
        else{
            foreach(KeyValuePair<int, Point> w in pos)
            {
                xc = Object_Width/2 + w.Value.X; 
                yc = Object_Height/2 + w.Value.Y;
                if(InsideCircle(xc,yc,Radius,e.X,e.Y))
                {
                    checkingMouseDown = true;
                    check2 = true;
                    if(Keys.Control == Control.ModifierKeys && selected != 0)
                    {
                        endpoint = w.Key;
                        if(graph.areConnected(selected, endpoint))
                        {
                            graph.disconnect(selected, endpoint);
                        }else
                        {
                            if(graph.exists(selected))
                            {
                                graph.connect(selected, endpoint);
                            }
                        }
                        Invalidate();   
                    }else
                    {
                        selected = w.Key;
                        Invalidate();
                    }    
                }
            }
            if (!check2)
            {
                selected = 0;
                Invalidate();
            }
        }
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if(checkingMouseDown && Control.ModifierKeys != Keys.Control){
            pos[selected] = new Point(e.X - Radius,e.Y - Radius);
            Invalidate();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        checkingMouseDown = false;
    }

    public void paintingVertex(PaintEventArgs e, Graphics g)
    {
        int x,y;
        Pen pen1 = new Pen(Brushes.Black, 5);
        Font f = new Font("Sans Serif", 12);
        StringFormat format = new StringFormat();
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = StringAlignment.Center;
        foreach(KeyValuePair<int, Point> w in pos)
        {
            x = w.Value.X;
            y = w.Value.Y;
            Rectangle rect = new Rectangle(x, y, Object_Width, Object_Height);
            changeVertexState(g,pen1,w,x,y,Object_Width,Object_Height,w.Key.ToString(), rect,f,format);
        }
        check2 = false;
        pen1.Dispose();
        f.Dispose();
    }

    public void changeVertexState(Graphics g, Pen p, KeyValuePair<int, Point> w, int x, int y, int Object_Width, int Object_Height, String id, Rectangle rect,Font f, StringFormat format)
    {
        if (w.Key==selected )
        {
            g.FillEllipse(Brushes.Black, x, y, Object_Width, Object_Height);
            g.DrawString(id, f, Brushes.White, rect,format);
        }else      
        {
            g.DrawEllipse(p, x, y, Object_Width, Object_Height);
            g.FillEllipse(Brushes.White, x, y, Object_Width, Object_Height);
            g.DrawString(id, f, Brushes.Black, rect, format);
        }
    }

    public void paintingEdge(PaintEventArgs e, Graphics g)
    {
        Pen pen2 = new Pen(Brushes.Black, 1);
        foreach(int i in graph.vertices())
        {
            drawEdge(g, pen2, i);
        }
        pen2.Dispose();
    }   
    public void drawEdge(Graphics g, Pen pen2, int i)
    {
        Point edgeFrom, edgeTo;
        foreach(int w2 in graph.neighbors(i))
            {
                edgeFrom = new Point(pos[i].X + Radius, pos[i].Y + Radius);
                edgeTo = new Point(pos[w2].X + Radius, pos[w2].Y + Radius);
                g.DrawLine(pen2, edgeFrom.X, edgeFrom.Y, edgeTo.X, edgeTo.Y);

                double denominator = Math.Sqrt(Math.Pow(edgeFrom.X - edgeTo.X,2) + Math.Pow(edgeFrom.Y - edgeTo.Y,2));
                Vector V = new Vector ( (edgeFrom.X - edgeTo.X) / denominator , (edgeFrom.Y - edgeTo.Y) / denominator );


                Vector tip = new Vector (edgeTo.X + Radius * V.X, edgeTo.Y + Radius * V.Y);
                Vector W = new Vector ((-1) * V.Y, V.X);

                int D = Radius * 1/2;
                int C = 2 * Radius;

                Vector line1 = new Vector (edgeTo.X +  C * V.X + D * W.X,  edgeTo.Y + C * V.Y + D * W.Y);
                Vector line2 = new Vector (edgeTo.X + C * V.X - D * W.X, edgeTo.Y + C * V.Y - D * W.Y);

                g.DrawLine (pen2, (int) line1.X, (int) line1.Y,  (int)tip.X, (int)tip.Y);
                g.DrawLine (pen2, (int)line2.X, (int)line2.Y, (int)tip.X, (int)tip.Y);
            }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        paintingEdge(e, g);
        paintingVertex(e, g);
    }
    public static bool InsideCircle (int xc, int yc, int r, int x, int y) 
    {
        int dx = xc-x;
        int dy = yc-y;
        return dx*dx+dy*dy <= r*r;
    }

}

delegate void Notify();

class Graph {
    private SortedDictionary<int, HashSet<int>> vertex = new SortedDictionary<int, HashSet<int>>();

    public Notify changed;
    
    public int addVertex(int id) {
        if(id != 0 && !vertex.ContainsKey(id))
            vertex[id] = new HashSet<int>();
        else
        {
            id  = 1;
            while (vertex.ContainsKey(id))
                ++id;
            vertex[id] = new HashSet<int>();
        }
        changed?.Invoke();
        return id;
    }

    public void connect(int i, int j) { 
        vertex[i].Add(j);
        changed?.Invoke();
    }   
    
    public void disconnect(int i, int j) {
        vertex[i].Remove(j);
        changed?.Invoke();
    } 
        
    public bool areConnected(int i, int j) 
    { 
        return vertex[i].Contains(j);
    }

    public bool exists(int v)
    {
        return vertex.ContainsKey(v);
    }
    
    public IEnumerable<int> vertices() 
    { 
        return vertex.Keys;
    } 
    
    public IEnumerable<int> neighbors(int id) 
    { 
        return vertex[id];
    }
    public void deleteVertex(int i) 
    { 
        vertex.Remove(i);
        changed?.Invoke();
    } 
    public void emptyDictionary()
    {
        int count = 1;
        while(vertex.Count != 0)
        {
            if(vertex.ContainsKey(count))
            {
                deleteVertex(count);
                changed?.Invoke();
            }
            count++;
        }
    }
}
