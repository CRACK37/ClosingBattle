using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace ClosingBattle.Classes;

[ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
public class SlashEffectManager : MonoSingleton<SlashEffectManager>
{
    private static readonly int TimeStart = Shader.PropertyToID("_TimeStart");
    private static readonly int SlashSin = Shader.PropertyToID("_SlashSin");
    private static readonly int SlashCos = Shader.PropertyToID("_SlashCos");
    private static readonly int SlashX = Shader.PropertyToID("_SlashX");
    private static readonly int SlashSide = Shader.PropertyToID("_SlashSide");
    public Material EffectMaterial = null!;

    public void Start()
    {
        EffectMaterial = Addressables.LoadAssetAsync<Material>("SlashMaterial").WaitForCompletion();
        PostProcessEffects.AddPostProcessingEffect(EffectMaterial, "SlashEffect");
    }

    bool _debugState = false;
    public void Update()
    {
        #if DEBUG
        if (Input.GetKeyDown(KeyCode.V))
        {
            _debugState = !_debugState;
            PostProcessEffects.SetPostProcessingEffect("SlashEffect", newState:_debugState);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            int ang = Random.Range(-35, 35);
            int pos = Random.Range(35, 75);
            int side = Random.Range(-2, 2);
            
            Slash(ang, pos, side > 0, -1.0f);
        }
        if (Input.GetKeyDown(KeyCode.C)) // Slash with lifetime
        {
            int ang = Random.Range(-35, 35);
            int pos = Random.Range(35, 75);
            int side = Random.Range(-2, 2);
            
            Slash(ang, pos, side > 0, 1.5f);
        }
        #endif
    }

    /// <summary>
    /// Spawns a slash effect on screen.
    /// WARNING: Do not call if a slash is already being displayed with a lifetime
    /// </summary>
    /// <param name="angle">Value 0-360 what angle should the slash be placed at</param>
    /// <param name="position">Value 0-100 where to place the slash on the screen</param>
    /// <param name="rightSide">False to offset left side, true to offset right side</param>
    /// <param name="lifetime">Set to a number that is less than 0 to disable</param>
    public void Slash(int angle, int position, bool rightSide, float lifetime = 1.5f)
    {
        EffectMaterial.SetFloat(TimeStart, Time.timeSinceLevelLoad);

        int ang = angle;
        int pos = position;
        int side = rightSide ? 1 : -1;
            
        EffectMaterial.SetFloat(SlashX, pos/100.0f);
        EffectMaterial.SetFloat(SlashSide, side);
            
        EffectMaterial.SetFloat(SlashSin, Mathf.Sin(ang * Mathf.Deg2Rad));
        EffectMaterial.SetFloat(SlashCos, Mathf.Cos(ang * Mathf.Deg2Rad));

        if (lifetime < 0)
        {
            PostProcessEffects.SetPostProcessingEffect("SlashEffect", newState:true);
            return;
        }
        
        StartCoroutine(_slashTimer(lifetime));
    }
    private IEnumerator _slashTimer(float lifetime)
    {
        PostProcessEffects.SetPostProcessingEffect("SlashEffect", newState:true);
        yield return new WaitForSeconds(lifetime);
        PostProcessEffects.SetPostProcessingEffect("SlashEffect", newState:false);
    }
}