using System;
using System.Collections;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Game
{
    public class ProceduralAnimations : UnityBehaviour
    {
        private static ProceduralAnimations Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("ObjectAnimations").AddComponent<ProceduralAnimations>();
                
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        private static ProceduralAnimations instance;
    
        public static void MoveTo(TransformWrapper wrapper, Vector3 from, Vector3 to, float time, bool usingLocal, 
            AnimationCurve curve, Action callback = null)
        {
            wrapper.isUsingLocal = usingLocal;
        
            wrapper.position = from;

            var curveTime = curve.keys[curve.length - 1].time;

            var curveDifference = curveTime / time;
        
            IEnumerator Animation()
            {
                var timer = 0f;
                var distance = to - from;
            
                var oldDistance = Vector3.zero;
                while (timer < time)
                {
                    timer += Time.deltaTime;
                
                    var currentDistance = distance * curve.Evaluate(timer * curveDifference);

                    wrapper.position += currentDistance - oldDistance;

                    oldDistance = currentDistance;
                
                    yield return null;
                }
            
                wrapper.position = to;
            
                callback?.Invoke();
            }

            Instance.StartAnimation(Animation());
        }
    
        public static void MoveTo(TransformWrapper wrapper, Vector3 from, Vector3 to, float time,
            AnimationCurve curve, Action callback = null) => MoveTo(wrapper, from, to, time, false, curve, callback);
    
        public static void MoveTo(TransformWrapper wrapper, Vector3 to, float time,
            AnimationCurve curve, Action callback = null) => MoveTo(wrapper, wrapper.position, to, time, false, curve, callback);
    
        public static void MoveTo(TransformWrapper wrapper, Vector3 to, float time, bool usingLocal,
            AnimationCurve curve, Action callback = null) => MoveTo(wrapper, wrapper.position, to, time, usingLocal, curve, callback);
    
        public static void RotateTo(TransformWrapper wrapper, Vector3 from, Vector3 to, float time, bool usingLocal, 
            AnimationCurve curve, Action callback = null)
        {
            wrapper.isUsingLocal = usingLocal;
        
            wrapper.rotation = from;

            var curveTime = curve.keys[curve.length - 1].time;

            var curveDifference = curveTime / time;
        
            IEnumerator Animation()
            {
                var timer = 0f;
                var distance = to - from;
            
                var oldDistance = Vector3.zero;
                while (timer < time)
                {
                    timer += Time.deltaTime;
                
                    var currentDistance = distance * curve.Evaluate(timer * curveDifference);

                    wrapper.rotation += currentDistance - oldDistance;

                    oldDistance = currentDistance;
                
                    yield return null;
                }
            
                wrapper.position = to;
            
                callback?.Invoke();
            }

            Instance.StartAnimation(Animation());
        }
    
        public static void RotateTo(TransformWrapper wrapper, Vector3 from, Vector3 to, float time,
            AnimationCurve curve, Action callback = null) => RotateTo(wrapper, from, to, time, false, curve, callback);
    
        public static void RotateTo(TransformWrapper wrapper, Vector3 to, float time,
            AnimationCurve curve, Action callback = null) => RotateTo(wrapper, wrapper.position, to, time, false, curve, callback);
    
        public static void RotateTo(TransformWrapper wrapper, Vector3 to, float time, bool usingLocal,
            AnimationCurve curve, Action callback = null) => RotateTo(wrapper, wrapper.position, to, time, usingLocal, curve, callback);
    
        public static void ScaleTo(TransformWrapper wrapper, Vector3 from, Vector3 to, float time, 
            AnimationCurve curve, Action callback = null)
        {
            wrapper.scale = from;

            var curveTime = curve.keys[curve.length - 1].time;

            var curveDifference = curveTime / time;
        
            IEnumerator Animation()
            {
                var timer = 0f;
                var distance = to - from;
            
                var oldDistance = Vector3.zero;
                while (timer < time)
                {
                    timer += Time.deltaTime;
                
                    var currentDistance = distance * curve.Evaluate(timer * curveDifference);

                    wrapper.scale += currentDistance - oldDistance;

                    oldDistance = currentDistance;
                
                    yield return null;
                }
            
                wrapper.position = to;
            
                callback?.Invoke();
            }

            Instance.StartAnimation(Animation());
        }
    
        public static void ScaleTo(TransformWrapper wrapper, Vector3 to, float time,
            AnimationCurve curve, Action callback = null) => ScaleTo(wrapper, wrapper.position, to, time, curve, callback);
    
        public static void ColorTo(ColorWrapper wrapper, Color from, Color to, float time, 
            AnimationCurve curve, Action callback = null)
        {
            wrapper.color = from;

            var curveTime = curve.keys[curve.length - 1].time;

            var curveDifference = curveTime / time;
        
            IEnumerator Animation()
            {
                var timer = 0f;
                var distance = to - from;

                var oldDistance = Color.clear;
            
                while (timer < time)
                {
                    timer += Time.deltaTime;
                
                    var currentDistance = distance * curve.Evaluate(timer * curveDifference);

                    wrapper.color += currentDistance - oldDistance;

                    oldDistance = currentDistance;
                
                    yield return null;
                }
            
                wrapper.color = to;
            
                callback?.Invoke();
            }

            Instance.StartAnimation(Animation());
        }
    
        public static void ColorTo(ColorWrapper wrapper, Color to, float time, 
            AnimationCurve curve, Action callback = null) => ColorTo(wrapper, wrapper.color, to, time, curve, callback);

        public static void Wait(float time, Action callback)
        {
            IEnumerator Animation()
            {
                yield return new WaitForSeconds(time);
                callback?.Invoke();
            }
        
            Instance.StartAnimation(Animation());
        }

        private void StartAnimation(IEnumerator coroutine) => StartCoroutine(coroutine);
    }
}
