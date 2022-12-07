using System.Reflection.Metadata.Ecma335;
using System.Xml.Xsl;

namespace AdventOfCode;

public class Day07 : BaseDay
{
    private readonly string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        dir root = new dir(null);
        
        dir cur = root;
        int i = 1;        
        do 
        {
            if (_input[i][0] == '$')
            {
                if (_input[i][2] == 'c')
                {
                    cmdCD(ref cur, _input[i]);
                }
                else if (_input[i][2] == 'l')
                {
                    cmdLS(cur);
                }
            }            
            else
            {
                addFileOrDirectory(cur, _input[i]);
            }            
            i++;
        } while (i < _input.Length) ;

        root.getSizeAllChildren(); // sets AllSizes for each dir
        
        List<dir> dirs = new List<dir>();
        root.getDirsUnderSize(dirs, 100000);
        long size = dirs.Select(x => x.AllSize).Sum();

        return new(size.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }

    private void addFileOrDirectory(dir cur, string line)
    {
        var str = line.Split(' ');
        if (str[0].ToString() == "dir")
        {
            cur.AddDir(str[1].ToString());
        }
        else
        {
            cur.Size += int.Parse(str[0]);
        }
    }

    private void cmdCD(ref dir cur, string line)
    {
        string[] str = line.Split(' ');
        if (str[2] == "..")
        {
            cur = cur.Parent;
        }
        else
        {
            cur = cur.GetSubDir(str[2]);
        }
    }

    private void cmdLS(dir cur)
    {                
        cur.Size = 0;                 
    }

    private class dir
    {        
        public dir(dir parent)
        {            
            subdirs = new Dictionary<string, dir>();
            Size = 0;
            Parent = parent;
        }       
        private Dictionary<string, dir> subdirs { get; set; }
        internal dir Parent { get; set; }
        internal long Size { get; set; }
        internal long AllSize { get; set; }

        internal void AddDir(string name)
        {
            subdirs.Add(name, new dir(this));
        }
        internal dir GetSubDir(string name)
        {
            return subdirs[name];
        }

        internal long getSizeAllChildren()
        {
            long allSize = 0; // dont know size of children
            allSize += Size; // add size of files in this dir
            foreach (dir d in subdirs.Values) // add allsize of children
            {                
                allSize += d.getSizeAllChildren();
            }            
            AllSize = allSize; // set this value
            return allSize;
        }

        internal void getDirsUnderSize(List<dir> dirs, int max)
        {
            if (this.AllSize < max) dirs.Add(this);
            foreach (dir d in subdirs.Values) // add allsize of children
            {
                d.getDirsUnderSize(dirs, max);
            }
        }
    }
}
