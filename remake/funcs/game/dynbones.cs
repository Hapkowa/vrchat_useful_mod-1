using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod.remake.util;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.CrashLog;
using VRC;

namespace TestMod.remake.funcs.game
{
    public class dynbones
    {
        public class bones
        {
            public DynamicBone bone { get; set; }
            public List<DynamicBoneCollider> Colliders { get; set; } = new List<DynamicBoneCollider>();
        }
        public class avatars
        {
            public List<bones> extra_collisions { get; set; } = new List<bones>();
        }
        public static Dictionary<string, avatars> map = new Dictionary<string, avatars>();
        public static void tracker(string avid, GameObject aviobj, string name = "")
        {
            var bones = aviobj.GetComponentsInChildren<DynamicBone>(true);
            var colliders = aviobj.GetComponentsInChildren<DynamicBoneCollider>(true);
            var bone_map = new List<bones>();
            MelonModLogger.Log("bones " + bones.Count + " | colliders " + colliders.Count + " | user " + name);            
            for(var i=0;i<bones.Count;i++)
            {
                bone_map.Add(new bones()
                {
                    bone = bones[i]
                });
            }
            var avimap = new avatars()
            {
                extra_collisions = bone_map
            };
            map.Add(avid, avimap);
            update(avid, colliders);
        }
        public static void update(string avid, Il2CppArrayBase<DynamicBoneCollider> cols)
        {
            foreach (KeyValuePair<string, avatars> k in map)
            {
                if (k.Key != avid)
                {
                    foreach (var m in k.Value.extra_collisions)
                    {
                        for (var i=0;i<cols.Count;i++)
                        {
                            var col = cols[i];
                            m.bone.m_Colliders.Add(col);
                            m.Colliders.Add(col);
                        }
                    }
                }
            }
        }
        public static void remove(string avid)
        {
            if (map.ContainsKey(avid) == false) return;
            var cur_map = map[avid];
            cur_map.extra_collisions.ForEach(bone =>
            {
                bone.Colliders.ForEach(col => bone.bone.m_Colliders.Remove(col));
            });
            map.Remove(avid);
        }
    }
}

