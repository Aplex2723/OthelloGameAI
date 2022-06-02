using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class GameBoard
    {
        public const int boardSize = 8;
        public enum playerToken
        {
            BlackPlayer = 'X',
            WhitePlayer = 'O'
        }

        private Cell[,] boardCell;

        public Cell[,] BoardCell { get => boardCell; set => boardCell = value; }

        //  Este constructor crea el tablero dependiendo del tamanio que se le mencione en la variable boardSize
        public GameBoard()
        {
            boardCell = new Cell[boardSize, boardSize];
            for (int row = 0; row < boardSize; row++)
            {
                for (int colIndex = 0; colIndex < boardSize; colIndex++)
                {
                    boardCell[row, colIndex] = new Cell(row, colIndex);
                }
            }
        }

        //  Metodo encargado de hacer una copia del actual tablero
        public GameBoard Copy()
        {
            GameBoard newBoard = new GameBoard();
            Cell cellToCopy;

            for (int row = 0; row < boardSize; row++)
            {
                for (int column = 0; column < boardSize; column++)
                {
                    cellToCopy = new Cell(row, column, boardCell[row, column].Icon);
                    newBoard.boardCell[row, column] = cellToCopy;
                }
            }

            return newBoard;
        }

        //  Metodo que actualiza la celda dependiendo de la posicion y el icono otorgado
        public void UpdateCell(int row, int column, char icon)
        {
            boardCell[row, column].Icon = icon;
        }

        //  Este metodo es el primero en inicializar el juego, se encarga de poner los iconos de cada jugador.
        public void Init()
        {
            clearBoard();
            boardCell[3, 3].Icon = (char)playerToken.WhitePlayer;
            boardCell[4, 3].Icon = (char)playerToken.BlackPlayer;
            boardCell[4, 4].Icon = (char)playerToken.WhitePlayer;
            boardCell[3, 4].Icon = (char)playerToken.BlackPlayer;
        }

        //  El metodo clearBoard() se encarga de limpiar 
        private void clearBoard()
        {
            foreach (Cell cell in boardCell)
            {
                cell.Icon = Cell.iconEmpty;
            }
        }

        //  Este metodo actualiza cierta celda del tablero con el icono que se le pase
        public void updateBoard(List<Cell> newCell, playerToken token)
        {
            foreach (Cell currentCell in newCell)
            {
                boardCell[currentCell.Row, currentCell.Column].Icon = (char)token;
            }

            newCell.Clear();
        }

        //  Metodo que verifica si la celda obtenida mediante la posicion se encuentra vacia
        public bool cellEmpty(int row, int column)
        {
            bool isEmpty;

            isEmpty = boardCell[row, column].IsEmpty();

            return isEmpty;
        }

        // Este metodo se encarga de verificar si la CELDA es ta dentro del rango del tablero
        public bool inBoard(Cell cell)
        {
            bool inBoard;

            inBoard = (cell.Row < boardSize) && (cell.Row >= 0) && (cell.Column < boardSize) && (cell.Column >= 0);

            return inBoard;
        }

        //  Metodo encargado de verificar si los valores dados estan dentro del rango del tablero
        public bool inBoard(int row, int column)
        {
            bool inBoard;

            inBoard = (row < boardSize) && (row >= 0) && (column < boardSize) && (column >= 0);

            return inBoard;
        }

        //  Este metodo es el encargado de revisar cada celda y buscar si en alguna de ellas esta el icono pasado.
        public int SearchIcon(char icon)
        {
            int counter = 0;

            foreach (Cell cell in boardCell)
            {
                if (cell.Icon == icon)
                {
                    counter++;
                }
            }

            return counter;
        }
    }
}