﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * 플레이 씬 UI 관련 스크립트
 */

public class SimUIManager : MonoBehaviour
{
    #region 변수
    public GameObject FinishPanel;
    public GameObject GameOverPanel;
    public GameObject BgmOption;
    public GameObject EffectOption;

    public int life;
    public GameObject[] LifeObject;

    public double _time = 0;
    public Text _timerText;
    public bool isPaused;
    public bool bgmOn = false;
    public bool effectOn = false;

    public Text levelText;
    public int firstText = 0;
    public int secondText = 0;

    public Text Result_Clear_Time;
    public Text Result_Clear_Correct;
    public Text Result_Clear_Uncorrect;

    public Text Result_Fail_Time;
    public Text Result_Fail_Correct;
    public Text Result_Fail_Uncorrect;

    public int Fail_Count = 0;
    public int Clear_Count = 0;
    public int starCount = 0;

    public GameObject[] Clear_Star;
    #endregion

    #region Singleton
    private static SimUIManager ui;
    public static SimUIManager UI
    {
        get { return ui; }
    }

    private void Awake()
    {
        ui = GetComponent<SimUIManager>();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
        LevelText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused) //플레이 화면
        {
            //시간 표시
            _time += Time.deltaTime;
            int minute = (int)_time / 60;
            int second = (int)_time - (minute * 60);
            int second1 = second / 10;
            int second2 = second % 10;

            if (minute / 10 != 0)
            {
                _timerText.text = (minute.ToString() + ":" + second1.ToString() + second2.ToString());
            }
            else
            {
                _timerText.text = ("0" + minute.ToString() + ":" + second1.ToString() + second2.ToString());
            }
        }
    }

    public void Option() //옵션 버튼 클릭 시
    {
        isPaused = true;
        Time.timeScale = 0f;

        //bgm/effect 소리 끔
        if (BgmOption.activeSelf == true)
        {
            bgmOn = true;
            SimSoundManager.Sound.BGMAudioSource.volume = 0f;
        }

        if (EffectOption.activeSelf == true)
        {
            effectOn = true;
            SimSoundManager.Sound.MoveAudioSource.volume = 0f;
            SimSoundManager.Sound.EffectAudioSource.volume = 0f;
        }
    }

    public void BGMOn() //배경음 ON
    {
        bgmOn = true;
    }

    public void BGMOff() //배경음 OFF
    {
        bgmOn = false;
        SimSoundManager.Sound.BGMAudioSource.volume = 0f;
    }

    public void EFOn() //효과음 ON
    {
        effectOn = true;
    }

    public void EFOff() //효과음 OFF
    {
        effectOn = false;
        SimSoundManager.Sound.MoveAudioSource.volume = 0f;
        SimSoundManager.Sound.EffectAudioSource.volume = 0f;
    }

    public void Continue() //계속하기
    {
        //소리 켜져있으면 게임 플레이 볼륨 다시 원래대로
        if (bgmOn == true)
            SimSoundManager.Sound.BGMAudioSource.volume = 0.8f;
        if (effectOn == true)
        {
            SimSoundManager.Sound.MoveAudioSource.volume = 1f;
            SimSoundManager.Sound.EffectAudioSource.volume = 1f;
        }

        isPaused = false;
        Time.timeScale = 1f;
    }

    public void ReGame() //다시시작
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit() //나가기
    {
        SceneManager.LoadScene("LevelSelect");
        Time.timeScale = 1f;
    }

    public void GameClear() //게임 클리어 시 결과 화면
    {
        Time.timeScale = 0f;

        int hour1 = (int)_time / 36000;
        int hour2 = (int)_time / 3600 - hour1 * 10;

        //시간, 맞춘개수, 틀린개수
        Result_Clear_Time.text = hour1 + "" + hour2 + ":" + _timerText.text;
        Result_Clear_Correct.text = Clear_Count + "개";
        Result_Clear_Uncorrect.text = Fail_Count + "개";
        
        //별 개수
        switch (Fail_Count)
        {
            case 0:
                Clear_Star[0].SetActive(true);
                Clear_Star[1].SetActive(true);
                Clear_Star[2].SetActive(true);
                starCount = 3;
                break;
            case 1:
                Clear_Star[0].SetActive(true);
                Clear_Star[1].SetActive(true);
                Clear_Star[2].SetActive(false);
                starCount = 2;
                break;
            case 2:
                Clear_Star[0].SetActive(true);
                Clear_Star[1].SetActive(false);
                Clear_Star[2].SetActive(false);
                starCount = 1;
                break;
            default:
                Clear_Star[0].SetActive(false);
                Clear_Star[1].SetActive(false);
                Clear_Star[2].SetActive(false);
                starCount = 0;
                break;
        }
        FinishPanel.SetActive(true);

        //데이터
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].PlayTime = Result_Clear_Time.text;
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].CorrectCnt = Clear_Count + "";
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].UncorrectCnt = Fail_Count + "";
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].StarCnt = starCount + "";

        try
        {
            Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level + 1].Lock = "false";
        }
        catch(Exception e)
        {
            
        }
    }

    public void GameOver() //게임오버 시 결과화면, 데이터
    {
        Time.timeScale = 0f;
        int hour1 = (int)_time / 36000;
        int hour2 = (int)_time / 3600 - hour1 * 10;

        Result_Fail_Time.text = hour1 + "" + hour2 + ":" + _timerText.text;
        Result_Fail_Correct.text = Clear_Count + "개";
        Result_Fail_Uncorrect.text = Fail_Count + "개";

        GameOverPanel.SetActive(true);

        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].PlayTime = Result_Clear_Time.text;
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].CorrectCnt = Clear_Count + "";
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].UncorrectCnt = Fail_Count + "";
        Mmr_Sim_Data.mmr_Sim[SimLevelManager.Selected_Level].StarCnt = "0";
    }

    public void NextButton() //다음 스테이지
    {
        if (SimLevelManager.Level.lightLevel != 8) //4,6이면
        {
            SimLevelManager.Level.lightLevel += 2;
            SceneManager.LoadScene("SampleScene");
        }
        else //8이면
        {
            SimLevelManager.Level.lightLevel = 4; //4로 초기화
            if (SimLevelManager.Level.timeLevel != 1) //시간이 3,2 이면
            {
                SimLevelManager.Level.timeLevel--;
                SceneManager.LoadScene("SampleScene");
            }
            else //1이면
            {
                if (SimLevelManager.Level.levelselect != 9)
                {
                    SimLevelManager.Level.levelselect += 2;
                    SimLevelManager.Level.timeLevel = 3; // 3으로 초기화
                    SceneManager.LoadScene("SampleScene");
                }
                else
                {
                    SceneManager.LoadScene("LevelSelect");
                    Time.timeScale = 1f;
                }
            }
        }
        SimLevelManager.Selected_Level++;
    }

    public void LevelText() //게임 화면 level 표시
    {
        switch (SimLevelManager.Level.levelselect)
        {
            case 4:
                firstText = 1;
                break;
            case 6:
                firstText = 2;
                break;
            case 9:
                firstText = 3;
                break;
        }

        switch (SimLevelManager.Level.timeLevel)
        {
            case 3:
                switch (SimLevelManager.Level.lightLevel)
                {
                    case 4:
                        secondText = 1;
                        break;
                    case 6:
                        secondText = 2;
                        break;
                    case 8:
                        secondText = 3;
                        break;
                }
                break;
            case 2:
                switch (SimLevelManager.Level.lightLevel)
                {
                    case 4:
                        secondText = 4;
                        break;
                    case 6:
                        secondText = 5;
                        break;
                    case 8:
                        secondText = 6;
                        break;
                }
                break;
            case 1:
                switch (SimLevelManager.Level.lightLevel)
                {
                    case 4:
                        secondText = 7;
                        break;
                    case 6:
                        secondText = 8;
                        break;
                    case 8:
                        secondText = 9;
                        break;
                }
                break;
        }
        levelText.text = (firstText) + "-" + (secondText) + " 단계";
    }
}