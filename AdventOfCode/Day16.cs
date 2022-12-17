using FFMpegCore.Enums;
using Raylib_cs;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace AdventOfCode;

public class Day16 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    private Graph _g;
    Valve _startValve;
    private int _bestPressure = 0;
    private int _maxCost = 30;
    private Dictionary<Valve, int> _valveMasks;
#if DEBUG
    private long _vChecked;
    private Stack<ValveAction> _stack;
    private Stack<(ValveAction,ValveAction)> _stack2;
#endif

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
        _g = new Graph();
        _startValve = setupGraph();
        collapseGraph();
        calcValveDist();
        BitVector32 opened = createIdx();

        ValveAction act = new ValveAction(_startValve, ActionEnum.MOVE, 0);
        _bestPressure = 0;
        _maxCost = 30;
#if DEBUG
        _vChecked = 0;
        _stack = new Stack<ValveAction>();
        _stack.Push(act);
#endif                
        //traverseGraph(act, act, opened, 0, 0);

        _partOne = _bestPressure.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        BitVector32 opened = createIdx();
        BitVector32 visited1 = new BitVector32(0);
        BitVector32 visited2 = new BitVector32(0);
        ValveAction act1 = new ValveAction(_startValve, ActionEnum.MOVE, 0);
        ValveAction act2 = new ValveAction(_startValve, ActionEnum.MOVE, 0);
        
        _bestPressure = 0;
        _maxCost = 26;
#if DEBUG
        _vChecked = 0;
        _stack2 = new Stack<(ValveAction,ValveAction)>();
        _stack2.Push((act1,act2));
#endif                
        traverseGraph2(visited1, visited2, act1, act2, 0, 0, opened, 0);

        _partTwo = _bestPressure.ToString();        
    }

    private void traverseGraph2(BitVector32 visited1, BitVector32 visited2, 
                                ValveAction acts1, ValveAction acts2, 
                                int curCost1, int curCost2, 
                                BitVector32 curOpened, int curPressure)
    {
#if DEBUG
        //Debug.Assert(curCost1 <= _maxCost && curCost2 <= _maxCost);
        _vChecked++;
#endif

        doAction(ref visited1, acts1, ref curCost1, ref curOpened, ref curPressure);
        doAction(ref visited2, acts2, ref curCost2, ref curOpened, ref curPressure);

        if (curPressure > _bestPressure)
        {
            _bestPressure = curPressure;
            Console.Out.WriteLine("Cost: {0}, Pressure: {1}", (curCost1, curCost2), curPressure);
#if DEBUG
            string path = pathFromStack(_stack2);
            //string path = lastAct.ToString() + " " + act.ToString();            
            Console.Out.WriteLine(path);
            Console.Out.WriteLine();
#endif
        }

        // pruning        
        if (curCost1 == _maxCost && curCost2 == _maxCost)
        {
#if DEBUG
            _ = _stack2.Pop();
#endif
            return;
        }
        // calculate max potential remaining pressure from all nodes that can still be opened.
        int potential = 0;
        foreach (KeyValuePair<Valve, int> kvp in _valveMasks)
        {
            if (!curOpened[kvp.Value])
            {
                int timeRemaining1 = _maxCost - curCost1;
                int timeRemaining2 = _maxCost - curCost2;
                if (kvp.Key.shortestDist[acts1.V] < timeRemaining1)
                {
                    potential += (timeRemaining1 - kvp.Key.shortestDist[acts1.V]) * kvp.Key.Rate;
                }
                if (kvp.Key.shortestDist[acts2.V] < timeRemaining2)
                {
                    potential += (timeRemaining2 - kvp.Key.shortestDist[acts2.V]) * kvp.Key.Rate;
                }
            }
        }
        if (potential + curPressure < _bestPressure)
        {
#if DEBUG
            _ = _stack2.Pop();
#endif
            return;
        }

        List<ValveAction> newActs1 = addEdgeActions(visited1, acts1, curCost1, ref curOpened);
        List<ValveAction> newActs2 = addEdgeActions(visited2, acts2, curCost2, ref curOpened);

        foreach (ValveAction v1 in newActs1)
        {
            foreach (ValveAction v2 in newActs2)
            {
#if DEBUG
                _stack2.Push((v1, v2));
#endif
                traverseGraph2(visited1, visited2, v1, v2, curCost1, curCost2, curOpened, curPressure);
            }
        }
#if DEBUG
        _ = _stack2.Pop();
#endif
    }

    private List<ValveAction> addEdgeActions(BitVector32 visited, ValveAction acts, int curCost, ref BitVector32 curOpened)
    {
        List<ValveAction> newActs = new List<ValveAction>();
        if (acts.V.Rate > 0 && !curOpened[_valveMasks[acts.V]] && curCost + 1 <= _maxCost)
        {
            ValveAction newAct = new ValveAction(acts.V, ActionEnum.OPEN, 1);
            newActs.Add(newAct);
        }
        foreach (KeyValuePair<Valve, int> edge in _g.AdjacencyList[acts.V])
        {
            if (!visited[_valveMasks[edge.Key]] && curCost + edge.Value <= _maxCost)
            {

                ValveAction newAct = new ValveAction(edge.Key, ActionEnum.MOVE, edge.Value);
                newActs.Add(newAct);
            }
        }
        return newActs;
    }

    private void doAction(ref BitVector32 visited, ValveAction acts, ref int curCost, ref BitVector32 curOpened, ref int curPressure)
    {
        if (acts.Type == ActionEnum.OPEN)
        {
            curCost += acts.Cost;
            // check we are not trying to open the same valve at the same time.
            if (!curOpened[_valveMasks[acts.V]])
            {
                curPressure += (_maxCost - curCost) * acts.V.Rate;
                curOpened[_valveMasks[acts.V]] = true;
            }
            visited = new BitVector32(0);
        }
        else
        {
            curCost += acts.Cost;
            visited[_valveMasks[acts.V]] = true;
        }
    }

    private void traverseGraph(ValveAction lastAct, ValveAction act, BitVector32 curOpened, int curCost, int curPressure)
    {
#if DEBUG
        Debug.Assert(curCost <= _maxCost);
        _vChecked++;
#endif                
        if (act.Type == ActionEnum.OPEN)
        {
            curCost += act.Cost;
            curPressure += (_maxCost - curCost) * act.V.Rate;
            curOpened[_valveMasks[act.V]] = true;                       
        }
        else
        {
            curCost += act.Cost;                       
        }

        if (curPressure > _bestPressure)
        {
            _bestPressure = curPressure;
#if DEBUG
            string path = pathFromStack(_stack);
            //string path = lastAct.ToString() + " " + act.ToString();
            Console.Out.WriteLine("Cost: {0}, Pressure: {1}, Checked: {2}", curCost, curPressure, _vChecked);
            Console.Out.WriteLine("Path: {0}", path);
            Console.Out.WriteLine();
#endif
        }

        // prune
        // calculate max potential remaining pressure from all nodes that can still be opened.
        if (curCost == _maxCost)
        {
#if DEBUG
            _ = _stack.Pop();
#endif
            return;
        }
        int potential = 0;
        foreach (KeyValuePair<Valve, int> kvp in _valveMasks)
        {
            if (!curOpened[kvp.Value]) potential += (_maxCost - curCost) * kvp.Key.Rate;
        }
        if (potential + curPressure < _bestPressure)
        {
#if DEBUG
            _ = _stack.Pop();
#endif
            return;
        }

        ValveAction newAct;
        foreach (KeyValuePair<Valve, int> edge in _g.AdjacencyList[act.V])
        {
            if (((lastAct.Type == ActionEnum.MOVE && lastAct.V.Name != edge.Key.Name) || lastAct.Type == ActionEnum.OPEN) 
                && curCost + edge.Value <= _maxCost)
            {
                newAct = new ValveAction(edge.Key, ActionEnum.MOVE, edge.Value);
#if DEBUG
                _stack.Push(newAct);
#endif
                traverseGraph(act, newAct, curOpened, curCost, curPressure);
            }
        }     
        
        if (act.V.Rate > 0 && !curOpened[_valveMasks[act.V]] && curCost + 1 <= _maxCost)
        {
            newAct = new ValveAction(act.V, ActionEnum.OPEN, 1);
#if DEBUG
            _stack.Push(newAct);
#endif
            traverseGraph(act, newAct, curOpened, curCost, curPressure);
        }
#if DEBUG
        _ = _stack.Pop();
#endif
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

    private string pathFromStack(Stack<(ValveAction,ValveAction)> stack)
    {
        var listPath = stack.ToList();
        listPath.Reverse();
        StringBuilder sb = new StringBuilder();
        foreach ((ValveAction, ValveAction) va in listPath)
        {
            string str = va.Item1.ToString();
            sb.Append(str);
            if (str.Length == 3) sb.Append(" ");
            else sb.Append("  ");
        }
        sb.Append(Console.Out.NewLine);
        foreach ((ValveAction, ValveAction) va in listPath)
        {
            string str = va.Item2.ToString();
            sb.Append(str);
            if (str.Length == 3) sb.Append(" ");
            else sb.Append("  ");
        }
        return sb.ToString();
    }

    private BitVector32 createIdx()
    {
        _valveMasks = new Dictionary<Valve, int>();
        BitVector32 bv = new BitVector32(0);
        var vs = _g.AdjacencyList.Keys.ToList();
        int lastBit = 0;
        for (int i = 0; i < _g.AdjacencyList.Count; i++)
        {
            int mask = BitVector32.CreateMask(lastBit);
            _valveMasks.Add(vs[i], mask);
            lastBit = mask;
        }        
        return bv;
    }

    private void calcValveDist()
    {
        foreach (Valve vFrom in _g.AdjacencyList.Keys)
        {
            foreach (Valve vTo in _g.AdjacencyList.Keys)
            {
                if (vFrom != vTo) 
                {
                    int dist = dijkstra(vFrom, vTo);
                    vFrom.shortestDist.Add(vTo, dist);
                }
                else
                {
                    vFrom.shortestDist.Add(vTo, 0);
                }
            }
        }
    }

    private int dijkstra(Valve vFrom, Valve vTo)
    {
        PriorityQueue<Valve, int> openQueue = new PriorityQueue<Valve, int>();
        Dictionary<Valve,int> visitedNodes = new Dictionary<Valve,int>();
        openQueue.Enqueue(vFrom, 0);
        visitedNodes.Add(vFrom, 0);

        Valve cur = null;
        bool foundEnd = false;

        while (openQueue.Count > 0 && !foundEnd)
        {
            cur = openQueue.Dequeue();
            if (cur == vTo) foundEnd = true;
            foreach (KeyValuePair<Valve, int> edge in _g.AdjacencyList[cur])
            {
                int g = visitedNodes[cur] + edge.Value;
                if (!visitedNodes.Keys.Contains(edge.Key)) 
                {
                    visitedNodes.Add(edge.Key, g);
                    openQueue.Enqueue(edge.Key, g);
                }                
                else if (g < visitedNodes[edge.Key] + edge.Value)
                {
                    visitedNodes[edge.Key] = g;
                }
            }
        }
        return visitedNodes[vTo];
    }

    private void collapseGraph()
    {
        bool collapsing = true;
        while (collapsing)
        {
            collapsing = false;
            Valve? toRemove = null;
            Valve[] v = new Valve[2];
            int cost = 0;
            foreach (KeyValuePair<Valve, Dictionary<Valve, int>> kvp in _g.AdjacencyList)
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
                _g.AdjacencyList.Remove(toRemove);
                _g.AdjacencyList[v[0]].Remove(toRemove);
                _g.AdjacencyList[v[1]].Remove(toRemove);
                _g.AddEdge(v[0], v[1], cost);
                _g.AddEdge(v[1], v[0], cost);
            }
        }
    }

    private Valve setupGraph()
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
            _g.AddVertex(v);
        }

        foreach (KeyValuePair<string, List<string>> kvp in edgesToAdd)
        {
            Valve from = vertices[kvp.Key];
            foreach (string edge in kvp.Value)
            {
                Valve to = vertices[edge];
                _g.AddEdge(from, to, 1);
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
        public Dictionary<Valve,int> shortestDist= new Dictionary<Valve,int>();
        public int g;
        public override string ToString() { return Name; }
    }

    //private class ValveG : Valve
    //{
        
    //}

    private enum ActionEnum { MOVE, OPEN, STOP };

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
}
