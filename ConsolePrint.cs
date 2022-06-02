using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class ConsolePrint : GameBoard
    {
        //  Metodo que borra los datos de la consola
        public static void Clear()
        {
            Console.Clear();
        }

        //  Metodo que cuando se llama, imprime el tablero
        public static void Show(GameBoard board, PlayerController whiteToken, AIController AIPlayer)
        {
            printPlayersScore(whiteToken, AIPlayer);
            Console.WriteLine("    A   B   C   D   E   F   G   H  ");
            Console.WriteLine("  *********************************");
            for (int i = 0; i < boardSize; i++)
            {
                printRow(board, i);
                Console.WriteLine("  *********************************");
            }
        }

        //  Metodo que imprime en consola la cantidad actual de fichas sobre el tablero
        private static void printPlayersScore(PlayerController whiteToken, AIController AIPlayer)
        {
            int whiteScore, blackScore;

            whiteScore = whiteToken.Score;
            blackScore = AIPlayer.Score;

            string scoreMessage = string.Format("{0} : {1}            {2} : {3}", "Your Points", whiteScore, "Bot Points", blackScore);
            Console.WriteLine(scoreMessage);
        }

        //  Metodo que imprime un mensaje y espera segundos para simular pensamiento
        public static void AIMessage()
        {
            string message = "AI turn...";
            Console.Write(message);

            int randomWait = new Random().Next(2000, 7000);
            System.Threading.Thread.Sleep(randomWait);
        }

        //  Imprime las filas dentro del tablero
        private static void printRow(GameBoard board, int row)
        {
            StringBuilder rowData = new StringBuilder(string.Empty, 36);
            rowData.AppendFormat("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} |", row + 1, board.BoardCell[row, 0].Icon, board.BoardCell[row, 1].Icon, board.BoardCell[row, 2].Icon, board.BoardCell[row, 3].Icon, board.BoardCell[row, 4].Icon, board.BoardCell[row, 5].Icon, board.BoardCell[row, 6].Icon, board.BoardCell[row, 7].Icon);
            Console.WriteLine(rowData);
        }


        public static void showInvalidOption()
        {
            clearLine(2);
            Console.WriteLine("Your choice is invalid, please put another different.");
            System.Threading.Thread.Sleep(3000);
            clearLine(1);
        }


        //  Metodo que valida valida que los datos otorgados son validos
        private static bool validChoise(string value)
        {
            bool validLength, validChar, result;

            validLength = value.Length == 1;
            validChar = value[0] == '1' || value[0] == '2';
            result = validLength && validChar;

            return result;
        }

        //  Metodo que pide al usuario que ponga su turno y valida
        public static string RequestPlayerToPlay()
        {
            bool isMoveValidate;
            string movement;

            Console.WriteLine(string.Format("it's your turn --> You are = {0}.", (char)playerToken.WhitePlayer));
            movement = Console.ReadLine();
            movement = movement.ToUpper();
            isMoveValidate = isPlayerStringValid(movement);
            while (!isMoveValidate)
            {
                clearLine(2);
                Console.WriteLine("The move is not valid, please try again");
                movement = Console.ReadLine();
                movement = movement.ToUpper();
                isMoveValidate = isPlayerStringValid(movement);
            }

            return movement;
        }

        // Metodo que valida que el movimiento que se eligio sea valido
        private static bool isPlayerStringValid(string movement)
        {
            bool validChar, secondValidChar, validLength, result;

            if (movement == "Q")
            {
                result = true;
            }
            else
            {
                validLength = movement.Length == 2;
                if (validLength)
                {
                    validChar = validateFirstChar(movement[0]);
                    secondValidChar = validateSecondChar(movement[1]);
                    result = validChar && secondValidChar;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        //  Metodo que valida el primer caracter dado
        private static bool validateFirstChar(char validateChar)
        {
            bool result;

            result = validateChar >= 'A' && validateChar <= 'H';
            return result;
        }
        //  Metodo que valida el segundo caracter dado
        private static bool validateSecondChar(char validateChar)
        {
            bool result;

            result = validateChar >= '1' && validateChar <= '8';
            return result;
        }

        //  Metodo que imrpime el ganador.
        public static void ShowWinner(int personScore, int AIScore, playerToken winnerToken)
        {
            StringBuilder winnerDeclerationMessage = new StringBuilder(string.Empty, 60);
            string winnerColor;

            if (winnerToken == playerToken.BlackPlayer)
            {
                winnerColor = "Black player X";
            }
            else
            {
                winnerColor = "White player O";
            }

            winnerDeclerationMessage.AppendFormat("White player score: {1}{0}BOT player score: {2}{0}The winner is: {3}!",
            Environment.NewLine, personScore, AIScore, winnerColor);
            Console.WriteLine(winnerDeclerationMessage);
        }

        //  Metodo que pregunta al usario si desea salir del juego o una revancha
        public static Manager.exitOrRematch showReturnOrExitMSG()
        {
            string choice;
            Manager.exitOrRematch rematchOrExit;
            bool isChoiceValid;
            int choiseInt;

            Console.WriteLine("Exit?{0}1.Rematch{0}2.Exit{0}", Environment.NewLine);
            choice = Console.ReadLine();
            Ex02.ConsoleUtils.Screen.Clear();
            isChoiceValid = validChoise(choice);
            while (!isChoiceValid)
            {
                Console.WriteLine("Invalid input, You want to exit?{0}1.Rematch{0}2.Exit{0}", Environment.NewLine);
                choice = Console.ReadLine();
                isChoiceValid = validChoise(choice);
            }

            choiseInt = choice[0] - '0';
            rematchOrExit = (Manager.exitOrRematch)choiseInt;

            return rematchOrExit;
        }

        //  Metodo que imprime si el juego acabo en revancha
        public static void showDrawMessage(int personScore, int AIScore)
        {
            string message = string.Format("White player score: {1}{0}BOT player score: {2}{0} DRAW!",
                Environment.NewLine, personScore, AIScore);
            Console.WriteLine(message);
        }

        //  Metodo que limpia la consola en cierta linea de esta
        private static void clearConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        //  Metodo que limpia la consola con referencia a un numero de linea
        private static void clearLine(int numLines)
        {
            for (int i = 0; i < numLines; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                clearConsoleLine();
            }
        }
    }
}
