using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class AIController
    {
        private int score;
        private bool canPlay = false;

        public int Score { get => score; set => score = value; }
        public bool CanPlay { get => canPlay; set => canPlay = value; }

        //  Este metodo activa el modo Player vs IA
        public void Play(GameBoard gameBoard, out int currentRow, out int currentColumn)
        {
            ConsolePrint.AIMessage();
            AI.AIPlay(gameBoard, out currentRow, out currentColumn);
        }

    }
}
