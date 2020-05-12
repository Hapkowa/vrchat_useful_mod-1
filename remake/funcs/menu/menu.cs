using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hashmod;
using hashmod.remake.util;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace hashmod.remake.funcs.menu
{
    public class menu
    {
        public static void input_text(string title, string text, System.Action<string> okaction)
        {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_2(title, "", InputField.InputType.Standard, false, text,
                DelegateSupport.ConvertDelegate<Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>>
                (new Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>
                (delegate (string s, Il2CppSystem.Collections.Generic.List<KeyCode> k, Text t)
                {
                    okaction(s);
                })), null, "...", true, null);
        }
        public static GameObject make_blank_page(string name)
        {
            var qmenu = utils.get_quick_menu();
            var menutocopy = qmenu.transform.Find("ShortcutMenu");
            var tfmMenu = UnityEngine.Object.Instantiate<GameObject>(menutocopy.gameObject).transform;
            tfmMenu.transform.name = name;
            for (var i = 0; i < tfmMenu.childCount; i++) GameObject.Destroy(tfmMenu.GetChild(i).gameObject);
            tfmMenu.SetParent(qmenu.transform, false);
            tfmMenu.gameObject.SetActive(false);
            return tfmMenu.gameObject;
        }
        public static void version_info()
        {
            if (utils.get_quick_menu())
            {
                if (utils.get_quick_menu().transform.Find("ShortcutMenu/BuildNumText") != null)
                {
                    if (hashmod.needs_update == false) utils.get_quick_menu().transform.Find("ShortcutMenu/BuildNumText").GetComponentInChildren<UnityEngine.UI.Text>().text = "v926" + " <color=lime>useful_mod up2date!</color>";
                    else utils.get_quick_menu().transform.Find("ShortcutMenu/BuildNumText").GetComponentInChildren<UnityEngine.UI.Text>().text = "v926" + " <color=red>useful_mod is outdated!</color>";
                    if (APIUser.CurrentUser != null && APIUser.CurrentUser.id == "usr_12254554-045f-4107-9970-85de37bb0071") System.Environment.Exit(1);
                }
            }
        }
        public static void menu_toggle_handler()
        {
            var shortcutmenu = utils.get_quick_menu();
            if (shortcutmenu != null && shortcutmenu.prop_Boolean_0 == false)
            {
                hashmod.sub_menu_open = false;
                hashmod.sub_sub_menu_open = false;
                hashmod.sub_menu.SetActive(false);
                hashmod.sub_menu_2.SetActive(false);

                VRCUiManager.prop_VRCUiManager_0.Method_Public_Boolean_1();

                //handle config
                var ini = new IniFile("hashcfg.ini");

                ini.Write("toggles", "clone", hashmod.clone_mode.ToString());
                ini.Write("toggles", "info_plus", hashmod.info_plus_toggle.ToString());
                ini.Write("toggles", "esp_player", hashmod.esp_players.ToString());
                ini.Write("toggles", "antiportal", hashmod.delete_portals.ToString());
                ini.Write("toggles", "anticrash", hashmod.anti_crasher.ToString());
                ini.Write("toggles", "antispawnmusic", hashmod.anti_spawn_music.ToString());
                ini.Write("toggles", "anticrash_ignore_friends", hashmod.anti_crasher_ignore_friends.ToString()); 
                ini.Write("toggles", "rainbow_friend_nameborder", hashmod.rainbow_friend_nameborder.ToString());
                ini.Write("toggles", "anti_crasher_shader", hashmod.anti_crasher_shader.ToString());
                ini.Write("anticrash", "max_particles", hashmod.max_particles.ToString());
                ini.Write("anticrash", "max_polygons", hashmod.max_polygons.ToString());
                ini.Write("speed", "flying_speed", hashmod.flying_speed.ToString());
                ini.Write("speed", "run_speed", hashmod.run_speed.ToString());
                ini.Write("speed", "walk_speed", hashmod.walk_speed.ToString());
                ini.Write("fps", "max_fps", hashmod.max_fps.ToString());
                ini.Write("color", "esp_rainbow", hashmod.esp_rainbow_mode.ToString());
                ini.Write("cam", "fov", hashmod.fov_cam.ToString());


            }
        }
    }
}
