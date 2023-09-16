using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioSource[] Bgms;
    public bool isBoss;
    public int bgmIndex;
    public float bgmVolume;

    
    

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public float bulletVolume;
    public int channels;  //���ÿ� ���� �Ҹ�(���� ������ �ǰ� �Ҹ� ��)�� ���� ���� channel�̿�// ����� �Ҹ� ����
    int channelIndex;
    AudioSource[] sfxPlayer; // ȿ���� ������ŭ 

   
    public enum bgm {FIght,Boss}
    public enum sfx { pDead,eDead,Shot,Boom,Item, Lose, Win }


    void LateUpdate()
    {
        if (isBoss)
        {
            if (Bgms[bgmIndex].volume <= 0.1)
            {
                Bgms[bgmIndex].Stop();
                Bgms[bgmIndex].volume = bgmVolume;
                isBoss = false;

                Invoke("PlayBossBgm", 2f);
            }
            Bgms[bgmIndex].volume -= 0.01f;
        }
    }

    void Awake()
    {
        instance = this;
        Init();

    }

    private void Init()
    {
        //ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPLayer");
        sfxObject.transform.parent = transform; //�ڽ� ������Ʈ ����
        sfxPlayer = new AudioSource[channels]; // ����� �ҽ� �迭 �ʱ�ȭ

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false; //�������� ��, ȿ�� �ȳ��� ����

            sfxPlayer[index].volume = sfxVolume; //ȿ���� �Ҹ� ũ�� �ʱ�ȭ
           
        }
        

    }

    public void PLayersfx(sfx sfx) //�Ҹ� ���� �Լ�
    {
        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            int loopindex = (index + channelIndex) % sfxPlayer.Length; //ä�� ������ŭ ��ȸ�ϵ��� ä���ε��� ���� Ȱ�� // �Ѿ�� �ʰ� �ϱ����� ���̷� ������

            if (sfxPlayer[loopindex].isPlaying) // ��� ���� �Ҹ��� ��Ű�� �ʵ��� �ϱ�����
                continue; //�ݺ��� ���� ���� ������ �ǳʶٴ� Ű����



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

    //#BGM
    public void PLayerBgm(bgm bgm)
    {
        if (bgm == bgm.Boss)
        {
            PlayerBoss();
            return;
        }

        for (int index =0; index < Bgms.Length; index++)
        {
            if (Bgms[index].isPlaying)
                Bgms[index].Stop();
        }

        

        Bgms[(int)bgm].Play();

    }

    public void StoperBgm()
    {
        for (int index = 0; index < Bgms.Length; index++)
        {
            if (Bgms[index].isPlaying)
                Bgms[index].Stop();
        }
    }

    public void PlayerBoss()
    {
        for(int index =0; index < Bgms.Length; index++)
        {
            if (Bgms[index].isPlaying)
            {
                index = bgmIndex;
                break;
            }
        }
        isBoss = true;

    }
    public void PlayBossBgm()
    {
        Bgms[(int)bgm.Boss].volume = bgmVolume - 0.1f;
        Bgms[(int)bgm.Boss].Play();
    }
}
