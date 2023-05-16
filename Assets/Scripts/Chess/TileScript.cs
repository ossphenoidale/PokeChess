using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour, IPointerDownHandler
{
    public PieceScript piece;

    public Material possibleMaterial;
    private Material defaultMaterial;

    public int row;
    public int col;

    private void Start()
    {
        defaultMaterial = GetComponent<SpriteRenderer>().material;
        char[] position = gameObject.name.ToCharArray();
        switch (position[0])
        {
            case 'a':
                row = 0;
                break;
            case 'b':
                row = 1;
                break;
            case 'c':
                row = 2;
                break;
            case 'd':
                row = 3;
                break;
            case 'e':
                row = 4;
                break;
            case 'f':
                row = 5;
                break;
            case 'g':
                row = 6;
                break;
            case 'h':
                row = 7;
                break;
            default:
                row = -1;
                break;
        }
        col = int.Parse(position[1].ToString()) - 1;
        if(piece != null)
            piece.tile = this;
        AddPhysics2DRaycaster();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameControl.Instance.moveTo(this, piece);
    }

    public void isPossible(bool possibility)
    {
        if(possibility)
        {
            GetComponent<SpriteRenderer>().material = possibleMaterial;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().material = defaultMaterial;
            GetComponent<BoxCollider2D>().enabled = false;
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
}
