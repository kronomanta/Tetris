using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int GridSize = 10;
    public GameObject BlockPrefab;


    private GameObject[][] _blocks;
    private Sprite[] _sprites;
    private Sprite _borderSprite;
    private Transform _myTransform;

    public void Init(Sprite[] sprites, Sprite borderSprite)
    {
        _sprites = sprites;
        _borderSprite = borderSprite;
        _myTransform = transform;
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
                UpdateBlock(x, y, boardRow[x]);
            }
        }

    }

    private void UpdateBlock(int x, int y, int spriteHash)
    {
        GameObject go = _blocks[y][x];

        if (spriteHash == 0)
        {
            //->0
            if (go == null || !go.activeSelf) return;
            go.SetActive(false);
        }
        else
        {
            //->1
            if (go == null)
            {
                go = Instantiate(BlockPrefab, _myTransform.position + new Vector3(x * GridSize, y * GridSize, 0), Quaternion.identity);
                go.name = "Block -";
                _blocks[y][x] = go;
            }

            int spriteId = spriteHash == 1 ? 1 : spriteHash - 2;

            if (go.name.Split('-')[1] != spriteId.ToString())
            {
                go.name = "Block -" + spriteId;
                go.GetComponent<SpriteRenderer>().sprite = spriteHash == 1 ? _borderSprite : _sprites[spriteId];
            }

            if (!go.activeSelf)
                go.SetActive(true);
        }
    }
}
