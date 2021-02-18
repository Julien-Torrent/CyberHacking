using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CyberHacking
{
    /// <summary>
    /// State of the game
    /// </summary>
    public enum State
    {
        Waiting,  // Waiting to start
        Playing,  // Playing the game
        Won,      // Game is won
        GameOver  // Game is finished
    }

    /// <summary>
    /// Available to the user
    /// </summary>
    enum ActionType
    {
        Horizontal, // Can choose a case on same line
        Vertical,   // Can choooe a case on same column
    }

    public class CyberHacking
    {
        /// <summary>
        /// Value used to indicate that a case has been used
        /// </summary>
        public static readonly int Used = -1;

        /// <summary>
        /// The possible values for the cases, this values are converted to X2 when printed
        /// Current Values are the following : E9 1C 55 BD
        /// </summary>
        public static readonly IList<int> PossibleValues = new List<int>() { 233, 28, 85, 189 }; //E9 1C 55 BD

        /// <summary>
        /// Random values used to generate the grid
        /// </summary>
        public static readonly Random random = new Random();

        /// <summary>
        /// Colors used to write the used cases to the console
        /// </summary>
        public static readonly ConsoleColor UsedColor = ConsoleColor.DarkGray;

        /// <summary>
        /// Colors used to write the higlighted rows or cols to the console
        /// </summary>
        public static readonly ConsoleColor Background = ConsoleColor.DarkYellow;


        /// <summary>
        /// True if the game is finished, else false
        /// </summary>
        public bool IsGameOver => IsBufferFull || GameState == State.GameOver || GameState == State.Won;

        /// <summary>
        /// The State of the game
        /// </summary>
        public State GameState { get; private set; } = State.Waiting;

        /// <summary>
        /// True if the Buffer is full, if so game is over
        /// </summary>
        public bool IsBufferFull => _buffer.Count >= BufferSize;

        /// <summary>
        /// The size of the buffer
        /// </summary>
        public int BufferSize { get; init; } = 5;

        /// <summary>
        /// Public access to the buffer, cannot be changed
        /// </summary>
        public IReadOnlyList<int> Buffer => _buffer;

        /// <summary>
        /// The sequence the player should find
        /// </summary>
        public IList<int> Sequence { get; init; } = new List<int> { 233, 28, 85, 189 };

        /// <summary>
        /// The grid the player must find the sequence in
        /// </summary>
        public int[,] Grid { get; private set; } = new int[5, 5];


        // The action the player can do. Horizontal => can click on same line. Vertical => Same Column
        private ActionType _availableAction = ActionType.Horizontal;

        // The currrent position of the player, combined with _availableAction
        private int _currentPosition = 0;

        // The buffer of the player, should contains Sequence to win
        private readonly List<int> _buffer = new List<int>();

        /// <summary>
        /// Create a new instance of the game with a random grid
        /// </summary>
        public CyberHacking()
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    Grid[x, y] = PossibleValues[random.Next(PossibleValues.Count)];
                }
            }
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public void Start()
        {
            GameState = State.Playing;
        }

        /// <summary>
        /// Allow the user to play a case, add to buffer
        /// Call Start() before this method
        /// </summary>
        /// <param name="pos">The col or row the user wants to play</param>
        public void Play(int pos)
        {
            // The game must be in playing state
            if (GameState != State.Playing) return;

            // If the user can play horizontally
            if (_availableAction == ActionType.Horizontal)
            {
                // Case has already been played
                if (Grid[_currentPosition, pos] == Used) return;

                Debug.WriteLine($"{_currentPosition},{pos} = {Grid[_currentPosition, pos]:X2}");

                // Add value to buffer and mark as used
                _buffer.Add(Grid[_currentPosition, pos]);
                Grid[_currentPosition, pos] = Used;

                // the player can now play vertically
                _availableAction = ActionType.Vertical;
            }
            // the user can play verticallly
            else if (_availableAction == ActionType.Vertical)
            {
                // Case has already been played
                if (Grid[pos, _currentPosition] == Used) return;

                Debug.WriteLine($"{pos},{_currentPosition} = {Grid[pos, _currentPosition]:X2}");

                // Add value to buffer and mark as used
                _buffer.Add(Grid[pos, _currentPosition]);
                Grid[pos, _currentPosition] = Used;

                // the player can now play horizontally
                _availableAction = ActionType.Horizontal;
            }

            // Save the new position
            _currentPosition = pos;

            // Check if game is over
            ValidateGame();
        }

        /// <summary>
        /// Checks if the game is finished, or the player has found the sequence
        /// </summary>
        public void ValidateGame()
        {
            if (_buffer.Count >= Sequence.Count)
            {
                if (Enumerable.Range(0, _buffer.Count - Sequence.Count + 1).Any(n => _buffer.Skip(n).Take(Sequence.Count).SequenceEqual(Sequence)))
                {
                    GameState = State.Won;
                    return;
                }
            }

            if (IsBufferFull)
            {
                GameState = State.GameOver;
            }
        }

        /// <summary>
        /// Prints the sequence to the console
        /// </summary>
        public void PrintSequence()
        {
            Console.Write("Sequence : ");

            foreach (var val in Sequence)
            {
                Console.Write($"[{val:X2}]");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print the buffer to the console
        /// </summary>
        public void PrintBuffer()
        {
            Console.Write("Buffer : ");

            for (int i = 0; i < BufferSize; i++)
            {
                if (i < Buffer.Count)
                    Console.Write($"[{Buffer[i]:X2}]");
                else
                    Console.Write($"[  ]");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Prints the grid to the console
        /// </summary>
        public void PrintGrid()
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    // Mark case a used
                    if (Grid[x, y] == Used)
                    {
                        Console.BackgroundColor = UsedColor;
                        Console.Write("[] ");
                    }
                    // Higlight the column
                    else if (_availableAction == ActionType.Vertical && _currentPosition == y)
                    {
                        Console.BackgroundColor = Background;
                        Console.Write($"{Grid[x, y]:X2} ");
                    }
                    // Higlight the row
                    else if (_availableAction == ActionType.Horizontal && _currentPosition == x)
                    {
                        Console.BackgroundColor = Background;
                        Console.Write($"{Grid[x, y]:X2} ");
                    }
                    // Write with default colors
                    else
                    {
                        Console.ResetColor();
                        Console.Write($"{Grid[x, y]:X2} ");
                    }
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}