using System.Collections;

namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string[] _input;

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public struct resultMap
    {
        public string input;
        public short score;
    }

    public override ValueTask<string> Solve_1()
    {
        resultMap ax;
        ax.input = "A X";
        ax.score = 4; //d

        resultMap ay;
        ay.input = "A Y";
        ay.score = 8; //w

        resultMap az;
        az.input = "A Z";
        az.score = 3; //l

        resultMap bx;
        bx.input = "B X";
        bx.score = 1; //l

        resultMap by;
        by.input = "B Y";
        by.score = 5; //d

        resultMap bz;
        bz.input = "B Z";
        bz.score = 9; //w

        resultMap cx;
        cx.input = "C X";
        cx.score = 7; //w

        resultMap cy;
        cy.input = "C Y";
        cy.score = 2; //l

        resultMap cz;
        cz.input = "C Z";
        cz.score = 6; //d

        resultMap[] r = new resultMap[] {ax,ay,az,bx,by,bz,cx,cy,cz};

        int score = 0;
        foreach (string line in _input)
        {
            foreach (resultMap res in r)
            {
                if (res.input == line)
                {
                    score += res.score;
                    break;
                }
            }
        }

        return new(score.ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        Dictionary<string,int> r = new Dictionary<string, int>();
        r.Add("A X", 3);
        r.Add("A Y", 4);
        r.Add("A Z", 8);
        r.Add("B X", 1);
        r.Add("B Y", 5);
        r.Add("B Z", 9);
        r.Add("C X", 2);
        r.Add("C Y", 6);
        r.Add("C Z", 7);

        int score = 0;
        foreach (string line in _input)
        {
            score += r[line];
        }

        return new(score.ToString());
    }
}
