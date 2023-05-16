using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance;

    public TileScript[] rowA, rowB, rowC, rowD, rowE, rowF, rowG, rowH;

    public bool whiteTurn;

    public PieceScript selectedOne;

    public GameObject[] white;
    public GameObject[] black;

    public GameObject winCanvas;
    public TextMeshProUGUI winText;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void moveTo(TileScript endPosition, PieceScript toBeat)
    {
        changeTurn();
        if (toBeat != null)
            if (!toBeat.onChallenge())
                return;
        selectedOne.currentRow = endPosition.row;
        selectedOne.currentColumn = endPosition.col;
        var end = endPosition.gameObject.transform.position;
        end.z = -1;
        selectedOne.gameObject.transform.position = end;
        selectedOne.deSelect();
        endPosition.piece = selectedOne;
        selectedOne.tile.piece = null;
        selectedOne.tile = endPosition;
        selectedOne = null;
    }

    public void moveTo(PieceScript endPosition, PieceScript toBeat)
    {
        changeTurn();
        if (toBeat != null)
            if (!toBeat.onChallenge())
                return;
        selectedOne.currentRow = endPosition.currentRow;
        selectedOne.currentColumn = endPosition.currentColumn;
        var end = endPosition.gameObject.transform.position;
        end.z = -1;
        selectedOne.gameObject.transform.position = end;
        selectedOne.deSelect();
        TileScript t = endPosition.tile;
        endPosition.tile.piece = selectedOne;
        selectedOne.tile.piece = null;
        selectedOne.tile = t;
        selectedOne = null;
    }

    public void changeTurn()
    {
        whiteTurn = !whiteTurn;
        foreach(GameObject piece in white)
            piece.GetComponent<BoxCollider2D>().enabled = whiteTurn;
        foreach (GameObject piece in black)
            piece.GetComponent<BoxCollider2D>().enabled = !whiteTurn;
    }

    public void Win(string color)
    {
        winCanvas.SetActive(true);
        winText.text = color + " hat gewonnen";
    }

    public void rematch()
    {
        SceneManager.LoadScene(1);
    }

    public void returnMenu()
    {
        SceneManager.LoadScene(0);
    }
}
