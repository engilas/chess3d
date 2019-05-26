using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Models
{
    public struct Position
    {
        public readonly int Col;
        public readonly int Row;
        public readonly int Pos;
        
        public Position(int col, int row)
        {
            Row = row;
            Col = col;
            Pos = row * 8 + col;
        }

        public Position(int pos)
        {
            Row = pos / 8;
            Col = pos % 8;
            Pos = pos;
        }

        public override string ToString()
        {
            return (char)('a' + Col) + (Row + 1).ToString();
        }
    }
}
