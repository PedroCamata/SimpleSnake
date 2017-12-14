using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

    public GameObject after;
    public int previousDirection = 0;
	public int direction = 0;

    public int blockPOS_X;
    public int blockPOS_Y;

    private RectTransform snakeRect;

    void Start()
    {
        snakeRect = GetComponent<RectTransform>();
    }

    public void walk(int d)
    {
        // Direction registry
        previousDirection = direction;
        direction = d;

        // Action of move
        move();

        // Pass direction to the after
        if (after != null)
        {
            after.GetComponent<Snake>().walk(previousDirection);
        }

    }

    private void move()
    {
        if (direction != -1 && previousDirection != -1)
        {
            Vector3 move = Vector3.up * Constant.MOVEMENT;
            switch (direction)
            {
                case 0:
                    move = Vector3.up * Constant.MOVEMENT;
                    blockPOS_Y++;
                    break;
                case 1:
                    move = Vector3.right * Constant.MOVEMENT;
                    blockPOS_X++;
                    break;
                case 2:
                    move = Vector3.down * Constant.MOVEMENT;
                    blockPOS_Y--;
                    break;
                case 3:
                    move = Vector3.left * Constant.MOVEMENT;
                    blockPOS_X--;
                    break;

            }
            snakeRect.localPosition += move;
        }
    }


}
