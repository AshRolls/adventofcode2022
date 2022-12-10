namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day10()
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
        int[] checkVals = new int[6] { 20, 60, 100, 140, 180, 220 };
        long sum = 0;

        int x = 1;
        int i = 0;
        bool adding = false;

        for( int c = 1; c <= 220; c++ )
        {
            if (i >= _input.Length) break;
            
            if (_input[i][0] == 'n')
            {
                i++;
            }
            else if (adding) 
            {
                x += int.Parse(_input[i][5..]);
                adding = false;
                i++;
            }
            else
            {
                adding = true;
            }

            if (checkVals.Contains(c)) sum += c * x;
        }

        _partOne = sum.ToString();
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
