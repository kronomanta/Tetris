using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    enum Movement
    {
        Left, Right, Down
    }

    enum GameState
    {
        Run,
        GameOver
    }

    public Vector2 BoardSize = new Vector2(11, 20);
    public float Speed = 1;
    public float SpeedStep = .1f;
    public int SpeedLineStep = 3;
    public int GUIFPS = 45;


    public Sprite[] BlockSprites;
    public Sprite BoardSprite;

    private int _completedLines = 0;
    private bool _boardChanged = false;

    private BoardManager _boardManager;

    private PatternContainer[] _blocks = new PatternContainer[]
    {
        #region I
        /*
        * 1111
        * 
        * 1
        * 1
        * 1
        * 1
        */
        new PatternContainer(
            new Pattern(new int[]{1,1,1,1}),
            new Pattern(new int[]{1}, new int[]{1}, new int[]{1}, new int[]{1})
        ),
        #endregion

        #region Z
        /*
        * 110
        * 011
        * 
        * 01
        * 11
        * 10
        */
        new PatternContainer(
            new Pattern(new int[]{1,1,0}, new int[] {0,1,1}),
            new Pattern(new int[]{0,1}, new int[]{1,1}, new int[] {1,0})
        ),
        #endregion

        #region S
        /*
        * 011
        * 110
        * 
        * 10
        * 11
        * 01
        */
        new PatternContainer(
            new Pattern(new int[]{0,1,1}, new int[] {1,1,0}),
            new Pattern(new int[]{1,0}, new int[]{1,1}, new int[] { 0,1})
        ),

        #endregion

        #region L
        /*
        * 10
        * 10
        * 11
        * 
        * 001
        * 111
        * 
        * 11
        * 01
        * 01
        * 
        * 111
        * 100
        */
        new PatternContainer(
            new Pattern(new int[]{1,0}, new int[] {1,0}, new int[] {1,1}),
            new Pattern(new int[]{0,0,1}, new int[] {1,1,1}),
            new Pattern(new int[]{1,1}, new int[] {0,1}, new int[] {0,1}),
            new Pattern(new int[]{1,1,1}, new int[] {1,0,0})
        ),

        #endregion

        #region J
        /*
        * 01
        * 01
        * 11
        * 
        * 111
        * 001
        * 
        * 11
        * 10
        * 10
        * 
        * 100
        * 111
        */
        new PatternContainer(
            new Pattern(new int[]{0,1}, new int[] {0,1}, new int[] {1,1}),
            new Pattern(new int[] {1,1,1}, new int[]{0,0,1}),
            new Pattern(new int[]{1,1}, new int[] {1,0}, new int[] {1,0}),
            new Pattern(new int[] {1,0,0}, new int[]{1,1,1})
        ),
        #endregion

        #region T
        /*
       * 111
       * 010
       * 
       * 10
       * 11
       * 10
       * 
       * 010
       * 111
       * 
       * 01
       * 11
       * 01
       */
        new PatternContainer(
            new Pattern(new int[]{1,1,1}, new int[] {0,1,0}),
            new Pattern(new int[] {1,0}, new int[]{1,1}, new int[]{1,0}),
            new Pattern(new int[] {0,1,0}, new int[]{1,1,1}),
            new Pattern(new int[]{0,1}, new int[] {1,1}, new int[] {0,1})
        ),

        #endregion

        #region T
        /*
       * 11
       * 11
       * 
       */
        new PatternContainer(
            new Pattern(new int[]{1,1}, new int[] {1,1})
        )

        #endregion
    };

    private List<int[]> _board;
    private Block _currentBlockScript = null;
    private Vector2 position;
    private GameState gamestate = GameState.Run;

    private void InitBoard()
    {
        //init board
        _board = new List<int[]>((int)BoardSize.y);
        for (int i = 0; i < _board.Capacity; i++)
        {
            _board.Add(CreateEmptyLine());
        }

        int width = (int)BoardSize.x;
        for (int i = 0; i < width; i++)
        {
            //last line
            _board[_board.Count - 1][i] = 1;
        }


    }

    private void Awake()
    {
        _boardManager = GetComponentInChildren<BoardManager>();
        _boardManager.Init(BlockSprites, BoardSprite);
        InitBoard();
    }

    void Start()
    {
        Spawn();
        StartCoroutine(MoveBlockDownCoroutine());
        StartCoroutine(DrawBoard());
    }

    void Spawn()
    {
        if (_currentBlockScript == null)
        {
            _currentBlockScript = new Block();
        }

        PatternContainer patternContainer = RandomizeBlockColor(_blocks[UnityEngine.Random.Range(0, _blocks.Length)], UnityEngine.Random.Range(0, BlockSprites.Length) + 2);
        
        _currentBlockScript.Init(patternContainer);

        position.y = 0;
        position.x = (int)BoardSize.x / 2;

        WriteBlockToBoard(_board, _currentBlockScript.GetPattern(), position, clear: false);
    }

    void WriteBlockToBoard(List<int[]> board)
    {
        WriteBlockToBoard(board, _currentBlockScript.GetPattern(), position, clear: false);
    }

    void WriteBlockToBoard(List<int[]> board, Pattern pattern, Vector2 position, bool clear)
    {
        for (int y = 0; y < pattern.SymbolRows.Length; y++)
        {
            for (int x = 0; x < pattern.SymbolRows[y].Length; x++)
            {
                int posy = y + (int)position.y;
                int posx = x + (int)position.x;
                if (clear || board[posy][posx] == 0)
                {
                    board[posy][posx] = clear ? 0 : pattern.SymbolRows[y][x];
                }
            }
        }

        _boardChanged = true;
    }

    IEnumerator MoveBlockDownCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(1 / Speed);

            //keep moving down
            MoveBlock(Movement.Down);
        }
        while (gamestate != GameState.GameOver);
    }

    PatternContainer RandomizeBlockColor(PatternContainer patternContainer, int spriteId)
    {
        patternContainer = patternContainer.Clone();
        for (int p = 0; p < patternContainer.Length; p++)
        {
            Pattern pattern = patternContainer[p];
            for (int y = 0; y < pattern.Length; y++)
            {
                int[] row = pattern[y];
                for (int x = 0; x < row.Length; x++)
                {
                    row[x] *= spriteId;
                }
            }
        }

        return patternContainer;
    }

    void RotateBlock()
    {
        Pattern pattern = _currentBlockScript.GetPattern();
        Pattern nextPattern = _currentBlockScript.GetNextRotationPattern();

        for (int y = 0; y < nextPattern.Length; y++)
        {
            for (int x = 0; x < nextPattern.Width; x++)
            {
                WriteBlockToBoard(_board, pattern, position, clear: true);

                if (nextPattern[y][x] != 0 && _board[y + (int)position.y][x + (int)position.x] != 0)
                {
                    WriteBlockToBoard(_board, pattern, position, clear: false);
                    return;
                }
            }
        }

        WriteBlockToBoard(_board, _currentBlockScript.RotatePattern(), position, clear: false);
    }

    void MoveBlock(Movement movement)
    {
        Vector2 lastPosition = position;
        Pattern pattern = _currentBlockScript.GetPattern();

        int onePos;
        switch (movement)
        {
            case Movement.Down:
                for (int x = 0; x < pattern.Width; x++)
                {
                    onePos = pattern.SymbolRows.LastIndexOfDifferent(0, x);
                    if (onePos >= 0 && _board[onePos + 1 + (int)position.y][x + (int)position.x] != 0)
                    {
                        //cannot step
                        WriteBlockToBoard(_board);
                        ClearLines();

                        if (position.y == 0)
                        {
                            //Gameover
                            gamestate = GameState.GameOver;
                        }
                        else
                        {
                            Spawn();
                        }

                        return; 
                    }
                }

                position.y++;
                break;
            case Movement.Left:
                for (int y = 0; y < pattern.Length; y++)
                {
                    onePos = pattern[y].FirstIndexOfDifferent(0);
                    if (onePos >= 0 && _board[y + (int)position.y][onePos - 1 + (int)position.x] != 0) return; //cannot step
                }
                position.x--;

                break;
            case Movement.Right:
                for(int y = 0; y < pattern.Length; y++)
                {
                    onePos = pattern[y].LastIndexOfDifferent(0);
                    if (onePos >= 0 && _board[y + (int)position.y][onePos + 1 + (int)position.x] != 0) return; //cannot step
                }

                position.x++;
                break;
        }

        WriteBlockToBoard(_board, pattern, lastPosition, clear: true);
        WriteBlockToBoard(_board, pattern, position, clear: false);
    }

    IEnumerator DrawBoard()
    {
        do
        {
            if (_boardChanged)
            {
                _boardChanged = false;
                DrawBoardToConsole(_board);
                _boardManager.BuildBoard(_board);
            }
            yield return new WaitForSeconds(1f / GUIFPS);

        } while (gamestate != GameState.GameOver);
    }

    void DrawBoardToConsole(List<int[]> board)
    {
        foreach (var row in board)
        {
            string line = "";
            foreach (var c in row)
                line += c;
            Debug.Log(line);
        }
        Debug.Log("\n\n");
    }

    void ClearLines()
    {
        
        for (int y = 0; y < _board.Count-1; y++)
        {
            if (_board[y].All(x =>x != 0))
            {
                //completed line
                _board.RemoveAt(y);
                y--;

                if ((++_completedLines) % SpeedLineStep == 0)
                {
                    Speed += SpeedStep;
                }

                Debug.Log("CompletedLines: " + _completedLines);
            }
        }

        for(int y = 0; y < BoardSize.y-_board.Count; y++)
        {
            _board.Insert(0, CreateEmptyLine());
        }


        
    }

    int[] CreateEmptyLine()
    {
        int[] row = new int[(int)BoardSize.x];
        row[0] = 1;
        row[(int)BoardSize.x -1] = 1;

        return row;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Move left"))
            MoveBlock(Movement.Left);


        if (Input.GetButtonDown("Move right"))
            MoveBlock(Movement.Right);


        if (Input.GetButtonDown("Move down"))
            MoveBlock(Movement.Down);

        if (Input.GetButtonDown("Rotate"))
            RotateBlock();
    }
}
