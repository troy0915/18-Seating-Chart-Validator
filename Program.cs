using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SeatingPlan
{
    private int Rows, Cols;
    private string[,] Grid;
    private List<string> People;
    private List<(string, string)> MustSitWith;
    private List<(string, string)> MustSeparate;

    public SeatingPlan(int rows, int cols, List<string> people,
                       List<(string, string)> mustSitWith,
                       List<(string, string)> mustSeparate)
    {
        Rows = rows;
        Cols = cols;
        Grid = new string[Rows, Cols];
        People = people;
        MustSitWith = mustSitWith;
        MustSeparate = mustSeparate;

        if (people.Count > Rows * Cols)
            throw new ArgumentException("Not enough seats for all people.");
    }

    public bool ArrangeSeats()
    {
        return Backtrack(0);
    }

    private bool Backtrack(int index)
    {
        if (index == People.Count)
            return ValidateAll();

        string person = People[index];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (Grid[r, c] == null)
                {
                    Grid[r, c] = person;

                    if (IsPartialValid(person, r, c))
                    {
                        if (Backtrack(index + 1))
                            return true;
                    }

                    Grid[r, c] = null;
                }
            }
        }

        return false;
    }

    private bool IsPartialValid(string person, int r, int c)
    {
        foreach (var (a, b) in MustSeparate)
        {
            if (person == a || person == b)
            {
                string other = (person == a) ? b : a;

                var pos = FindPerson(other);
                if (pos != null)
                {
                    int dr = Math.Abs(pos.Value.r - r);
                    int dc = Math.Abs(pos.Value.c - c);
                    if (dr == 0 && dc == 0) return false; 
                    if (dr <= 0 && dc <= 1) return false; 
                    if (dr <= 1 && dc <= 0) return false; 
                }
            }
        }

        return true;
    }

    private bool ValidateAll()
    {
        foreach (var (a, b) in MustSitWith)
        {
            var posA = FindPerson(a);
            var posB = FindPerson(b);

            if (posA == null || posB == null) return false;

            int dr = Math.Abs(posA.Value.r - posB.Value.r);
            int dc = Math.Abs(posA.Value.c - posB.Value.c);

            if (dr + dc != 1)
                return false;
        }

        foreach (var (a, b) in MustSeparate)
        {
            var posA = FindPerson(a);
            var posB = FindPerson(b);

            if (posA == null || posB == null) return false;

            int dr = Math.Abs(posA.Value.r - posB.Value.r);
            int dc = Math.Abs(posA.Value.c - posB.Value.c);

            if (dr + dc <= 1)
                return false;
        }

        return true;
    }

    private (int r, int c)? FindPerson(string name)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (Grid[r, c] == name)
                    return (r, c);
        return null;
    }

    public void PrintPlan()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Console.Write((Grid[r, c] ?? "[ ]").PadRight(10));
            }
            Console.WriteLine();
        }
    }
}


namespace _18__Seating_Chart_Validator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var people = new List<string> { "Alice", "Bob", "Charlie", "Diana", "Eve" };
            var friends = new List<(string, string)>
        {
            ("Alice", "Bob")
        };
            var enemies = new List<(string, string)>
        {
            ("Charlie", "Diana")
        };

            var plan = new SeatingPlan(2, 3, people, friends, enemies);

            if (plan.ArrangeSeats())
            {
                Console.WriteLine("Seating arrangement found:");
                plan.PrintPlan();
            }
            else
            {
                Console.WriteLine("Impossible to seat with given constraints.");
            }
        }
    }
}




