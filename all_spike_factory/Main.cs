﻿using MelonLoader;
using Harmony;

using Assets.Scripts.Unity.UI_New.InGame;

using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using System;
using System.Text.RegularExpressions;
using System.IO;
using Assets.Main.Scenes;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using BTD_Mod_Helper.Extensions;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Bloons.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Simulation.Track;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.GenericBehaviors;

namespace all_spike_factory
{
    public class Main : MelonMod
    {



        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            //EventRegistry.instance.listen(typeof(Main));
            Console.WriteLine("all_spike_factory loaded");
        }

        static TowerModel baseSpac;





        [HarmonyPatch(typeof(TitleScreen), "Start")]
        public class Awake_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                var models = Game.instance.model.towers;
                baseSpac = Game.instance.model.GetTowerFromId("SpikeFactory");


                for (int i = 0; i < models.Count; i++)
                {
                    var tower = models[i];
                    if (tower.name.ToLower().Contains("helipilot")) continue;
                    if (tower.name.ToLower().Contains("monkeyace")) continue;
                    if (Regex.IsMatch(tower.name, "DartlingGunner-4..") || Regex.IsMatch(tower.name, "DartlingGunner-5..") || Regex.IsMatch(tower.name, "BoomerangMonkey-5..")) continue;
                    //Console.WriteLine(tower.name);


                    try
                    {
                        if (tower.HasBehavior<AttackModel>())// && tower.GetBehavior<AttackModel>().weapons[0].projectile.HasBehavior<TravelStraitModel>()
                        {
                            var baseSpacAttackClone = baseSpac.GetBehavior<AttackModel>().Duplicate();
                            var baseSpacAttackWeapon0Clone = baseSpac.GetBehavior<AttackModel>().weapons[0].Duplicate();
                            bool hasProjectiles = false;

                            foreach (var proj in tower.GetBehavior<AttackModel>().GetAllProjectiles())
                            {
                                if (proj.HasBehavior<TravelStraitModel>() || tower.name.ToLower().Contains("boomer"))
                                {
                                    hasProjectiles = true;

                                }
                            }
                            if (hasProjectiles)
                            {
                                var oldAttack = tower.GetBehavior<AttackModel>().Duplicate();
                                baseSpacAttackClone.range = oldAttack.range;


                                int j = 0;
                                bool modified = false;
                                foreach (var wep in tower.GetBehavior<AttackModel>().weapons)
                                {
                                    if (wep.projectile.HasBehavior<TravelStraitModel>() || tower.name.ToLower().Contains("boomer"))
                                    {
                                        if (modified)
                                        {
                                            baseSpacAttackClone.AddWeapon(baseSpacAttackWeapon0Clone.Duplicate());
                                        }


                                        modified = true;
                                        baseSpacAttackClone.weapons[j].Rate = wep.Rate;
                                        int pierceMultiplier = 1;
                                        try { pierceMultiplier = wep.emission.Cast<RandomArcEmissionModel>().Count; } catch { }
                                        try { pierceMultiplier = wep.emission.Cast<ArcEmissionModel>().Count; } catch { }
                                        try { pierceMultiplier = wep.emission.Cast<RandomEmissionModel>().count; } catch { }
                                        try { pierceMultiplier = wep.emission.Cast<AdoraEmissionModel>().count; } catch { }
                                        try { pierceMultiplier = wep.emission.Cast<AlternatingArcEmissionModel>().count; } catch { }


                                        //baseSpacAttackClone.weapons[0].projectile.GetBehavior<SetSpriteFromPierceModel>().sprites = new UnhollowerBaseLib.Il2CppStringArray(8) { proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display, proj.GetBehavior<DisplayModel>().display };
                                        //baseSpacAttackClone.weapons[0].projectile.GetBehavior<DisplayModel>().display = proj.GetBehavior<DisplayModel>().display;
                                        //baseSpacAttackClone.weapons[0].projectile.RemoveBehavior<SetSpriteFromPierceModel>();
                                        //baseSpacAttackClone.weapons[0].projectile.RemoveBehavior<DisplayModel>();


                                        wep.projectile.RemoveBehavior<TravelStraitModel>();
                                        wep.projectile.RemoveBehavior<FollowPathModel>();
                                        wep.projectile.RemoveBehavior<DisplayModel>();
                                        wep.projectile.RemoveBehavior<TrackTargetWithinTimeModel>();
                                        wep.projectile.RemoveBehavior<TrackTargetModel>();
                                        baseSpacAttackClone.weapons[j].projectile.collisionPasses = wep.projectile.collisionPasses;

                                        if (wep.projectile.HasBehavior<DamageModel>())
                                        {
                                            baseSpacAttackClone.weapons[j].projectile.GetBehavior<DamageModel>().damage = wep.projectile.GetBehavior<DamageModel>().damage;
                                            baseSpacAttackClone.weapons[j].projectile.GetBehavior<DamageModel>().immuneBloonProperties = wep.projectile.GetBehavior<DamageModel>().immuneBloonProperties;
                                        }
                                        else
                                        {
                                            baseSpacAttackClone.weapons[j].projectile.RemoveBehavior<DamageModel>();
                                        }

                                        foreach (var bev in wep.projectile.behaviors)
                                        {
                                            baseSpacAttackClone.weapons[j].projectile.AddBehavior(bev.Duplicate());
                                        }


                                        baseSpacAttackClone.weapons[j].projectile.pierce = wep.projectile.pierce * pierceMultiplier;
                                        baseSpacAttackClone.weapons[j].projectile.maxPierce = wep.projectile.maxPierce * pierceMultiplier;


                                        j++;

                                        //this shouldn't be there. if I remove this then the boat shoots really fast for some reason
                                        break;
                                    }
                                }
                                tower.RemoveBehavior<AttackModel>();
                                tower.AddBehavior(baseSpacAttackClone);
                                tower.TargetTypes = baseSpac.TargetTypes.Duplicate();
                            }




                        }
                    }
                    catch
                    {
                        Console.WriteLine(tower.name + " failed");
                    }





                }

            }
        }



        public override void OnUpdate()
        {
            base.OnUpdate();

            bool inAGame = InGame.instance != null && InGame.instance.bridge != null;
            if (inAGame)
            {

            }
        }








    }

}