using Il2CppSystem.Net;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMod.remake.util;
using UnityEngine;
using VRC;

namespace TestMod.remake.funcs.game
{
    public class antispawn_sound
    {
        public struct avi_data
        {
            public string userid;
            public float check_time;
        }
        public static Dictionary<string, avi_data> avatar_list = new Dictionary<string, avi_data>();
        public static bool check_sounds(Player user)
        {
            var audio_components = user.GetComponentsInChildren<AudioSource>();
            if (audio_components == null) return false;
            if (audio_components.Count == 0) return false;
            var did_stop_annoying_sound_that_no_one_in_this_world_wants_to_hear_whatsoever = false;
            for (var b = 0; b < audio_components.Count; b++)
            {
                var obj = audio_components[b];
                if (obj == null) continue;
                if (!obj.isPlaying) continue;
                if (obj.name.Contains("USpeak")) continue;/*just why is this here dont combine audio sources with uspeaker*/
                obj.Stop();
                did_stop_annoying_sound_that_no_one_in_this_world_wants_to_hear_whatsoever = true;
            }
            if (did_stop_annoying_sound_that_no_one_in_this_world_wants_to_hear_whatsoever) return true;
            return false;
        }
        public static void delete_user_avis(Player user)
        {
            try//can fail if none are present but hey who fucking cares right
            {
                if (avatar_list.Count == 0) return;
                foreach (var obj in avatar_list)
                {
                    if (obj.Key == null) continue; if (obj.Value.userid == null) continue;
                    if (obj.Key.Contains(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id)) continue; /*dont delete the current avis*/
                    if (obj.Value.userid.Contains(user.field_Private_APIUser_0.id)) avatar_list.Remove(obj.Key);
                }
            }
            catch (Exception e) { }
        }
        public static bool has_switched_avatar(Player user)
        {
            if (avatar_list.Count == 0) return false;
            var avatar_known = avatar_list.ContainsKey(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id);
            if (avatar_known)
            {
                if (avatar_list[user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id].userid.Contains(user.field_Private_APIUser_0.id)) return false;
                else return true;
            }
            return true;
        }
        public static bool should_check_user(Player user)
        {
            //see if possibly someone is now using the avatar and should be re-checked?
            var should_recheck = has_switched_avatar(user);
            if (should_recheck)
            {
                avatar_list.Remove(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id);
                delete_user_avis(user);
            }
            if (avatar_list.ContainsKey(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id))
            {
                var data_for_usr = avatar_list[user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id];
                if (data_for_usr.check_time > Time.time)
                {
                    //still within checking time
                    return true;
                }                
                return false;
            }
            avatar_list.Add(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id, new avi_data() { userid = user.field_Private_APIUser_0.id, check_time = Time.time + 2 });
            return true;
        }
        public static void anti_spawn_sound()
        {
            var users = utils.get_all_player();
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (user == null) continue; if (user.field_Private_APIUser_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0 == null) continue;
                if (user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0 == null) continue;
                if (Enum.GetName(typeof(hashmod.NHDDDDJNDMB), user.field_Internal_VRCPlayer_0.prop_VRCAvatarManager_0.prop_EnumNPublicSealedva9vUnique_0).Contains("Custom") == false) continue;
                if (should_check_user(user) == false) continue;
                if (check_sounds(user)) MelonModLogger.Log("[!!!] spawn sound was nuked for user \"" + user.field_Private_APIUser_0.displayName.ToString() + "\"");
            }
        }
    }
}
