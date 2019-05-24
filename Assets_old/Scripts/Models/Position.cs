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
        
        public Position(int col, int row)
        {
            Row = row;
            Col = col;
        }

        public override string ToString()
        {
            return ('a' + Col) + Row.ToString();
        }
    }
}
