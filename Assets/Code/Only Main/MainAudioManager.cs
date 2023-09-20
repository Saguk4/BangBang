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
    public int channels;  //���ÿ� ���� �Ҹ�(���� ������ �ǰ� �Ҹ� ��)�� ���� ���� channel�̿�// ����� �Ҹ� ����
    int channelIndex;
    AudioSource[] sfxPlayer; // ȿ���� ������ŭ 

    public enum sfx {Shot, Boom}
    public enum bgm { Main }
   
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
 
}
