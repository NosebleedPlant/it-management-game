using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private GameStatusData _gameData;
    [SerializeField] private TextMeshProUGUI RevenuCounter;
    [SerializeField] private Slider StorageBar;
    [SerializeField] private TextMeshProUGUI StorageETA;
    [SerializeField] private Slider NetworkBar;
    [SerializeField] private TextMeshProUGUI NetworkETA;
    [SerializeField] private TextMeshProUGUI ComplaintCounter;
    [SerializeField] private Slider ComplaintBar;
    [SerializeField] private Image[] SecurityLives;
    [SerializeField] private TextMeshProUGUI SecurityCounter;
    [SerializeField] private GameStatusData _gameData;
    [SerializeField] private Volume PostProcVolume;
    [SerializeField] private Transform MiniGameArea;
    [SerializeField] private Transform[] PopupPrefabs;
    [SerializeField] private Transform AlertPrefab;
    private AudioSource _crowdSfx;
    private Vignette _vignette;
    private Color baseVColor;
    private float baseVIntensity;
    private Color hurtColor = new Color(1f,0.0518868f,0.1453752f);
    private float hurtIntensity = 0.499f;
    //000D26
    
    private void Awake()
    {
        //initalize command
        StartCoroutine(_gameData.UpdateRevenue());
        StartCoroutine(_gameData.NetworkGameData.UpdateNetworkUse());
        StartCoroutine(_gameData.SecurityGameData.UpdateSecurity());
        StartCoroutine(PopUp());
        StartCoroutine(SpawnAlert());
        _crowdSfx = GetComponent<AudioSource>();
    }
    
    private void Start() 
    {
        PostProcVolume.profile.TryGet<Vignette>(out _vignette);
        baseVColor = _vignette.color.value;
        baseVIntensity = _vignette.intensity.value;
    }
    
    private void Update()
    {
        RevenuCounter.text = "[$<color=#FF3369><b>"+_gameData.CurrentRevenue.ToString("D8")+"</b></color>]";
        
        StorageBar.value = _gameData.StorageGameData.CurrentFill;
        StorageETA.text = "[ETA:"+_gameData.StorageGameData.CurrentFill*100+"%]";
        
        NetworkBar.value = _gameData.NetworkGameData.CurrentFill;
        NetworkETA.text = "[ETA:"+_gameData.NetworkGameData.CurrentFill*100+"%]";
        
        ComplaintCounter.text = "[<color=#FF3369><b>"+_gameData.ComplaintGameData.ComplaintCount+"</b></color> current open/"+_gameData.ComplaintGameData.MaxComplaintCount+" max]";
        ComplaintBar.value = _gameData.ComplaintGameData.CurrentFill;

        UpdateLives();
        SecurityCounter.text = "["+_gameData.SecurityGameData.CurrentDamage+"/7]";

        AdjustVColor();

        if(_gameData.ComplaintGameData.ComplaintCount > 2)
        {
            CrowdVolume(_gameData.ComplaintGameData.ComplaintCount);
        }
        else
        {
            CrowdVolume(0);
        }
    }

    private void OnDisable()
    {
        _gameData.ResetData();
        StopAllCoroutines();
    }

    private void UpdateLives()
    {
        for(int i = 0;i<SecurityLives.Length;i++)
        {
            if(i>=_gameData.SecurityGameData.CurrentDamage)
            {
                SecurityLives[i].enabled=false;
            }
            else
            {
                SecurityLives[i].enabled=true;
            }
        }
    }

    float _cumulator;
    int direction = 1;
    float _transitionTime = 0.5f;
    private void AdjustVColor()
    {
        if(_gameData.StorageGameData.MaxReached||_gameData.NetworkGameData.MaxReached||_gameData.SecurityGameData.MaxReached||_gameData.ComplaintGameData.MaxReached)
        {
            _cumulator+=Time.deltaTime*direction;
            float percentage = _cumulator/_transitionTime;
            _vignette.color.value = new Color(
                Mathf.Lerp(baseVColor.r,hurtColor.r,percentage),
                Mathf.Lerp(baseVColor.g,hurtColor.g,percentage),
                Mathf.Lerp(baseVColor.b,hurtColor.b,percentage)
            );
            _vignette.intensity.value = Mathf.Lerp(baseVIntensity,hurtIntensity,percentage);
            if(percentage<=0||percentage>=1){direction*=-1;}
        }
        else
        {
            _vignette.color.value = baseVColor;
            _vignette.intensity.value = baseVIntensity;
            _cumulator=0f;
            direction = 1;
        }
    }

    private IEnumerator PopUp()
    {
        while(enabled)
        {
            if(_gameData.SecurityGameData.MaxReached)
            {
                for(int i=0;i<4;i++)
                {
                    Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-257,257),UnityEngine.Random.Range(-230,230),0.137251f);
                    Transform popup = Instantiate(PopupPrefabs[UnityEngine.Random.Range(0,PopupPrefabs.Length)],MiniGameArea);
                    popup.localPosition = spawnPosition;
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnAlert()
    {
        while(enabled)
        {
            if(_gameData.StorageGameData.MaxReached)
            {
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-257,257),UnityEngine.Random.Range(-190,190),0.137251f);
                for(int i=0;i<4;i++)
                {
                    Transform popup = Instantiate(AlertPrefab,MiniGameArea);
                    popup.localPosition = spawnPosition;
                    spawnPosition = new Vector3(spawnPosition.x+10f,spawnPosition.y+10f,spawnPosition.z);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void CrowdVolume(float value)
    {
        _crowdSfx.volume = Mathf.Lerp(_crowdSfx.volume, (value/10)*0.2f, 0.05f * Time.deltaTime);
    }
}
