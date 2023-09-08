using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    float res = 0.1f;
    const int DEFAULT_WIDTH = 1920; 
    const int DEFAULT_HEIGHT = 1080;
    int width;
    int height;
    Texture2D screenTexture;
    float[,] fire;
    float[,] matter;
    float[,] smoke;


    const float FIRE_BURN_RATE = 0.008f; //how fast can the fire burn the matter down
    const float MIN_FIRE_TO_SPREAD = 0.4f; //how strong does a fire have to be to spread
    const float FIRE_SPREAD_RATE = 0.0058f; //how fast does the fire spread
    const float SMOKE_DECAY_RATE = 0.0166f;
    const float SMOKE_SPREAD_RATE = 0.00166f;
    void Awake()
    {
        width =  (int) (DEFAULT_WIDTH * res);
        height = (int) (DEFAULT_HEIGHT * res);

        InitializeRandomizedSimulation();
        screenTexture.filterMode = FilterMode.Point;



        UpdateScreenTexture();

    }


    private void InitializeRandomizedSimulation()
    {
        fire = new float[width, height];
        matter = new float[width, height];
        smoke = new float[width, height];
        screenTexture = new Texture2D(width, height);



        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                matter[x,y] = Random.Range(0f, 1f);


            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (matter[x,y] > 0.5f)
                {
                    if (Random.Range(0, 100f) > 111111199.5)
                    {
                        fire[x, y] = 1f;
                    }
                }
            }
        }


        fire[width/2, height/2] = 1f;
        fire[width / 2 +1, height / 2] = 1f;

        UpdateScreenTexture();


    }


    private void SimulationStep()
    {


        DecayFire();
        DecaySmoke();
        BurnMatter();
        SpreadFire();
        SpreadSmoke();
        SmokeNoise();

        CapValues();


        void SmokeNoise()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float randomness = 0.0166f;

                    if (smoke[x,y] > 0.15f)
                    {
                        smoke[x, y] += (Random.Range(-randomness, randomness) + Random.Range(-randomness, randomness) + Random.Range(-randomness, randomness) / 3f);
                    }

                }
            }
        }

        void DecaySmoke()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    smoke[x, y] -= smoke[x, y] * SMOKE_DECAY_RATE;

                }
            }
        }

        void SpreadSmoke()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (smoke[x, y] > 0)
                    {
                        //up
                        if (y + 1 < height)
                        {
                            if (matter[x, y + 1] < 0.5f && fire[x, y + 1] < 0.5f)
                            {
                                smoke[x, y + 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }


                        }

                        //up right
                        if (y + 1 < height && x + 1 < width)
                        {
                            if (matter[x + 1, y + 1] < 0.5f && fire[x + 1, y + 1] < 0.5f)
                            {
                                smoke[x + 1, y + 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //right
                        if (x + 1 < width)
                        {
                            if (matter[x + 1, y] < 0.5f && fire[x + 1, y] < 0.5f)
                            {
                                smoke[x + 1, y] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //rigt down
                        if (y - 1 >= 0 && x + 1 < width)
                        {

                            if (matter[x + 1, y - 1] < 0.5f && fire[x + 1, y - 1] < 0.5f)
                            {
                                smoke[x + 1, y - 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //down
                        if (y - 1 >= 0)
                        {
                            if (matter[x, y - 1] < 0.5f && fire[x, y - 1] < 0.5f)
                            {
                                smoke[x, y - 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //down left
                        if (y - 1 >= 0 && x - 1 >= 0)
                        {
                            if (matter[x - 1, y - 1] < 0.5f && fire[x - 1, y - 1] < 0.5f)
                            {
                                smoke[x - 1, y - 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //left
                        if (x - 1 >= 0)
                        {
                            if (matter[x - 1, y] < 0.5f && fire[x - 1, y] < 0.5f)
                            {
                                fire[x - 1, y] += (smoke[x, y] * SMOKE_SPREAD_RATE);
                            }

                        }
                        //left up
                        if (y + 1 < height && x - 1 >= 0)
                        {
                            if (matter[x - 1, y + 1] < 0.5f && fire[x - 1, y + 1] < 0.5f)
                            {
                                smoke[x - 1, y + 1] += (smoke[x, y] * SMOKE_SPREAD_RATE);

                            }
                        }
                    }


                }
            }
        }





            void SpreadFire()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (fire[x, y] >= MIN_FIRE_TO_SPREAD)
                    {
                        //up
                        if (y + 1 < height)
                        {

                            fire[x, y + 1] += (fire[x, y] * FIRE_SPREAD_RATE);
                            

                        }

                        //up right
                        if (y + 1 < height && x + 1 < width)
                        {

                            fire[x+1, y + 1] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //right
                        if (x + 1 < width)
                        {

                            fire[x+1, y] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //rigt down
                        if (y - 1 >= 0  && x + 1 < width)
                        {

                            fire[x + 1, y - 1] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //down
                        if (y - 1 >= 0)
                        {

                            fire[x, y - 1] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //down left
                        if (y - 1 >= 0 && x-1 >= 0)
                        {

                            fire[x -1, y - 1] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //left
                        if (x - 1 >= 0)
                        {

                            fire[x -1, y] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                        //left up
                        if (y + 1 < height && x - 1 >= 0)
                        {

                            fire[x - 1, y + 1] += (fire[x, y] * FIRE_SPREAD_RATE);


                        }
                    }

     
                }
            }
        }


        void CapValues()
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    fire[x, y] = Mathf.Clamp(fire[x, y], 0, 1);
                    matter[x, y] = Mathf.Clamp(matter[x, y], 0, 1);
                    smoke[x, y] = Mathf.Clamp(smoke[x, y], 0, 1);
                }
            }

        }


        void DecayFire()
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (matter[x,y] < 0.1f)
                    {
                        smoke[x, y] += fire[x, y] * 0.5f;
                        fire[x,y] = fire[x,y] * 0.5f;
                    }
                    if (matter[x,y] <=0)
                    {
                        smoke[x, y] += fire[x, y];
                        fire[x, y] = 0;
                    }
                }
            }

        }


        void BurnMatter()
        {

            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {

                        matter[x, y] -= fire[x, y] * FIRE_BURN_RATE;

                    }
                }

            }
        }

    }

    void ProgressStep()
    {
        SimulationStep();
        UpdateScreenTexture();
    }

    private void UpdateScreenTexture()
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Color32 pixelColor = new Color32((byte)(fire[x, y] * 255), (byte)(matter[x, y] * 0.5 * 255), 0, 255);
                //add smoke:
                byte smokeLevel = (byte) (smoke[x, y] * 200);
                pixelColor.r += smokeLevel;
                pixelColor.g += smokeLevel;
                pixelColor.b += smokeLevel;

                pixelColor.r =  (byte)Mathf.Clamp(pixelColor.r, 0, 255);
                pixelColor.g = (byte)Mathf.Clamp(pixelColor.g, 0, 255);
                pixelColor.b = (byte)Mathf.Clamp(pixelColor.b, 0,255);

                screenTexture.SetPixel(x, y,pixelColor);
            }
        }

        screenTexture.Apply();

        gameObject.GetComponent<Renderer>().material.SetTexture("_screenTex", screenTexture); //check if can be removed from here TODO



    }



    float targetFps = 200;
    float timePassed = 0;
    void Update()
    {
        
        if (Input.GetKeyDown("space"))
        {
            ProgressStep();
        }
        timePassed += Time.deltaTime;
        if (timePassed > (1f/targetFps))
        {
            timePassed -= (1f / targetFps);
            if (Input.GetKey("a"))
            {
                ProgressStep();
            }
        }

    }
}
