using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum pieceType
{
    rook, knight, bishop, queen, king, pawn
}
public enum pieceColor
{
    White, Black
}
public class PieceScript : MonoBehaviour, IPointerDownHandler
{
    public pieceType type;
    public pieceColor color;
    public int currentRow;
    public int currentColumn;

    public Material selectedMaterial;
    private Material defaultMaterial;

    private List<TileScript> possibleTiles;

    public TileScript tile;

    private void Start()
    {
        defaultMaterial = GetComponent<SpriteRenderer>().material;
        AddPhysics2DRaycaster();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(GameControl.Instance.selectedOne == null)
        {
            GetComponent<SpriteRenderer>().material = selectedMaterial;
            GameControl.Instance.selectedOne = this;
            GameObject[] gameObjects = new GameObject[0];
            if (color == pieceColor.White)
                gameObjects = GameControl.Instance.white;
            else
                gameObjects = GameControl.Instance.black;
            foreach (GameObject piece in gameObjects)
                piece.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            getPossible();
        }
        else
        {
            if(GameControl.Instance.selectedOne == this)
            {
                deSelect();
                GameObject[] gameObjects = new GameObject[0];
                if (color == pieceColor.White)
                    gameObjects = GameControl.Instance.white;
                else
                    gameObjects = GameControl.Instance.black;
                foreach (GameObject piece in gameObjects)
                    piece.GetComponent<BoxCollider2D>().enabled = true;
                GameControl.Instance.selectedOne = null;
            }
            else
            {
                GameControl.Instance.moveTo(this, this);
            }
        }
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    private void getPossible()
    {
        possibleTiles = new List<TileScript>();
        if(type == pieceType.pawn)
        {
            if(color == pieceColor.White)
            {
                int possibleRow = currentRow + 1;
                if (possibleRow > 7)
                    return;

                int possibleColumne = currentColumn - 1;
                if (getField(possibleRow, possibleColumne) != null && getField(possibleRow, possibleColumne).piece != null && getField(possibleRow, possibleColumne).piece.color == pieceColor.Black)
                    possibleTiles.Add(getField(possibleRow, possibleColumne));

                possibleColumne = currentColumn + 1;
                if (getField(possibleRow, possibleColumne) != null && getField(possibleRow, possibleColumne).piece != null && getField(possibleRow, possibleColumne).piece.color == pieceColor.Black)
                    possibleTiles.Add(getField(possibleRow, possibleColumne));

                if(getField(possibleRow, currentColumn) != null && getField(possibleRow, currentColumn).piece == null)
                    possibleTiles.Add(getField(possibleRow, currentColumn));
                if (currentRow == 1 && getField(possibleRow, currentColumn) != null && getField(possibleRow, currentColumn).piece == null && getField(possibleRow + 1, currentColumn).piece == null)
                    possibleTiles.Add(getField(possibleRow + 1, currentColumn));

            }
            if (color == pieceColor.Black)
            {
                int possibleRow = currentRow - 1;
                if (possibleRow < 0)
                    return;

                int possibleColumne = currentColumn - 1;
                if (getField(possibleRow, possibleColumne) != null && getField(possibleRow, possibleColumne).piece != null && getField(possibleRow, possibleColumne).piece.color == pieceColor.White)
                    possibleTiles.Add(getField(possibleRow, possibleColumne));
                possibleColumne = currentColumn + 1;
                if (getField(possibleRow, possibleColumne) != null && getField(possibleRow, possibleColumne).piece != null && getField(possibleRow, possibleColumne).piece.color == pieceColor.White)
                    possibleTiles.Add(getField(possibleRow, possibleColumne));

                if (getField(possibleRow, currentColumn) != null && getField(possibleRow, currentColumn).piece == null)
                    possibleTiles.Add(getField(possibleRow, currentColumn));
                if (currentRow == 6 && getField(possibleRow, currentColumn) != null && getField(possibleRow, currentColumn).piece == null && getField(possibleRow - 1, currentColumn).piece == null)
                    possibleTiles.Add(getField(possibleRow - 1, currentColumn));

            }
        }

        if(type == pieceType.king)
        {
            int[] rows = { currentRow + 1, currentRow, currentRow - 1 };
            int[] cols = { currentColumn + 1, currentColumn, currentColumn - 1 };
            foreach(int row in rows)
                foreach(int col in cols)
                    if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                        possibleTiles.Add(getField(row, col));
        }

        if(type == pieceType.queen)
        {
            bool stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn + i) != null)
                    if (getField(currentRow + i, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn + i) != null)
                    if (getField(currentRow - i, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }

            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn - i) != null)
                    if (getField(currentRow + i, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn - i) != null)
                    if (getField(currentRow - i, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }

            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn) != null)
                    if (getField(currentRow + i, currentColumn).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn) != null)
                    if (getField(currentRow - i, currentColumn).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn));
                        stop = true;
                    }
                else
                    stop = true;
            }

            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow, currentColumn + i) != null)
                    if (getField(currentRow, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow, currentColumn - i) != null)
                    if (getField(currentRow, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }
        }

        if(type == pieceType.rook)
        {
            bool stop = false;
            for(int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn) != null)
                    if (getField(currentRow + i, currentColumn).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn) != null)
                    if (getField(currentRow - i, currentColumn).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn));
                        stop = true;
                    }
                else
                    stop = true;
            }

            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow, currentColumn + i) != null)
                    if (getField(currentRow, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow, currentColumn - i) != null)
                    if (getField(currentRow, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }
        }

        if(type == pieceType.bishop)
        {
            bool stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn + i) != null)
                    if (getField(currentRow + i, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn + i) != null)
                    if (getField(currentRow - i, currentColumn + i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn + i));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn + i).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn + i));
                        stop = true;
                    }
                else
                    stop = true;
            }

            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow + i, currentColumn - i) != null)
                    if (getField(currentRow + i, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow + i, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow + i, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow + i, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }
            stop = false;
            for (int i = 1; !stop; i++)
            {
                if (getField(currentRow - i, currentColumn - i) != null)
                    if (getField(currentRow - i, currentColumn - i).piece == null)
                    {
                        possibleTiles.Add(getField(currentRow - i, currentColumn - i));
                    }
                    else
                    {
                        if (getField(currentRow - i, currentColumn - i).piece.color != color)
                            possibleTiles.Add(getField(currentRow - i, currentColumn - i));
                        stop = true;
                    }
                else
                    stop = true;
            }
        }

        if(type == pieceType.knight)
        {
            int row = currentRow + 2;
            int col = currentColumn + 1;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow + 2;
            col = currentColumn - 1;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow - 2;
            col = currentColumn + 1;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow - 2;
            col = currentColumn - 1;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));

            row = currentRow + 1;
            col = currentColumn + 2;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow + 1;
            col = currentColumn - 2;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow - 1;
            col = currentColumn + 2;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
            row = currentRow - 1;
            col = currentColumn - 2;
            if (getField(row, col) != null && (getField(row, col).piece == null || (getField(row, col).piece != null && getField(row, col).piece.color != color)))
                possibleTiles.Add(getField(row, col));
        }

        showPossible();
    }

    private void showPossible()
    {
        foreach (TileScript possibleTile in possibleTiles)
        {
            possibleTile.isPossible(true);
        }
    }

    private TileScript getField(int row, int columne)
    {
        if(columne < 0 || columne > 7)
            return null;
        switch(row)
        {
            case 0:
                return GameControl.Instance.rowA[columne];
            case 1:
                return GameControl.Instance.rowB[columne];
            case 2:
                return GameControl.Instance.rowC[columne];
            case 3:
                return GameControl.Instance.rowD[columne];
            case 4:
                return GameControl.Instance.rowE[columne];
            case 5:
                return GameControl.Instance.rowF[columne];
            case 6:
                return GameControl.Instance.rowG[columne];
            case 7:
                return GameControl.Instance.rowH[columne];
            default:
                return null;
        }
    }

    public void deSelect()
    {
        GetComponent<SpriteRenderer>().material = defaultMaterial;
        foreach (TileScript possibleTile in possibleTiles)
        {
            possibleTile.isPossible(false);
        }
        possibleTiles = null;
    }

    public bool onChallenge()
    {
        this.gameObject.SetActive(false);
        if (type == pieceType.king)
            GameControl.Instance.Win(color == pieceColor.White ? "Schwarz" : "Weiﬂ");
        return true;
    }
}