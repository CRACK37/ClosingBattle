using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClosingBattle.Classes;

[RequireComponent(typeof(Camera))]
public class PostProcessEffects : MonoBehaviour
{
    public class PostProcessingEffect
    {
        public Material Mat;
        public bool Active;

        public PostProcessingEffect(Material mat)
        {
            Mat = mat;
        }
    }
    
    private static Dictionary<string, PostProcessingEffect> _effects = [];

    public static void AddPostProcessingEffect(Material effect, string id)
    {
        _effects.Add(id, new PostProcessingEffect(effect));
    }
    public static void AddPostProcessingEffect(Shader effect, string id)
    {
        Material mat = new Material(effect);
        _effects.Add(id, new PostProcessingEffect(mat));
    }

    public static void SetPostProcessingEffect(string id, bool? newState = null, Material newMat = null!)
    {
        if(!_effects.ContainsKey(id)) return;
        var effect = _effects[id];

        if (newState != null)
        {
            effect.Active = newState.Value;
        }
        if (newMat != null)
        {
            effect.Mat = newMat;
        }
    }
    

    private RenderTexture tmp1 = null!;
    private RenderTexture tmp2 = null!;
    public void Start()
    {
        tmp1 = new RenderTexture(Screen.width, Screen.height, 24);
        tmp2 = new RenderTexture(Screen.width, Screen.height, 24);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int w = source.width;
        int h = source.height;
        //Destination can be null
        if (destination != null)
        {
            w = destination.width;
            h = destination.height;
        }

        if (tmp1.width != w || tmp1.height != h)
        {
            Destroy(tmp1);
            tmp1 = new RenderTexture(w, h, 24);
        }
        if (tmp2.width != w || tmp2.height != h)
        {
            Destroy(tmp2);
            tmp2 = new RenderTexture(w, h, 24);
        }

        Graphics.Blit(source, tmp1);
        
        RenderTexture src = tmp1;
        RenderTexture dst = tmp2;
        foreach (var effect in _effects.Values)
        {
            if(!effect.Active) continue;
            Graphics.Blit(src, dst, effect.Mat);
            (src, dst) = (dst, src);
        }
        Graphics.Blit(src, destination);
    }
}