using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class AI : GameBoard
    {
        /*      USO DE ALGORITMO MINIMAX PARA LA RECREACION DEL JUEGO      
         *   El metodo Minimax es el encargado de regresar una lista de valores pares con 
         *   el puntaje tanto del jugador como de la IA por referencias.
         */

        public static int Minimax(GameBoard board, int i_Depth, playerToken pToken, ref Cell cell, ref List<KeyValuePair<int, List<Cell>>> keyValues)
        {
            Manager gameMangaerAI = new Manager(board, pToken);
            List<Cell> optionsList = new List<Cell>();
            List<Cell> movementsList = new List<Cell>();
            KeyValuePair<int, List<Cell>> cellListPair;
            GameBoard copyBoard;
            int eval, minEval, maxEval;

            if (i_Depth == 0 || isGameOver(board, pToken))
            {
                return heuristic(board, pToken);
            }
            else
            {
                gameMangaerAI.updatePlayersOptions();
                if (pToken == playerToken.BlackPlayer)
                {
                    maxEval = int.MinValue;
                    foreach (Cell cellI in gameMangaerAI.blackAIPlayerOptions)
                    {
                        copyBoard = gameMangaerAI.Board.Copy();
                        gameMangaerAI.playerBlockingEnemy(cellI.Row, cellI.Column, ref optionsList);
                        copyBoard.updateBoard(optionsList, pToken);
                        eval = Minimax(copyBoard, i_Depth - 1, playerToken.WhitePlayer, ref cell, ref keyValues);
                        if (eval > maxEval)
                        {
                            movementsList.Add(cell);
                            cellListPair = new KeyValuePair<int, List<Cell>>(eval, movementsList);
                            keyValues.Add(cellListPair);
                            cell.Row = cellI.Row;
                            cell.Column = cellI.Column;
                            maxEval = eval;
                        }
                    }

                    return maxEval;
                }
                else
                {
                    minEval = int.MaxValue;
                    foreach (Cell cellI in gameMangaerAI.WhitePlayerOptions)
                    {
                        copyBoard = gameMangaerAI.Board.Copy();
                        gameMangaerAI.playerBlockingEnemy(cellI.Row, cellI.Column, ref optionsList);
                        copyBoard.updateBoard(optionsList, pToken);
                        eval = Minimax(copyBoard, i_Depth - 1, playerToken.BlackPlayer, ref cell, ref keyValues);
                        if (eval < minEval)
                        {
                            movementsList.Add(cell);
                            cellListPair = new KeyValuePair<int, List<Cell>>(eval, movementsList);
                            keyValues.Add(cellListPair);
                            minEval = eval;
                        }
                    }

                    return minEval;
                }
            }
        }

        //  Metodo que calcula la diferencia entre los tokens del jugador y los de la IA
        private static int differenceScoreAIHuman(GameBoard board)
        {
            int whiteTokens, blackTokens, difference;

            whiteTokens = board.SearchIcon((char)playerToken.WhitePlayer);
            blackTokens = board.SearchIcon((char)playerToken.BlackPlayer);
            difference = blackTokens - whiteTokens;

            return difference;
        }

        //  Metodo que recorre la lista de celdas y si puede hacer un maximizer
        private static bool isGameOver(GameBoard board, playerToken pToken)
        {
            Manager tempGameManager = new Manager(board, pToken);
            List<Cell> cellLists = new List<Cell>();
            bool addToCellsList, isCellAnOption, currentGameNotOver;

            addToCellsList = false;
            currentGameNotOver = false;
            foreach (Cell cellI in board.BoardCell)
            {
                isCellAnOption = tempGameManager.playerBlockingEnemy(cellI.Row, cellI.Column, ref cellLists, addToCellsList);
                if (isCellAnOption)
                {
                    return currentGameNotOver;
                }
            }

            currentGameNotOver = true;

            return currentGameNotOver;
        }

        /*  Metodo que escoge el movimiento correcto o esperado usando el algoritmo Minimax
         *  este metodo recorre la lista de los score obtenidos, despues selecciona el mejor puntaje 
         */
        public static void AIPlay(GameBoard board, out int currentRow, out int currentColumn)
        {
            int output;
            Cell chosenCell = new Cell();
            List<KeyValuePair<int, List<Cell>>> scoresAndMovements = new List<KeyValuePair<int, List<Cell>>>();
            output = Minimax(board, 2, playerToken.BlackPlayer, ref chosenCell, ref scoresAndMovements);

            scoresAndMovements.Sort((x, y) => x.Key.CompareTo(y.Key));  //  En esta parte ordena la lista por su puntuacion
            currentRow = scoresAndMovements[scoresAndMovements.Count - 1].Value[0].Row;
            currentColumn = scoresAndMovements[scoresAndMovements.Count - 1].Value[0].Column;
        }

        //  Metodo que regresa el score calculado
        private static int getCornersHeuristic(GameBoard board, char Token)
        {
            int result, boadEdges;

            boadEdges = boardSize - 1;
            result = 0;
            Cell[] boardCorners =
                {
                board.BoardCell[0, 0],
                board.BoardCell[0, boadEdges],
                board.BoardCell[boadEdges, 0],
                board.BoardCell[boadEdges, boadEdges],
            };

            foreach (Cell corner in boardCorners)
            {
                if (corner.Icon == Token)
                {
                    result += 20;
                }
            }

            return result;
        }

        /* Se implemento el metodo heuristic para que la IA pudiese verificar
         * cuando el jugador o la IA van ganando*/
        private static int heuristic(GameBoard board, playerToken pToken)
        {
            int result;
            int differenceAIHuman;
            char palyerTurn;

            result = 0;
            differenceAIHuman = differenceScoreAIHuman(board);
            palyerTurn = pToken == playerToken.BlackPlayer ? (char)playerToken.BlackPlayer : (char)playerToken.WhitePlayer;
            result += getCornersHeuristic(board, palyerTurn);
            return result + differenceAIHuman;
        }
    }
}
