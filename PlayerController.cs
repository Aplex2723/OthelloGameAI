using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloGame
{
    public class PlayerController : GameBoard
    {
        public enum UserRequest
        {
            Exit = -1
        }

        private int score;
        private bool canPlay = false;
        private playerToken playerIcon;

        //  El metodo Play le pide al usuario que ingrese los valores validos
        public void Play(out int playerRowMove, out int playerColumnMove)
        {
            string movement;
            bool wantExit;

            movement = ConsolePrint.RequestPlayerToPlay();
            wantExit = movement.Length == 1;
            if (wantExit)
            {
                playerColumnMove = (int)UserRequest.Exit;
                playerRowMove = (int)UserRequest.Exit;
            }
            else
            {
                playerColumnMove = movement[0] - 'A';
                playerRowMove = movement[1] - '1';
            }
        }

        //  Constructor que pide el token para el jugador principal
        public PlayerController(playerToken icon)
        {
            playerIcon = icon;
        }

        public bool CanPlay { get => canPlay; set => canPlay = value; }
        public int Score { get => score; set => score = value; }
        public playerToken PlayerIcon { get => playerIcon; set => playerIcon = value; }
    }
}
