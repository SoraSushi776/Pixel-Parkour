using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheatCodeDetect : MonoBehaviour
{
    [Serializable]
    class CheatCodeSet
    {
        public KeyCode[] needPress;
        public UnityEvent action;
    }

    [SerializeField] private CheatCodeSet[] cheatCodeSets;
    [SerializeField] private float keyPressInterval = 0.5f;

    private class CheatCodeState
    {
        public int currentIndex = 0;
        public float lastKeyPressTime = 0f;
        public bool isActive = false;
    }

    private Dictionary<CheatCodeSet, CheatCodeState> cheatStates = new Dictionary<CheatCodeSet, CheatCodeState>();

    void Start()
    {
        foreach (var cheatCodeSet in cheatCodeSets)
        {
            cheatStates[cheatCodeSet] = new CheatCodeState();
        }
    }

    void Update()
    {
        foreach (var cheatCodeSet in cheatCodeSets)
        {
            var state = cheatStates[cheatCodeSet];
            KeyCode expectedKey = cheatCodeSet.needPress[state.currentIndex];

            if (state.isActive && Time.time - state.lastKeyPressTime > keyPressInterval)
            {
                ResetState(state);
                continue;
            }

            if (Input.anyKeyDown)
            {
                bool pressedExpectedKey = Input.GetKeyDown(expectedKey);
                bool pressedWrongKey = false;

                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key) && key != expectedKey)
                    {
                        pressedWrongKey = true;
                        break;
                    }
                }

                if (!pressedExpectedKey && pressedWrongKey && state.isActive)
                {
                    ResetState(state);
                    continue;
                }

                if (pressedExpectedKey)
                {
                    state.currentIndex++;
                    state.lastKeyPressTime = Time.time;
                    state.isActive = true;

                    if (state.currentIndex == cheatCodeSet.needPress.Length)
                    {
                        Debug.Log($"作弊码输入成功: {string.Join(", ", cheatCodeSet.needPress)}");
                        cheatCodeSet.action.Invoke();
                        ResetState(state);
                    }
                }
            }
        }
    }

    private void ResetState(CheatCodeState state)
    {
        state.currentIndex = 0;
        state.isActive = false;
        state.lastKeyPressTime = 0f;
    }
}