using Harmony;
using MelonLoader;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using VRCSDK2;
using System.Net.Http;
using VRC;
using VRTK.Controllables.ArtificialBased;
using Transmtn.DTO;
using hashmod;
using UnityEngine.UI;
using VRC.Core;
using VRC.UI;
using ThirdParty.iOS4Unity;
using BestHTTP;
using VRC.Core.BestHTTP;
using Il2CppSystem.Threading.Tasks;
using Transmtn;
using System.Threading.Tasks;
using Il2CppSystem.Threading;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Il2CppMono.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using hashmod.remake.util;
using hashmod.remake.btn;
using hashmod.remake.funcs.game;
using hashmod.remake.funcs.menu;
using Org.BouncyCastle.Math.Raw;
using UnityEngine.SceneManagement;
using DiskWars;
using UnhollowerRuntimeLib;
using UnityEngine.Events;
using VRC.SDKBase;
using VRCSDK2.Validation.Performance.Scanners;
using Steamworks;
using WebSocketSharp;
using Transmtn.DTO.Notifications;
using Il2CppSystem.Security.Cryptography;
using UnityEngine.EventSystems;

namespace hashmod
{
    public static class BuildInfo
    {
        public const string Name = "useful stuff mod"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "hash"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.3.3.7"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class hashmod : MelonMod
    {
        public static Text slider_fov_txt;
        public static Text slider_flyspeed_txt;
        public static Text slider_runspeed_txt;
        public static Text slider_walkspeed_txt;
        public static float fov_cam = 60f;
        public static bool needs_update = true;
        public static string mod_version = "33";
        public static int latest_version = 0;
        public static bool should_show_update_notice = false;
        public static bool fly_mode = false;
        public static bool fly_mode_onpress = false;
        public static bool clone_mode = true;
        public static bool delete_portals = false;
        public static bool anti_crasher = false;
        public static bool rainbow_friend_nameborder = true;
        public static bool esp_players = false;
        public static bool info_plus_toggle = false;
        public static bool show_blocked_avatar = false;
        public static bool should_use_fetched_list = true;
        public static float last_refresh = 0;
        public static bool anti_spawn_music = true;
        public static bool speed_hacks = false;
        public static float flying_speed = 4f;
        public static float run_speed = 10f;
        public static float walk_speed = 8f;
        public static int max_fps = 144;
        public static int max_particles = 50000;
        public static int max_polygons = 500000;
        public static bool anti_crasher_ignore_friends = false;
        public static bool anti_crasher_shader = false;
        public static bool esp_rainbow_mode = true;
        public static bool sub_menu_open = false;
        public static bool sub_sub_menu_open = false;
        public static bool init_social_refresh = false;
        public static GameObject main_menu_mod = null;
        public static GameObject main_menu_page2_mod = null;
        public static GameObject main_menu_utils = null;
        public static bool fly_down;
        public static bool fly_up;
        public static bool setup_button;
        public static bool setup_userinfo_button;
        public static bool utils_menu_active = false;
        public static bool in_input_shit = false;
        public static bool isNoclip = false;
        public static List<int> noClipToEnable = new List<int>();
        public LayerMask collisionLayers = -1;
        public static UiAvatarList avatarslist;
        static float last_routine;
        public static avatar_ui_button fav_btn;
        public static avatar_ui fav_list = new avatar_ui();
        public static avatar_ui pub_list = new avatar_ui();

        public override void OnApplicationStart()
        {
            anticrash.shader_list = new string[] { }; shader_menu.set_canvas = null; main_menu_utils = null;
            var ini = new IniFile("hashcfg.ini");
            avatar_config.load(); avatar_config.avatar_list.Reverse(); ini.setup();
            if (File.Exists("hashmod_shaderlist.txt")) anticrash.shader_list_local = System.IO.File.ReadAllLines("hashmod_shaderlist.txt").ToList();
            var v = utils.check_version();
            latest_version = v;
            if (mod_version.Contains(v.ToString())) needs_update = false;
            else
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

                    HttpWebResponse httpWebResponse = (HttpWebResponse)WebRequest.Create("https://raw.githubusercontent.com/kichiro1337/vrchat_useful_mod/master/latest.txt").GetResponse();

                    //after suffering to convert this from a raw dll file to anything saveable i just decided to go with base64 

                    string[] files = Directory.GetFiles("Mods");
                    foreach (string file in files)
                    {
                        if (file.Contains("useful_mod") == false) continue;
                        File.Delete(file);
                    }

                    File.WriteAllBytes("Mods/vrchat_useful_mod_" + latest_version + ".dll", Convert.FromBase64String(new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd()));
                    MelonModLogger.Log("[!!!] Updated mod to version " + latest_version + " make sure to restart to apply the new changes");
                    should_show_update_notice = true;

                    return;
                }
                catch (Exception ex)
                {
                    MelonModLogger.Log(ex.ToString());
                    return;
                }
            }
        }
        public override void OnLevelWasLoaded(int level)
        {
            anticrash.anti_crash_list.Clear(); anticrash.player_data.Clear();
            dynbones.map.Clear();
            antispawn_sound.avatar_list.Clear();
        }
        public override void OnLevelWasInitialized(int level)
        {
            anticrash.anti_crash_list.Clear(); anticrash.player_data.Clear();
            dynbones.map.Clear();
            antispawn_sound.avatar_list.Clear();
        }
        public enum NHDDDDJNDMB
        {
            // Token: 0x04005A9C RID: 23196
            Undefined,
            // Token: 0x04005A9D RID: 23197
            Loading,
            // Token: 0x04005A9E RID: 23198
            Error,
            // Token: 0x04005A9F RID: 23199
            Blocked,
            // Token: 0x04005AA0 RID: 23200
            Safety,
            // Token: 0x04005AA1 RID: 23201
            Substitute,
            // Token: 0x04005AA2 RID: 23202
            Performance,
            // Token: 0x04005AA3 RID: 23203
            Custom
        }
        public static void error_type_poput(string Title, string Content)//error type message popup with confirm
        {
            Resources.FindObjectsOfTypeAll<VRCUiPopupManager>()[0].Method_Public_Void_String_String_Single_0(Title, Content, 20f);
        }
        public static List<GameObject> created_listing_objects = new List<GameObject>();
        public static List<List<string>> shader_pages = new List<List<string>>();
        public static int last_shader_page = 0;
        public static string object_path(GameObject obj)
        {
            var path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        public static void find_all_child_objects(GameObject obj)
        {
            MelonModLogger.Log(object_path(obj));
            for (var i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                find_all_child_objects(child.gameObject);
            }
        }
        public static void delete_all_objects(GameObject obj)
        {
            MelonModLogger.Log(object_path(obj));
            UnityEngine.Object.Destroy(obj);
            for (var i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                delete_all_objects(child.gameObject);
            }
        }
        public static float last_friend_update = 0f;
        public static List<string> friend_list = new List<string>();//for whatever reason il2cpp shit rapes performance so hard so this is a workaround for this
        public static void update_friend_list()
        {
            if (Time.time > last_friend_update)
            {
                //update every 10 sec because fuck this?
                last_friend_update = Time.time + 10;
                friend_list.Clear();
                for (var i = 0; i < APIUser.CurrentUser.friendIDs.Count; i++)
                {
                    var id = APIUser.CurrentUser.friendIDs[i];
                    friend_list.Add(id);
                }
            }
        }
        public override void OnUpdate()
        {
            try
            {
                if (RoomManagerBase.prop_Boolean_3 == false) return;
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null && should_show_update_notice == true && Enum.GetName(typeof(hashmod.NHDDDDJNDMB), VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCAvatarManager_0.prop_EnumNPublicSealedva9vUnique_0).Contains("Custom") == true) { should_show_update_notice = false; error_type_poput("Mod was updated!", "useful_mod was updated to a new version, make sure to restart your game to apply the update!"); }
                menu.version_info();
                if (delete_portals) antiportal.auto_delete_portals();
                if (isNoclip || fly_mode) flying.height_adjust();
                if (anti_spawn_music) antispawn_sound.anti_spawn_sound();
                if (Time.time > last_routine)
                {
                    last_routine = Time.time + 1;
                    if (anti_crasher) anticrash.work();
                    if (speed_hacks) speed.set_speeds(walk_speed, run_speed);
                    if (info_plus_toggle) infoplus.info_plus();
                    if (esp_players) visuals.esp_player();
                    fov.set_cam_fov(fov_cam);
                    if (init_social_refresh == false) setup_refresh_button_social();
                    update_friend_list();
                }
                if (clone_mode) direct_clone.direct_menu_clone();
                if (sub_menu_open || sub_sub_menu_open || utils_menu_active || shader_menu.open_menu) menu.menu_toggle_handler();
                if (anti_crasher_shader) shader_menu.work();
                visuals.update_color();
                if (rainbow_friend_nameborder) name_border_rbg.name_border_clr();
            }
            catch (Exception e)
            {
                MelonModLogger.Log("Error in the main routine! " + e.Message + " in " + e.Source + " Stack: " + e.StackTrace);
            }
        }

        private static void setup_refresh_button_social()
        {
            var found_edit_btn = GameObject.Find("Screens/Social/Current Status/StatusButton");
            if (found_edit_btn != null)
            {

                init_social_refresh = true;
                var button = found_edit_btn.transform.GetComponentInChildren<Button>();

                var edit_btn_clone = UnityEngine.Object.Instantiate<GameObject>(button.gameObject);
                edit_btn_clone.gameObject.name = "EditBtnClone";
                edit_btn_clone.transform.localPosition += new Vector3(250, 0, 0);
                edit_btn_clone.GetComponentInChildren<UnityEngine.UI.Text>().text = $"Refresh";
                edit_btn_clone.GetComponentInChildren<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                edit_btn_clone.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(new Action(() =>
                {
                    if (Time.time > last_refresh)
                    {
                        last_refresh = Time.time + 30;
                        var page = Resources.FindObjectsOfTypeAll<UiVRCList>();
                        if (page != null)
                        {
                            for (var i = 0; i < page.Count; i++)
                            {
                                var p = page[i];
                                if (p == null) continue;

                                p.Method_Public_Void_2();
                                p.Method_Public_Void_1();
                                p.Method_Public_Void_0();
                            }
                        }
                    }
                    else
                    {
                        var sec_left = last_refresh - Time.time;
                        error_type_poput("Function is still on cooldown!", "Please wait " + Math.Floor(sec_left) + " seconds before trying again!");
                    }
                }));

                var parent = GameObject.Find("Screens/Social/Current Status");
                edit_btn_clone.transform.SetParent(parent.transform, false);

                //move text a bit away
                var found_text_name = GameObject.Find("Screens/Social/Current Status/StatusText");
                found_text_name.GetComponent<Text>().transform.localPosition += new Vector3(250, 0, 0);

                //move the icon
                var found_statusicon = GameObject.Find("Screens/Social/Current Status/StatusIcon");
                found_statusicon.transform.localPosition += new Vector3(250, 0, 0);
            }
        }

        public override void OnFixedUpdate()
        {

        }
        public override void OnLateUpdate()
        {

        }
        public override void OnGUI()
        {

        }
        public override void OnApplicationQuit()
        {

        }
        public override void OnModSettingsApplied()
        {

        }
        public static float last_apicall = 0;
        public static float last_msg_apicall = 0;
        public static void social_btn(float xpos, float ypos, string txt, System.Action listener)
        {
            var ting = GameObject.Find("Screens").transform.Find("UserInfo");
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/Moderator/Actions/Warn").gameObject;
            var Btn = UnityEngine.Object.Instantiate(BtnObj, BtnObj.transform, true).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(Btn.transform.localPosition.x - 275 + xpos, Btn.transform.localPosition.y - 50 + ypos, Btn.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.enabled = true; Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = Color.green;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(25, 0);
            Btn.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 10);
            Btn.onClick.AddListener(listener);
            Btn.transform.SetParent(ting.transform);
        }
        public static void social_btn(Color bordger, float xpos, float ypos, string txt, System.Action listener, Transform parent)
        {
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/Moderator/Actions/Warn").gameObject;
            var Btn = UnityEngine.Object.Instantiate(BtnObj, parent.transform, false).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(parent.transform.localPosition.x - 2000 + xpos, parent.transform.localPosition.y - ypos, parent.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.enabled = true; Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = bordger;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 0);
            Btn.GetComponent<RectTransform>().sizeDelta -= new Vector2(3, 10);
            Btn.onClick.AddListener(listener);
            Btn.transform.SetParent(parent);
        }
        public override void VRChat_OnUiManagerInit()
        {
            var shortcutmenu = utils.get_quick_menu().transform.Find("ShortcutMenu");

            var screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
            if (!setup_userinfo_button && screensmenu != null && utils.get_quick_menu().transform.Find("ShortcutMenu/BuildNumText") != null)
            {
                setup_userinfo_button = true;

                /*setup of stuff*/
                pubavatar.setup_user_avatars_list();
                favplus.setup_fav_plus();

                /*clones*/
                social_btn(0f, 0f, "Teleport", new Action(() => { social.do_clone_to_social(); }));
                social_btn(130f, 0f, "Log asset", new Action(() => { social.log_asset_to_social(); }));
                social_btn(260f, 0f, "Clone", new Action(() => { social.do_clone_to_social(); }));
                social_btn(0f, -65f, "Add Fav+", new Action(() => { favplus.save_social_to_favplus(); }));
                social_btn(130f, -65f, "Send msg", new Action(() => { social.send_msg_to_social(); }));
            }

            if (shortcutmenu != null && setup_button == false)
            {
                setup_button = true;

                main_menu_mod = menu.make_blank_page("sub_menu");
                main_menu_page2_mod = menu.make_blank_page("sub_menu_2");
                main_menu_utils = menu.make_blank_page("sub_menu_3");

                //menu entrance
                var menubutton = btn_utils.create_btn(false, ButtonType.Default, "Open menu", "Opens the mod menu", Color.white, Color.red, -4, 3, shortcutmenu,
                new Action(() =>
                {
                    sub_menu_open = true;
                    main_menu_mod.SetActive(true);
                    shortcutmenu.gameObject.SetActive(false);
                }),
                new Action(() =>
                {

                }));
                //menu-menu entrance
                var submenubutton = btn_utils.create_btn(false, ButtonType.Default, "Next page", "Next page of the mod", Color.white, Color.red, -4, 3, main_menu_mod.transform,
                new Action(() =>
                {
                    sub_menu_open = false;
                    sub_sub_menu_open = true;
                    main_menu_mod.SetActive(false);
                    main_menu_page2_mod.SetActive(true);
                    shortcutmenu.gameObject.SetActive(false);
                }),
                new Action(() =>
                {

                }));

                main_menu();
                direct_menu();

                //menu 2
                slider_fov_txt = utils.make_slider(main_menu_page2_mod, delegate (float v)
                {
                    fov_cam = v;
                    slider_fov_txt.text = "  Cam FOV (" + String.Format("{0:0.##}", v) + ")";
                }, -3, 4, "  Cam FOV (" + String.Format("{0:0.##}", fov_cam) + ")", fov_cam, 100, 60, 350);

                slider_flyspeed_txt = utils.make_slider(main_menu_page2_mod, delegate (float v)
                {
                    flying_speed = v;
                    slider_flyspeed_txt.text = "  Fly-speed (" + String.Format("{0:0.##}", v) + ")";
                }, -1, 4, "  Fly-speed (" + String.Format("{0:0.##}", flying_speed) + ")", flying_speed, 18, 1, 350);

                slider_runspeed_txt = utils.make_slider(main_menu_page2_mod, delegate (float v)
                {
                    run_speed = v;
                    slider_runspeed_txt.text = "  Run-speed (" + String.Format("{0:0.##}", v) + ")";
                }, -3, 3, "  Run-speed (" + String.Format("{0:0.##}", run_speed) + ")", run_speed, 24, 4, 200);

                slider_walkspeed_txt = utils.make_slider(main_menu_page2_mod, delegate (float v)
                {
                    walk_speed = v;
                    slider_walkspeed_txt.text = "  Walk-speed (" + String.Format("{0:0.##}", v) + ")";
                }, -1, 3, "  Walk-speed (" + String.Format("{0:0.##}", walk_speed) + ")", walk_speed, 20, 2, 200);

                Application.targetFrameRate = max_fps;
            }
        }

        private static void direct_menu()
        {
            var tp_to_user = btn_utils.create_btn(false, ButtonType.Default, "Teleport", "Tps you to user selected", Color.white, Color.red, -3, 3, main_menu_utils.transform,
            new Action(() =>
            {
                var SelectedPlayer = utils.get_quick_menu().get_selected_player();
                VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = SelectedPlayer.transform.position;
            }),
            new Action(() =>
            {

            }));

            var direct_favplus = btn_utils.create_btn(false, ButtonType.Default, "Add to Fav+", "Adds the persons avatar to Fav+ silently", Color.white, Color.red, -2, 3, main_menu_utils.transform,
            new Action(() =>
            {
                favplus.save_direct_to_favplus();
            }),
            new Action(() =>
            {

            }));

            var pubavatar_show = btn_utils.create_btn(false, ButtonType.Default, "Show public avatars", "Attempts to show all public avatars made by the selected user", Color.white, Color.red, -1, 3, main_menu_utils.transform,
            new Action(() =>
            {
                if (Time.time > last_apicall)
                {
                    last_apicall = Time.time + 60;
                    var found_player = utils.get_quick_menu().get_selected_player();
                    if (found_player == null) return;
                    if (found_player.get_api() == null) return;
                    pubavatar.update_public_user_list(found_player.get_api().id);
                    MelonModLogger.Log("pub users for usr: " + found_player.get_api().id);
                    VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1();
                }
                else
                {
                    var sec_left = last_apicall - Time.time;
                    error_type_poput("Function is still on cooldown!", "Please wait " + Math.Floor(sec_left) + " seconds before trying again!");
                }
            }),
            new Action(() =>
            {

            }));

            var dynbones_toggle = btn_utils.create_btn(false, ButtonType.Default, "Add dynamic bones", "Attempt to add all dynamic bones by the user", Color.white, Color.red, 0, 3, main_menu_utils.transform,
            new Action(() =>
            {
                var found_player = utils.get_quick_menu().get_selected_player();
                if (found_player == null) return;
                if (found_player.get_api() == null) return;
                dynbones.remove(found_player.field_Internal_VRCPlayer_0.prop_ApiAvatar_0.id);
                dynbones.tracker(found_player.field_Internal_VRCPlayer_0.prop_ApiAvatar_0.id, found_player.field_Internal_VRCPlayer_0.gameObject, found_player.field_Internal_VRCPlayer_0.prop_VRCPlayerApi_0.displayName);

                dynbones.remove(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id);
                dynbones.tracker(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.displayName);

            }),
            new Action(() =>
            {

            }));

            // new row 2
            var send_message_to_user = btn_utils.create_btn(false, ButtonType.Default, "Send message", "Sends a custom invite message to select user", Color.white, Color.red, -3, 2, main_menu_utils.transform,
            new Action(() =>
            {
                if (Time.time > last_msg_apicall)
                {
                    last_msg_apicall = Time.time + 30;
                    var found_player = utils.get_quick_menu().get_selected_player();
                    if (found_player == null || found_player.field_Private_APIUser_0 == null) return;

                    in_input_shit = true;
                    menu.input_text("Enter the text to send", "A message to send to the target", new Action<string>((a) =>
                    {
                        in_input_shit = false;

                        VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.field_Private_Api_0.PostOffice.Send(Invite.Create(found_player.field_Private_APIUser_0.id, "", new Location("", new Transmtn.DTO.Instance("", found_player.field_Private_APIUser_0.id, "", "", "", false)), a));
                    }));
                }
                else
                {
                    in_input_shit = false;

                    var sec_left = last_msg_apicall - Time.time;
                    error_type_poput("Function is still on cooldown!", "Please wait " + Math.Floor(sec_left) + " seconds before trying again!");
                }
            }),
            new Action(() =>
            {

            }));

            // add to the real menu toggle for the sub utils menu
            var utils_menu_toggle = btn_utils.create_btn(false, ButtonType.Default, "User-options", "Shows user options from useful_mod", Color.white, Color.green, 0, 0, utils.get_quick_menu().transform.Find("UserInteractMenu"),
            new Action(() =>
            {
                var o = utils.get_quick_menu().transform.Find("UserInteractMenu");
                o.gameObject.SetActive(false);
                main_menu_utils.SetActive(true);
                utils_menu_active = true;
            }),
            new Action(() =>
            {

            }), 1);
        }

        private static void main_menu()
        {           
            //page 1 mains
            var ck_flight = utils.make_text_toggle(0, 0, "Flight", fly_mode, main_menu_mod, new Action<bool>((a) =>
            {
                if (a == true)
                {
                    fly_mode = true;
                    if (isNoclip) return;
                    Physics.gravity = new Vector3(0, 0, 0);
                }
                if (a == false)
                {
                    fly_mode = false;
                    if (isNoclip) return;
                    Physics.gravity = VRCSDK2.VRC_SceneDescriptor.Instance.gravity;
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<VRCMotionState>().Method_Public_Void_0();
                }
            }));

            var ck_noclip = utils.make_text_toggle(1100, 0, "No-clip", isNoclip, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                {
                    if (fly_mode == true) fly_mode = false;
                    isNoclip = true;
                    flying.noclip();
                }
                else
                {
                    isNoclip = false;
                    flying.noclip();
                }
            }));

            var ck_forceclone = utils.make_text_toggle(0, 150, "Force-clone", clone_mode, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { clone_mode = true; }
                else
                { clone_mode = false; }
            }));

            var ck_playeresp = utils.make_text_toggle(1100, 150, "Player-ESP", esp_players, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { esp_players = true; }
                else
                {
                    esp_players = false;
                    GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < array.Length; i++) if (array[i].transform.Find("SelectRegion")) utils.toggle_outline(array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), false);
                }
            }));

            var ck_portalkiller = utils.make_text_toggle(0, 300, "Delete-portals", delete_portals, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { delete_portals = true; }
                else
                { delete_portals = false; }
            }));

            var ck_infoplus = utils.make_text_toggle(1100, 300, "Info+", info_plus_toggle, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { info_plus_toggle = true; }
                else
                {
                    info_plus_toggle = false;

                    var users = utils.get_all_player();
                    var self = utils.get_local();
                    if (self == null || users == null) return;
                    for (int i = 0; i < users.Count; i++)
                    {
                        var user = users[i];
                        if (user == null || user.field_Private_APIUser_0 == null) continue;
                        var canvas = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag");
                        var canvas_2 = user.transform.Find("Canvas - Profile (1)/Text/Text - NameTag Drop");
                        if (canvas == null) continue;
                        if (canvas_2 == null) continue;
                        var text_object = canvas.GetComponent<UnityEngine.UI.Text>();
                        var text_object_2 = canvas_2.GetComponent<UnityEngine.UI.Text>();
                        if (text_object == null || text_object_2 == null || text_object.enabled == false) continue;
                        text_object.supportRichText = true;
                        text_object_2.text = $"{user.field_Private_APIUser_0.displayName}";
                        text_object.text = $"{user.field_Private_APIUser_0.displayName}";
                    }
                }
            }));

            var ck_speedup = utils.make_text_toggle(0, 450, "Speed+", speed_hacks, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { speed_hacks = true; }
                else
                { speed_hacks = false; speed.set_speeds(2f, 4f); }
            }));

            var ck_ignorefriend = utils.make_text_toggle(1100, 450, "IgnoreFriends", anti_crasher_ignore_friends, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { anti_crasher_ignore_friends = true; }
                else
                { anti_crasher_ignore_friends = false; }
            }));

            var ck_esprainbow = utils.make_text_toggle(0, 600, "ESP-rainbow", esp_rainbow_mode, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { esp_rainbow_mode = true; }
                else
                { esp_rainbow_mode = false; }
            }));

            var ck_antispawnsound = utils.make_text_toggle(1100, 600, "AntiSpawnMusic", anti_spawn_music, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { anti_spawn_music = true; }
                else
                { anti_spawn_music = false; }
            }));

            var ck_rainbowfriends = utils.make_text_toggle(0, 750, "Friend-rainbow", rainbow_friend_nameborder, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { rainbow_friend_nameborder = true; }
                else
                {
                    rainbow_friend_nameborder = false;
                    var users = utils.get_all_player();
                    if (users == null) return;
                    for (var i = 0; i < users.Count; i++)
                    {
                        var obj = users[i];
                        if (obj == null) continue;
                        if (obj.field_Private_APIUser_0 == null) continue;
                        if (utils.is_friend(obj) == false) continue;
                        obj.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(1f, 1f, 0f));
                    }
                }
            }));

            var ck_antishader = utils.make_text_toggle(1100, 750, "Anti-shader", anti_crasher_shader, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { anti_crasher_shader = true; }
                else
                { anti_crasher_shader = false; shader_menu.reset_all(); }
            }));

            var ck_antishader_fetched = utils.make_text_toggle(0, 900, "Anti-shader-fetched", should_use_fetched_list, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { should_use_fetched_list = true; }
                else
                { should_use_fetched_list = false; }
            }));

            var ck_flying_onpress = utils.make_text_toggle(1100, 900, "Fly onpress", fly_mode_onpress, main_menu_mod, new Action<bool>((a) =>
            {
                if (a)
                { fly_mode_onpress = true; }
                else
                { fly_mode_onpress = false; }
            }));

            //half2
            var b_jump = btn_utils.create_btn(false, ButtonType.Default, "Allow jump", "Enables jumping in a world that has it disabled", Color.white, Color.green, -3, 1, main_menu_mod.transform, new Action(() => { if (VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.GetComponent<PlayerModComponentJump>() == null) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.AddComponent<PlayerModComponentJump>(); }), new Action(() => { }), 2);
            var b_avi_by_id = btn_utils.create_btn(false, ButtonType.Default, "Avi-by-id", "Saved a avatar to Fav+ just by using the avatars ID", Color.white, Color.green, -2, 1, main_menu_mod.transform, new Action(() =>
            {
                menu.input_text("Enter the avatar ID (avtr_...)", "Confirm", new Action<string>((a) =>
                {
                    if (!a.Contains("avtr_"))
                    {
                        MelonModLogger.Log("Invalid avatar id!");
                        return;
                    }

                    var model = new ApiAvatar();
                    model.id = a;

                    model.Get(DelegateSupport.ConvertDelegate<Il2CppSystem.Action<ApiContainer>>
                    (new Action<ApiContainer>
                    (delegate (ApiContainer t)
                    {
                        MelonModLogger.Log("Found avatar with name:" + model.name);
                        MelonModLogger.Log("Avatar status is: " + model.releaseStatus);
                        MelonModLogger.Log("Avatar asset URL: " + model.assetUrl);
                        if (model.releaseStatus.Contains("public"))
                        {
                            //add to list
                            avatar_utils.add_to_list(model);
                            avatar_utils.update_list(avatar_config.avatar_list.Select(x => x.avatar_ident), hashmod.fav_list.listing_avatars);
                            error_type_poput("Done!", "Added avatar to Fav+ (" + model.name + ")" + " by " + model.authorName);
                        }
                        else
                        {
                            MelonModLogger.Log("Avatar is private, can not add to Fav+!");
                        }
                    })));
                }));
            }), new Action(() => { }), 2);
            var b_pubs_by_id = btn_utils.create_btn(false, ButtonType.Default, "Pub-by-id", "Shows all public avatars for a user by his user ID", Color.white, Color.green, -1, 1, main_menu_mod.transform, new Action(() =>
            {
                if (Time.time > last_apicall)
                {
                    last_apicall = Time.time + 60;
                    menu.input_text("Enter the User ID (usr_...)", "Confirm", new Action<string>((a) =>
                    {
                        if (!a.Contains("usr_"))
                        {
                            MelonModLogger.Log("Invalid user id!");
                            return;
                        }

                        pubavatar.update_public_user_list(a);
                        VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1();
                    }));
                }
                else
                {
                    var sec_left = last_apicall - Time.time;
                    error_type_poput("Function is still on cooldown!", "Please wait " + Math.Floor(sec_left) + " seconds before trying again!");
                }
            }), new Action(() => { }), 2);
            var b_reset_dynbone = btn_utils.create_btn(false, ButtonType.Default, "Reset-bones", "Dsiables and clears current dynamic bone configurations", Color.white, Color.green, 0, 1, main_menu_mod.transform, new Action(() =>
            {
                foreach (var a in dynbones.map) dynbones.remove(a.Key);
            }), new Action(() => { }), 2);
        }
    }
}
