using System;
using System.Collections.Generic;

namespace OthelloGame
{
    public class Manager : GameBoard
    {
        //  Enumerator encargado de registrar el valor de salida o de revancha
        public enum exitOrRematch
        {
            Rematch = 1,
            Exit = 2
        }

        //  Enumerator encargado de guardar las direcciones en el juego
        public enum Directions
        {
            Up = -1,
            Down = 1,
            Left = -1,
            Right = 1,
            NoDirection = 0
        }

        private GameBoard board;
        private List<Cell> blackAIPlayerOpt = new List<Cell>();
        private List<Cell> whitePlayerOpt = new List<Cell>();
        private playerToken tokenTurn;

        //  Metodo principal que general el juego
        public void Run()
        {
            //  Se crean los objetos jugadores
            PlayerController whiteHumanPlayer = new PlayerController(playerToken.WhitePlayer);
            AIController blackAIPlayer = new AIController();
            int currentRowMovement, currentColumnMovement;
            bool isMovingLegally, gameOver = false;
            List<Cell> cellsToUpdate = new List<Cell>();
            exitOrRematch rematchOrExit;

            generalSettings(whiteHumanPlayer, blackAIPlayer);
            initialize(whiteHumanPlayer, blackAIPlayer);
            while (true)
            {
                ConsolePrint.Show(board, whiteHumanPlayer, blackAIPlayer);
                do
                {
                    tellCurrentPlayerToPlay(whiteHumanPlayer, blackAIPlayer, blackAIPlayerOpt, out currentRowMovement, out currentColumnMovement);
                    if (currentColumnMovement == (int)PlayerController.UserRequest.Exit)
                    {
                        gameOver = true;
                        break;
                    }

                    isMovingLegally = isLegalMove(currentRowMovement, currentColumnMovement, ref cellsToUpdate);
                    if (!isMovingLegally)
                    {
                        ConsolePrint.showInvalidOption();
                    }
                }
                while (!isMovingLegally);

                //  Metodo que cierra el programa
                if (gameOver)
                {
                    break;
                }

                board.updateBoard(cellsToUpdate, tokenTurn);
                updatePlayersOptions();
                updatePlayersScore(whiteHumanPlayer, blackAIPlayer);
                turnChangingManager();
                gameOver = isGameOver();
                Console.Clear();
                if (gameOver)
                {
                    declareWinner();
                    rematchOrExit = ConsolePrint.showReturnOrExitMSG();
                    if (rematchOrExit == exitOrRematch.Rematch)
                    {
                        gameOver = false;
                        restartGame(whiteHumanPlayer, blackAIPlayer);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Console.WriteLine("Thank you for playing Othello!");
            System.Threading.Thread.Sleep(5000);
        }

        //  Constructor principal del juego
        public Manager(GameBoard board1, playerToken pToken)
        {
            GameBoard copyBoard = board1;
            board = copyBoard;
            tokenTurn = pToken;
        }

        public Manager()
        {
        }

        //  Metodo que actualiza la puntuacion
        private void updatePlayersScore(PlayerController whiteToken, AIController blackAIToken)
        {
            int whitePlayerScore, blackPlayerScore;

            //  Contamos la suma de todas las fichas en el tablero de cada jugador y despues la actualizamos
            whitePlayerScore = board.SearchIcon((char)playerToken.WhitePlayer);
            blackPlayerScore = board.SearchIcon((char)playerToken.BlackPlayer);
            whiteToken.Score = whitePlayerScore;
            blackAIToken.Score = blackPlayerScore;
        }

        //  Metodo que habilita la jugabilidad para el bot y el token blanco, creando un nuevo tablero
        public void generalSettings(PlayerController whiteToken, AIController blackAIToken)
        {

            whiteToken.CanPlay = true;
            blackAIToken.CanPlay = true;

            board = new GameBoard();
        }

        //  Metodo que reinicia el juego
        private void restartGame(PlayerController whiteToken, AIController blackAIToken)
        {
            initialize(whiteToken, blackAIToken);
        }

        //  Metodo que configura la posicion y el puntuaje inicial del tablero
        private void initialize(PlayerController whiteToken, AIController blackAIToken)
        {
            board.Init();

            if (blackAIPlayerOpt.Count != 0)
            {
                blackAIPlayerOpt.Clear();
            }

            if (whitePlayerOpt.Count != 0)
            {
                whitePlayerOpt.Clear();
            }

            //  Metodos encargados de inicializar los tokens en sus debidas posiciones
            initBlackTokens();
            initWhiteTokens();


            //  Inicializando la puntuacion de los jugadores
            whiteToken.Score = 2;
            blackAIToken.Score = 2;

            tokenTurn = playerToken.WhitePlayer;
        }

        //  Metodo que maneja el cambio de turnos
        private void turnChangingManager()
        {
            if (tokenTurn == playerToken.BlackPlayer && whitePlayerOpt.Count > 0)
            {
                tokenTurn = playerToken.WhitePlayer;
            }
            else if (tokenTurn == playerToken.WhitePlayer && blackAIPlayerOpt.Count > 0)
            {
                tokenTurn = playerToken.BlackPlayer;
            }
            else if ((tokenTurn == playerToken.BlackPlayer && whitePlayerOpt.Count == 0)
                 || (tokenTurn == playerToken.WhitePlayer && blackAIPlayerOpt.Count == 0))
            {
                Console.WriteLine("Its your turn again, the enemy has options");
            }
        }

        //  Metodo que recive los jugadores y decide el turno de juego de cada uno de ellos
        private void tellCurrentPlayerToPlay(PlayerController whiteToken, AIController blackAIToken, List<Cell> blackAITokenOptions, out int currentRow, out int currentColumn)
        {
            if (tokenTurn == playerToken.BlackPlayer)
            {
                blackAIToken.Play(board, out currentRow, out currentColumn);
            }
            else
            {
                whiteToken.Play(out currentRow, out currentColumn);
            }
        }


        public List<Cell> blackAIPlayerOptions { get => blackAIPlayerOpt; }

        public List<Cell> WhitePlayerOptions { get => whitePlayerOpt; }

        //  Metodo que actualiza la liste de jugadores
        public void updatePlayersOptions()
        {
            List<Cell> cellList = new List<Cell>();
            playerToken playerTurn;
            bool isCellAnOption, shouldMethodAddCellsToUpdateList;

            whitePlayerOpt.Clear();
            blackAIPlayerOpt.Clear();
            playerTurn = tokenTurn;
            shouldMethodAddCellsToUpdateList = false;
            foreach (Cell cell in board.BoardCell)
            {
                if (cell.Icon == Cell.iconEmpty)
                {
                    tokenTurn = playerToken.WhitePlayer;
                    isCellAnOption = playerBlockingEnemy(cell.Row, cell.Column, ref cellList, shouldMethodAddCellsToUpdateList);
                    if (isCellAnOption)
                    {
                        whitePlayerOpt.Add(cell);
                    }

                    tokenTurn = playerToken.BlackPlayer;
                    isCellAnOption = playerBlockingEnemy(cell.Row, cell.Column, ref cellList, shouldMethodAddCellsToUpdateList);
                    if (isCellAnOption)
                    {
                        blackAIPlayerOpt.Add(cell);
                    }
                }
            }

            tokenTurn = playerTurn;
        }

        public playerToken Turn { get => tokenTurn; set => tokenTurn = value; }

        public GameBoard Board { get => board; set => board = value; }

        //  Metodo que recive un movimiento y verifica si es valido respecto a las reglas del juego
        private bool isLegalMove(int rowMovement, int columnMovement, ref List<Cell> udateThisCell)
        {
            bool isMovingLegally, emptyCell, movementBlockByEnemy, playerCanMove;

            playerCanMove = validMove(board.BoardCell[rowMovement, columnMovement]);
            if (playerCanMove)
            {
                emptyCell = board.cellEmpty(rowMovement, columnMovement);
                movementBlockByEnemy = playerBlockingEnemy(rowMovement, columnMovement, ref udateThisCell);
                isMovingLegally = emptyCell && movementBlockByEnemy;
            }
            else
            {
                isMovingLegally = false;
            }

            return isMovingLegally;
        }

        //  Metodo que recive una celda y verifica si esta en la lista de movimientos posibles
        private bool validMove(Cell searchCell)
        {
            bool canNotMove, equalCell;

            canNotMove = false;
            if (tokenTurn == playerToken.BlackPlayer)
            {
                foreach (Cell cell in blackAIPlayerOptions)
                {
                    equalCell = equalsCells(cell, searchCell);
                    if (equalCell)
                    {
                        canNotMove = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (Cell cell in WhitePlayerOptions)
                {
                    equalCell = equalsCells(cell, searchCell);
                    if (equalCell)
                    {
                        canNotMove = true;
                        break;
                    }
                }
            }

            return canNotMove;
        }

        //  Metodo que recive 2 celdas y verifica si son iguales o diferentes
        private bool equalsCells(Cell cellOne, Cell cellTwo)
        {
            bool equalCell;

            equalCell = cellOne.Row == cellTwo.Row && cellOne.Column == cellTwo.Column;

            return equalCell;
        }

        //  !Metodo que revisa las distintas direcciones y verifica que otra celda no este bloqueada por un token enemigo
        //  Tambien actualiza la lista de celdas que el jugador tiene disponibles para moverse
        public bool playerBlockingEnemy(int rowMovement, int columnMovement, ref List<Cell> udateThisCell, bool concatCellToList = true)
        {
            bool verticalLock, horizontalLock, firstDiagonalLock, secondDiagonalLock, movementBlockByEnemy;

            verticalLock = verticallyLocked(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Up, (int)Directions.NoDirection, concatCellToList);
            verticalLock = verticallyLocked(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Down, (int)Directions.NoDirection, concatCellToList) || verticalLock;
            horizontalLock = horizontallyLocked(rowMovement, columnMovement, ref udateThisCell, (int)Directions.NoDirection, (int)Directions.Left, concatCellToList);
            horizontalLock = horizontallyLocked(rowMovement, columnMovement, ref udateThisCell, (int)Directions.NoDirection, (int)Directions.Right, concatCellToList) || horizontalLock;
            firstDiagonalLock = firstDiagonalIsLock(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Up, (int)Directions.Right, concatCellToList);
            firstDiagonalLock = firstDiagonalIsLock(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Down, (int)Directions.Left, concatCellToList) || firstDiagonalLock;
            secondDiagonalLock = secondDiagonalIsLock(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Up, (int)Directions.Left, concatCellToList);
            secondDiagonalLock = secondDiagonalIsLock(rowMovement, columnMovement, ref udateThisCell, (int)Directions.Down, (int)Directions.Right, concatCellToList) || secondDiagonalLock;
            movementBlockByEnemy = verticalLock || horizontalLock || firstDiagonalLock || secondDiagonalLock;

            return movementBlockByEnemy;
        }

        //  Metodo que verifica si hay una secuencia de bloqueos
        private bool isBlockingLine(int rowMovement, int columnMovement, int verticalDir, int horizontalDir, out Cell o_cellIter)
        {
            int currentRow, currentColumn;
            Cell cellIter;
            bool isBlockFound, onBoardLimit;

            currentRow = rowMovement + verticalDir;
            currentColumn = columnMovement + horizontalDir;
            onBoardLimit = inBoard(currentRow, currentColumn);
            isBlockFound = false;
            if (onBoardLimit)
            {
                cellIter = new Cell(currentRow, currentColumn, board.BoardCell[currentRow, currentColumn].Icon);
                isBlockFound = isSeriesFound(ref cellIter, verticalDir, horizontalDir);
            }
            else
            {
                cellIter = null;
            }

            o_cellIter = cellIter;

            return isBlockFound;
        }

        //  Metodo que verifica si una diagonal vertical en direccion -y esta bloqueada
        private bool secondDiagonalIsLock(int rowMovement, int columnMovement, ref List<Cell> udateThisCell, int verticalDir, int horizontalDir, bool concatCellToList)
        {
            Cell cellIter = null;
            bool isBlockFound;
            int row;

            isBlockFound = isBlockingLine(rowMovement, columnMovement, verticalDir, horizontalDir, out cellIter);
            if (isBlockFound && concatCellToList == true)
            {
                if (horizontalDir == (int)Directions.Left && verticalDir == (int)Directions.Up)
                {
                    row = rowMovement;
                    for (int column = columnMovement; column > cellIter.Column; column--)
                    {
                        udateThisCell.Add(board.BoardCell[row, column]);
                        row += (int)Directions.Up;
                    }
                }
                else
                {
                    row = rowMovement;
                    for (int i = columnMovement; i < cellIter.Column; i++)
                    {
                        udateThisCell.Add(board.BoardCell[row, i]);
                        row += (int)Directions.Down;
                    }
                }
            }

            return isBlockFound;
        }

        //  Metodo que verifica si una diagonal vertical en direccion +y esta bloqueada
        private bool firstDiagonalIsLock(int rowMovement, int columnMovement, ref List<Cell> udateThisCell, int verticalDir, int horizontalDir, bool concatCellToList)
        {
            Cell cellIter = null;
            bool isBlockFound;
            int row;

            isBlockFound = isBlockingLine(rowMovement, columnMovement, verticalDir, horizontalDir, out cellIter);
            if (isBlockFound && concatCellToList == true)
            {
                if (horizontalDir == (int)Directions.Left && verticalDir == (int)Directions.Down)
                {
                    row = rowMovement;
                    for (int column = columnMovement; column > cellIter.Column; column--)
                    {
                        udateThisCell.Add(board.BoardCell[row, column]);
                        row += (int)Directions.Down;
                    }
                }
                else
                {
                    row = rowMovement;
                    for (int i = columnMovement; i < cellIter.Column; i++)
                    {
                        udateThisCell.Add(board.BoardCell[row, i]);
                        row += (int)Directions.Up;
                    }
                }
            }

            return isBlockFound;
        }

        //  Metodo que verifica si se encuentra bloqueada la direccion horizontal
        private bool horizontallyLocked(int rowMovement, int columnMovement, ref List<Cell> udateThisCell, int verticalDir, int horizontalDir, bool concatCellToList)
        {
            Cell cellIter = null;
            bool isBlockFound;

            isBlockFound = isBlockingLine(rowMovement, columnMovement, verticalDir, horizontalDir, out cellIter);
            if (isBlockFound && concatCellToList == true)
            {
                if (horizontalDir == (int)Directions.Left)
                {
                    for (int column = columnMovement; column > cellIter.Column; column--)
                    {
                        udateThisCell.Add(board.BoardCell[rowMovement, column]);
                    }
                }
                else
                {
                    for (int column = columnMovement; column < cellIter.Column; column++)
                    {
                        udateThisCell.Add(board.BoardCell[rowMovement, column]);
                    }
                }
            }

            return isBlockFound;
        }

        //  Metodo que verifica si la direccion vertical se encuentra bloqueada
        private bool verticallyLocked(int rowMovement, int columnMovement, ref List<Cell> udateThisCell, int verticalDir, int horizontalDir, bool concatCellToList)
        {
            Cell cellIter = null;
            bool isBlockFound;

            isBlockFound = isBlockingLine(rowMovement, columnMovement, verticalDir, horizontalDir, out cellIter);
            if (isBlockFound && concatCellToList == true)
            {
                if (verticalDir == (int)Directions.Up)
                {
                    for (int row = rowMovement; row > cellIter.Row; row--)
                    {
                        udateThisCell.Add(board.BoardCell[row, columnMovement]);
                    }
                }
                else
                {
                    for (int row = rowMovement; row < cellIter.Row; row++)
                    {
                        udateThisCell.Add(board.BoardCell[row, columnMovement]);
                    }
                }
            }

            return isBlockFound;
        }

        //  Metodo que verifica si se encontraron mas de 1 bloqueo en serie
        private bool isSeriesFound(ref Cell cellIter, int verticalDir, int horizontalDir)
        {
            bool isCellEnemy, onBoardLimit;
            bool isBlockingLine, isCharSimilarToMeFound;

            isCellEnemy = false;
            isBlockingLine = false;
            onBoardLimit = board.inBoard(cellIter);
            if (onBoardLimit)
            {
                isCellEnemy = isCellAnEnemy(cellIter, Turn);
            }

            if (onBoardLimit && isCellEnemy)
            {
                do
                {
                    cellIter.Row += verticalDir;
                    cellIter.Column += horizontalDir;
                    onBoardLimit = board.inBoard(cellIter);
                    if (onBoardLimit)
                    {
                        cellIter.Icon = board.BoardCell[cellIter.Row, cellIter.Column].Icon;
                        isCellEnemy = isCellAnEnemy(cellIter, Turn);
                    }
                }
                while (onBoardLimit && isCellEnemy);

                if (onBoardLimit)
                {
                    isCharSimilarToMeFound = cellIter.Icon == (char)Turn;
                    if (isCharSimilarToMeFound)
                    {
                        isBlockingLine = true;
                    }
                }
            }

            return isBlockingLine;
        }


        //  Metodo que verifica si la celda dada es un enemigo checando si el signo es igual al otorgado.
        private bool isCellAnEnemy(Cell i_cellIter, playerToken currentToken)
        {
            bool isCellEnemy;

            isCellEnemy = i_cellIter.Icon != (char)currentToken && i_cellIter.Icon != Cell.iconEmpty;

            return isCellEnemy;
        }

        //  Methodo que recive un icono o token y verifica si tiene opciones de movimiento
        private bool isPlayerOptionEmpty(playerToken token)
        {
            bool isEmpty;

            isEmpty = whitePlayerOpt.Count == 0;

            return isEmpty;
        }

        //  Verifica si el juego se acabo, esto comprobando si alguno de los 2 no tienes mas opciones de movimiento
        private bool isGameOver()
        {
            bool hasNoOption;

            hasNoOption = isPlayerOptionEmpty(playerToken.BlackPlayer) && isPlayerOptionEmpty(playerToken.WhitePlayer);

            return hasNoOption;
        }

        //  Metodo que declara el ganador
        private void declareWinner()
        {
            int whitePlayerScore, blackPlayerScore;
            playerToken winner;

            //  Contamos la suma de todas las fichas en el tablero de cada jugador
            whitePlayerScore = board.SearchIcon((char)playerToken.WhitePlayer);
            blackPlayerScore = board.SearchIcon((char)playerToken.BlackPlayer);
            if (whitePlayerScore > blackPlayerScore)
            {
                winner = playerToken.WhitePlayer;
                ConsolePrint.ShowWinner(whitePlayerScore, blackPlayerScore, winner);
            }
            else if (whitePlayerScore < blackPlayerScore)
            {
                winner = playerToken.BlackPlayer;
                ConsolePrint.ShowWinner(whitePlayerScore, blackPlayerScore, winner);
            }
            else
            {
                ConsolePrint.showDrawMessage(whitePlayerScore, blackPlayerScore);
            }
        }

        //  Metodo que inicializa los valores de la IA en el tablero
        private void initBlackTokens()
        {
            Cell cellToBeAddedToOptions1;
            Cell cellToBeAddedToOptions2;
            Cell cellToBeAddedToOptions3;
            Cell cellToBeAddedToOptions4;

            cellToBeAddedToOptions1 = new Cell(2, 3);
            cellToBeAddedToOptions2 = new Cell(3, 2);
            cellToBeAddedToOptions3 = new Cell(5, 4);
            cellToBeAddedToOptions4 = new Cell(4, 5);

            blackAIPlayerOpt.Add(cellToBeAddedToOptions1);
            blackAIPlayerOpt.Add(cellToBeAddedToOptions2);
            blackAIPlayerOpt.Add(cellToBeAddedToOptions3);
            blackAIPlayerOpt.Add(cellToBeAddedToOptions4);
        }

        //  Metodo que inicializa los valores en el tablero
        private void initWhiteTokens()
        {
            Cell cellToBeAddedToOptions1;
            Cell cellToBeAddedToOptions2;
            Cell cellToBeAddedToOptions3;
            Cell cellToBeAddedToOptions4;

            cellToBeAddedToOptions1 = new Cell(2, 4);
            cellToBeAddedToOptions2 = new Cell(3, 5);
            cellToBeAddedToOptions3 = new Cell(4, 2);
            cellToBeAddedToOptions4 = new Cell(5, 3);

            whitePlayerOpt.Add(cellToBeAddedToOptions1);
            whitePlayerOpt.Add(cellToBeAddedToOptions2);
            whitePlayerOpt.Add(cellToBeAddedToOptions3);
            whitePlayerOpt.Add(cellToBeAddedToOptions4);
        }


    }
}