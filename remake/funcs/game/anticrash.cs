using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRCSDK2;
using System.Net.Http;
using VRC;
using VRTK.Controllables.ArtificialBased;
using Transmtn.DTO;
using UnityEngine.UI;
using VRC.Core;
using MelonLoader;
using hashmod.remake.util;
using Il2CppSystem.Net;
using Il2CppMono.Security.X509;
using Il2CppSystem.Security.Cryptography.X509Certificates;
using Il2CppSystem.Net.Security;

using Il2CppMono.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace hashmod.remake.funcs.game
{
    public static class anticrash
    {
        public static Dictionary<string, avatar_data> anti_crash_list = new Dictionary<string, avatar_data>();
        private static int get_poly_count(GameObject player)
        {
            var poly_count = 0;
            var skinmeshs = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var obj in skinmeshs)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += count_poly_meshes(obj.sharedMesh);
                }
            }
            var meshfilters = player.GetComponentsInChildren<MeshFilter>(true);
            foreach (var obj in meshfilters)
            {
                if (obj != null)
                {
                    if (obj.sharedMesh == null) continue;
                    poly_count += count_poly_meshes(obj.sharedMesh);
                }
            }
            return poly_count;
        }
        internal static int count_polys(Renderer r)
        {
            int num = 0;
            var skinnedMeshRenderer = r as SkinnedMeshRenderer;
            if (skinnedMeshRenderer != null)
            {
                if (skinnedMeshRenderer.sharedMesh == null) return 0;
                num += count_poly_meshes(skinnedMeshRenderer.sharedMesh);
            }
            return num;
        }
        private static int count_poly_meshes(Mesh sourceMesh)
        {
            bool flag = false;
            Mesh mesh;
            if (sourceMesh.isReadable) mesh = sourceMesh;
            else
            {
                mesh = UnityEngine.Object.Instantiate<Mesh>(sourceMesh);
                flag = true;
            }
            int num = 0;
            for (int i = 0; i < mesh.subMeshCount; i++) num += mesh.GetTriangles(i).Length / 3;
            if (flag) UnityEngine.Object.Destroy(mesh);
            return num;
        }
        public struct user_data
        {
            public string avid;
            public bool was_checked;
        }
        public static Dictionary<string, user_data> player_data = new Dictionary<string, user_data>();
        public static user_data user_by_id(string id)
        {
            foreach (var obj in player_data) if (obj.Key.Contains(id)) return obj.Value;
            return new user_data() { avid = "why cant this return be null what the fuck" };
        }
        public static bool has_changed_avatar(Player user)
        {
            if (player_data.Count == 0) return false;
            var avatar_known = player_data.ContainsKey(user.field_Private_APIUser_0.id);
            if (avatar_known)
            {
                if (player_data[user.field_Private_APIUser_0.id].avid.Contains(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id)) return false;
                else return true;
            }
            return false;
        }
        public static void delete_user_avis(Player user)
        {
            try//can fail if none are present but hey who fucking cares right
            {
                if (player_data.Count == 0) return;
                foreach (var obj in player_data)
                {
                    if (obj.Key == null) continue; if (obj.Value.avid == null) continue;
                    if (obj.Key.Contains(user.field_Private_APIUser_0.id)) continue; /*dont delete the current avis*/
                    if (obj.Value.avid.Contains(user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id)) player_data.Remove(obj.Key);
                }
            }
            catch (Exception e) { }
        }
        public static bool should_check_user(Player user)
        {
            if (hashmod.anti_crasher_ignore_friends) if (user.get_api().isFriend) return false;
            try//yeah no one cares sdfjnjsdfjdsfjfnfsdn
            {
                /*see if the user is even using fucking avatar that is loaded in*/
                if (Enum.GetName(typeof(hashmod.NHDDDDJNDMB), user.field_Internal_VRCPlayer_0.prop_VRCAvatarManager_0.prop_EnumNPublicSealedva9vUnique_0).Contains("Custom") == false) return false;
                var entry = user_by_id(user.field_Private_APIUser_0.id);
                /*means we have not checked the user yet neither his shit so do that?*/
                if (entry.avid.Contains("why cant this return be null what the fuck")) return true;
                else
                {
                    /*remove that guy and uh can i have like a medium coke with that, and his avatar from all other lists if used*/
                    if (has_changed_avatar(user))
                    {
                        player_data.Remove(user.field_Private_APIUser_0.id);
                        delete_user_avis(user);
                        return true;
                    }
                    if (entry.was_checked == true) return false;
                    else
                    {
                        player_data.Remove(user.field_Private_APIUser_0.id);
                        return true;
                    }
                }
            }
            catch (Exception e) { return false; }
        }
        public static bool particle_check(Player user)
        {
            var particle_sys = user.GetComponentsInChildren<ParticleSystem>();
            if (particle_sys == null ||
                particle_sys.Count == 0) return false;
            var total_particles = 0; var count_particles = 0;
            for (var i = 0; i < particle_sys.Count; i++)
            {
                var obj = particle_sys[i];
                if (obj == null) continue;
                total_particles += obj.maxParticles; count_particles += obj.particleCount;
            }
            if (total_particles >= hashmod.max_particles || count_particles >= hashmod.max_particles)
            {
                for (var i = 0; i < particle_sys.Count; i++)
                {
                    var obj = particle_sys[i];
                    if (obj == null) continue;
                    var particle_renderer = obj.GetComponent<ParticleSystemRenderer>();
                    if (particle_renderer == null) continue;
                    if (particle_renderer.enabled == false) continue;
                    obj.Stop(true);
                    particle_renderer.enabled = false;
                }
                return true;
            }
            return false;
        }
        public static bool polygon_check(Player user, int polys)
        {
            //MelonModLogger.Log("polys for user \"" + user.field_Private_VRCPlayerApi_0.displayName + "\" " + polys);
            if (polys >= hashmod.max_polygons)
            {
                var renderers = user.field_Private_VRCAvatarManager_0.GetComponentsInChildren<Renderer>();
                for (var i = 0; i < renderers.Count; i++)
                {
                    var obj = renderers[i];
                    if (obj == null ||
                        obj.enabled == false) continue;
                    obj.enabled = false;
                    UnityEngine.Object.Destroy(obj);
                }
                return true;
            }
            return false;
        }
        public static string[] shader_list;
        public static List<string> shader_list_local = new List<string>();
        public static bool shader_check(Player user)
        {
            if (user.get_api().id == APIUser.CurrentUser.id) return false;
            var renderers = user.field_Private_VRCAvatarManager_0.GetComponentsInChildren<Renderer>(true);
            var default_shader = Shader.Find("Standard"); var did_change = false;
            for (var i = 0; i < renderers.Count; i++)
            {
                var obj = renderers[i];
                if (obj == null) continue;
                for (var m = 0; m < obj.materials.Count; m++)
                {
                    var mat = obj.materials[m];
                    var should_normalize = false;
                    if (hashmod.should_use_fetched_list)
                    {
                        foreach (var n in anticrash.shader_list)
                        {
                            if (mat.shader.name.Equals(n))
                            {
                                should_normalize = true;
                                break;
                            }
                        }
                    }
                    foreach (var n in anticrash.shader_list_local)
                    {
                        if (mat.shader.name.Equals(n))
                        {
                            should_normalize = true;
                            break;
                        }
                    }
                    if (should_normalize == true)
                    {
                        UnityEngine.Object.Destroy(mat);
                        did_change = true;
                    }
                }
            }
            if (did_change == false) return false;
            else return true;
        }
        public static void work()
        {
            if (shader_list.Length == 0) shader_list = utils.get_shader_blacklist();
            var users = utils.get_all_player();
            if (users == null || users.Count == 0) return;
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (user == null) continue;
                /*i swear to fucking fgnngfhngfnhgnfhngfhn*/
                if (user.field_Private_APIUser_0 == null ||
                    user.field_Private_VRCAvatarManager_0 == null ||
                    user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0 == null ||
                    user.field_Internal_VRCPlayer_0 == null ||
                    user.field_Internal_VRCPlayer_0.prop_VRCAvatarManager_0 == null) continue;
                if (should_check_user(user) == false) continue;
                /*add the guy first before we die or smth*/
                var data = user_by_id(user.field_Private_APIUser_0.id);
                if (data.avid.Contains("why cant this return be null what the fuck")) player_data.Add(user.field_Private_APIUser_0.id, new user_data() { avid = user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id, was_checked = false });
                var polygon_count = get_poly_count(user.gameObject);
                if (polygon_count <= 2420)//still loading wwwwwwwwwwww
                {
                    player_data.Remove(user.field_Private_APIUser_0.id);
                    delete_user_avis(user);
                    continue;
                }
                var blocked_particles = particle_check(user);
                var blocked_avatar = polygon_check(user, polygon_count);
                if (hashmod.anti_crasher_shader)
                {
                    var blocked_shaders = shader_check(user);
                    if (blocked_shaders) MelonModLogger.Log("[!!!] nuked shaders for \"" + user.field_Private_APIUser_0.displayName.ToString() + "\"");
                }
                if (blocked_particles) MelonModLogger.Log("[!!!] nuked particles for \"" + user.field_Private_APIUser_0.displayName.ToString() + "\"");
                if (blocked_avatar) MelonModLogger.Log("[!!!] nuked avatar for \"" + user.field_Private_APIUser_0.displayName.ToString() + "\"");
                /*why the actual fuck is this not one line able erjnkgnjrkewgbjnwerjngerwjngwerg*/
                //player_data[user.field_Private_APIUser_0.id].was_checked = true; HOW DOES IT NOT WORK
                //this is shit do not ever do this, c# is shit
                player_data.Remove(user.field_Private_APIUser_0.id);
                player_data.Add(user.field_Private_APIUser_0.id, new user_data() { avid = user.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id, was_checked = true });
            }
        }
    }
}
