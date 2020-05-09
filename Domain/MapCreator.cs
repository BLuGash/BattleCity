using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleCity.Domain
{
    public static class MapCreator
    {
        public static Map CreateMapFromText(string text)
        {
            return CreateMapFromLines(text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static Map CreateMapFromLines(string[] lines)
        {
            var height = lines.Length - 1;
            var width = lines[1].Length;
            var playerPos = Game.DefaultPoint;
            var emptyCells = new HashSet<Point>();
            var wallCells = new HashSet<Point>();
            for (var i = 1; i <= height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    switch (lines[i][j])
                    {
                        case '#':
                            wallCells.Add(new Point(j, i - 1));
                            break;
                        case 'P':
                            playerPos = new Point(j, i - 1);
                            emptyCells.Add(new Point(j, i - 1));
                            break;
                        case ' ':
                            emptyCells.Add(new Point(j, i - 1));
                            break;
                    }
                }
            }
            var numbers = lines[0].Split(' ');
            if (playerPos == Game.DefaultPoint)
                throw new InvalidOperationException("There is no player");
            return new Map(width, height, playerPos, Direction.Up,
                int.Parse(numbers[1]), int.Parse(numbers[0]), emptyCells, wallCells);
        }
    }
}
