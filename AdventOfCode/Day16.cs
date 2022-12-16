using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AdventOfCode;

public class Day16 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day16()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        Graph g = new Graph();
        Valve startValve = setupGraph(g);
        collapseGraph(g);

        const int maxCost = 30;

        ValveAction act = new ValveAction(startValve, ActionEnum.MOVE, 0);
        HashSet<Valve> opened = new HashSet<Valve>();
        stack.Push(act);
        traverseGraph(g, act, act, opened, 0, maxCost, 0);

        _partOne = bestPressure.ToString();
    }

    private int bestPressure = 0;   
    private long vChecked = 0;
    private Stack<ValveAction> stack = new Stack<ValveAction>();
    private void traverseGraph(Graph graph, ValveAction lastAct, ValveAction act, HashSet<Valve> opened, int curCost, int maxCost, int curPressure)
    {        
        Debug.Assert(curCost <= maxCost);
        vChecked++;       
        if (curPressure > bestPressure)
        {
            string path = pathFromStack(stack);
            Console.Out.WriteLine("Cost: {0}, Pressure: {1}, Checked: {2}", curCost, curPressure, vChecked);
            Console.Out.WriteLine("Path: {0}", path);
            Console.Out.WriteLine();
            bestPressure = curPressure;
        }
        if (act.Type == ActionEnum.OPEN)
        {
            curCost += act.Cost;
            curPressure += (maxCost - curCost) * act.V.Rate;                
            
            HashSet<Valve> newOpened = new HashSet<Valve>();
            foreach (Valve v in opened) newOpened.Add(v);
            newOpened.Add(act.V);
            
            //Console.Out.WriteLine("Cost: {0}, Valve: {1}, Pressure: {2}", curCost, act.V.ToString(), curPressure);
            addNeighbours(graph, lastAct, act, newOpened, curCost, maxCost, curPressure);
        }
        else
        {
            curCost += act.Cost;
            //Console.Out.WriteLine("Cost: {0}, Valve: {1}, Pressure: {2}", curCost, act.V.ToString(), curPressure);
            addNeighbours(graph, lastAct, act, opened, curCost, maxCost, curPressure);            
        }
        _ = stack.Pop();

    }

    private void addNeighbours(Graph graph, ValveAction lastAct, ValveAction act, HashSet<Valve> opened, int curCost, int maxCost, int curPressure)
    {
        foreach (var neighbour in graph.AdjacencyList[act.V])
        {
            if (((lastAct.Type == ActionEnum.MOVE && lastAct.V != neighbour.Key) || lastAct.Type == ActionEnum.OPEN) && curCost + neighbour.Value <= maxCost)
            {
                ValveAction newAct = new ValveAction(neighbour.Key, ActionEnum.MOVE, neighbour.Value);
                stack.Push(newAct);
                traverseGraph(graph, act, newAct, opened, curCost, maxCost, curPressure);
            }
        }
        if (act.V.Rate > 0 && !opened.Contains(act.V) && curCost + 1 <= maxCost)
        {
            ValveAction newAct = new ValveAction(act.V, ActionEnum.OPEN, 1);
            stack.Push(newAct);
            traverseGraph(graph, act, newAct, opened, curCost, maxCost, curPressure);    
        }
    }

    private string pathFromStack(Stack<ValveAction> stack)
    {
        var listPath = stack.ToList();
        listPath.Reverse();
        StringBuilder sb = new StringBuilder();
        foreach (ValveAction va in listPath)
        {
            sb.Append(va.ToString());
            sb.Append(" ");
        }
        return sb.ToString();
    }

    private void checkBestPressure(int pressure)
    {
        if (pressure > bestPressure)
        {
            bestPressure = pressure;
        }         
    }

    private static void collapseGraph(Graph g)
    {
        bool collapsing = true;
        while (collapsing)
        {
            collapsing = false;
            Valve? toRemove = null;
            Valve[] v = new Valve[2];
            int cost = 0;
            foreach (KeyValuePair<Valve, Dictionary<Valve, int>> kvp in g.AdjacencyList)
            {
                if (kvp.Key.Rate == 0 && kvp.Value.Count == 2)
                {
                    collapsing = true;
                    toRemove = kvp.Key;
                    int i = 0;
                    foreach (KeyValuePair<Valve, int> edge in kvp.Value)
                    {
                        v[i++] = edge.Key;
                        cost += edge.Value;
                    }
                    break;
                }
            }

            if (toRemove != null)
            {
                g.AdjacencyList.Remove(toRemove);
                g.AdjacencyList[v[0]].Remove(toRemove);
                g.AdjacencyList[v[1]].Remove(toRemove);
                g.AddEdge(v[0], v[1], cost);
                g.AddEdge(v[1], v[0], cost);
            }
        }
    }

    private Valve setupGraph(Graph g)
    {
        Valve startValve;
        Dictionary<String, Valve> vertices = new Dictionary<string, Valve>();
        Dictionary<string, List<string>> edgesToAdd = new Dictionary<string, List<string>>();
        foreach (string line in _input)
        {
            var strs = line.Split(',');
            Valve v = new Valve();
            v.Name = strs[0][6..8];
            vertices.Add(v.Name, v);
            v.Rate = AoCHelper.GetNumsFromStr(strs[0])[0];
            List<string> edges = new List<string>();
            foreach (string s in strs)
            {
                edges.Add(s[^2..^0]);
            }
            edgesToAdd.Add(v.Name, edges);
            g.AddVertex(v);
        }

        foreach (KeyValuePair<string, List<string>> kvp in edgesToAdd)
        {
            Valve from = vertices[kvp.Key];
            foreach (string edge in kvp.Value)
            {
                Valve to = vertices[edge];
                g.AddEdge(from, to, 1);
            }
        }
        startValve = vertices["AA"];
        vertices.Clear();
        edgesToAdd.Clear();
        return startValve;
    }

    // Vertex
    private class Valve
    {
        public string Name;
        public int Rate;
        public override string ToString() { return Name; }
    }

    private enum ActionEnum { MOVE, OPEN };

    private struct ValveAction
    {
        public ValveAction(Valve v, ActionEnum type, int cost)
        {
            V = v;
            Type = type;
            Cost = cost;
        }

        public Valve V;
        public ActionEnum Type;
        public int Cost;

        public override string ToString() { return (Type==ActionEnum.OPEN?"O":"") + V.Name; }
    }

    private class Graph
    {
        public Dictionary<Valve, Dictionary<Valve,int>> AdjacencyList { get; } = new Dictionary<Valve, Dictionary<Valve, int>>();

        public void AddVertex(Valve vertex)
        {
            AdjacencyList[vertex] = new Dictionary<Valve, int>();
        }

        public void AddEdge(Valve t1,Valve t2 ,int cost)
        {
            _ = AdjacencyList[t1].TryAdd(t2, cost);           
        }
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        _partTwo = "Not Solved";
    }
}
