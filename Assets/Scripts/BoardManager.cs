using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int GridSize = 10;
    public GameObject BlockPrefab;


    private GameObject[][] _blocks;
    private Sprite[] _sprites;
    private Sprite _borderSprite;

    public void Init(Sprite[] sprites, Sprite borderSprite)
    {
        _sprites = sprites;
        _borderSprite = borderSprite;
    }

    public void BuildBoard(List<int[]> board)
    {
        if (_blocks == null)
        {
            _blocks = new GameObject[board.Count][];
        }

        for (int yindex = 0; yindex < board.Count; yindex++)
        {
            int y = board.Count - yindex - 1;

            if (_blocks[y] == null)
                _blocks[y] = new GameObject[board[yindex].Length];

            var row = _blocks[y];
            var boardRow = board[yindex];
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] == null && boardRow[x] != 0)
                {
                    //0 -> 1
                    CreateBlock(x, y, boardRow[x]);
                }
                else if (row[x] != null && boardRow[x] == 0)
                {
                    //1 -> 0
                    //delete block
                    Destroy(row[x]);
                    row[x] = null;
                }
            }
        }
    }

    private void CreateBlock(int x, int y, int spriteHash)
    {
        if (spriteHash == 0)
        {
            _blocks[y][x] = null;
            return;
        }

        GameObject block = Instantiate(BlockPrefab, transform.position + new Vector3(x * GridSize, y * GridSize, 0), Quaternion.identity);

        if (spriteHash == 1)
        {
            block.GetComponent<SpriteRenderer>().sprite = _borderSprite;
        }
        else
        {
            int spriteId = spriteHash - 2;
            block.GetComponent<SpriteRenderer>().sprite = _sprites[spriteId];
        }


        _blocks[y][x] = block;
    }
}
