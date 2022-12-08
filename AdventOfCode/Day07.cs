using System.Collections.Immutable;
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
        Dir root = new Dir(null);
        setupFS(root);

        List<Dir> dirs = new List<Dir>();
        root.getDirsUnderSize(dirs, 100000);
        long size = dirs.Select(x => x.AllSize).Sum();

        return new(size.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        Dir root = new Dir(null);
        setupFS(root);

        long unusedSpace = 70000000 - root.AllSize;
        long spaceToFree = 30000000 - unusedSpace;

        List<Dir> dirs = new List<Dir>();
        root.getDirsUnderSize(dirs, 70000000);
        var d = dirs.Select(x => x.AllSize).Where(x => x > spaceToFree).Min();

        return new(d.ToString());
    }

    private void setupFS(Dir root)
    {
        Dir cur = root;
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
        } while (i < _input.Length);

        root.getSizeAllChildren(); // sets AllSizes for each dir
    }

    private void addFileOrDirectory(Dir cur, string line)
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

    private void cmdCD(ref Dir cur, string line)
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

    private void cmdLS(Dir cur)
    {                
        cur.Size = 0;                 
    }

    private class Dir
    {        
        public Dir(Dir parent)
        {            
            subdirs = new Dictionary<string, Dir>();
            Size = 0;
            Parent = parent;
        }       
        private Dictionary<string, Dir> subdirs { get; set; }
        internal Dir Parent { get; set; }
        internal long Size { get; set; }
        internal long AllSize { get; set; }

        internal void AddDir(string name)
        {
            subdirs.Add(name, new Dir(this));
        }
        internal Dir GetSubDir(string name)
        {
            return subdirs[name];
        }

        internal long getSizeAllChildren()
        {
            long allSize = 0; // dont know size of children
            allSize += Size; // add size of files in this dir
            foreach (Dir d in subdirs.Values) // add allsize of children
            {                
                allSize += d.getSizeAllChildren();
            }            
            AllSize = allSize; // set this value
            return allSize;
        }

        internal void getDirsUnderSize(List<Dir> dirs, int max)
        {
            if (this.AllSize < max) dirs.Add(this);
            foreach (Dir d in subdirs.Values) // add allsize of children
            {
                d.getDirsUnderSize(dirs, max);
            }
        }
    }
}
