using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioManager : MonoBehaviour
{
    public static MainAudioManager instance;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public float bulletVolume;
    public int channels;  //동시에 여러 소리(몬스터 마다의 피격 소리 등)을 내기 위해 channel이용// 오디오 소리 개수
    int channelIndex;
    AudioSource[] sfxPlayer; // 효과음 개수만큼 

    public enum sfx {Shot, Boom}
    public enum bgm { Main }
   
    void Awake()
    {
        instance = this;
        Init();
    }

    private void Init()
    {
        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPLayer");
        sfxObject.transform.parent = transform; //자식 오브젝트 지정
        sfxPlayer = new AudioSource[channels]; // 오디오 소스 배열 초기화

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false; //시작했을 때, 효과 안나게 설정

            sfxPlayer[index].volume = sfxVolume; //효과음 소리 크기 초기화

        }


    }

    public void PLayersfx(sfx sfx) //소리 실행 함수
    {
        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            int loopindex = (index + channelIndex) % sfxPlayer.Length; //채널 개수만큼 순회하도록 채널인덱스 변수 활용 // 넘어가지 않게 하기위해 길이로 나누기

            if (sfxPlayer[loopindex].isPlaying) // 재생 중인 소리가 끝키지 않도록 하기위해
                continue; //반복문 도중 다음 루프로 건너뛰는 키워드



            channelIndex = loopindex;
            sfxPlayer[loopindex].clip = sfxClips[(int)sfx];
            if (sfx == sfx.Shot)
                sfxPlayer[loopindex].volume = bulletVolume;
            else
                sfxPlayer[loopindex].volume = sfxVolume;

            sfxPlayer[loopindex].Play();
            break;
        }


    }
 
}
