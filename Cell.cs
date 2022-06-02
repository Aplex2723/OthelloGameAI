using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class Cell
    {
        public const char iconEmpty = ' ';
        private char icon = iconEmpty;
        private int row, column;

        public Cell(int Row, int Column, char Sign = iconEmpty)
        {
            // cell c'tor
            row = Row;
            column = Column;
            icon = Sign;
        }
        public Cell()
        {
        }

        public int Row { get => row; set => row = value; }
        public int Column { get => column; set => column = value; }
        public char Icon { get => icon; set => icon = value; }

        //  Metodo encargado de checar si la celda del objeto esta vacia
        public bool IsEmpty()
        {

            bool isEmpty;

            if (icon == iconEmpty)
            {
                isEmpty = true;
            }
            else
            {
                isEmpty = false;
            }

            return isEmpty;
        }
    }
}
