using System;
using System.Collections.Generic;
// using TweenKey;
// using TweenKey.Interpolation;
using UnityEngine;

namespace Scenes
{
    public class TweenTest : MonoBehaviour
    {
        [SerializeField] private int _testInstances = 100;
        
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private static List<TweenTest> instances = new List<TweenTest>();

        private void Start()
        {
            instances.Add(this);
            if (instances.Count < _testInstances)
            {
                GameObject.Instantiate(this.gameObject, transform.position + 15 * UnityEngine.Random.insideUnitSphere, Quaternion.identity);
            }
            TestTweenKey();
        }

        private void TestTweenKey()
        {
            /*transform.TweenLoopByRotation(Quaternion.AngleAxis(45, transform.up), 4f, Loop.Continue, null, Easing.Linear);

            var sequence = new Sequence<Vector3>(curve, LerpFunctions.Vector3, OffsetFunctions.Vector3, f => transform.localScale * f, 60);
            sequence.Append(sequence.Reverse());
            transform.TweenLoopScaleSequence(sequence);
            
            MoveUp();
            
            void MoveUp()
            {
                transform.TweenLoopMove(transform.position + 15 * Vector3.right, 2.5f);
            }*/
        }

    }
}