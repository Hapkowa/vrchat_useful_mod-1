using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hashmod.remake.funcs.game;
using hashmod.remake.util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using Il2CppSystem.IO;
using VRC.Core;
using Transmtn;
using hashmod.remake.btn;
using UnityEngine.EventSystems;
using Il2CppSystem.Threading;

namespace hashmod.remake.funcs.menu
{
    public class shader_menu
    {
        public static List<string> tmp_list = new List<string>();
        public static List<List<string>> pages = new List<List<string>>();
        private static List<GameObject> list_objects = new List<GameObject>();
        public static Player set_player;
        private static bool did_setup_button = false;
        public static Button next_page_button;
        public static int current_selected_page = 0;
        public static Canvas set_canvas = null;
        public static CanvasScaler set_canvas_scaler;
        public static GraphicRaycaster set_graphicsray;
        public static VRCSDK2.VRC_UiShape set_uishape;
        public static bool canvas_setup = false;
        public static GameObject shader_menu_page = null;
        public static bool is_known(string shader)
        {
            if (hashmod.should_use_fetched_list == true) if (anticrash.shader_list.Contains(shader)) return true;
            if (anticrash.shader_list_local.Contains(shader)) return true;
            return false;
        }
        public static bool is_playing(string shader)
        {
            var active_renderers = set_player.GetComponentsInChildren<Renderer>(false);
            for (var i = 0; i < active_renderers.Count; i++)
            {
                var obj = active_renderers[i];
                if (obj == null) continue;
                for (var c = 0; c < obj.materials.Count; c++)
                {
                    var jbo = obj.materials[c];
                    if (jbo == null) continue;
                    if (jbo.shader.name.Contains(shader)) return true;
                }
            }
            return false;
        }
        static public void set_page(int page)
        {
            var startpos = 100;

            foreach (var obj in list_objects) UnityEngine.Object.Destroy(obj);
            list_objects.Clear();
            foreach (var obj in pages[page])
            {
                startpos = page_display_routine(startpos, shader_menu_page, obj);
            }
            next_page_button.enabled = true;
        }
        static public void go_next_page()
        {
            /*loop back to start if we are the end it doesnt even matter*/            
            current_selected_page++; if (current_selected_page > pages.Count - 1) current_selected_page = 0;
            set_page(current_selected_page);
        }
        static public bool setup_listing()
        {
            var renderers = set_player.prop_VRCAvatarManager_0.GetComponentsInChildren<Renderer>(true);
            var startpos = 0;

            for (var i = 0; i < renderers.Count; i++)
            {
                var obj = renderers[i];
                if (obj == null) continue;
                for (var m = 0; m < obj.materials.Count; m++)
                {
                    var mat = obj.materials[m];
                    if (tmp_list.Contains(mat.shader.name) == false)
                    {
                        bool contained = false;
                        foreach (var a in pages) if (a.Contains(mat.shader.name)) { contained = true; break; }
                        if (contained == true) continue;
                        if (tmp_list.Count == 12)
                        {
                            //split to a new page
                            pages.Add(tmp_list);
                            tmp_list = new List<string>();
                        }
                        tmp_list.Add(mat.shader.name);

                    }
                }
            }
            if (tmp_list.Count != 0)
            {
                //leftovers go to the last page
                pages.Add(tmp_list);
                tmp_list = new List<string>();
            }
            //MelonModLogger.Log("created pages " + pages.Count());
            foreach (var obj in pages.First())
            {
                startpos = page_display_routine(startpos, shader_menu_page, obj);
            }

            next_page_button.gameObject.SetActive(true);
            next_page_button.enabled = true;

            return true;
        }

        private static int page_display_routine(int startpos, GameObject asdasd, string obj)
        {
            var txt = new GameObject("Text"); txt.transform.SetParent(asdasd.transform, false);

            var trigger = txt.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            txt.AddComponent<RectTransform>();
            trigger.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 100);
            entry.callback.AddListener(new Action<BaseEventData>((d) =>
            {
                var self = txt.GetComponentInChildren<Text>();
                //MelonModLogger.Log("has clicked object \"" + self.text + "\"");
                if (anticrash.shader_list_local.Contains(self.text) == false)
                {
                    anticrash.shader_list_local.Add(self.text);
                    System.IO.File.WriteAllLines("hashmod_shaderlist.txt", anticrash.shader_list_local);
                    anticrash.shader_check(utils.get_quick_menu().get_selected_player());
                    //MelonModLogger.Log("added shader to local blacklist \"" + self.text + "\"");
                    self.color = Color.red;
                }
                else
                {
                    self.color = Color.white;
                    anticrash.shader_list_local.Remove(self.text);
                    System.IO.File.WriteAllLines("hashmod_shaderlist.txt", anticrash.shader_list_local);
                    //MelonModLogger.Log("removing shader from local blacklist \"" + self.text + "\"");
                }
            }));
            trigger.triggers.Add(entry);

            var txt_component = txt.AddComponent<Text>();
            txt_component.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); txt_component.fontSize = 64; txt_component.text = obj;
            txt_component.transform.localPosition = txt.transform.localPosition;
            txt_component.transform.localPosition += new Vector3(0, 800, 0);
            txt_component.transform.localPosition -= new Vector3(0, startpos, 0);
            txt_component.transform.localPosition += new Vector3(txt_component.fontSize * obj.Count() / 5, 75);
            txt_component.enabled = true;
            txt_component.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_component.fontSize * obj.Count(), 100);
            txt_component.alignment = TextAnchor.MiddleLeft;

            if (is_known(obj)) txt_component.color = Color.red;
            else if (is_playing(obj)) txt_component.color = Color.green;

            list_objects.Add(txt);
  
            startpos += 100;
            return startpos;
        }

        static public void reset_all()
        {
            tmp_list.Clear();
            pages.Clear();
            set_player = null;
            current_selected_page = 0;
            next_page_button.enabled = false;
            next_page_button.gameObject.SetActive(false);
            if (list_objects.Count != 0) foreach (var o in list_objects) UnityEngine.Object.Destroy(o);
        }
        static public bool open_menu = false;
        static private bool setup_back_button()
        {
            shader_menu_page = menu.make_blank_page("shaderstuff");

            var page_up_btn = GameObject.Find("UserInterface/QuickMenu/EmojiMenu/PageUp");
            var clone_page_btn = UnityEngine.Object.Instantiate<GameObject>(page_up_btn.gameObject);

            var btn_object = clone_page_btn.GetComponent<Button>();
            btn_object.onClick = new Button.ButtonClickedEvent();
            btn_object.onClick.AddListener(new Action(() => { go_next_page(); }));

            btn_object.transform.localRotation = shader_menu_page.transform.localRotation;
            btn_object.transform.localPosition += new Vector3(500,0,0);

            next_page_button = btn_object;

            btn_object.enabled = false;
            btn_object.transform.SetParent(shader_menu_page.transform, false);
            did_setup_button = true;
            next_page_button.gameObject.SetActive(false);

            var open_menu_toggle = btn_utils.create_btn(false, ButtonType.Default, "Show shader menu", "Shows the shader menu to blacklist shaders, green=running shader / red=will be blocked / white=not running", Color.white, Color.red, -2, 0, utils.get_quick_menu().transform.Find("UserInteractMenu"),
            new Action(() =>
            {
                if (hashmod.anti_crasher_shader == false)
                {
                    hashmod.error_type_poput("Feature missing!", "Enable anti-shader to make this feature work!");
                    return;
                }
                var bg = utils.get_quick_menu().transform.Find("UserInteractMenu");
                bg.gameObject.SetActive(false);
                shader_menu_page.SetActive(true);
                open_menu = true;
            }),
            new Action(() =>
            {

            }));

            return true;
        }
        static public void work()
        {
            if (hashmod.anti_crasher_shader == false) return;
            if (did_setup_button == false) setup_back_button();
            var cur_player = utils.get_quick_menu().get_selected_player();
            if (APIUser.CurrentUser == null) return;
            if (cur_player == null && set_player != null)
            {
                reset_all();
                open_menu = false;
                shader_menu_page.SetActive(false);
                return;
            }
            if (set_player != cur_player && APIUser.CurrentUser.id != cur_player.field_Private_APIUser_0.id)
            {
                /*clear and swap shader list*/
                reset_all();
                set_player = cur_player; /*setup the next player*/
                setup_listing();
                set_page(0);
            }
            if (cur_player == set_player && open_menu == true) //while we at it keep forcing out menu
            {
                var bg = utils.get_quick_menu().transform.Find("UserInteractMenu");
                bg.gameObject.SetActive(false);
                shader_menu_page.SetActive(true);
            }
        }
    }
}


