﻿using MelonLoader;
using Harmony;
using Assets.Scripts.Unity.UI_New.InGame.Races;
using Assets.Scripts.Simulation.Towers.Weapons;
using Assets.Scripts.Simulation;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.Main;
using Assets.Scripts.Simulation.Bloons;
using Assets.Scripts.Models.Towers;

using Assets.Scripts.Unity;



using Assets.Scripts.Simulation.Towers;

using Assets.Scripts.Utils;

using Il2CppSystem.Collections;
using Assets.Scripts.Unity.UI_New.Popups;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Models;
using TMPro;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using System;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using System.Collections.Generic;
using System.Linq;
using Assets.Main.Scenes;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Simulation.Towers.Projectiles;
using UnityEngine;

namespace dps_display
{
    public class Main : MelonMod
    {

        static GameObject upgradeTreeButton;
        static TowerToSimulation lastSelected;
        static long lastDamage = 0;
        static float timer = 0;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            Console.WriteLine("dps_display loaded");
        }


        static void Write(string t)
        {
            if (upgradeTreeButton == null)
            {
                upgradeTreeButton = GameObject.Find("UpgradeTreeButton");
            }
            upgradeTreeButton.GetComponentInChildren<TextMeshProUGUI>().text = t;
        }




        public override void OnUpdate()
        {
            base.OnUpdate();

            bool inAGame = InGame.instance != null && InGame.instance.bridge != null;
            if (inAGame)
            {

                if (InGame.instance.inputManager.SelectedTower != null)
                {
                    lastSelected = InGame.instance.inputManager.SelectedTower;
                    if (timer == 0)
                    {
                        timer = 0.001f;
                        lastDamage = lastSelected.damageDealt;
                    }
                    timer += UnityEngine.Time.deltaTime;
                    if (timer > 1)
                    {
                        timer = 0;
                        Write("" + (lastSelected.damageDealt - lastDamage));
                    }
                }
                else
                {
                    timer = 0;
                    lastDamage = 0;
                    if (upgradeTreeButton != null)
                        Write("UPGRADES");

                }



            }
        }








    }

}