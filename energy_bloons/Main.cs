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

//using Il2CppSystem.Collections;
using Assets.Scripts.Unity.UI_New.Popups;
using System.Reflection;
using Assets.Scripts.Models;
using System.Collections.Generic;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.GenericBehaviors;
using System;
using System.Linq;
using Assets.Scripts.Models.ServerEvents;
using Assets.Scripts.Data.Cosmetics.Pets;
using Assets.Main.Scenes;
using UnhollowerBaseLib;

using Assets.Scripts.Models.Rounds;
using Assets.Scripts.Models.Store;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Models.Map;

namespace energy_bloons
{
    public class Main : MelonMod
    {


        public static System.Random r = new System.Random();
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            Console.WriteLine("energy_bloons loaded");
        }

        [HarmonyPatch(typeof(UnityToSimulation), "InitMap")]
        public class InitMap_Patch
        {

            [HarmonyPrefix]
            public static bool Prefix(UnityToSimulation __instance, ref MapModel map)
            {
                map.mapWideBloonSpeed = 5;

                //map.areas[0].polygon.


                for (int p = 0; p < map.paths.Count; p++)
                {
                    Il2CppReferenceArray<PointInfo> arr = new Il2CppReferenceArray<PointInfo>(map.paths[p].points.Count);
                    for (int i = 0; i < map.paths[p].points.Count; i++)
                    {
                        PointInfo pointinfo = map.paths[p].points[i];
                        var vec = pointinfo.point;
                        //map.paths[0].points[i]
                        pointinfo.point = new Assets.Scripts.Simulation.SMath.Vector3(vec.x + ((float)r.NextDouble() * 10), vec.y + ((float)r.NextDouble() * 10), vec.z);
                        arr[i] = pointinfo;
                    }
                    map.paths[p].points = arr;
                }




                return true;
            }

        }

    }
}