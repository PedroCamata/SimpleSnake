using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // Time and Speed
    double timePace = 0;
    double speed = Constant.START_SPEED;

    // Score
    int score = 0;
    public Text score_text;
    int best = 0;
    public Text best_text;

    // Snake GameObject
    public GameObject snakeHead;
    public GameObject snakeBody;
    public int snakeLength = 3;

    // Constrols
    public GameObject[] controls;
    int direction;
    bool pause = false;
    public Text text_pause;
    public Image image_pause;

    // Fruity
    public GameObject fruit;
    public int fruit_blockX = 7;
    public int fruit_blockY = 22;

    // Lose popup
    public GameObject lose_popup;

    // Sounds
    public AudioSource sound_eat;
    public AudioSource sound_lose;

    // Tutorial
    bool tutorial;
    public GameObject tutorial_vertical;
    public GameObject tutorial_horizontal;

    // Use this for initialization
    void Start () {
        // Start new fruit position
        fruit.GetComponent<RectTransform>().anchoredPosition = fruitNewPosition();

        best = PlayerPrefs.GetInt("best");
        best_text.text = "Best: " + best;

        tutorial = false;
        if(best <= 5)
        {
            // Activate Tutorial
            tutorial = true;
        }
    }

    // Map of Body
    bool[,] blocksUsed = new bool[Constant.BLOCKS_X, Constant.BLOCKS_Y];

    // Update is called once per frame
    void Update () {
        pace();
	}

    void pace()
    {
        if (!pause)
        {
            timePace += Time.deltaTime;

            if (timePace >= 1 / speed)
            {
                timePace = 0;
                cycle();
            }
        }
    }

    public void btnPause(bool p)
    {
        pause = !pause;
        if(pause)
        {
            text_pause.color = Color.white;
            image_pause.color = Color.red;
        } else
        {
            text_pause.color = Color.black;
            image_pause.color = Color.white;
        }
    }

    public void btnRestart()
    {
        SceneManager.LoadScene("game", LoadSceneMode.Single);
    }

    void cycle()
    {
        // Fill the blocksXused and blocksYused
        mapOfBody();

        // if isn't going to crash, move
        snakeHeadWalk();

        // Change controls
        activeControls(direction);

        // Fruit control
        fruity();
    }

    public void btnDirection(int d)
    {
        // 0 Up
        // 1 Right
        // 2 Down
        // 3 Left

        // if game isn't paused
        if (!pause)
        {
            // Debug.Log("btn: " + d);
            direction = d;

            timePace = 0;
            cycle();
        }
    }

    public void snakeHeadWalk()
    {
        // Check if snake is going to crash
        if (isGoingToCrash(direction))
        {
            // Lose
            Debug.Log("Lose");
            pause = true;
            tutorialClose();
            lose_popup.SetActive(true);
            sound_lose.Play();

        } else
        {
            snakeHead.GetComponent<Snake>().walk(direction);
        }
    }

    public void activeControls(int d)
    {
        // if walking vertical, only horizontal btns work
        if (d % 2 == 0)
        {
            // Active Horizontal btns
            controls[1].transform.SetSiblingIndex(0);

            if (tutorial)
            {
                tutorial_horizontal.SetActive(false);
                tutorial_vertical.SetActive(true);
            }
        }
        else
        {
            // Active Horizontal btns
            controls[0].transform.SetSiblingIndex(0);

            if (tutorial)
            {
                tutorial_horizontal.SetActive(true);
                tutorial_vertical.SetActive(false);
            }
        }
    }

    

    // Tells if snake is going to crash
    public bool isGoingToCrash(int d)
    {

        int crashBlockX = snakeHead.GetComponent<Snake>().blockPOS_X;
        int crashBlockY = snakeHead.GetComponent<Snake>().blockPOS_Y;

        // Check if snake is going to crash the borders
        switch(d)
        {
            case 0:
                if(snakeHead.GetComponent<Snake>().blockPOS_Y >= Constant.BLOCKS_Y -1)
                {
                    return true;
                }
                crashBlockY++;
                break;
            case 1:
                if (snakeHead.GetComponent<Snake>().blockPOS_X >= Constant.BLOCKS_X - 1)
                {
                    return true;
                }
                crashBlockX++;
                break;
            case 2:
                if (snakeHead.GetComponent<Snake>().blockPOS_Y <= 0)
                {
                    return true;
                }
                crashBlockY--;
                break;
            case 3:
                if (snakeHead.GetComponent<Snake>().blockPOS_X <= 0)
                {
                    return true;
                }
                crashBlockX--;
                break;
        }

        // Check if snake is going to crash herself
        if(blocksUsed[crashBlockX, crashBlockY])
        {
            return true;
        }
        
        return false;

    }

    

    public void fruity()
    {
        if (snakeHead.GetComponent<Snake>().blockPOS_X == fruit_blockX
         && snakeHead.GetComponent<Snake>().blockPOS_Y == fruit_blockY)
        {
            Debug.Log("Comeu!");
            sound_eat.Play();

            // Score
            score++;
            score_text.text = "Score: " + score;

            if(score > 5)
            {
                tutorialClose();
            }

            if(score > best)
            {
                best = score;
                best_text.text = "Best: " + best;
                PlayerPrefs.SetInt("best", score);
            }


            // Speed
            if (speed < Constant.MAX_SPEED) {
                speed += Constant.SPEED_INCREASE;
            }


            // Grow Snake
            GameObject body = snakeHead.GetComponent<Snake>().after;
            while (body.GetComponent<Snake>().after != null)
            {
                body = body.GetComponent<Snake>().after;
            }

            GameObject newBody = Instantiate(snakeBody, new Vector3(0,0,0), Quaternion.identity);

            newBody.transform.SetParent(snakeHead.transform.parent);

            newBody.GetComponent<RectTransform>().localScale = body.GetComponent<RectTransform>().localScale;
            newBody.GetComponent<RectTransform>().anchoredPosition = body.GetComponent<RectTransform>().anchoredPosition;
            newBody.transform.localPosition = new Vector3(newBody.transform.localPosition.x, newBody.transform.localPosition.y, 0);

            body.GetComponent<Snake>().after = newBody;

            newBody.GetComponent<Snake>().blockPOS_X = body.GetComponent<Snake>().blockPOS_X;
            newBody.GetComponent<Snake>().blockPOS_Y = body.GetComponent<Snake>().blockPOS_Y;

            // Eat Fruit
            fruit.GetComponent<RectTransform>().anchoredPosition = fruitNewPosition();
        }
    }

    private void tutorialClose()
    {
        tutorial = false;
        tutorial_horizontal.SetActive(false);
        tutorial_vertical.SetActive(false);
    }

    // Map of body
    public void mapOfBody()
    {
        
        // Pre start the array
        for (int i = 0; i < blocksUsed.GetLength(0); i++)
        {
            for (int j = 0; j < blocksUsed.GetLength(1); j++)
            {
                blocksUsed[i, j] = false;
            }
        }

        // if snake body in position turn it true
        // Exception the last body part
        GameObject body = snakeHead;
        while (body.GetComponent<Snake>().after)
        {
            blocksUsed[body.GetComponent<Snake>().blockPOS_X, body.GetComponent<Snake>().blockPOS_Y] = true;

            body = body.GetComponent<Snake>().after;
        }
    }

    // After Fruit be ate, shows it in another position
    public Vector3 fruitNewPosition()
    {

        if (snakeLength >= Constant.MAX_SNAKE_LENGTH)
        {
            // Win
        }

        

        // Appear Fruit Position
        int fruitX = Mathf.RoundToInt(UnityEngine.Random.value * 100) % Constant.BLOCKS_X;
        int fruitY = Mathf.RoundToInt(UnityEngine.Random.value * 100) % Constant.BLOCKS_Y;

        int count = 0;
        while (blocksUsed[fruitX, fruitY])
        {
            if (count % 2 == 0)
            {
                fruitX = (fruitX + 1) % Constant.BLOCKS_X;
            }
            else
            {
                fruitY = (fruitY + 1) % Constant.BLOCKS_Y;
            }
            count++;
            Debug.Log(fruitX + " - " + fruitY);
        }
        

        fruit_blockX = fruitX;
        fruit_blockY = fruitY;

        return BlockPositionToWorldPosition(fruitX, fruitY);
    }

    public Vector3 BlockPositionToWorldPosition(int x, int y)
    {
        x %= Constant.BLOCKS_X;
        y %= Constant.BLOCKS_Y;

        float posX = Constant.LIMIT_LEFT + (x * Constant.MOVEMENT);
        float posY = Constant.LIMIT_BOTTOM - Constant.MOVEMENT + (y * Constant.MOVEMENT);

        return new Vector3(posX, posY, 0);
    }
}
