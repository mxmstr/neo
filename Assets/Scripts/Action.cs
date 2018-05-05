﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;
using UnityEditor;

public class Action : MonoBehaviour
{

    [System.Serializable]
    public class ActionData
    {

        public string name;
        public string animation;
        public bool rotation;
        public bool movement;
        public bool blendlegs;
        public Vector3 velocity;
        public float speed;
        public float damage;

    }

    [System.Serializable]
    public class ActionTable
    {
        public ActionData[] actions;
    }

    ActionData action;

    private Character m_Character;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private AnimatorOverrideController m_AnimatorOverride;
    private ActionTable table;
    private AnimationClip clip;
    private string filename = "Actions.json";


    void Start ()
    {

        m_Character = GetComponent<Character>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        m_AnimatorOverride = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_AnimatorOverride;

    }


    public bool IsReset()
    {

        return action.name == "Default";

    } 


    private void LoadActionData(string actionName)
    {
        
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<ActionTable>(File.ReadAllText(filePath));
        
        foreach (ActionData data in table.actions)
        {
            if (data.name == actionName)
            {
                action = data;
                return;
            }
        }
        
    }


    public void StartAction(string actionName)
    {

        LoadActionData(actionName);
        
        clip = Resources.Load("Animations/" + action.animation, typeof(AnimationClip)) as AnimationClip;
        m_AnimatorOverride["neo_reference_skeleton|Action_Default"] = clip;

        m_Animator.Play("Action", 1, 0);
        m_Animator.Play("Action", 2, 0);
        
        m_Animator.SetFloat("ActionSpeed", action.speed);
        

    }


    public void ResetAction()
    {

        StartAction("Default");

    }


    public ActionData GetAction()
    {
        return action;
    }


    void Update ()
    {   

        if (action.name != "Default")
        {
            if (action.blendlegs && m_Rigidbody.velocity.magnitude > 0.1f)
            {
                m_Animator.SetLayerWeight(1, 0.0f);
                m_Animator.SetLayerWeight(2, 1.0f);
            }
            else
            {
                m_Animator.SetLayerWeight(1, 1.0f);
                m_Animator.SetLayerWeight(2, 0.0f);
            }
        }
        else
        {
            m_Animator.SetLayerWeight(1, 0.0f);
            m_Animator.SetLayerWeight(2, 0.0f);
        }
        
    }


}
